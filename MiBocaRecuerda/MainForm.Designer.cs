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
            this.chboxComplete = new System.Windows.Forms.CheckBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.chboxQuizNum = new System.Windows.Forms.CheckBox();
            this.chboxChapterTitle = new System.Windows.Forms.CheckBox();
            this.chboxChapterExample = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripQuizFile = new System.Windows.Forms.ToolStripComboBox();
            this.siguienteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chboxExercise = new System.Windows.Forms.CheckBox();
            this.chboxResult = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.lbl_PruebaChallengeCount = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtQuiz
            // 
            this.txtQuiz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuiz.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtQuiz.Location = new System.Drawing.Point(25, 37);
            this.txtQuiz.Multiline = true;
            this.txtQuiz.Name = "txtQuiz";
            this.txtQuiz.Size = new System.Drawing.Size(371, 44);
            this.txtQuiz.TabIndex = 0;
            // 
            // txtAnswer
            // 
            this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtAnswer.Location = new System.Drawing.Point(25, 93);
            this.txtAnswer.Multiline = true;
            this.txtAnswer.Name = "txtAnswer";
            this.txtAnswer.Size = new System.Drawing.Size(371, 65);
            this.txtAnswer.TabIndex = 0;
            // 
            // btnAnswer
            // 
            this.btnAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnswer.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAnswer.Location = new System.Drawing.Point(308, 243);
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
            // chboxComplete
            // 
            this.chboxComplete.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxComplete.Location = new System.Drawing.Point(105, 250);
            this.chboxComplete.Name = "chboxComplete";
            this.chboxComplete.Size = new System.Drawing.Size(20, 20);
            this.chboxComplete.TabIndex = 4;
            this.chboxComplete.UseVisualStyleBackColor = true;
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtConsole.Location = new System.Drawing.Point(23, 190);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.Size = new System.Drawing.Size(370, 47);
            this.txtConsole.TabIndex = 5;
            // 
            // chboxQuizNum
            // 
            this.chboxQuizNum.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxQuizNum.Location = new System.Drawing.Point(120, 164);
            this.chboxQuizNum.Name = "chboxQuizNum";
            this.chboxQuizNum.Size = new System.Drawing.Size(20, 20);
            this.chboxQuizNum.TabIndex = 6;
            this.chboxQuizNum.UseVisualStyleBackColor = true;
            // 
            // chboxChapterTitle
            // 
            this.chboxChapterTitle.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxChapterTitle.Location = new System.Drawing.Point(146, 164);
            this.chboxChapterTitle.Name = "chboxChapterTitle";
            this.chboxChapterTitle.Size = new System.Drawing.Size(20, 20);
            this.chboxChapterTitle.TabIndex = 6;
            this.chboxChapterTitle.UseVisualStyleBackColor = true;
            // 
            // chboxChapterExample
            // 
            this.chboxChapterExample.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxChapterExample.Location = new System.Drawing.Point(172, 164);
            this.chboxChapterExample.Name = "chboxChapterExample";
            this.chboxChapterExample.Size = new System.Drawing.Size(20, 20);
            this.chboxChapterExample.TabIndex = 6;
            this.chboxChapterExample.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingToolStripMenuItem,
            this.startToolStripMenuItem,
            this.toolStripQuizFile,
            this.siguienteToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(408, 27);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(56, 23);
            this.settingToolStripMenuItem.Text = "Setting";
            this.settingToolStripMenuItem.Click += new System.EventHandler(this.settingToolStripMenuItem_Click);
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
            // chboxExercise
            // 
            this.chboxExercise.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxExercise.Location = new System.Drawing.Point(43, 164);
            this.chboxExercise.Name = "chboxExercise";
            this.chboxExercise.Size = new System.Drawing.Size(20, 20);
            this.chboxExercise.TabIndex = 8;
            this.chboxExercise.UseVisualStyleBackColor = true;
            // 
            // chboxResult
            // 
            this.chboxResult.Appearance = System.Windows.Forms.Appearance.Button;
            this.chboxResult.Location = new System.Drawing.Point(78, 164);
            this.chboxResult.Name = "chboxResult";
            this.chboxResult.Size = new System.Drawing.Size(20, 20);
            this.chboxResult.TabIndex = 9;
            this.chboxResult.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(4, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "☺";
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(198, 164);
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
            this.lbl_PruebaChallengeCount.Location = new System.Drawing.Point(291, 250);
            this.lbl_PruebaChallengeCount.Name = "lbl_PruebaChallengeCount";
            this.lbl_PruebaChallengeCount.Size = new System.Drawing.Size(11, 12);
            this.lbl_PruebaChallengeCount.TabIndex = 12;
            this.lbl_PruebaChallengeCount.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 279);
            this.Controls.Add(this.lbl_PruebaChallengeCount);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chboxResult);
            this.Controls.Add(this.chboxExercise);
            this.Controls.Add(this.chboxChapterExample);
            this.Controls.Add(this.chboxChapterTitle);
            this.Controls.Add(this.chboxQuizNum);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.chboxComplete);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnShowAnswer);
            this.Controls.Add(this.btnAnswer);
            this.Controls.Add(this.txtAnswer);
            this.Controls.Add(this.txtQuiz);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MBR";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtQuiz;
        private System.Windows.Forms.TextBox txtAnswer;
        private System.Windows.Forms.Button btnAnswer;
        private System.Windows.Forms.Button btnShowAnswer;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.CheckBox chboxComplete;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.CheckBox chboxQuizNum;
        private System.Windows.Forms.CheckBox chboxChapterTitle;
        private System.Windows.Forms.CheckBox chboxChapterExample;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.CheckBox chboxExercise;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.CheckBox chboxResult;
        private System.Windows.Forms.ToolStripComboBox toolStripQuizFile;
        private System.Windows.Forms.ToolStripMenuItem siguienteToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.Label lbl_PruebaChallengeCount;
    }
}

