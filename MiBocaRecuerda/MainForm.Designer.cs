namespace MiBocaRecuerda
{
    partial class MainForm
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtQuiz = new System.Windows.Forms.TextBox();
            this.txtAnswer = new System.Windows.Forms.TextBox();
            this.btnAnswer = new System.Windows.Forms.Button();
            this.btnShowAnswer = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_setting = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_quizInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_lista = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.optionTSMI_prueba = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_resultados = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_progresoVisual = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuizFile = new System.Windows.Forms.ToolStripComboBox();
            this.siguienteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.lbl_PruebaChallengeCount = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtQuiz
            // 
            this.txtQuiz.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtQuiz.Location = new System.Drawing.Point(12, 3);
            this.txtQuiz.Multiline = true;
            this.txtQuiz.Name = "txtQuiz";
            this.txtQuiz.Size = new System.Drawing.Size(382, 58);
            this.txtQuiz.TabIndex = 0;
            // 
            // txtAnswer
            // 
            this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtAnswer.Location = new System.Drawing.Point(12, 94);
            this.txtAnswer.Multiline = true;
            this.txtAnswer.Name = "txtAnswer";
            this.txtAnswer.Size = new System.Drawing.Size(382, 58);
            this.txtAnswer.TabIndex = 0;
            // 
            // btnAnswer
            // 
            this.btnAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAnswer.Location = new System.Drawing.Point(308, 247);
            this.btnAnswer.Name = "btnAnswer";
            this.btnAnswer.Size = new System.Drawing.Size(85, 23);
            this.btnAnswer.TabIndex = 1;
            this.btnAnswer.Text = "Responder";
            this.btnAnswer.UseVisualStyleBackColor = true;
            this.btnAnswer.Click += new System.EventHandler(this.btnAnswer_Click);
            // 
            // btnShowAnswer
            // 
            this.btnShowAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnShowAnswer.Location = new System.Drawing.Point(12, 247);
            this.btnShowAnswer.Name = "btnShowAnswer";
            this.btnShowAnswer.Size = new System.Drawing.Size(86, 23);
            this.btnShowAnswer.TabIndex = 2;
            this.btnShowAnswer.Text = "Respuesta";
            this.btnShowAnswer.UseVisualStyleBackColor = true;
            this.btnShowAnswer.Click += new System.EventHandler(this.btnShowAnswer_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblResult.ForeColor = System.Drawing.Color.Red;
            this.lblResult.Location = new System.Drawing.Point(295, 168);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(31, 18);
            this.lblResult.TabIndex = 3;
            this.lblResult.Text = "NO!";
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtConsole.Location = new System.Drawing.Point(14, 189);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.Size = new System.Drawing.Size(382, 48);
            this.txtConsole.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionTSMI,
            this.startToolStripMenuItem,
            this.toolStripQuizFile,
            this.siguienteToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(408, 27);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionTSMI
            // 
            this.optionTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionTSMI_setting,
            this.optionTSMI_quizInfo,
            this.optionTSMI_lista,
            this.toolStripMenuItem1,
            this.optionTSMI_prueba,
            this.optionTSMI_resultados,
            this.optionTSMI_progresoVisual});
            this.optionTSMI.Name = "optionTSMI";
            this.optionTSMI.Size = new System.Drawing.Size(56, 23);
            this.optionTSMI.Text = "Option";
            // 
            // optionTSMI_setting
            // 
            this.optionTSMI_setting.Name = "optionTSMI_setting";
            this.optionTSMI_setting.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_setting.Text = "Setting";
            this.optionTSMI_setting.Click += new System.EventHandler(this.optionTSMI_setting_Click);
            // 
            // optionTSMI_quizInfo
            // 
            this.optionTSMI_quizInfo.Name = "optionTSMI_quizInfo";
            this.optionTSMI_quizInfo.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_quizInfo.Text = "QuizInfo";
            this.optionTSMI_quizInfo.Click += new System.EventHandler(this.optionTSMI_quizInfo_Click);
            // 
            // optionTSMI_lista
            // 
            this.optionTSMI_lista.Name = "optionTSMI_lista";
            this.optionTSMI_lista.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_lista.Text = "Mostrar Lista de Pruebas";
            this.optionTSMI_lista.Click += new System.EventHandler(this.optionTSMI_lista_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 6);
            // 
            // optionTSMI_prueba
            // 
            this.optionTSMI_prueba.Name = "optionTSMI_prueba";
            this.optionTSMI_prueba.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_prueba.Text = "Prueba";
            this.optionTSMI_prueba.Click += new System.EventHandler(this.optionTSMI_prueba_Click);
            // 
            // optionTSMI_resultados
            // 
            this.optionTSMI_resultados.Name = "optionTSMI_resultados";
            this.optionTSMI_resultados.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_resultados.Text = "Resultados";
            this.optionTSMI_resultados.Click += new System.EventHandler(this.optionTSMI_resultados_Click);
            // 
            // optionTSMI_progresoVisual
            // 
            this.optionTSMI_progresoVisual.Name = "optionTSMI_progresoVisual";
            this.optionTSMI_progresoVisual.Size = new System.Drawing.Size(203, 22);
            this.optionTSMI_progresoVisual.Text = "Progreso Visual";
            this.optionTSMI_progresoVisual.Click += new System.EventHandler(this.optionTSMI_progresoVisual_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(71, 23);
            this.startToolStripMenuItem.Text = "Comenzar";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // toolStripQuizFile
            // 
            this.toolStripQuizFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripQuizFile.Name = "toolStripQuizFile";
            this.toolStripQuizFile.Size = new System.Drawing.Size(121, 23);
            // 
            // siguienteToolStripMenuItem
            // 
            this.siguienteToolStripMenuItem.Name = "siguienteToolStripMenuItem";
            this.siguienteToolStripMenuItem.Size = new System.Drawing.Size(68, 23);
            this.siguienteToolStripMenuItem.Text = "Siguiente";
            this.siguienteToolStripMenuItem.Click += new System.EventHandler(this.siguienteToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "☺";
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(113, 250);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(20, 20);
            this.btnTranslate.TabIndex = 11;
            this.btnTranslate.UseVisualStyleBackColor = true;
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // lbl_PruebaChallengeCount
            // 
            this.lbl_PruebaChallengeCount.AutoSize = true;
            this.lbl_PruebaChallengeCount.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_PruebaChallengeCount.Location = new System.Drawing.Point(291, 254);
            this.lbl_PruebaChallengeCount.Name = "lbl_PruebaChallengeCount";
            this.lbl_PruebaChallengeCount.Size = new System.Drawing.Size(11, 12);
            this.lbl_PruebaChallengeCount.TabIndex = 12;
            this.lbl_PruebaChallengeCount.Text = "0";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtQuiz);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(408, 67);
            this.panel2.TabIndex = 14;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(408, 279);
            this.Controls.Add(this.txtAnswer);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lbl_PruebaChallengeCount);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnShowAnswer);
            this.Controls.Add(this.btnAnswer);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MBR";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtQuiz;
        private System.Windows.Forms.TextBox txtAnswer;
        private System.Windows.Forms.Button btnAnswer;
        private System.Windows.Forms.Button btnShowAnswer;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripQuizFile;
        private System.Windows.Forms.ToolStripMenuItem siguienteToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.Label lbl_PruebaChallengeCount;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_setting;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_quizInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_prueba;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_resultados;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_progresoVisual;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_lista;
        private System.Windows.Forms.Panel panel2;
    }
}

