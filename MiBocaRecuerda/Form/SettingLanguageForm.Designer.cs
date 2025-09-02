namespace MiBocaRecuerda
{
    partial class SettingLanguageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbBoxSelectLang = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.settingLanguageEnglish1 = new MiBocaRecuerda.SettingLanguageElement.SettingLanguageEnglish();
            this.settingLanguageSpanish1 = new MiBocaRecuerda.SettingLanguageElement.SettingLanguageSpanish();
            this.SuspendLayout();
            // 
            // cmbBoxSelectLang
            // 
            this.cmbBoxSelectLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxSelectLang.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbBoxSelectLang.FormattingEnabled = true;
            this.cmbBoxSelectLang.Location = new System.Drawing.Point(12, 12);
            this.cmbBoxSelectLang.Name = "cmbBoxSelectLang";
            this.cmbBoxSelectLang.Size = new System.Drawing.Size(121, 26);
            this.cmbBoxSelectLang.TabIndex = 2;
            this.cmbBoxSelectLang.SelectedIndexChanged += new System.EventHandler(this.cmbBoxSelectLang_SelectedIndexChanged);
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnApply.Location = new System.Drawing.Point(181, 352);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(271, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // settingLanguageEnglish1
            // 
            this.settingLanguageEnglish1.LanguageName = "en";
            this.settingLanguageEnglish1.Location = new System.Drawing.Point(12, 41);
            this.settingLanguageEnglish1.Name = "settingLanguageEnglish1";
            this.settingLanguageEnglish1.Size = new System.Drawing.Size(317, 312);
            this.settingLanguageEnglish1.TabIndex = 1;
            // 
            // settingLanguageSpanish1
            // 
            this.settingLanguageSpanish1.LanguageName = "es";
            this.settingLanguageSpanish1.Location = new System.Drawing.Point(12, 41);
            this.settingLanguageSpanish1.Name = "settingLanguageSpanish1";
            this.settingLanguageSpanish1.Size = new System.Drawing.Size(317, 312);
            this.settingLanguageSpanish1.TabIndex = 0;
            // 
            // SettingLanguageForm
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(358, 387);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.cmbBoxSelectLang);
            this.Controls.Add(this.settingLanguageEnglish1);
            this.Controls.Add(this.settingLanguageSpanish1);
            this.Name = "SettingLanguageForm";
            this.Text = "Setting Language";
            this.ResumeLayout(false);

        }

        #endregion

        private SettingLanguageElement.SettingLanguageSpanish settingLanguageSpanish1;
        private SettingLanguageElement.SettingLanguageEnglish settingLanguageEnglish1;
        private System.Windows.Forms.ComboBox cmbBoxSelectLang;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}