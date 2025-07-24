namespace MiBocaRecuerda
{
    partial class InputDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnNO = new System.Windows.Forms.Button();
            this.nudDesde = new System.Windows.Forms.NumericUpDown();
            this.nudHasta = new System.Windows.Forms.NumericUpDown();
            this.btnAhora = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudDesde)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHasta)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(20, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "desde";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(127, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "hasta";
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.Location = new System.Drawing.Point(138, 63);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(52, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "VAMOS";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnNO
            // 
            this.btnNO.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnNO.Location = new System.Drawing.Point(198, 63);
            this.btnNO.Name = "btnNO";
            this.btnNO.Size = new System.Drawing.Size(52, 23);
            this.btnNO.TabIndex = 2;
            this.btnNO.Text = "NO";
            this.btnNO.UseVisualStyleBackColor = true;
            this.btnNO.Click += new System.EventHandler(this.btnNO_Click);
            // 
            // nudDesde
            // 
            this.nudDesde.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudDesde.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudDesde.Location = new System.Drawing.Point(61, 22);
            this.nudDesde.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDesde.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDesde.Name = "nudDesde";
            this.nudDesde.Size = new System.Drawing.Size(54, 19);
            this.nudDesde.TabIndex = 3;
            this.nudDesde.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudHasta
            // 
            this.nudHasta.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudHasta.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudHasta.Location = new System.Drawing.Point(168, 23);
            this.nudHasta.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudHasta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHasta.Name = "nudHasta";
            this.nudHasta.Size = new System.Drawing.Size(54, 19);
            this.nudHasta.TabIndex = 3;
            this.nudHasta.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnAhora
            // 
            this.btnAhora.Font = new System.Drawing.Font("MeiryoKe_Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAhora.Location = new System.Drawing.Point(22, 63);
            this.btnAhora.Name = "btnAhora";
            this.btnAhora.Size = new System.Drawing.Size(52, 23);
            this.btnAhora.TabIndex = 2;
            this.btnAhora.Text = "AHORA";
            this.btnAhora.UseVisualStyleBackColor = true;
            this.btnAhora.Click += new System.EventHandler(this.btnAhora_Click);
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 98);
            this.Controls.Add(this.nudHasta);
            this.Controls.Add(this.nudDesde);
            this.Controls.Add(this.btnNO);
            this.Controls.Add(this.btnAhora);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "InputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputDialog";
            ((System.ComponentModel.ISupportInitialize)(this.nudDesde)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHasta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnNO;
        private System.Windows.Forms.NumericUpDown nudDesde;
        private System.Windows.Forms.NumericUpDown nudHasta;
        private System.Windows.Forms.Button btnAhora;
    }
}