using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingLanguageForm : Form
    {
        private Dictionary<string, SettingLanguageBase> _settingLanguagesDic = new Dictionary<string, SettingLanguageBase>();
        private List<SettingLanguageBase> _settingLanguages = new List<SettingLanguageBase>();
        private SettingLanguageBase _currentSettingLanguage;
        private string _currentLang => cmbBoxSelectLang.SelectedItem?.ToString();

        public SettingLanguageForm(string currentLang)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            _settingLanguages.Add(settingLanguageSpanish1);
            _settingLanguages.Add(settingLanguageEnglish1);

            List<string> langs = new List<string> { "Spanish", "English" };

            for (int i = 0; i < Math.Min(langs.Count, _settingLanguages.Count); i++)
            {
                _settingLanguagesDic[langs[i]] = _settingLanguages[i];
            }

            if (currentLang == "") _settingLanguages.ForEach(sl => sl.Visible = false);
            _settingLanguages.ForEach(sl => sl.LoadConfig());

            cmbBoxSelectLang.Items.AddRange(langs.ToArray());
            if (currentLang != "") cmbBoxSelectLang.SelectedIndex = AppRom.LenguaIndex[currentLang];
        }

        private void cmbBoxSelectLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settingLanguages.ForEach(sl => sl.Visible = true);

            string item = cmbBoxSelectLang.SelectedItem.ToString();

            if (_settingLanguagesDic.ContainsKey(item))
            {
                _settingLanguagesDic[item].BringToFront();
                _currentSettingLanguage = _settingLanguagesDic[item];
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_currentSettingLanguage == null) return;

            LanguageConfig lc = new LanguageConfig(_currentSettingLanguage.GetAux());

            SettingManager.LanguageConfigManager[AppRom.EnglishToCode[_currentLang]] = lc;

            string filePath = PathManager.SettingLanguage(AppRom.EnglishToCode[_currentLang]);

            string folderPath = Path.GetDirectoryName(filePath);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                CommonFunction.XmlWrite(lc, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
