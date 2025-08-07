using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingForm : Form
    {
        string CurrentFile;
        bool close_permition = true;
        List<SettingBase> settingBases = new List<SettingBase>();

        // 言語ごとのタブインデックス
        static Dictionary<string, int> LenguaTabIndex = new Dictionary<string, int> { { "es", 0 }, { "en", 1 } };

        public SettingForm(string currentFile)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            CurrentFile = currentFile;

            settingBases.Add(settingSpanish1);
            settingBases.Add(settingEnglish1);

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
            settingBases.ForEach(sb => sb.LoadConfig(CurrentFile));

            string select_lang = "";

            // 現在のファイルを捜索
            foreach (KeyValuePair<string, Dictionary<string, CommonConfig>> kvp in SettingManager.CommonConfigManager)
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
            tabControl1.SelectedIndex = LenguaTabIndex[select_lang];
        }

        private bool SaveConfig()
        {
            string cacheFile = "";
            QuizFileConfig common = new QuizFileConfig();
            LenguaConfig lengua = new LenguaConfig();

            int index = tabControl1.SelectedIndex;

            cacheFile = settingBases[index].SelectedFileName;
            common = settingBases[index].GetCommon();
            lengua = settingBases[index].GetLang();

            // 不可な設定検証
            if (common.Validation() == false) return false;

            CommonFunction.XmlWrite(common, $"{SettingManager.RomConfig.QuizFilePath}\\cache\\common\\{cacheFile}_common.xml");
            CommonFunction.XmlWrite(lengua, $"{SettingManager.RomConfig.QuizFilePath}\\cache\\lang\\{cacheFile}_lang.xml");

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

            mensaje_de_ayuda.Add("───────");
            mensaje_de_ayuda.Add($"Version    : {version.Major}.{version.Minor}");
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
