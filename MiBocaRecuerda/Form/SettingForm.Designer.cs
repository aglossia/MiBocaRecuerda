namespace MiBocaRecuerda
{
    partial class SettingForm
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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSpanish = new System.Windows.Forms.TabPage();
            this.settingSpanish1 = new MiBocaRecuerda.SettingElement.SettingSpanish();
            this.tabPageEnglish = new System.Windows.Forms.TabPage();
            this.settingEnglish1 = new MiBocaRecuerda.SettingElement.SettingEnglish();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnAyudar = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageSpanish.SuspendLayout();
            this.tabPageEnglish.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnApply.Location = new System.Drawing.Point(138, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Aplicar";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(219, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSpanish);
            this.tabControl1.Controls.Add(this.tabPageEnglish);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(298, 249);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPageSpanish
            // 
            this.tabPageSpanish.Controls.Add(this.settingSpanish1);
            this.tabPageSpanish.Location = new System.Drawing.Point(4, 27);
            this.tabPageSpanish.Name = "tabPageSpanish";
            this.tabPageSpanish.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSpanish.Size = new System.Drawing.Size(290, 218);
            this.tabPageSpanish.TabIndex = 0;
            this.tabPageSpanish.Text = "Español";
            this.tabPageSpanish.UseVisualStyleBackColor = true;
            // 
            // settingSpanish1
            // 
            this.settingSpanish1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingSpanish1.LangIndex = 0;
            this.settingSpanish1.Location = new System.Drawing.Point(3, 3);
            this.settingSpanish1.Name = "settingSpanish1";
            this.settingSpanish1.Size = new System.Drawing.Size(284, 212);
            this.settingSpanish1.TabIndex = 0;
            // 
            // tabPageEnglish
            // 
            this.tabPageEnglish.Controls.Add(this.settingEnglish1);
            this.tabPageEnglish.Location = new System.Drawing.Point(4, 27);
            this.tabPageEnglish.Name = "tabPageEnglish";
            this.tabPageEnglish.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEnglish.Size = new System.Drawing.Size(290, 218);
            this.tabPageEnglish.TabIndex = 1;
            this.tabPageEnglish.Text = "English";
            this.tabPageEnglish.UseVisualStyleBackColor = true;
            // 
            // settingEnglish1
            // 
            this.settingEnglish1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingEnglish1.LangIndex = 1;
            this.settingEnglish1.Location = new System.Drawing.Point(3, 3);
            this.settingEnglish1.Name = "settingEnglish1";
            this.settingEnglish1.Size = new System.Drawing.Size(284, 212);
            this.settingEnglish1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(298, 249);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnAyudar);
            this.panel2.Controls.Add(this.btnApply);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 249);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(298, 40);
            this.panel2.TabIndex = 8;
            // 
            // btnAyudar
            // 
            this.btnAyudar.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAyudar.Location = new System.Drawing.Point(12, 6);
            this.btnAyudar.Name = "btnAyudar";
            this.btnAyudar.Size = new System.Drawing.Size(75, 23);
            this.btnAyudar.TabIndex = 1;
            this.btnAyudar.Text = "Ayuda";
            this.btnAyudar.UseVisualStyleBackColor = true;
            this.btnAyudar.Click += new System.EventHandler(this.btnAyudar_Click);
            // 
            // SettingForm
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(298, 289);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SettingForm";
            this.Text = "Setting";
            this.tabControl1.ResumeLayout(false);
            this.tabPageSpanish.ResumeLayout(false);
            this.tabPageEnglish.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSpanish;
        private System.Windows.Forms.TabPage tabPageEnglish;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private SettingElement.SettingSpanish settingSpanish1;
        private SettingElement.SettingEnglish settingEnglish1;
        private System.Windows.Forms.Button btnAyudar;
    }
}