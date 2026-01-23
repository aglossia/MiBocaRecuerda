namespace MiBocaRecuerda.SettingElement
{
    partial class SettingSpanish
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
            this.chboxCapital.Size = new System.Drawing.Size(164, 16);
            this.chboxCapital.TabIndex = 6;
            this.chboxCapital.Text = "大文字・小文字を区別しない";
            this.chboxCapital.UseVisualStyleBackColor = true;
            // 
            // chboxComaPunto
            // 
            this.chboxComaPunto.AutoSize = true;
            this.chboxComaPunto.Location = new System.Drawing.Point(30, 276);
            this.chboxComaPunto.Name = "chboxComaPunto";
            this.chboxComaPunto.Size = new System.Drawing.Size(152, 16);
            this.chboxComaPunto.TabIndex = 6;
            this.chboxComaPunto.Text = "コンマ・ピリオドを要求しない";
            this.chboxComaPunto.UseVisualStyleBackColor = true;
            // 
            // SettingSpanish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.chboxComaPunto);
            this.Controls.Add(this.chboxCapital);
            this.Name = "SettingSpanish";
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
