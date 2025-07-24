using System;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class InputDialog : Form
    {
        public int Desde { get; private set; }
        public int Hasta { get; private set; }

        public InputDialog(int desde = 1, int hasta = 1)
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            nudDesde.Value = desde;
            nudHasta.Value = hasta;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int desde, hasta;

            desde = (int)nudDesde.Value;
            hasta = (int)nudHasta.Value;

            if(desde > hasta)
            {
                MessageBox.Show("有効な数値を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Desde = desde;
            Hasta = hasta;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNO_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAhora_Click(object sender, EventArgs e)
        {
            nudDesde.Value = MainForm.QuizFileConfig.MinChapterToIndex;
            nudHasta.Value = MainForm.QuizFileConfig.MaxChapterToIndex;
        }
    }
}
