using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace MiBocaRecuerda
{
    public partial class SettingBase : UserControl
    {
        [Browsable(true)]
        [Category("表示")]
        public int LangIndex { get; set; }

        public string SelectedFileName => chboxFileName.SelectedItem.ToString();

        List<QuizFileConfig> langs;

        public SettingBase()
        {
            InitializeComponent();

            chboxFileName.SelectedIndexChanged += (o, e) =>
            {
                SetValue(langs[chboxFileName.SelectedIndex]);
            };
        }

        private void SetValue(QuizFileConfig lang)
        {
            nudMinChapter.Value = lang.MinChapter;
            nudMaxChapter.Value = lang.MaxChapter;
            nudQuizNum.Value = lang.QuizNum;
            chboxCapital.Checked = lang.Capital;
            chboxComaPunto.Checked = lang.ComaPunto;
            nudErrorAllow.Value = lang.ErrorAllow;
            chBoxErrorAllowAll.Checked = lang.ErrorAllowAll;
        }

        public void LoadConfig(Dictionary<string, QuizFileConfig> lengua, string currentFile)
        {
            langs = lengua.Select(l => l.Value).ToList();
            chboxFileName.Items.AddRange(lengua.Select(p =>Path.GetFileNameWithoutExtension(p.Key)).ToArray());

            chboxFileName.SelectedItem = currentFile;
        }

        public QuizFileConfig GetLang()
        {
            QuizFileConfig lang = new QuizFileConfig
            {
                MinChapter = (int)nudMinChapter.Value,
                MaxChapter = (int)nudMaxChapter.Value,
                QuizNum = (int)nudQuizNum.Value,
                Capital = chboxCapital.Checked,
                ComaPunto = chboxComaPunto.Checked,
                ErrorAllow = (int)nudErrorAllow.Value,
                ErrorAllowAll = chBoxErrorAllowAll.Checked
            };

            return lang;
        }
    }
}
