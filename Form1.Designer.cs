namespace CupLabelIssuer;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pbL2 = new PictureBox();
        pbL1 = new PictureBox();
        pbC = new PictureBox();
        pbR1 = new PictureBox();
        pbR2 = new PictureBox();
        lblA1 = new Label();
        lblA2 = new Label();
        lblA3 = new Label();
        lblA4 = new Label();
        title = new Label();
        lblStatus = new Label();
        hinbanBox = new GroupBox();
        resultBox = new GroupBox();
        btnRead = new Button();
        txtHinban = new TextBox();
        lblBrand = new TextBox();
        btnWrite = new Button();
        ((System.ComponentModel.ISupportInitialize)pbL2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbL1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbC).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbR1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbR2).BeginInit();
        hinbanBox.SuspendLayout();
        resultBox.SuspendLayout();
        SuspendLayout();
        // 
        // pbL2
        // 
        pbL2.Location = new Point(39, 79);
        pbL2.Name = "pbL2";
        pbL2.Size = new Size(94, 132);
        pbL2.TabIndex = 0;
        pbL2.TabStop = false;
        // 
        // pbL1
        // 
        pbL1.Location = new Point(158, 79);
        pbL1.Name = "pbL1";
        pbL1.Size = new Size(94, 132);
        pbL1.TabIndex = 0;
        pbL1.TabStop = false;
        // 
        // pbC
        // 
        pbC.Location = new Point(281, 79);
        pbC.Name = "pbC";
        pbC.Size = new Size(94, 132);
        pbC.TabIndex = 0;
        pbC.TabStop = false;
        // 
        // pbR1
        // 
        pbR1.Location = new Point(403, 79);
        pbR1.Name = "pbR1";
        pbR1.Size = new Size(94, 132);
        pbR1.TabIndex = 0;
        pbR1.TabStop = false;
        // 
        // pbR2
        // 
        pbR2.Location = new Point(530, 79);
        pbR2.Name = "pbR2";
        pbR2.Size = new Size(94, 132);
        pbR2.TabIndex = 0;
        pbR2.TabStop = false;
        // 
        // lblA1
        // 
        lblA1.AutoSize = true;
        lblA1.Location = new Point(120, 138);
        lblA1.Name = "lblA1";
        lblA1.Size = new Size(50, 20);
        lblA1.TabIndex = 1;
        lblA1.Text = "label1";
        // 
        // lblA2
        // 
        lblA2.AutoSize = true;
        lblA2.Location = new Point(239, 138);
        lblA2.Name = "lblA2";
        lblA2.Size = new Size(50, 20);
        lblA2.TabIndex = 1;
        lblA2.Text = "label1";
        // 
        // lblA3
        // 
        lblA3.AutoSize = true;
        lblA3.Location = new Point(363, 138);
        lblA3.Name = "lblA3";
        lblA3.Size = new Size(50, 20);
        lblA3.TabIndex = 1;
        lblA3.Text = "label1";
        // 
        // lblA4
        // 
        lblA4.AutoSize = true;
        lblA4.Location = new Point(488, 138);
        lblA4.Name = "lblA4";
        lblA4.Size = new Size(50, 20);
        lblA4.TabIndex = 1;
        lblA4.Text = "label1";
        // 
        // title
        // 
        title.AutoSize = true;
        title.Location = new Point(251, 28);
        title.Name = "title";
        title.Size = new Size(148, 20);
        title.TabIndex = 2;
        title.Text = "カップヌードルラベル発行";
        title.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(298, 259);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(54, 20);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "発行中";
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // hinbanBox
        // 
        hinbanBox.Controls.Add(txtHinban);
        hinbanBox.Location = new Point(83, 259);
        hinbanBox.Name = "hinbanBox";
        hinbanBox.Size = new Size(113, 96);
        hinbanBox.TabIndex = 4;
        hinbanBox.TabStop = false;
        hinbanBox.Text = "品番入力";
        // 
        // resultBox
        // 
        resultBox.Controls.Add(lblBrand);
        resultBox.Location = new Point(488, 259);
        resultBox.Name = "resultBox";
        resultBox.Size = new Size(113, 96);
        resultBox.TabIndex = 4;
        resultBox.TabStop = false;
        resultBox.Text = "結果表示";
        // 
        // btnRead
        // 
        btnRead.Location = new Point(83, 379);
        btnRead.Name = "btnRead";
        btnRead.Size = new Size(113, 29);
        btnRead.TabIndex = 5;
        btnRead.Text = "読込";
        btnRead.UseVisualStyleBackColor = true;
        // 
        // txtHinban
        // 
        txtHinban.Location = new Point(6, 40);
        txtHinban.Name = "txtHinban";
        txtHinban.Size = new Size(101, 27);
        txtHinban.TabIndex = 0;
        // 
        // lblBrand
        // 
        lblBrand.Location = new Point(6, 40);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(101, 27);
        lblBrand.TabIndex = 0;
        // 
        // btnWrite
        // 
        btnWrite.Location = new Point(488, 379);
        btnWrite.Name = "btnWrite";
        btnWrite.Size = new Size(113, 29);
        btnWrite.TabIndex = 6;
        btnWrite.Text = "書込";
        btnWrite.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(663, 759);
        Controls.Add(btnWrite);
        Controls.Add(btnRead);
        Controls.Add(resultBox);
        Controls.Add(hinbanBox);
        Controls.Add(lblStatus);
        Controls.Add(title);
        Controls.Add(lblA4);
        Controls.Add(lblA3);
        Controls.Add(lblA2);
        Controls.Add(lblA1);
        Controls.Add(pbR2);
        Controls.Add(pbR1);
        Controls.Add(pbC);
        Controls.Add(pbL1);
        Controls.Add(pbL2);
        Name = "Form1";
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)pbL2).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbL1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbC).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbR1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbR2).EndInit();
        hinbanBox.ResumeLayout(false);
        hinbanBox.PerformLayout();
        resultBox.ResumeLayout(false);
        resultBox.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private PictureBox pbL2;
    private PictureBox pbL1;
    private PictureBox pbC;
    private PictureBox pbR1;
    private PictureBox pbR2;
    private Label lblA1;
    private Label lblA2;
    private Label lblA3;
    private Label lblA4;
    private Label title;
    private Label lblStatus;
    private GroupBox hinbanBox;
    private TextBox txtHinban;
    private GroupBox resultBox;
    private TextBox lblBrand;
    private Button btnRead;
    private Button btnWrite;
}
