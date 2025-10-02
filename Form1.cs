using System;                       // 基本・イベントなど
using System.Collections.Generic;   // List<T> などのコレクション
using System.Drawing;               // 画像や描画の基本
using System.Drawing.Drawing2D;     // 角丸パスなどの高機能描画
using System.IO;                    // ファイル入出力
using System.Linq;                  // Where, Select など
using System.Windows.Forms;         // WinForms のコントロール群

namespace CupLabelIssuer            // プロジェクトの名前空間
{
    public partial class Form1 : Form // Form1 クラスは Form を継承
    {
        // 画像のリスト
        private readonly List<Image> _imgs = new();
        private int _center = 0;

        public Form1()
        {
            // デザイナが配置した UI を初期化
            InitializeComponent();

            // 矢印キーをフォームで受ける
            KeyPreview = true;

            // 5つの PictureBox の表示モード（保険で）
            foreach (var pb in new[] { pbL2, pbL1, pbC, pbR1, pbR2 })
                pb.SizeMode = PictureBoxSizeMode.Zoom;

            // 矢印ラベルの見た目＆クリックで次へ
            foreach (var a in new[] { lblA1, lblA2, lblA3, lblA4 })
            {
                a.AutoSize = false;
                a.Size = new Size(24, 24);
                a.Text = "⇔";
                a.TextAlign = ContentAlignment.MiddleCenter;
                a.ForeColor = Color.Gray;
                a.BackColor = Color.Transparent;
                a.Cursor = Cursors.Hand;
                a.Click += (_, __) => Next();
            }

            // ホイール・キーで左右
            MouseWheel += (_, e) => { if (e.Delta < 0) Next(); else Prev(); };
            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Right) Next();
                if (e.KeyCode == Keys.Left) Prev();
            };

            // 中央だけ角丸二重枠
            pbC.Paint += (_, pe) => DrawCenterHighlight(pe);

            // 画像読み込み → 初回描画
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "images");
            LoadHeaderImages(dir);
            RenderHeader();

            // 各ピクチャのサイズ/位置変更で矢印再配置
            Resize += (_, __) => LayoutArrows();
            pbL2.SizeChanged += (_, __) => LayoutArrows();
            pbL1.SizeChanged += (_, __) => LayoutArrows();
            pbC.SizeChanged += (_, __) => LayoutArrows();
            pbR1.SizeChanged += (_, __) => LayoutArrows();
            pbR2.SizeChanged += (_, __) => LayoutArrows();
            pbL2.LocationChanged += (_, __) => LayoutArrows();
            pbL1.LocationChanged += (_, __) => LayoutArrows();
            pbC.LocationChanged += (_, __) => LayoutArrows();
            pbR1.LocationChanged += (_, __) => LayoutArrows();
            pbR2.LocationChanged += (_, __) => LayoutArrows();
        }

        // 画像読み込み（画像フォルダのパス受け取り）
        private void LoadHeaderImages(string dir)
        {
            _imgs.Clear();

            // フォルダがあれば読み込む
            if (Directory.Exists(dir))
            {
                foreach (var p in Directory.EnumerateFiles(dir)
                    .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                             || p.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                             || p.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
                {
                    // ファイルロック回避：いったんメモリへ読み切ってから Image 化
                    using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    _imgs.Add(Image.FromStream(ms));
                }
            }

            // 一枚もなければダミーを生成（※修正済み：== 0）
            if (_imgs.Count == 0)
            {
                var bmp = new Bitmap(120, 120);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.WhiteSmoke);
                g.DrawRectangle(Pens.Silver, 10, 10, 100, 100);
                g.DrawString("NO IMAGE", Font, Brushes.Gray, 18, 50);
                _imgs.Add(bmp);
            }

            // 中央を先頭に
            _center = 0;
        }

        // インデックスを循環させる（マイナス対応）
        private int W(int i) => (_imgs.Count + i) % _imgs.Count;

        // 5つの画像を PictureBox に割り当て
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

            pbC.Invalidate();
            LayoutArrows();
        }

        // 次・前
        private void Next() { _center = W(_center + 1); RenderHeader(); }
        private void Prev() { _center = W(_center - 1); RenderHeader(); }

        // 矢印の位置計算
        private void LayoutArrows()
        {
            // 任意の Control のフォーム基準の矩形を得る
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

            // 矢印の縦位置（中央）
            int centerY = rC.Top + rC.Height / 2 - lblA1.Height / 2;

            // 左右の矩形の中点Xに ↔ を置く
            void Place(Label a, Rectangle left, Rectangle right)
            {
                int midX = (left.Right + right.Left) / 2 - a.Width / 2;
                a.Location = new Point(midX, centerY);
                a.BringToFront();
            }

            Place(lblA1, rL2, rL1);
            Place(lblA2, rL1, rC);
            Place(lblA3, rC, rR1);
            Place(lblA4, rR1, rR2);
        }

        // 中央画像のハイライト（角丸の二重枠）
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

        // 角丸矩形のパス
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

        // ===== デザイナ自動生成の空ハンドラ（残してOK） =====
        private void label1_Click(object sender, EventArgs e) { }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void lblA1_Click(object sender, EventArgs e) { }
        private void pbL2_Click(object sender, EventArgs e) { }
    }
}
