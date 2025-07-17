namespace MiBocaRecuerda
{
    partial class SettingBase
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudQuizNum = new System.Windows.Forms.NumericUpDown();
            this.nudMinChapter = new System.Windows.Forms.NumericUpDown();
            this.nudMaxChapter = new System.Windows.Forms.NumericUpDown();
            this.cmbboxFileName = new System.Windows.Forms.ComboBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudErrorAllow = new System.Windows.Forms.NumericUpDown();
            this.chBoxErrorAllowAll = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinChapter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxChapter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudErrorAllow)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "クイズ数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(25, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "最小章";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(25, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "最大章";
            // 
            // nudQuizNum
            // 
            this.nudQuizNum.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudQuizNum.Location = new System.Drawing.Point(78, 14);
            this.nudQuizNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudQuizNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudQuizNum.Name = "nudQuizNum";
            this.nudQuizNum.Size = new System.Drawing.Size(56, 25);
            this.nudQuizNum.TabIndex = 1;
            this.nudQuizNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudMinChapter
            // 
            this.nudMinChapter.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudMinChapter.Location = new System.Drawing.Point(78, 41);
            this.nudMinChapter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMinChapter.Name = "nudMinChapter";
            this.nudMinChapter.Size = new System.Drawing.Size(56, 25);
            this.nudMinChapter.TabIndex = 1;
            this.nudMinChapter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudMaxChapter
            // 
            this.nudMaxChapter.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudMaxChapter.Location = new System.Drawing.Point(78, 67);
            this.nudMaxChapter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxChapter.Name = "nudMaxChapter";
            this.nudMaxChapter.Size = new System.Drawing.Size(56, 25);
            this.nudMaxChapter.TabIndex = 1;
            this.nudMaxChapter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmbboxFileName
            // 
            this.cmbboxFileName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbboxFileName.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbboxFileName.FormattingEnabled = true;
            this.cmbboxFileName.Location = new System.Drawing.Point(3, 29);
            this.cmbboxFileName.Name = "cmbboxFileName";
            this.cmbboxFileName.Size = new System.Drawing.Size(304, 26);
            this.cmbboxFileName.TabIndex = 3;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblFileName.Location = new System.Drawing.Point(4, 11);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(63, 18);
            this.lblFileName.TabIndex = 4;
            this.lblFileName.Text = "FileName";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(16, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "ミス許容";
            // 
            // nudErrorAllow
            // 
            this.nudErrorAllow.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudErrorAllow.Location = new System.Drawing.Point(78, 94);
            this.nudErrorAllow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudErrorAllow.Name = "nudErrorAllow";
            this.nudErrorAllow.Size = new System.Drawing.Size(56, 25);
            this.nudErrorAllow.TabIndex = 1;
            // 
            // chBoxErrorAllowAll
            // 
            this.chBoxErrorAllowAll.AutoSize = true;
            this.chBoxErrorAllowAll.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chBoxErrorAllowAll.Location = new System.Drawing.Point(158, 16);
            this.chBoxErrorAllowAll.Name = "chBoxErrorAllowAll";
            this.chBoxErrorAllowAll.Size = new System.Drawing.Size(99, 22);
            this.chBoxErrorAllowAll.TabIndex = 2;
            this.chBoxErrorAllowAll.Text = "ミス許容全体\r\n";
            this.chBoxErrorAllowAll.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(7, 61);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(300, 163);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.chBoxErrorAllowAll);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.nudQuizNum);
            this.tabPage1.Controls.Add(this.nudMinChapter);
            this.tabPage1.Controls.Add(this.nudErrorAllow);
            this.tabPage1.Controls.Add(this.nudMaxChapter);
            this.tabPage1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(292, 137);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Quiz Setting";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(292, 137);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Other";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // SettingBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.cmbboxFileName);
            this.Name = "SettingBase";
            this.Size = new System.Drawing.Size(317, 236);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinChapter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxChapter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudErrorAllow)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudQuizNum;
        private System.Windows.Forms.NumericUpDown nudMinChapter;
        private System.Windows.Forms.NumericUpDown nudMaxChapter;
        private System.Windows.Forms.ComboBox cmbboxFileName;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudErrorAllow;
        private System.Windows.Forms.CheckBox chBoxErrorAllowAll;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}
