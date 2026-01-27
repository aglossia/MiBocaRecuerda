namespace MiBocaRecuerda
{
    partial class ResultForm
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
            this.components = new System.ComponentModel.Container();
            this.CMS_supl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.CMS_copy = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS_copy_designate = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS_copy_all = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS_copy_quiz_all = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS_copy_answer_all = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.CMS_quiz_hide = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TS_cmbRegion = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // CMS_supl
            // 
            this.CMS_supl.Name = "CMS_supl";
            this.CMS_supl.Size = new System.Drawing.Size(137, 22);
            this.CMS_supl.Text = "補足を表示";
            this.CMS_supl.Click += new System.EventHandler(this.CMS_supl_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
            // 
            // CMS_copy
            // 
            this.CMS_copy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CMS_copy_designate,
            this.CMS_copy_all,
            this.CMS_copy_quiz_all,
            this.CMS_copy_answer_all});
            this.CMS_copy.Name = "CMS_copy";
            this.CMS_copy.Size = new System.Drawing.Size(137, 22);
            this.CMS_copy.Text = "コピー";
            // 
            // CMS_copy_designate
            // 
            this.CMS_copy_designate.Name = "CMS_copy_designate";
            this.CMS_copy_designate.Size = new System.Drawing.Size(180, 22);
            this.CMS_copy_designate.Text = "指定箇所をコピー";
            this.CMS_copy_designate.Click += new System.EventHandler(this.CMS_copy_designate_Click);
            // 
            // CMS_copy_all
            // 
            this.CMS_copy_all.Name = "CMS_copy_all";
            this.CMS_copy_all.Size = new System.Drawing.Size(180, 22);
            this.CMS_copy_all.Text = "表全体をコピー";
            // 
            // CMS_copy_quiz_all
            // 
            this.CMS_copy_quiz_all.Name = "CMS_copy_quiz_all";
            this.CMS_copy_quiz_all.Size = new System.Drawing.Size(180, 22);
            this.CMS_copy_quiz_all.Text = "問題全体をコピー";
            this.CMS_copy_quiz_all.Click += new System.EventHandler(this.CMS_copy_quiz_all_Click);
            // 
            // CMS_copy_answer_all
            // 
            this.CMS_copy_answer_all.Name = "CMS_copy_answer_all";
            this.CMS_copy_answer_all.Size = new System.Drawing.Size(180, 22);
            this.CMS_copy_answer_all.Text = "答え全体をコピー";
            // 
            // CMS_edit
            // 
            this.CMS_edit.Name = "CMS_edit";
            this.CMS_edit.Size = new System.Drawing.Size(137, 22);
            this.CMS_edit.Text = "編集";
            this.CMS_edit.Click += new System.EventHandler(this.CMS_edit_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 6);
            // 
            // CMS_quiz_hide
            // 
            this.CMS_quiz_hide.Name = "CMS_quiz_hide";
            this.CMS_quiz_hide.Size = new System.Drawing.Size(137, 22);
            this.CMS_quiz_hide.Text = "クイズ非表示";
            this.CMS_quiz_hide.Click += new System.EventHandler(this.CMS_quiz_hide_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CMS_supl,
            this.toolStripMenuItem1,
            this.CMS_copy,
            this.CMS_edit,
            this.toolStripMenuItem2,
            this.CMS_quiz_hide});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(138, 104);
            // 
            // TS_cmbRegion
            // 
            this.TS_cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TS_cmbRegion.Name = "TS_cmbRegion";
            this.TS_cmbRegion.Size = new System.Drawing.Size(75, 23);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TS_cmbRegion});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(339, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dgv
            // 
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 27);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 21;
            this.dgv.Size = new System.Drawing.Size(339, 439);
            this.dgv.TabIndex = 0;
            this.dgv.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_CellMouseDown);
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 466);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ResultForm";
            this.Text = "Resultado";
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem CMS_supl;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem CMS_copy;
        private System.Windows.Forms.ToolStripMenuItem CMS_copy_designate;
        private System.Windows.Forms.ToolStripMenuItem CMS_copy_all;
        private System.Windows.Forms.ToolStripMenuItem CMS_copy_quiz_all;
        private System.Windows.Forms.ToolStripMenuItem CMS_copy_answer_all;
        private System.Windows.Forms.ToolStripMenuItem CMS_edit;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem CMS_quiz_hide;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripComboBox TS_cmbRegion;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridView dgv;
    }
}