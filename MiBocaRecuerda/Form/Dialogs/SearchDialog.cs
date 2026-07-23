using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SearchDialog : Form
    {
        public string Input => txtBuscar.Text;
        public int Mode => rdButtonAnswer.Checked ? 0 : 1;

        public SearchDialog()
        {
            InitializeComponent();

            txtBuscar.KeyPress += SettingManager.LangCtrl.KeyPress;

            Text = $"検索 - {SettingManager.CurrentQuizDB}";
        }
    }
}
