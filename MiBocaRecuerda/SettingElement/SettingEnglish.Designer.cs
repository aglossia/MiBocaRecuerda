namespace MiBocaRecuerda.SettingElement
{
    partial class SettingEnglish
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
            this.chboxCapital = new System.Windows.Forms.CheckBox();
            this.chboxComaPunto = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chboxCapital
            // 
            this.chboxCapital.AutoSize = true;
            this.chboxCapital.Location = new System.Drawing.Point(30, 240);
            this.chboxCapital.Name = "chboxCapital";
            this.chboxCapital.Size = new System.Drawing.Size(117, 16);
            this.chboxCapital.TabIndex = 6;
            this.chboxCapital.Text = "Capital distinction";
            this.chboxCapital.UseVisualStyleBackColor = true;
            // 
            // chboxComaPunto
            // 
            this.chboxComaPunto.AutoSize = true;
            this.chboxComaPunto.Location = new System.Drawing.Point(30, 276);
            this.chboxComaPunto.Name = "chboxComaPunto";
            this.chboxComaPunto.Size = new System.Drawing.Size(96, 16);
            this.chboxComaPunto.TabIndex = 6;
            this.chboxComaPunto.Text = "Coma y Punto";
            this.chboxComaPunto.UseVisualStyleBackColor = true;
            // 
            // SettingEnglish
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.chboxComaPunto);
            this.Controls.Add(this.chboxCapital);
            this.Name = "SettingEnglish";
            this.Size = new System.Drawing.Size(317, 310);
            this.Controls.SetChildIndex(this.chboxCapital, 0);
            this.Controls.SetChildIndex(this.chboxComaPunto, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chboxCapital;
        private System.Windows.Forms.CheckBox chboxComaPunto;
    }
}
