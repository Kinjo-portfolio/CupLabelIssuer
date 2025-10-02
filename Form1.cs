using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CupLabelIssuer
{
    public partial class Form1 : Form
    {
        // ===== 画像まわり =====
        private readonly List<Image> _imgs = new();
        private int _center = 0; // 中央に出す _imgs のインデックス
        private readonly Label[] _arrows = new Label[4];
        private int _gap = 40;

        // ===== 画面コントロール（実行時に探して代入する） =====
        private TextBox? _tbHinban;     // 左の品番入力 TextBox
        private Button? _btnRead;      // 読込ボタン
        private TextBox? _tbResult;     // 右の結果表示 TextBox
        private Button? _btnWrite;     // 書込ボタン（ダミー）

        // ===== 品番 → 画像インデックス / 品名 の対応 =====
        // 画像の読み順（左から）：チリトマト, カレー, カップヌードル, もつ鍋醤油, シーフード
        // 中央はカップヌードル想定（_center=2 でも良いが、ここでは読み込み順0を中央にしているので
        // 画像ファイルは  image1=カップヌードル, image2=もつ鍋醤油, image3=シーフード, image4=チリトマト, image5=カレー
        // の順で配置するか、下の配列/辞書を実際の表示順に合わせて調整してね）
        private readonly string[] _brandNames = new[]
        {
            "カップヌードル",  // index 0 … 初期の中央
            "もつ鍋醤油",      // index 1
            "シーフード",      // index 2
            "チリトマト",      // index 3
            "カレー"           // index 4
        };

        // 品番（数字/CPxx どちらでもOK）→ _imgs のインデックス
        private readonly Dictionary<string, int> _codeToIndex = new(StringComparer.OrdinalIgnoreCase)
        {
            { "1", 0 }, { "CP01", 0 },
            { "2", 1 }, { "CP02", 1 },
            { "3", 2 }, { "CP03", 2 },
            { "4", 3 }, { "CP04", 3 },
            { "5", 4 }, { "CP05", 4 },
        };

        // 逆引き（中央ダブルクリックで品番を入力欄へ）
        private readonly string[] _indexToCode = { "1", "2", "3", "4", "5" };

        public Form1()
        {
            InitializeComponent();

            // デザイナに残った label1 は削除（残っていてもOKだが邪魔なので）
            RemovePlaceholderLabels();

            // 画面上の “品番入力 / 結果表示 / 読込 / 書込” を実行時に探す（名前が textBox1 でも動く）
            BindRuntimeControls();

            // ←→ をフォームで拾う
            KeyPreview = true;

            // 5つのPictureBox共通設定
            foreach (var pb in new[] { pbL2, pbL1, pbC, pbR1, pbR2 })
                pb.SizeMode = PictureBoxSizeMode.Zoom;

            // ⇔ 矢印ラベルを作成
            CreateArrows();

            // 入力操作
            MouseWheel += (_, e) => { if (e.Delta < 0) Next(); else Prev(); };
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Right) Next(); if (e.KeyCode == Keys.Left) Prev(); };

            // 中央だけ角丸の二重枠
            pbC.Paint += (_, pe) => DrawCenterHighlight(pe);

            // 中央画像ダブルクリック → 現在の品番を入力欄へ
            pbC.DoubleClick += (_, __) =>
            {
                if (_tbHinban != null)
                {
                    int ix = W(_center); // 念のため循環済みで
                    if (ix >= 0 && ix < _indexToCode.Length) _tbHinban.Text = _indexToCode[ix];
                }
            };

            // 画像読み込み → 表示
            LoadHeaderImages(ResolveImagesDir());
            RenderHeader();

            // リサイズや配置変更で再レイアウト
            Resize += (_, __) => { LayoutHeaderPositions(); LayoutArrows(); };
            foreach (var pb in new[] { pbL2, pbL1, pbC, pbR1, pbR2 })
            {
                pb.SizeChanged += (_, __) => { LayoutHeaderPositions(); LayoutArrows(); };
                pb.LocationChanged += (_, __) => LayoutArrows();
            }

            // 読込/書込イベント（書込はダミー）
            if (_btnRead != null) _btnRead.Click += (_, __) => ReadHinbanAndShow();
            if (_btnWrite != null) _btnWrite.Click += (_, __) => MessageBox.Show("書込はダミーです。");
        }

        // ===== 実行時に画面コントロールを拾う =====
        private void BindRuntimeControls()
        {
            // “品番入力”の GroupBox 内の TextBox を品番入力欄として取得
            var gbHin = Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("品番"));
            _tbHinban = gbHin?.Controls.OfType<TextBox>().FirstOrDefault();
            _btnRead = Controls.OfType<Button>().FirstOrDefault(b => b.Text.Contains("読込"));

            // “結果表示”の GroupBox 内の TextBox を結果欄に
            var gbRes = Controls.OfType<GroupBox>().FirstOrDefault(g => g.Text.Contains("結果"));
            _tbResult = gbRes?.Controls.OfType<TextBox>().FirstOrDefault();
            _btnWrite = Controls.OfType<Button>().FirstOrDefault(b => b.Text.Contains("書込"));
        }

        // ===== 品番読込 → 画像と結果表示 =====
        private void ReadHinbanAndShow()
        {
            if (_tbHinban == null) return;

            var code = (_tbHinban.Text ?? "").Trim();
            if (string.IsNullOrEmpty(code)) return;

            if (_codeToIndex.TryGetValue(code, out int ix))
            {
                _center = W(ix);     // 中央をそのインデックスへ
                RenderHeader();

                if (_tbResult != null && ix >= 0 && ix < _brandNames.Length)
                    _tbResult.Text = _brandNames[ix];
            }
            else
            {
                // 不明コード
                _tbResult?.Clear();
                MessageBox.Show($"品番 '{code}' は未登録です。1〜5 または CP01〜CP05 を入力してください。");
            }
        }

        // ===== 画像ディレクトリの解決 =====
        private string ResolveImagesDir()
        {
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var dir1 = Path.Combine(exeDir, "data", "images"); // 実行フォルダ
            var dir2 = Path.GetFullPath(Path.Combine(exeDir, @"..\..\..", "data", "images")); // プロジェクト直下
            return Directory.Exists(dir1) ? dir1
                 : Directory.Exists(dir2) ? dir2
                 : dir1;
        }

        // ===== 余分な label1 を消す =====
        private void RemovePlaceholderLabels()
        {
            var trash = Controls.OfType<Label>()
                                .Where(l => string.Equals(l.Text, "label1", StringComparison.OrdinalIgnoreCase))
                                .ToList();
            foreach (var l in trash) { Controls.Remove(l); l.Dispose(); }
        }

        // ===== ⇔ 矢印を作成 =====
        private void CreateArrows()
        {
            for (int i = 0; i < _arrows.Length; i++)
            {
                var a = new Label
                {
                    AutoSize = false,
                    Size = new Size(24, 24),
                    Text = "⇔",
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Gray,
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };
                a.Click += (_, __) => Next();
                _arrows[i] = a;
                Controls.Add(a);
                a.BringToFront();
            }
        }

        // ===== 画像読み込み =====
        private void LoadHeaderImages(string dir)
        {
            _imgs.Clear();

            if (Directory.Exists(dir))
            {
                foreach (var p in Directory.EnumerateFiles(dir)
                             .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                      || p.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                      || p.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                             .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                {
                    using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    _imgs.Add(Image.FromStream(ms));
                }
            }

            if (_imgs.Count == 0)
            {
                var bmp = new Bitmap(120, 120);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.WhiteSmoke);
                g.DrawRectangle(Pens.Silver, 10, 10, 100, 100);
                g.DrawString("NO IMAGE", SystemFonts.MenuFont, Brushes.Gray, 18, 50);
                _imgs.Add(bmp);
            }

            _center = 0; // 最初は0番を中央に
        }

        // ===== 表示更新 =====
        private int W(int i) => (_imgs.Count + i) % _imgs.Count;

        private void RenderHeader()
        {
            if (_imgs.Count == 0) return;

            pbL2.Image = _imgs[W(_center - 2)];
            pbL1.Image = _imgs[W(_center - 1)];
            pbC.Image = _imgs[W(_center)];
            pbR1.Image = _imgs[W(_center + 1)];
            pbR2.Image = _imgs[W(_center + 2)];

            pbL2.Size = new Size(90, 90);
            pbL1.Size = new Size(90, 90);
            pbC.Size = new Size(120, 120);
            pbR1.Size = new Size(90, 90);
            pbR2.Size = new Size(90, 90);

            LayoutHeaderPositions();
            pbC.Invalidate();
            LayoutArrows();
        }

        // 5枚を等間隔に中央寄せ（pbC.Top を基準に）
        private void LayoutHeaderPositions()
        {
            int y = pbC.Top;
            int totalW = pbL2.Width + pbL1.Width + pbC.Width + pbR1.Width + pbR2.Width + _gap * 4;
            int startX = (ClientSize.Width - totalW) / 2;
            int x = Math.Max(startX, 10);

            pbL2.Location = new Point(x, y); x += pbL2.Width + _gap;
            pbL1.Location = new Point(x, y); x += pbL1.Width + _gap;
            pbC.Location = new Point(x, y); x += pbC.Width + _gap;
            pbR1.Location = new Point(x, y); x += pbR1.Width + _gap;
            pbR2.Location = new Point(x, y);
        }

        private void Next() { _center = W(_center + 1); RenderHeader(); }
        private void Prev() { _center = W(_center - 1); RenderHeader(); }

        // ⇔ の位置
        private void LayoutArrows()
        {
            Rectangle RectOnForm(Control c)
            {
                var screenTL = c.PointToScreen(Point.Empty);
                var formTL = this.PointToClient(screenTL);
                return new Rectangle(formTL, c.Size);
            }

            var rL2 = RectOnForm(pbL2);
            var rL1 = RectOnForm(pbL1);
            var rC = RectOnForm(pbC);
            var rR1 = RectOnForm(pbR1);
            var rR2 = RectOnForm(pbR2);

            int arrowH = _arrows[0]?.Height ?? 24;
            int centerY = rC.Top + rC.Height / 2 - arrowH / 2;

            void Place(Label a, Rectangle left, Rectangle right)
            {
                int midX = (left.Right + right.Left) / 2 - a.Width / 2;
                a.Location = new Point(midX, centerY);
                a.BringToFront();
            }

            Place(_arrows[0], rL2, rL1);
            Place(_arrows[1], rL1, rC);
            Place(_arrows[2], rC, rR1);
            Place(_arrows[3], rR1, rR2);
        }

        // 中央画像の強調枠
        private void DrawCenterHighlight(PaintEventArgs e)
        {
            var r = new Rectangle(1, 1, pbC.Width - 3, pbC.Height - 3);
            using var path = RoundedRect(r, 12);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var penOuter = new Pen(Color.FromArgb(60, Color.SteelBlue), 7);
            using var penInner = new Pen(Color.SteelBlue, 3);
            e.Graphics.DrawPath(penOuter, path);
            e.Graphics.DrawPath(penInner, path);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var gp = new GraphicsPath();
            gp.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            gp.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            gp.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            gp.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }
    }
}
