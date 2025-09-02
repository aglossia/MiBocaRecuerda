using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingLanguageForm : Form
    {
        private Dictionary<string, SettingLanguageBase> SettingLanguagesDic = new Dictionary<string, SettingLanguageBase>();
        private List<SettingLanguageBase> SettingLanguages = new List<SettingLanguageBase>();
        private SettingLanguageBase CurrentSettingLanguage;
        private string CurrentLang => cmbBoxSelectLang.SelectedItem?.ToString();

        public SettingLanguageForm(string currentLang)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            SettingLanguages.Add(settingLanguageSpanish1);
            SettingLanguages.Add(settingLanguageEnglish1);

            List<string> langs = new List<string> { "Spanish", "English" };

            for (int i = 0; i < Math.Min(langs.Count, SettingLanguages.Count); i++)
            {
                SettingLanguagesDic[langs[i]] = SettingLanguages[i];
            }

            if (currentLang == "") SettingLanguages.ForEach(sl => sl.Visible = false);
            SettingLanguages.ForEach(sl => sl.LoadConfig());

            cmbBoxSelectLang.Items.AddRange(langs.ToArray());
            if (currentLang != "") cmbBoxSelectLang.SelectedIndex = AppRom.LenguaIndex[currentLang];
        }

        private void cmbBoxSelectLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            SettingLanguages.ForEach(sl => sl.Visible = true);

            string item = cmbBoxSelectLang.SelectedItem.ToString();

            if (SettingLanguagesDic.ContainsKey(item))
            {
                SettingLanguagesDic[item].BringToFront();
                CurrentSettingLanguage = SettingLanguagesDic[item];
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (CurrentSettingLanguage == null) return;

            LanguageConfig lc = new LanguageConfig(CurrentSettingLanguage.GetAux());

            SettingManager.LanguageConfigManager[AppRom.EnglishToCode[CurrentLang]] = lc;

            string filePath = PathManager.SettingLanguage(AppRom.EnglishToCode[CurrentLang]);

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
