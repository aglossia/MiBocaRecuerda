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
            this.chboxCapital = new System.Windows.Forms.CheckBox();
            this.chboxComaPunto = new System.Windows.Forms.CheckBox();
            this.chboxFileName = new System.Windows.Forms.ComboBox();
            this.lblFileName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinChapter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxChapter)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(14, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "クイズ数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(14, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "最小章";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(14, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "最大章";
            // 
            // nudQuizNum
            // 
            this.nudQuizNum.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudQuizNum.Location = new System.Drawing.Point(67, 66);
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
            this.nudMinChapter.Location = new System.Drawing.Point(67, 91);
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
            this.nudMaxChapter.Location = new System.Drawing.Point(67, 119);
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
            // chboxCapital
            // 
            this.chboxCapital.AutoSize = true;
            this.chboxCapital.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chboxCapital.Location = new System.Drawing.Point(147, 69);
            this.chboxCapital.Name = "chboxCapital";
            this.chboxCapital.Size = new System.Drawing.Size(88, 40);
            this.chboxCapital.TabIndex = 2;
            this.chboxCapital.Text = "Capital\r\nDistinction";
            this.chboxCapital.UseVisualStyleBackColor = true;
            // 
            // chboxComaPunto
            // 
            this.chboxComaPunto.AutoSize = true;
            this.chboxComaPunto.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chboxComaPunto.Location = new System.Drawing.Point(147, 103);
            this.chboxComaPunto.Name = "chboxComaPunto";
            this.chboxComaPunto.Size = new System.Drawing.Size(72, 40);
            this.chboxComaPunto.TabIndex = 2;
            this.chboxComaPunto.Text = "Coma y\r\nPunto";
            this.chboxComaPunto.UseVisualStyleBackColor = true;
            // 
            // chboxFileName
            // 
            this.chboxFileName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chboxFileName.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chboxFileName.FormattingEnabled = true;
            this.chboxFileName.Location = new System.Drawing.Point(3, 29);
            this.chboxFileName.Name = "chboxFileName";
            this.chboxFileName.Size = new System.Drawing.Size(223, 26);
            this.chboxFileName.TabIndex = 3;
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
            // SettingBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.chboxFileName);
            this.Controls.Add(this.chboxComaPunto);
            this.Controls.Add(this.chboxCapital);
            this.Controls.Add(this.nudMaxChapter);
            this.Controls.Add(this.nudMinChapter);
            this.Controls.Add(this.nudQuizNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SettingBase";
            this.Size = new System.Drawing.Size(235, 153);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinChapter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxChapter)).EndInit();
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
        private System.Windows.Forms.CheckBox chboxCapital;
        private System.Windows.Forms.CheckBox chboxComaPunto;
        private System.Windows.Forms.ComboBox chboxFileName;
        private System.Windows.Forms.Label lblFileName;
    }
}
