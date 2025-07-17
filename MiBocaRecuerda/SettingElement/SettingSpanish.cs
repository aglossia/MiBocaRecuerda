using System.Collections.Generic;
using System.Linq;

namespace MiBocaRecuerda.SettingElement
{
    public partial class SettingSpanish : SettingBase
    {
        List<LenguaConfig> lengua;

        public SettingSpanish()
        {
            InitializeComponent();

            _cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                SetValue(lengua[_cmbboxFileName.SelectedIndex]);
            };
        }

        // SettingBaseが持ってるcmbboxFileNameを教えてもらってそのイベントを拾ってそれぞれでSetValueを実行している
        public void SetValue(LenguaConfig _lengua)
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

        public override LenguaConfig GetLang()
        {
            LenguaConfig lang = new LenguaConfig
            {
                Name = LanguageName,
                Capital = chboxCapital.Checked,
                ComaPunto = chboxComaPunto.Checked
            };

            return lang;
        }
    }
}
