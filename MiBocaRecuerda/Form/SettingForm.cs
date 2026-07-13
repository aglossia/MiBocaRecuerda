using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingForm : Form
    {
        private readonly List<SettingBase> _settingBases = new List<SettingBase>();
        private readonly Dictionary<string, SettingBase> _settings = new Dictionary<string, SettingBase>();
        private readonly Dictionary<string, bool> _validLanguage = new Dictionary<string, bool>() { { "es", false }, { "en", false } };
        private readonly Dictionary<string, string> _codeToLanguage = new Dictionary<string, string>() { { "es", "Spanish" }, { "en", "English" } };

        private SettingBase _currentSettingLaunguage => _settingBases[tabLanguage.SelectedIndex];
        private string _currentLanguage = "";

        public SettingForm()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            settingSpanish1.SomethingChanged += QuizConfigSomethingChanged;
            settingEnglish1.SomethingChanged += QuizConfigSomethingChanged;

            tabPageSpanish.Tag = settingSpanish1;
            tabPageEnglish.Tag = settingEnglish1;

            _settings["es"] = settingSpanish1;
            _settings["en"] = settingEnglish1;

            foreach (string lang in _settings.Keys)
            {
                if (SettingManager.CommonConfigManager.ContainsKey(lang))
                {
                    _settingBases.Add(_settings[lang]);
                    _validLanguage[lang] = true;
                }
                else
                {
                    var page = tabLanguage.TabPages[$"tabPage{_codeToLanguage[lang]}"];
                    tabLanguage.TabPages.Remove(page);
                }
            }

            if (tabLanguage.TabPages.Count < 1)
            {
                MessageBox.Show("有効な設定対象がありません");
                btnApply.Enabled = false;
            }

            Load += (o, e) =>
            {
                LoadConfig();
            };

            tabLanguage.SelectedIndexChanged += (o, e) =>
            {
                _currentLanguage = (tabLanguage.SelectedTab.Tag as SettingBase).LanguageName;

                btnApply.Enabled = _validLanguage[_currentLanguage];
            };

            _currentLanguage = (tabLanguage.SelectedTab.Tag as SettingBase).LanguageName;
        }

        private void LoadConfig()
        {
            _settingBases.ForEach(sb => sb.LoadConfig(SettingManager.CurrentQuizDB));

            string selectLang = "";

            // 現在のファイルを捜索
            foreach (KeyValuePair<string, Dictionary<string, CommonConfig>> kvp in SettingManager.CommonConfigManager)
            {
                // valueのkeyがファイルパス
                foreach (string file in kvp.Value.Keys)
                {
                    // ファイルパスにひっかかった言語を抽出
                    if (Path.GetFileNameWithoutExtension(file) == SettingManager.CurrentQuizDB)
                    {
                        selectLang = kvp.Key;
                    }
                }
            }

            // 指定ファイルの言語のタブに切り替える
            if (selectLang != "")
            {
                tabLanguage.SelectedIndex = AppRom.LenguaIndex[selectLang];
            }
            else
            {
                // 指定ファイルがないまま設定が開かれたときは操作不可にする
                _currentSettingLaunguage.ChangeEnabled(false);
                btnApply.Enabled = false;
            }
        }

        private void QuizConfigSomethingChanged(object o, EventArgs e)
        {
            // 不可な設定があれば保存しないようにする
            btnApply.Enabled = _currentSettingLaunguage.IsValid;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string cacheFile = _currentSettingLaunguage.SelectedFileName;
            QuizFileConfig common = SettingManager.CommonConfigManager[_currentLanguage][cacheFile].QuizFileConfig;
            FileLenguaConfig lengua = _currentSettingLaunguage.GetLang();

            common.Copy(_currentSettingLaunguage.GetCommon());

            if (cacheFile == null) return;

            CommonFunction.XmlWrite(common, PathManager.QuizFileSettingCommon(cacheFile));
            CommonFunction.XmlWrite(lengua, PathManager.QuizFileSettingLang(cacheFile));
        }

        private void btnAyudar_Click(object sender, EventArgs e)
        {
            List<string> mensaje_de_ayuda = new List<string>();

            string xml_s = @"<table>
                                  <tbody>
                                    <tr>
                                      <th>Comenzar un nuevo ejercicio</th>
                                      <td>Ctrl + Q</td>
                                      </tr>
                                    <tr>
                                      <th>Mostrar la respuesta correcta</th>
                                      <td>Ctrl + R</td>
                                      </tr>
                                    <tr>
                                      <th>Enviar tu respuesta</th>
                                      <td>Shift + Enter</td>
                                    </tr>
                                    <tr>
                                      <th>Traducir lo que escribiste</th>
                                      <td>F1</td>
                                    </tr>
                                    <tr>
                                      <th>Siguiente +</th>
                                      <td>Ctrl + Shift + N</td>
                                    </tr>
                                    <tr>
                                      <th>Siguiente -</th>
                                      <td>Ctrl + Shift + B</td>
                                    </tr>
                                  </tbody>
                                </table>";

            mensaje_de_ayuda.AddRange(ParseXML.ConvertTextWithTable(xml_s).Split('\n'));

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName asmName = assembly.GetName();
            Version version = asmName.Version;

            // メジャーバージョンとマイナーバージョンは手動指定のみ
            // ビルド番号は2000年1月1日からの経過日数
            // リビジョンはその日の00:00:00からの経過秒数/2
            string date = ConvertToDateTime(version.Build, version.Revision).ToString("yyyy/MM/dd HH:mm:ss");

            System.Diagnostics.FileVersionInfo fileVer =
                System.Diagnostics.FileVersionInfo.GetVersionInfo(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            mensaje_de_ayuda.Add("───────");
            mensaje_de_ayuda.Add($"Version    : {fileVer.FileVersion}");
            mensaje_de_ayuda.Add($"Build time : {date}");

            MessageForm s = new MessageForm(mensaje_de_ayuda, "AYUDA", MessageForm.TipoDeUbicacion.CENTRO, this, true)
            {
                ShowInTaskbar = false,
                ShowIcon = false
            };

            s.Show();
        }

        private DateTime ConvertToDateTime(int daysSince2000, int secondsOfDay)
        {
            // 基準日: 2000年1月1日
            DateTime baseDate = new DateTime(2000, 1, 1);

            // 経過日数を加算
            DateTime dateWithDays = baseDate.AddDays(daysSince2000);

            // 経過秒数を加算
            DateTime finalDateTime = dateWithDays.AddSeconds(secondsOfDay * 2);

            return finalDateTime;
        }
    }
}
