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
        [Description("言語設定")]
        public string LanguageName { get; set; }

        public string SelectedFileName => cmbboxFileName.SelectedItem.ToString();

        List<QuizFileConfig> qfc;

        protected ComboBox _cmbboxFileName => cmbboxFileName;

        public SettingBase()
        {
            InitializeComponent();

            cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                SetValue(qfc[cmbboxFileName.SelectedIndex]);
            };
        }

        private void SetValue(QuizFileConfig lang)
        {
            nudMinChapter.Value = lang.MinChapter;
            nudMaxChapter.Value = lang.MaxChapter;
            nudQuizNum.Value = lang.QuizNum;
            nudErrorAllow.Value = lang.ErrorAllow;
            chboxErrorAllowAll.Checked = lang.ErrorAllowAll;
            chboxErrorReset.Checked = lang.ErrorReset;
        }

        public virtual void LoadConfig(string currentFile)
        {
            Dictionary<string, CommonConfig> cc = SettingManager.CommonConfigManager[LanguageName];

            qfc = cc.Values.Select(s => s.QuizFileConfig).ToList();
            cmbboxFileName.Items.AddRange(cc.Select(p =>Path.GetFileNameWithoutExtension(p.Key)).ToArray());

            cmbboxFileName.SelectedItem = currentFile;
        }

        public QuizFileConfig GetCommon()
        {
            QuizFileConfig lang = new QuizFileConfig
            {
                MinChapter = (int)nudMinChapter.Value,
                MaxChapter = (int)nudMaxChapter.Value,
                QuizNum = (int)nudQuizNum.Value,
                ErrorAllow = (int)nudErrorAllow.Value,
                ErrorAllowAll = chboxErrorAllowAll.Checked,
                ErrorReset = chboxErrorReset.Checked
            };

            return lang;
        }

        // LenguaConfigは継承先で設定するため、このメソッドは継承先で必ず実装すること
        // SettingFormで共通で使うために、ここで宣言しておく必要があった
        public virtual LenguaConfig GetLang() { return new LenguaConfig(); }
    }
}
