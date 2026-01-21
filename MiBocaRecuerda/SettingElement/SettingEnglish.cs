using System.Collections.Generic;
using System.Linq;

namespace MiBocaRecuerda.SettingElement
{
    public partial class SettingEnglish : SettingBase
    {
        List<FileLenguaConfig> lengua;

        public SettingEnglish()
        {
            InitializeComponent();

            SetRegion(new string[] { "us" });

            _cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                SetValue(lengua[_cmbboxFileName.SelectedIndex]);
            };
        }

        // SettingBaseが持ってるcmbboxFileNameを教えてもらってそのイベントを拾ってそれぞれでSetValueを実行している
        public void SetValue(FileLenguaConfig _lengua)
        {
            chboxCapital.Checked = _lengua.Capital;
            chboxComaPunto.Checked = _lengua.ComaPunto;
        }

        public override void LoadConfig(string currentFile)
        {
            lengua = SettingManager.CommonConfigManager[LanguageName].Values.Select(s => s.LenguaConfig).ToList();

            // こっちではcommonの方の設定
            base.LoadConfig(currentFile);
        }

        public override FileLenguaConfig GetLang()
        {
            FileLenguaConfig lang = new FileLenguaConfig
            {
                Capital = chboxCapital.Checked,
                ComaPunto = chboxComaPunto.Checked
            };

            return lang;
        }
    }
}
