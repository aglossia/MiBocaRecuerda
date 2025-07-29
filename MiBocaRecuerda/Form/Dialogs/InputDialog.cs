using System;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class InputDialog : Form
    {
        public int Desde { get; private set; }
        public int Hasta { get; private set; }
        public bool IsIndex { get; private set; }

        public InputDialog(int desde = 1, int hasta = 1, bool isIndex = true)
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            nudDesde.Value = desde;
            nudHasta.Value = hasta;
            chboxIndex.Checked = isIndex;
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
            IsIndex = chboxIndex.Checked;

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
            nudDesde.Value = chboxIndex.Checked ? MainForm.QuizFileConfig.MinChapterToIndex : MainForm.QuizFileConfig.MinChapter;
            nudHasta.Value = chboxIndex.Checked ? MainForm.QuizFileConfig.MaxChapterToIndex : MainForm.QuizFileConfig.MaxChapter;
        }

        private void chboxChapter_CheckedChanged(object sender, EventArgs e)
        {
            bool check = (sender as CheckBox).Checked;

            nudDesde.Increment = check ? 10 : 1;
            nudHasta.Increment = check ? 10 : 1;

            lblModo.Text = $"Selección de {(check ? "Índice" : "Capítulo")}";
        }
    }
}
