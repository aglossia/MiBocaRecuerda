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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.optionTSMI_prueba = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_resultados = new System.Windows.Forms.ToolStripMenuItem();
            this.optionTSMI_progresoVisual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionTSMI_DarkMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuizFile = new System.Windows.Forms.ToolStripComboBox();
            this.operationTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.operationTSMI_start = new System.Windows.Forms.ToolStripMenuItem();
            this.operationTSMI_siguiente = new System.Windows.Forms.ToolStripMenuItem();
            this.operationTSMI_anterior = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTSMI_pruebaLista = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTSMI_prueba_e = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTSMI_chapterList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTSMI_translate = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl_PruebaChallengeCount = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_ErrorAllowCount = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtQuiz
            // 
            this.txtQuiz.Font = new System.Drawing.Font("MeiryoKe_Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtQuiz.Location = new System.Drawing.Point(12, 3);
            this.txtQuiz.Multiline = true;
            this.txtQuiz.Name = "txtQuiz";
            this.txtQuiz.Size = new System.Drawing.Size(402, 58);
            this.txtQuiz.TabIndex = 0;
            // 
            // txtAnswer
            // 
            this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnswer.Font = new System.Drawing.Font("MeiryoKe_Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtAnswer.Location = new System.Drawing.Point(12, 94);
            this.txtAnswer.Multiline = true;
            this.txtAnswer.Name = "txtAnswer";
            this.txtAnswer.Size = new System.Drawing.Size(403, 58);
            this.txtAnswer.TabIndex = 0;
            // 
            // btnAnswer
            // 
            this.btnAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAnswer.Location = new System.Drawing.Point(329, 247);
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
            this.txtConsole.Font = new System.Drawing.Font("MeiryoKe_Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtConsole.Location = new System.Drawing.Point(14, 189);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.Size = new System.Drawing.Size(403, 48);
            this.txtConsole.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionTSMI,
            this.toolStripQuizFile,
            this.operationTSMI,
            this.toolTSMI});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(429, 25);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionTSMI
            // 
            this.optionTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionTSMI_setting,
            this.optionTSMI_quizInfo,
            this.toolStripMenuItem1,
            this.optionTSMI_prueba,
            this.optionTSMI_resultados,
            this.optionTSMI_progresoVisual,
            this.toolStripMenuItem2,
            this.optionTSMI_DarkMode});
            this.optionTSMI.Name = "optionTSMI";
            this.optionTSMI.Size = new System.Drawing.Size(56, 23);
            this.optionTSMI.Text = "Option";
            // 
            // optionTSMI_setting
            // 
            this.optionTSMI_setting.Name = "optionTSMI_setting";
            this.optionTSMI_setting.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_setting.Text = "Setting";
            this.optionTSMI_setting.Click += new System.EventHandler(this.optionTSMI_setting_Click);
            // 
            // optionTSMI_quizInfo
            // 
            this.optionTSMI_quizInfo.Name = "optionTSMI_quizInfo";
            this.optionTSMI_quizInfo.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_quizInfo.Text = "QuizInfo";
            this.optionTSMI_quizInfo.Click += new System.EventHandler(this.optionTSMI_quizInfo_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // optionTSMI_prueba
            // 
            this.optionTSMI_prueba.Name = "optionTSMI_prueba";
            this.optionTSMI_prueba.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_prueba.Text = "Prueba";
            this.optionTSMI_prueba.Click += new System.EventHandler(this.optionTSMI_prueba_Click);
            // 
            // optionTSMI_resultados
            // 
            this.optionTSMI_resultados.Name = "optionTSMI_resultados";
            this.optionTSMI_resultados.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_resultados.Text = "Resultados";
            this.optionTSMI_resultados.Click += new System.EventHandler(this.optionTSMI_resultados_Click);
            // 
            // optionTSMI_progresoVisual
            // 
            this.optionTSMI_progresoVisual.Name = "optionTSMI_progresoVisual";
            this.optionTSMI_progresoVisual.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_progresoVisual.Text = "Progreso Visual";
            this.optionTSMI_progresoVisual.Click += new System.EventHandler(this.optionTSMI_progresoVisual_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 6);
            // 
            // optionTSMI_DarkMode
            // 
            this.optionTSMI_DarkMode.Name = "optionTSMI_DarkMode";
            this.optionTSMI_DarkMode.Size = new System.Drawing.Size(155, 22);
            this.optionTSMI_DarkMode.Text = "Dark Mode";
            this.optionTSMI_DarkMode.Click += new System.EventHandler(this.optionTSMI_DarkMode_Click);
            // 
            // toolStripQuizFile
            // 
            this.toolStripQuizFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripQuizFile.Name = "toolStripQuizFile";
            this.toolStripQuizFile.Size = new System.Drawing.Size(121, 23);
            // 
            // operationTSMI
            // 
            this.operationTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationTSMI_start,
            this.operationTSMI_siguiente,
            this.operationTSMI_anterior});
            this.operationTSMI.Name = "operationTSMI";
            this.operationTSMI.Size = new System.Drawing.Size(59, 23);
            this.operationTSMI.Text = "Manejo";
            // 
            // operationTSMI_start
            // 
            this.operationTSMI_start.Name = "operationTSMI_start";
            this.operationTSMI_start.Size = new System.Drawing.Size(150, 22);
            this.operationTSMI_start.Text = "Comenzar";
            this.operationTSMI_start.Click += new System.EventHandler(this.operationTSMI_start_Click);
            // 
            // operationTSMI_siguiente
            // 
            this.operationTSMI_siguiente.Name = "operationTSMI_siguiente";
            this.operationTSMI_siguiente.Size = new System.Drawing.Size(150, 22);
            this.operationTSMI_siguiente.Text = "Siguiente Quiz";
            this.operationTSMI_siguiente.Click += new System.EventHandler(this.operationTSMI_siguiente_Click);
            // 
            // operationTSMI_anterior
            // 
            this.operationTSMI_anterior.Name = "operationTSMI_anterior";
            this.operationTSMI_anterior.Size = new System.Drawing.Size(150, 22);
            this.operationTSMI_anterior.Text = "Anterior Quiz";
            this.operationTSMI_anterior.Click += new System.EventHandler(this.operationTSMI_anterior_Click);
            // 
            // toolTSMI
            // 
            this.toolTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolTSMI_pruebaLista,
            this.toolTSMI_prueba_e,
            this.toolTSMI_chapterList,
            this.toolTSMI_translate});
            this.toolTSMI.Name = "toolTSMI";
            this.toolTSMI.Size = new System.Drawing.Size(84, 23);
            this.toolTSMI.Text = "Herramienta";
            // 
            // toolTSMI_pruebaLista
            // 
            this.toolTSMI_pruebaLista.Name = "toolTSMI_pruebaLista";
            this.toolTSMI_pruebaLista.Size = new System.Drawing.Size(206, 22);
            this.toolTSMI_pruebaLista.Text = "Mostrar Lista de Pruebas";
            this.toolTSMI_pruebaLista.Click += new System.EventHandler(this.toolTSMI_pruebaLista_Click);
            // 
            // toolTSMI_prueba_e
            // 
            this.toolTSMI_prueba_e.Name = "toolTSMI_prueba_e";
            this.toolTSMI_prueba_e.Size = new System.Drawing.Size(206, 22);
            this.toolTSMI_prueba_e.Text = "Mostrar lista especificada";
            this.toolTSMI_prueba_e.Click += new System.EventHandler(this.toolTSMI_prueba_e_Click);
            // 
            // toolTSMI_chapterList
            // 
            this.toolTSMI_chapterList.Name = "toolTSMI_chapterList";
            this.toolTSMI_chapterList.Size = new System.Drawing.Size(206, 22);
            this.toolTSMI_chapterList.Text = "Chapter list";
            this.toolTSMI_chapterList.Click += new System.EventHandler(this.toolTSMI_chapterList_Click);
            // 
            // toolTSMI_translate
            // 
            this.toolTSMI_translate.Name = "toolTSMI_translate";
            this.toolTSMI_translate.Size = new System.Drawing.Size(206, 22);
            this.toolTSMI_translate.Text = "Traducir";
            this.toolTSMI_translate.Click += new System.EventHandler(this.toolTSMI_translate_Click);
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
            this.panel2.Controls.Add(this.txtQuiz);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(429, 67);
            this.panel2.TabIndex = 14;
            // 
            // lbl_ErrorAllowCount
            // 
            this.lbl_ErrorAllowCount.AutoSize = true;
            this.lbl_ErrorAllowCount.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lbl_ErrorAllowCount.Location = new System.Drawing.Point(192, 254);
            this.lbl_ErrorAllowCount.Name = "lbl_ErrorAllowCount";
            this.lbl_ErrorAllowCount.Size = new System.Drawing.Size(11, 12);
            this.lbl_ErrorAllowCount.TabIndex = 12;
            this.lbl_ErrorAllowCount.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(429, 279);
            this.Controls.Add(this.txtAnswer);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lbl_ErrorAllowCount);
            this.Controls.Add(this.lbl_PruebaChallengeCount);
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
        private System.Windows.Forms.ToolStripComboBox toolStripQuizFile;
        private System.Windows.Forms.Label lbl_PruebaChallengeCount;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_setting;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_quizInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_prueba;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_resultados;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_progresoVisual;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem toolTSMI;
        private System.Windows.Forms.ToolStripMenuItem toolTSMI_translate;
        private System.Windows.Forms.ToolStripMenuItem operationTSMI;
        private System.Windows.Forms.ToolStripMenuItem operationTSMI_start;
        private System.Windows.Forms.ToolStripMenuItem operationTSMI_siguiente;
        private System.Windows.Forms.ToolStripMenuItem operationTSMI_anterior;
        private System.Windows.Forms.ToolStripMenuItem toolTSMI_chapterList;
        private System.Windows.Forms.ToolStripMenuItem toolTSMI_pruebaLista;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem optionTSMI_DarkMode;
        private System.Windows.Forms.Label lbl_ErrorAllowCount;
        private System.Windows.Forms.ToolStripMenuItem toolTSMI_prueba_e;
    }
}

