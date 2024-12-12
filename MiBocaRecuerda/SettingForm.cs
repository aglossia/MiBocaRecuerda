using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingForm : Form
    {
        Dictionary<string, Dictionary<string, QuizFileConfig>> ArchivosDeLengua;
        string CurrentFile;
        bool close_permition = true;

        public SettingForm(Dictionary<string, Dictionary<string, QuizFileConfig>> adl, string currentFile)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            ArchivosDeLengua = adl;
            CurrentFile = currentFile;

            Load += (o, e) =>
            {
                LoadConfig();
            };

            FormClosing += (o, e) =>
            {
                // 不可な設定によって閉じないようにする
                if (close_permition == false)
                {
                    close_permition = true;
                    e.Cancel = true;
                }
            };
        }

        private void LoadConfig()
        {
            settingSpanish1.LoadConfig(ArchivosDeLengua["es"], CurrentFile);
            settingEnglish1.LoadConfig(ArchivosDeLengua["en"], CurrentFile);

            string select_lang = "";

            // ArchivosDeLenguaから全部の情報を操作
            foreach (KeyValuePair<string, Dictionary<string, QuizFileConfig>> kvp in ArchivosDeLengua)
            {
                // valueのkeyがファイルパス
                foreach (string file in kvp.Value.Keys)
                {
                    // ファイルパスにひっかかった言語を抽出
                    if(Path.GetFileNameWithoutExtension(file) == CurrentFile)
                    {
                        select_lang = kvp.Key;
                    }
                }
            }

            // 指定ファイルの言語のタブに切り替える
            switch (select_lang)
            {
                case "es":
                    tabControl1.SelectedIndex = 0;
                    break;
                case "en":
                    tabControl1.SelectedIndex = 1;
                    break;
            }
        }

        private bool SaveConfig()
        {
            string cacheFile = "";
            QuizFileConfig lang = new QuizFileConfig();

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    cacheFile = settingSpanish1.SelectedFileName;
                    lang = settingSpanish1.GetLang();
                    break;
                case 1:
                    cacheFile = settingEnglish1.SelectedFileName;
                    lang = settingEnglish1.GetLang();
                    break;
            }

            // 不可な設定検証
            if (lang.Validation() == false) return false;

            CommonFunction.XmlWrite(lang, $"{SettingManager.RomConfig.QuizFilePath}\\cache\\{cacheFile}.xml");
            SettingManager.AppConfig.quizFileConfig = lang;
            CommonFunction.XmlWrite(SettingManager.AppConfig, "MBR.config");

            return true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if(SaveConfig() == false)
            {
                MessageBox.Show("Hay una entrada no válida");
                close_permition = false;

                return;
            }
        }

        private void btnAyudar_Click(object sender, EventArgs e)
        {
            List<string> mensaje_de_ayuda = new List<string>();

            mensaje_de_ayuda.Add("Comenzar un nuevo ejercicio: Ctrl + Q");
            mensaje_de_ayuda.Add("Mostrar la respuesta correcta: Ctrl + R");
            mensaje_de_ayuda.Add("Enviar tu respuesta: Shift + Enter");
            mensaje_de_ayuda.Add("Traducir lo que escribiste: F1");
            mensaje_de_ayuda.Add("Siguiente +: Ctrl + Shift + →");
            mensaje_de_ayuda.Add("Siguiente -: Ctrl + Shift + ←");

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName asmName = assembly.GetName();
            Version version = asmName.Version;

            mensaje_de_ayuda.Add("───────");
            mensaje_de_ayuda.Add($"Ver:{version.Major}.{version.Minor}");

            MessageForm s = new MessageForm(mensaje_de_ayuda, "AYUDA", MessageForm.TipoDeUbicacion.CENTRO, this, true)
            {
                ShowInTaskbar = false,
                ShowIcon = false
            };

            s.Show();
        }
    }
}
