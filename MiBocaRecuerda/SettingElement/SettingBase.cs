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

        public string SelectedFileName => cmbboxFileName.SelectedItem?.ToString();

        List<QuizFileConfig> qfc;

        protected ComboBox _cmbboxFileName => cmbboxFileName;

        public SettingBase()
        {
            InitializeComponent();

            cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                SetValue(qfc[cmbboxFileName.SelectedIndex]);
            };

            UpdateErrorControls();
        }

        private void SetValue(QuizFileConfig lang)
        {
            nudMinChapter.Value = lang.MinChapter;
            nudMaxChapter.Value = lang.MaxChapter;
            nudQuizNum.Value = lang.QuizNum;
            nudErrorAllow.Value = lang.ErrorAllowCnt;
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
                ErrorAllowCnt = (int)nudErrorAllow.Value,
                ErrorAllowAll = chboxErrorAllowAll.Checked,
                ErrorReset = chboxErrorReset.Checked
            };

            return lang;
        }

        // LenguaConfigは継承先で設定するため、このメソッドは継承先で必ず実装すること
        // SettingFormで共通で使うために、ここで宣言しておく必要があった
        public virtual FileLenguaConfig GetLang() { return new FileLenguaConfig(); }

        private void btnQuizMax_Click(object sender, System.EventArgs e)
        {
            int min = (int)nudMinChapter.Value;
            int max = (int)nudMaxChapter.Value;

            if (max - min < 0) return;

            nudQuizNum.Value = (max - min + 1) * 10;
        }

        private void chboxErrorAllowAll_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }

        private void chboxErrorReset_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }

        private void UpdateErrorControls()
        {
            bool check_eaa = chboxErrorAllowAll.Checked;
            bool check_er = chboxErrorReset.Checked;
            int error_cnt = (int)nudErrorAllow.Value;

            if(error_cnt > 0)
            {
                lblErrorAll.Text = $"ミス許容範囲：{(check_eaa ? "セッション" : "問題")}";
                lblErrorReset.Text = $"ミス満了リセット：{(check_er ? "はい" : "いいえ")}";

                chboxErrorAllowAll.Enabled = true;
                chboxErrorReset.Enabled = true;
            }
            else
            {
                lblErrorAll.Text = $"ミス許容範囲：-";
                lblErrorReset.Text = $"ミス満了リセット：-";

                chboxErrorAllowAll.Enabled = false;
                chboxErrorReset.Enabled = false;
            }
        }

        private void nudErrorAllow_ValueChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }
    }
}
