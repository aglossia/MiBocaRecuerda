using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class MainForm : ResizableForm
    {
        private ExerciseRepository ExerRepo;
        private List<Label> label_progress = new List<Label>();
        private List<Label> label_bar = new List<Label>();
        private NumericUpDown nudProgress;
        private List<List<AppRom.ProgressState>> progress_state = new List<List<AppRom.ProgressState>>();
        private List<string> respuestas = new List<string>();

        private Label lblNumericProgress;

        private ResultForm resultForm = new ResultForm();
        private MessageForm MessageForm_respuesta = new MessageForm();
        private MessageForm MessageForm_traducir = new MessageForm();
        private MessageForm MessageForm_quizInfo = new MessageForm();
        private MessageForm MessageForm_SectionList = new MessageForm();

        // Resultadoに表示する為に蓄積するやつ
        private List<QuizResult> QuizResult = new List<QuizResult>();
        // 現在の読込ファイルの設定
        public static QuizFileConfig QuizFileConfig;
        // 前回のクイズ設定
        private int preMinChapter;
        private int preMaxChapter;
        // 現在の問題集(InitQuizで作成)
        private List<QuizContents> QuizContents = new List<QuizContents>();
        // セクションリスト(InitQuizで作成)
        private List<string> SectionList = new List<string>();

        private bool IsLoaded = false;
        // 待機中かどうかは解答ボタンのEnabledで判断
        private bool IsIdle => !btnAnswer.Enabled;

        // 現在のクイズDB
        public static string CurrentQuizDB;
        // クイズファイルの最大行(設定オーバーを対応するため)
        private int QuizCountMax = 0;
        // 起動時のエラー情報
        private List<string> InitError = new List<string>();
        // 現在のクイズ言語
        private static string LangType = "";
        // 現在の問題のインデックス
        private int curProgress = -1;

        private int PruebaChallengeCount = -1;
        private Counter ErrorAllowCount = new Counter(-1);
        private Counter ErrorResetCount = new Counter(-1);

        // 答えの表を出すときの指定インデックス記憶用
        private int cacheDesde = -1;
        private int cacheHasta = -1;
        private bool cacheIsIndex = false;

        // タイトルバーのベースとなる文字列
        private string BaseTitle = "";

        // 言語ごとの入力補助を切り替える用
        public static Dictionary<string, IManageInput> ManageLanguage_Dic = new Dictionary<string, IManageInput>();

        public static IManageInput LangCtrl => ManageLanguage_Dic[LangType];

        //public ClassResize _form_resize;

        // ダークモード制御用
        private Dictionary<string, Color> preControlBackColor = new Dictionary<string, Color>();
        private Dictionary<string, Color> preControlForeColor = new Dictionary<string, Color>();

        #region DLL Import

        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool DestroyCaret();

        #endregion

        public MainForm()
        {
            InitializeComponent();

            DBTSMI_QuizDB.Enabled = false;

            RegisterEvent();

            #region デザイナを使わないイベント登録

            int labelSize = 18;

            // グループ切り替え
            for (int i = 0; i <= 10; i++)
            {
                Label l = new Label
                {
                    Location = new Point(txtAnswer.Location.X + (i % 10) * (labelSize + 1), txtAnswer.Location.Y + txtAnswer.Size.Height),
                    //Text = "―",
                    Size = new Size(labelSize, labelSize / 3),
                    Font = new Font("MeiryoKe_Console", 7F, FontStyle.Regular, GraphicsUnit.Point, 128),
                    Name = $"progress_group_label{i}"
                };

                //l.BorderStyle = BorderStyle.FixedSingle;
                l.TextAlign = ContentAlignment.MiddleCenter;
                l.BackColor = AppRom.ColorNeutral;
                l.Visible = false;

                l.Click += Label_bar_Click;
                l.MouseHover += Label_hover;
                l.MouseLeave += Label_leave;

                Controls.Add(l);
                label_bar.Add(l);
            }

            // 問題別
            for (int i = 0; i < 10; i++)
            {
                Label l = new Label
                {
                    Location = new Point(txtAnswer.Location.X + (i % 10) * (labelSize + 1), txtAnswer.Location.Y + txtAnswer.Size.Height + (labelSize / 3)),
                    //Text = progressStateCharacter_Neutral,
                    Size = new Size(labelSize, labelSize),
                    Font = new Font("MeiryoKe_Console", 9F, FontStyle.Regular, GraphicsUnit.Point, 128),
                    Name = $"progress_label{i}"
                };

                //l.BorderStyle = BorderStyle.FixedSingle;
                l.TextAlign = ContentAlignment.MiddleCenter;

                l.Click += LabelClick;
                l.Visible = false;

                Controls.Add(l);
                label_progress.Add(l);
            }

            lblNumericProgress = new Label
            {
                Location = new Point(txtAnswer.Location.X, txtAnswer.Location.Y + txtAnswer.Size.Height + 10),
                Text = "1000/1000",
                //Size = new Size(labelSize, labelSize),
                Font = new Font("MeiryoKe_Console", 9F, FontStyle.Regular, GraphicsUnit.Point, 128),
                Visible = false,
                Name = "NumericProgress"
            };

            Controls.Add(lblNumericProgress);

            nudProgress = new NumericUpDown();

            nudProgress.Location = new Point(label_bar[9].Location.X + 50, label_bar[9].Location.Y);
            nudProgress.Size = new Size(40, 20);
            nudProgress.Name = "hyper_group";
            nudProgress.Minimum = 0;
            nudProgress.Visible = false;

            nudProgress.ValueChanged += nud_ValueChanged;

            Controls.Add(nudProgress);

            ErrorAllowCount.PropertyChanged += ErrorCountPropertyChanged;
            ErrorResetCount.PropertyChanged += ErrorCountPropertyChanged;

            #endregion

            #region デザイナを使わないコントロールプロパティ設定

#if DEBUG
            Text += " [debug]";
            chboxDebug.Visible = true;
#endif

            lblResult.Visible = false;
            btnAnswer.Enabled = false;
            lbl_PruebaChallengeCount.Visible = false;
            //lbl_ErrorAllowCount.Visible = false;
            txtQuiz.ReadOnly = true;
            txtQuiz.BackColor = Color.White;

            optionTSMI_quizInfo.ShortcutKeys = Keys.Control | Keys.I;
            optionTSMI_prueba.ShortcutKeys = Keys.Control | Keys.P;
            optionTSMI_resultados.ShortcutKeys = Keys.Control | Keys.T;
            optionTSMI_progresoVisual.ShortcutKeys = Keys.Control | Keys.O;

            operationTSMI_start.ShortcutKeys = Keys.Control | Keys.Q;
            operationTSMI_siguiente.ShortcutKeys = Keys.Control | Keys.Shift | Keys.N;
            operationTSMI_anterior.ShortcutKeys = Keys.Control | Keys.Shift | Keys.B;
            operationTSMI_Undo_p.ShortcutKeys = Keys.Control | Keys.U;
            operationTSMI_Undo_e.ShortcutKeys = Keys.Control | Keys.Z;

            toolTSMI_prueba_Order.ShortcutKeys = Keys.Control | Keys.L;
            toolTSMI_translate.ShortcutKeys = Keys.Control | Keys.F1;

            DBTSMI_QuizDB.ShortcutKeys = Keys.Control | Keys.D;
            DBTSMI_Progress.ShortcutKeys = Keys.Control | Keys.G;

            resultForm.Dispose();
            MessageForm_respuesta.Dispose();
            MessageForm_traducir.Dispose();

            txtAnswer.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtAnswer.KeyDown += TextAnswerKeyDown;
            txtQuiz.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtConsole.KeyDown += TextBoxKeyDown_AvoidBeep;

            #endregion

            ManageLanguage_Dic["es"] = new Spanish();

            //_form_resize = new ClassResize(this);

            // 各コントロールの現在の色を保持
            foreach (Control ctrl in Controls)
            {
                preControlBackColor[ctrl.Name] = ctrl.BackColor;
                preControlForeColor[ctrl.Name] = ctrl.ForeColor;

                if (ctrl.GetType() == typeof(Panel))
                {
                    foreach (Control ctrl2 in (ctrl as Panel).Controls)
                    {
                        preControlBackColor[ctrl2.Name] = ctrl2.BackColor;
                        preControlForeColor[ctrl2.Name] = ctrl2.ForeColor;
                    }
                }

                if (ctrl.GetType() == typeof(MenuStrip))
                {
                    foreach (Control ctrl2 in (ctrl as MenuStrip).Controls)
                    {
                        preControlBackColor[ctrl2.Name] = ctrl2.BackColor;
                        preControlForeColor[ctrl2.Name] = ctrl2.ForeColor;
                    }
                }
            }

            LoadConfig();

            ParseFile();
        }

        #region 内部処理

        // クイズファイルの読み込み
        private void ParseFile()
        {
            string[] QuizFiles = Directory.GetFiles(PathManager.QuizDB, "*.db");

            QuizFiles = QuizFiles.Where(s => !Path.GetFileName(s).StartsWith('~'.ToString())).ToArray();

            ExerciseRepository exerRepo;
            string type = "";

            foreach (string file in QuizFiles)
            {
                try
                {
                    exerRepo = new ExerciseRepository($"Data Source={file}");

                    type = exerRepo.GetLanguage();

                    if (!SettingManager.CommonConfigManager.ContainsKey(type))
                    {
                        SettingManager.CommonConfigManager[type] = new Dictionary<string, CommonConfig>();
                    }
                }
                catch (Exception ex)
                {
                    InitError.Add($"{ex.GetType().Name};{ex.Message};{file}");
                }

                string fileName = Path.GetFileNameWithoutExtension(file);

                QuizFileConfig qfc = new QuizFileConfig();
                FileLenguaConfig lc = new FileLenguaConfig();

                // クイズキャッシュがある場合に、キャッシュを設定
                if (Directory.Exists(SettingManager.RomConfig.ResourcePath + "\\cache\\quiz"))
                {
                    // クイズ設定と言語設定のキャッシュを読み込んで共通設定を完成させる
                    string cacheFile_common = PathManager.QuizFileSettingCommon(fileName);
                    string cacheFile_lang = PathManager.QuizFileSettingLang(fileName);

                    try
                    {
                        if (File.Exists(cacheFile_common)) qfc = CommonFunction.XmlRead<QuizFileConfig>(cacheFile_common);
                        if (File.Exists(cacheFile_lang)) lc = CommonFunction.XmlRead<FileLenguaConfig>(cacheFile_lang);
                    }
                    catch (Exception ex)
                    {
                        InitError.Add($"{ex.GetType().Name};{ex.Message};{cacheFile_common} or {cacheFile_lang}");
                    }
                }

                // クイズ設定と言語設定の読み込み
                SettingManager.CommonConfigManager[type][fileName] = new CommonConfig(qfc, lc);
            }

            // 言語キャッシュがある場合に、キャッシュを設定
            if (Directory.Exists(SettingManager.RomConfig.ResourcePath + "\\cache\\language"))
            {
                string[] langFiles = new string[0];

                try
                {
                    langFiles = Directory.GetFiles(SettingManager.RomConfig.ResourcePath + "\\cache\\language", "*.xml");
                }
                catch (DirectoryNotFoundException ex)
                {
                    InitError.Add($"{ex.GetType().Name};{ex.Message};cache");
                }

                string lang;

                foreach (string file in langFiles)
                {
                    lang = Path.GetFileNameWithoutExtension(file);

                    if (!AppRom.LenguaIndex.ContainsKey(lang)) continue;

                    try
                    {
                        SettingManager.LanguageConfigManager[lang] = CommonFunction.XmlRead<LanguageConfig>(file);
                    }
                    catch (Exception ex)
                    {
                        InitError.Add($"{ex.GetType().Name};{ex.Message};{file};{lang}");
                    }
                }
            }
        }

        // 非表示記憶用
        private string tmp1 = "", tmp2 = "", tmp3 = "", tmp4 = "";
        private bool result = false;
        private bool ba = false;
        private bool isHide = false;
        private int selectionStart;
        private int selectionLength;

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);


        // 表示されてる文字を非表示にする
        private void HideText()
        {
            if (!isHide)
            {
                // 非表示

                selectionStart = txtAnswer.SelectionStart;
                selectionLength = txtAnswer.SelectionLength;

                tmp1 = txtQuiz.Text;
                txtQuiz.Text = "";

                tmp2 = txtAnswer.Text;
                txtAnswer.Text = "";

                tmp3 = txtConsole.Text;
                txtConsole.Text = "";

                tmp4 = Text;
                Text = "oculto";

                ba = btnAnswer.Enabled;
                btnAnswer.Enabled = false;
                btnShowAnswer.Enabled = false;
                optionTSMI_prueba.Enabled = false;
                optionTSMI_progresoVisual.Enabled = false;
                optionTSMI_resultados.Enabled = false;

                if (optionTSMI_DarkMode.Checked == false)
                {
                    txtQuiz.BackColor = SystemColors.Control;
                    txtAnswer.BackColor = SystemColors.Control;
                    txtConsole.BackColor = SystemColors.Control;
                }

                if (resultForm.IsDisposed == false)
                {
                    resultForm.Visible = false;
                    result = true;
                }

                HideCaret(txtAnswer.Handle);
            }
            else
            {
                // 表示

                txtQuiz.Text = tmp1;
                txtAnswer.Text = tmp2;
                txtAnswer.Select(txtAnswer.Text.Length, 0);
                txtConsole.Text = tmp3;
                Text = tmp4;

                txtAnswer.SelectionStart = selectionStart;
                txtAnswer.SelectionLength = selectionLength;

                btnAnswer.Enabled = ba;
                btnShowAnswer.Enabled = true;
                optionTSMI_prueba.Enabled = true;
                optionTSMI_progresoVisual.Enabled = true;
                optionTSMI_resultados.Enabled = true;

                if (optionTSMI_DarkMode.Checked == false)
                {
                    txtQuiz.BackColor = Color.White;
                    txtAnswer.BackColor = Color.White;
                    txtConsole.BackColor = Color.White;
                }

                if (resultForm.IsDisposed == false) resultForm.Visible = result;
                txtAnswer.Focus();

                ShowCaret(txtAnswer.Handle);
            }

            txtAnswer.ReadOnly = !txtAnswer.ReadOnly;
            txtConsole.ReadOnly = !txtConsole.ReadOnly;

            isHide = !isHide;
        }

        // MBRの初期設定
        private void LoadConfig()
        {
            string[] QuizFiles;

            if (File.Exists("rom.config"))
            {
                SettingManager.RomConfig = CommonFunction.XmlRead<RomConfig>("rom.config");
            }

            QuizFiles = Directory.GetFiles(PathManager.QuizDB, "*.db");

            toolStripQuizFile.Items.AddRange(QuizFiles
                .Where(s => !Path.GetFileName(s).StartsWith('~'.ToString()))
                .Select(s => Path.GetFileNameWithoutExtension(s)).ToArray());

            if (File.Exists("cache.xml"))
            {
                SettingManager.InputCache = CommonFunction.XmlRead<InputCache>("cache.xml");
            }

            optionTSMI_prueba.Checked = SettingManager.InputCache.Complete;
            optionTSMI_progresoVisual.Checked = SettingManager.InputCache.Exercise;
            optionTSMI_resultados.Checked = SettingManager.InputCache.Result;

            if (toolStripQuizFile.Items.Contains(SettingManager.InputCache.QuizFileName))
            {
                toolStripQuizFile.SelectedItem = SettingManager.InputCache.QuizFileName;
            }

            optionTSMI_DarkMode.Checked = SettingManager.InputCache.DarkMode;
        }

        private int preLastQuiz = -1;
        public string CurrentQuizDBPath = "";

        // クイズ開始
        private void InitQuiz(bool manual)
        {
            // 非表示中はクイズを始めない
            if (isHide)
            {
                MessageBox.Show("No se puede continuar con la prueba mientras está oculto", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (toolStripQuizFile.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un archivo.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 前回の言語の補助入力を解除
            if (ManageLanguage_Dic.ContainsKey(LangType))
            {
                txtAnswer.KeyPress -= ManageLanguage_Dic[LangType].KeyPress;
            }

            txtAnswer.Focus();
            btnAnswer.Enabled = true;

            CurrentQuizDB = toolStripQuizFile.SelectedItem.ToString();
            CurrentQuizDBPath = $"{PathManager.QuizDB}\\{CurrentQuizDB}.db";

            // 問題集DBを読み込む
            ExerRepo = new ExerciseRepository($"Data Source={CurrentQuizDBPath}");

            LangType = ExerRepo.GetLanguage();
            QuizCountMax = ExerRepo.GetExerciseCount();
            SectionList = ExerRepo.GetAllSection();

            QuizFileConfig = SettingManager.CommonConfigManager[LangType][CurrentQuizDB].QuizFileConfig;

            // 新しい言語の補助入力を登録
            if (ManageLanguage_Dic.ContainsKey(LangType))
            {
                txtAnswer.KeyPress += ManageLanguage_Dic[LangType].KeyPress;
            }

            if (manual) txtConsole.Text = "";
            curProgress = -1;
            correctAnswerNum = 0;
            QuizResult.Clear();
            QuizContents.Clear();
            respuestas.Clear();
            ErrorResetCount.Cnt = 0;
            // ErrorAllowCount,ErrorResetCountの表示に関わっているものはErrorAllowCountのプロパティを変化する前に変化させておく必要がある
            ErrorAllowCount.Cnt = 0;

            // 進捗表示作成
            CreateQuizProgress();

            // nからmまでの整数のリストを作成
            List<int> numberList = new List<int>();
            for (int i = QuizFileConfig.MinChapterToIndex; i <= QuizFileConfig.MaxChapterToIndex; i++)
            {
                numberList.Add(i);
            }

            // リストをシャッフルしてランダムな数列を作成
            List<int> randomSequence;

            do
            {
                // 最後に表示していた問題が、次の最初の問題になるとシャッフルをやり直す
                randomSequence = UtilityFunction.ShuffleList(numberList);
            }
            while (randomSequence[0] == preLastQuiz);

            QuizContents = CreateQuizContents(randomSequence);

            RefreshDisplay();

            ShowQuestion();

            // 今回のクイズ設定を保持
            preMinChapter = QuizFileConfig.MinChapter;
            preMaxChapter = QuizFileConfig.MaxChapter;
        }

        // インデックスリストから問題を取得する
        private List<QuizContents> CreateQuizContents(List<int> indexList)
        {
            List<QuizContents> quizContents = new List<QuizContents>();

            foreach (int index in indexList)
            {
                // DBが取得できなかった場合は設定しない
                if (ExerRepo.GetByNum(index) is ExerciseDB edb)
                {
                    quizContents.Add(new QuizContents(edb));
                }
            }

            return quizContents;
        }

        private void RefreshDisplay()
        {
            // 前回とクイズ設定が違っていたらチャレンジ回数を初期化する
            if ((preMinChapter != QuizFileConfig.MinChapter) ||
                (preMaxChapter != QuizFileConfig.MaxChapter))
            {
                PruebaChallengeCount = 0;
            }
            else
            {
                // pruebaモードの時だけ
                if (optionTSMI_prueba.Checked)
                {
                    PruebaChallengeCount++;
                }
            }

            lbl_PruebaChallengeCount.Text = $"Try: {PruebaChallengeCount}";
            lbl_PruebaChallengeCount.Visible = optionTSMI_prueba.Checked;

            // POR HACER:settingで切り替える
            //lbl_ErrorAllowCount.Visible = false;

            string baseTitle = $"MBR [{QuizFileConfig.MinChapterToIndex}~{QuizFileConfig.MaxChapterToIndex}]";

            // pruebaモードのとき
            if (optionTSMI_prueba.Checked)
            {
                if (QuizFileConfig.ErrorAllowCnt > 0)
                {
                    lbl_ErrorAllowCount.Visible = true;
                }

                // 練習が1章だけならPRUEBA回数を表示する
                if (QuizFileConfig.MinChapter == QuizFileConfig.MaxChapter)
                {
                    string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{CurrentQuizDB}_p.csv";

                    if (File.Exists(path))
                    {
                        string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                        // prueba回数
                        baseTitle += $" [PR {int.Parse(lines[QuizFileConfig.MinChapter - 1].Split(',')[1]).ToString()}]";
                        // 最近のprueba日
                        baseTitle += $" {lines[QuizFileConfig.MinChapter - 1].Split(',')[0].Substring(2)}";
                    }
                }
            }

            BaseTitle = baseTitle;
        }

        // OKとかNGとかを表示させる
        private async void DisplayResult(string mensaje, int time)
        {
            lblResult.Text = mensaje;
            lblResult.Visible = true;

            CancellationToken token;
            lock (lockObject)
            {
                cts.Cancel();
                cts = new CancellationTokenSource();
                token = cts.Token;
            }

            try
            {
                await Task.Delay(time, token);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            lblResult.Visible = false;
        }

        // 問題を表示する
        private void ShowQuestion()
        {
            // 現在の問題のインデックスを進める
            curProgress++;

            // タイトル更新
            Text = $"{BaseTitle} {QuizContents[curProgress].Section}";

            preLastQuiz = QuizContents[curProgress].QuizNum;

            if ((QuizFileConfig.ErrorAllowAll == false) && (QuizFileConfig.ErrorReset == true))
            {
                // ミス許容が全体ではないときに問題ごとのミスを初期化する
                ErrorAllowCount.Cnt = 0;
            }

            // 進捗ビジュアルモード
            if (optionTSMI_progresoVisual.Checked)
            {
                progress_state[UtilityFunction.Suelo(curProgress, 10)][curProgress % 10] = AppRom.ProgressState.CurrentQuiz;

                RedrawProgress(curProgress);
            }
            else
            {
                int totalNum = QuizFileConfig.MaxQuizNum > QuizCountMax ? QuizCountMax : QuizFileConfig.MaxQuizNum;
                lblNumericProgress.Text = $"{curProgress + 1}/{QuizFileConfig.QuizNum}";
            }

            txtQuiz.Text = QuizContents[curProgress].Quiz;

            if (MessageForm_quizInfo.Visible == true)
            {
                // 同じ処理がTSMI_quizInfoにあるので冗長
                List<string> input_h = new List<string>() { "Quiz Number", "Quiz Title" };
                List<string> input_d = new List<string>() { QuizContents[curProgress].QuizNum.ToString(), QuizContents[curProgress].Section };
                List<string> quizInfo = new List<string>();

                string xml_s = UtilityFunction.GenerateXmlTable(input_h, input_d);

                quizInfo.AddRange(ParseXML.ConvertTextWithTable(xml_s).Split('\n'));

                MessageForm_quizInfo.MessageUpdate(quizInfo);
            }
        }

        // 進捗表示を作る
        private void CreateQuizProgress()
        {
            // 進捗ビジュアルモード
            if (optionTSMI_progresoVisual.Checked)
            {
                lblNumericProgress.Visible = false;

                current_bar_index = 0;
                label_bar[0].BackColor = AppRom.ColorCurrentGroup;

                int nudSize = UtilityFunction.Suelo(QuizFileConfig.QuizNum - 1, 100);

                nudProgress.Maximum = nudSize;
                nudProgress.Visible = nudSize == 0 ? false : true;

                progress_state = new List<List<AppRom.ProgressState>>(
                        new List<int>[UtilityFunction.Techo(QuizFileConfig.QuizNum, 10)]
                            .Select(_ => new List<AppRom.ProgressState>(new AppRom.ProgressState[10]))
                    );

                RedrawProgress(0);
            }
            else
            {
                label_progress.ForEach(l1 => l1.Visible = false);
                label_bar.ForEach(l1 => l1.Visible = false);
                nudProgress.Visible = false;
                lblNumericProgress.Visible = true;
            }
        }

        // 進捗表示を更新する
        private void RedrawProgress(int progress_num)
        {
            int hyper_index = UtilityFunction.Suelo(progress_num, 100);

            nudProgress.Value = hyper_index;

            current_bar_index = UtilityFunction.GetNDigit(progress_num, 2);

            // hyper group(100~)とbar index(10の位)の差をとって進捗ラベルをどこまで表示するか
            int progSize = QuizFileConfig.QuizNum - ((int)nudProgress.Value * 100 + current_bar_index * 10);

            // hyper groupが最上位にいっているかを調べる
            int barSize = UtilityFunction.Techo(QuizFileConfig.QuizNum - ((int)nudProgress.Value * 100), 10);

            // 進捗ラベルを指定箇所まで表示する
            label_progress.Select((label, index) => new { label, index })
                        .ToList()
                        .ForEach(item => item.label.Visible = item.index < progSize);

            // バーラベルを指定箇所まで表示する
            label_bar.Select((label, index) => new { label, index })
                        .ToList()
                        .ForEach(item => item.label.Visible = item.index < barSize);

            // バーラベルを選択したやつは選択色に変えてそれ以外は未選択色
            label_bar.Select((label, index) => new { label, index })
                      .ToList()
                      .ForEach(item => item.label.BackColor = (current_bar_index != item.index) ? Color.LightBlue : Color.Turquoise);

            string chara = "";

            for (int cnt = 0; cnt < 10; cnt++)
            {
                switch (progress_state[(int)nudProgress.Value * 10 + current_bar_index][cnt])
                {
                    case AppRom.ProgressState.Neutral:
                        chara = AppRom.ProgressStateCharacter_Neutral;
                        break;
                    case AppRom.ProgressState.Correct:
                        chara = AppRom.ProgressStateCharacter_Correct;
                        break;
                    case AppRom.ProgressState.Incorrect:
                        chara = AppRom.ProgressStateCharacter_Incorrect;
                        break;
                    case AppRom.ProgressState.CurrentQuiz:
                        chara = AppRom.ProgressStateCharacter_CurrentQuiz;
                        break;
                }

                label_progress[cnt].Text = chara;
                //label_progress[cnt].ForeColor = chara == progressStateCharacter_CurrentQuiz ? colorOnProgress : colorOffProgress;
            }
        }

        // Siguiente制御
        private void MoveQuiz(bool isForward)
        {
            if (QuizFileConfig == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int diff = QuizFileConfig.MaxChapter - QuizFileConfig.MinChapter + 1;

            if (isForward)
            {
                // 最小章がMAX数を超えていたら何もすることがない
                if (QuizFileConfig.MinChapter + diff > QuizCountMax / 10) return;

                if (QuizFileConfig.MaxChapter + diff > QuizCountMax / 10)
                {
                    // 最大章がMAX数を超えていたら最大章をMAX数にする
                    QuizFileConfig.MinChapter += diff;
                    QuizFileConfig.MaxChapter = QuizCountMax / 10;
                }
                else
                {
                    // 基準の差分分を順シフトする
                    QuizFileConfig.MinChapter += diff;
                    QuizFileConfig.MaxChapter += diff;
                }
            }
            else
            {
                // 最大章が0以下だと何もすることがない
                if (QuizFileConfig.MaxChapter - diff < 1) return;

                if (QuizFileConfig.MinChapter - diff < 1)
                {
                    // 最大章が0以下だと最小の1にする
                    QuizFileConfig.MinChapter = 1;
                    QuizFileConfig.MaxChapter -= diff;
                }
                else
                {
                    // 基準の差分分を逆シフトする
                    QuizFileConfig.MinChapter -= diff;
                    QuizFileConfig.MaxChapter -= diff;
                }
            }

            InitQuiz(true);
        }

        // 正誤表表示
        private void ShowFeDeErratas(int progNum)
        {
            if (progNum > 10) return;

            int bar_index = label_bar.FindIndex(label => label.BackColor == AppRom.ColorCurrentGroup);
            int quizNum = (int)nudProgress.Value * 100 + bar_index * 10 + progNum;

            if (respuestas.Count <= quizNum || quizNum < 0) return;

            List<string> tmp = new List<string>();

            string answer = "";

            foreach (KeyValuePair<string, List<Answer>> kvp in QuizContents[quizNum].CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    answer += $"{kvp.Key}:{ans.Sentence}\n";
                }
            }

            tmp.Add(answer);
            tmp.Add("───────");
            tmp.Add(respuestas[quizNum]);

            MessageForm s = new MessageForm(tmp, "FE DE ERRATAS", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            s.Show();
        }

        // 進捗ファイルひな形作成
        private void CreateNewProgressFile()
        {
            string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{CurrentQuizDB}_p.csv";
            DateTime defaultDate = new DateTime(1970, 1, 1);

            // ファイル作成 & 書き込み
            using (StreamWriter writer = new StreamWriter(path, false)) // false = 上書き
            {
                foreach (string chapter in SectionList)
                {
                    writer.WriteLine($"{defaultDate.ToString("yyyy/MM/dd")},000,{chapter}");
                }
            }
        }

        #endregion

        #region 登録用イベント

        private void _CaretWidthChange(object o, EventArgs e)
        {
            TextBox t = o as TextBox;

            // キャレットの幅と高さを指定
            int caretWidth = 5; // キャレットの幅を太く設定
            int caretHeight = t.Font.Height;

            // キャレットを作成
            CreateCaret(t.Handle, IntPtr.Zero, caretWidth, caretHeight);

            // キャレットを表示
            ShowCaret(t.Handle);
        }

        // テキストボックスフォーカス中のグローバルショートカットでビープ音が出るのを防ぐ
        private void TextBoxKeyDown_AvoidBeep(object o, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Q:
                    case Keys.R:
                        e.SuppressKeyPress = true;
                        break;
                }
            }
        }

        private void TextAnswerKeyDown(object o, KeyEventArgs e)
        {
            if (e.Control)
            {
                int selectionStart = txtAnswer.SelectionStart;
                string insertText = "";

                switch (e.KeyCode)
                {
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:

                        // Dn
                        int num = (int.Parse(e.KeyCode.ToString()[1].ToString()) + 9) % 10;

                        if (e.Shift)
                        {
                            // 言語ごとの補助入力
                            insertText = SettingManager.LanguageConfigManager[LangType].InputSupport[num];
                        }
                        else
                        {
                            // ファイルごとの補助入力
                            if (QuizContents[curProgress].AutoNombre.Count <= num) return;

                            insertText = QuizContents[curProgress].AutoNombre[num];
                        }

                        break;
                }

                if (insertText != "")
                {
                    txtAnswer.Text = txtAnswer.Text.Insert(selectionStart, insertText);
                    txtAnswer.SelectionStart = selectionStart + insertText.Length;
                    e.SuppressKeyPress = true; // 元のキー入力をキャンセル
                }
            }
        }

        private void LabelClick(object sender, EventArgs e)
        {
            int bar_index = label_bar.FindIndex(label => label.BackColor == AppRom.ColorCurrentGroup);
            int progress_index = label_progress.IndexOf(sender as Label);
            int quizNum = (int)nudProgress.Value * 100 + bar_index * 10 + progress_index;

            if (respuestas.Count <= quizNum) return;

            List<string> tmp = new List<string>();

            string answer = "";

            foreach (KeyValuePair<string, List<Answer>> kvp in QuizContents[quizNum].CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    answer += $"{kvp.Key}:{ans.Sentence}\n";
                }
            }

            tmp.Add(answer);
            tmp.Add("───────");
            tmp.Add(respuestas[quizNum]);

            MessageForm s = new MessageForm(tmp, "FE DE ERRATAS", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            s.Show();
        }

        private int current_bar_index = 0;

        private void Label_bar_Click(object sender, EventArgs e)
        {
            int bar_idx = label_bar.IndexOf(sender as Label);

            // バーラベルを選択したやつは選択色に変えてそれ以外は未選択色
            label_bar.Select((label, index) => new { label, index })
                      .ToList()
                      .ForEach(item => item.label.BackColor = (bar_idx != item.index) ? Color.LightBlue : Color.Turquoise);

            RedrawProgress((int)nudProgress.Value * 100 + bar_idx * 10);
        }

        private void Label_hover(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = AppRom.ColorHover;
        }

        private void Label_leave(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = label_bar[current_bar_index] == l ? AppRom.ColorCurrentGroup : AppRom.ColorNeutral;
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            label_bar.ForEach(l => l.BackColor = Color.LightBlue);

            int hyper_group = (int)nudProgress.Value * 100;

            RedrawProgress(hyper_group);
        }

        // txtAnswer KeyPressの全言語共通イベント
        private void txtAnswer_KeyPress_All(object o, KeyPressEventArgs e)
        {
            // シフトキー（Shift）が押されているかを確認
            bool shiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;

            bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

            bool escPressed = e.KeyChar == (char)Keys.Escape;

            // エンターキー（Enter）が押されているかを確認
            bool enterPressed = e.KeyChar == (char)Keys.Enter;

            // シフトキーとエンターキーが同時に押されたかを確認
            if (shiftPressed && enterPressed)
            {
                e.Handled = true;
                btnAnswer.PerformClick();
            }

            if (escPressed)
            {
                HideText();

                e.Handled = true;
            }
        }

        // エラーカウントの表示更新イベント
        private void ErrorCountPropertyChanged(object o, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Counter.Cnt))
            {
                lbl_ErrorAllowCount.Text = $"{ErrorAllowCount.Cnt}/{QuizFileConfig.ErrorAllowCnt}";
                if (QuizFileConfig.ErrorAllowAll)
                {
                    lbl_ErrorAllowCount.Text = $"Todo[{ErrorResetCount.Cnt}]: {lbl_ErrorAllowCount.Text}";
                }
            }
        }

        private void RegisterEvent()
        {
            #region Form

            Load += (o, e) =>
            {
                if (InitError.Count != 0)
                {
                    MessageForm s = new MessageForm(InitError, "Load error", MessageForm.TipoDeUbicacion.CENTRO, this, true, true, true)
                    {
                        ShowIcon = false
                    };

                    s.Show();
                }
            };

            SizeChanged += (o, e) =>
            {
                _form_resize._resize(false);
            };

            KeyDown += (o, e) =>
            {
                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                bool shiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;

                if (ctrlPressed)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.NumPad0:
                        case Keys.NumPad1:
                        case Keys.NumPad2:
                        case Keys.NumPad3:
                        case Keys.NumPad4:
                        case Keys.NumPad5:
                        case Keys.NumPad6:
                        case Keys.NumPad7:
                        case Keys.NumPad8:
                        case Keys.NumPad9:

                            // KeyCodeをToStringすると"NumPadn"がでてくるから7文字目を取ってcharからstringにして
                            // 9+してmod10したら1+9 mod 10 =0だし0+9 mod 10 = 9になる
                            int num = (int.Parse(e.KeyCode.ToString()[6].ToString()) + 9) % 10;

                            ShowFeDeErratas(num);

                            break;
                        case Keys.R:
                            // Respuesta
                            btnShowAnswer.PerformClick();
                            break;
                    }
                }
            };

            KeyUp += (o, e) =>
            {
            };

            Shown += (o, e) =>
            {
                IsLoaded = true;
            };

            FormClosing += (o, e) =>
            {
                SettingManager.InputCache.Complete = optionTSMI_prueba.Checked;
                SettingManager.InputCache.Exercise = optionTSMI_progresoVisual.Checked;
                SettingManager.InputCache.Result = optionTSMI_resultados.Checked;
                SettingManager.InputCache.QuizFileName = toolStripQuizFile.SelectedItem?.ToString();
                SettingManager.InputCache.DarkMode = optionTSMI_DarkMode.Checked;

                CommonFunction.XmlWrite(SettingManager.InputCache, "cache.xml");
            };

            #endregion

            #region OtherControl

            txtAnswer.KeyPress += txtAnswer_KeyPress_All;

            txtAnswer.LostFocus += (o, e) =>
            {
                DestroyCaret();
            };

            btnShowAnswer.MouseDown += (o, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    InitQuiz(true);
                }
            };

            #endregion

            #region TSMI

            optionTSMI_prueba.CheckedChanged += (o, e) =>
            {
                if (!IsLoaded) return;
                PruebaChallengeCount = -1;
                InitQuiz(true);
            };

            optionTSMI_progresoVisual.CheckedChanged += (o, e) =>
            {
                if (!IsLoaded) return;
                InitQuiz(true);
            };

            optionTSMI_DarkMode.CheckedChanged += (o, e) =>
            {
                bool ch = (o as ToolStripMenuItem).Checked;

                Color baseColor = Color.FromArgb(80, 80, 80);
                Color textBackColor = Color.FromArgb(60, 60, 60);

                if (ch)
                {
                    txtAnswer.GotFocus += _CaretWidthChange;
                    txtAnswer.FontChanged += _CaretWidthChange;

                    BackColor = baseColor;

                    foreach (Control ctrl in Controls)
                {
                        if (ctrl.GetType() == typeof(Button))
                        {
                            ctrl.BackColor = Color.Gray;
                            ctrl.ForeColor = Color.White;
                        }
                        else if (ctrl.GetType() == typeof(TextBox))
                        {
                            ctrl.BackColor = textBackColor;
                            ctrl.ForeColor = Color.White;
                        }
                        else if (ctrl.GetType() == typeof(Label))
                        {
                            ctrl.BackColor = baseColor;
                            ctrl.ForeColor = Color.White;
                        }
                        else if (ctrl.GetType() == typeof(ToolStrip))
                        {
                            ctrl.BackColor = Color.Black;
                            ctrl.ForeColor = Color.White;
                        }
                        else if (ctrl.GetType() == typeof(MenuStrip))
                        {
                            ctrl.BackColor = baseColor;
                            ctrl.ForeColor = Color.White;

                            foreach (Control ctrl2 in (ctrl as MenuStrip).Controls)
                            {
                                ctrl2.BackColor = Color.Gray;
                                ctrl2.ForeColor = Color.White;
                            }
                        }
                        else if (ctrl.GetType() == typeof(ToolStripComboBox))
                        {
                            ctrl.BackColor = Color.Gray;
                            ctrl.ForeColor = Color.White;
                        }
                        else if (ctrl.GetType() == typeof(Panel))
                        {
                            ctrl.BackColor = baseColor;

                            foreach (Control ctrl2 in (ctrl as Panel).Controls)
                            {
                                ctrl2.BackColor = textBackColor;
                                ctrl2.ForeColor = Color.White;
                            }
                        }
                        else
                        {
                            ctrl.BackColor = baseColor;
                        }
                    }
                    }
                    else
                    {
                        txtAnswer.GotFocus -= _CaretWidthChange;
                        txtAnswer.FontChanged -= _CaretWidthChange;

                    BackColor = SystemColors.Control;

                    foreach (Control ctrl in Controls)
                    {
                        if (ctrl.GetType() == typeof(ToolStrip))
                        {

                        }
                        ctrl.BackColor = preControlBackColor[ctrl.Name];
                        ctrl.ForeColor = preControlForeColor[ctrl.Name];

                        if (ctrl.GetType() == typeof(Panel))
                        {
                            foreach (Control ctrl2 in (ctrl as Panel).Controls)
                            {
                                ctrl2.BackColor = preControlBackColor[ctrl2.Name];
                                ctrl2.ForeColor = preControlForeColor[ctrl2.Name];
                            }
                        }

                        if (ctrl.GetType() == typeof(MenuStrip))
                        {
                            foreach (Control ctrl2 in (ctrl as MenuStrip).Controls)
                            {
                                ctrl2.BackColor = preControlBackColor[ctrl2.Name];
                                ctrl2.ForeColor = preControlForeColor[ctrl2.Name];
                            }
                        }
                    }
                    }
                };

            operationTSMI.MouseDown += (o, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    HideText();
                }
            };

            #endregion
        }

        #endregion

        #region イベント

        // 正解数
        private int correctAnswerNum = 0;

        CancellationTokenSource cts = new CancellationTokenSource();
        static object lockObject = new object();

        // 解答ボタンクリック(responder)
        private void btnAnswer_Click(object sender, EventArgs e)
        {
            if (ExerRepo == null) return;

            cts.Cancel();

            txtConsole.Text = "";

            // POR HACER:20260106:region指定でやるモードも検討
            var check = CoreProcess.CheckAnswer(txtAnswer.Text, QuizContents[curProgress].Answers().ToList());

#if DEBUG
            if (chboxDebug.Checked) check.isCorrect = true;
#endif

            DisplayResult(check.isCorrect ? "¡Sí!" : "¡No!", 1000);

            txtConsole.Text = check.adopt_str;

            IsFirstMistake = false;

            if (check.isCorrect)
            {
                correctAnswerNum++;
            }
            else
            {
                if (optionTSMI_prueba.Checked)
                {
                    // pruebaモード

                    if (QuizFileConfig.ErrorAllowCnt > 0)
                    {
                        // ミス許容が設定されているとき

                        if (ErrorAllowCount.Cnt < QuizFileConfig.ErrorAllowCnt)
                        {
                            // ミス許容未満のあいだはミス数を加算してやり直し
                            ErrorAllowCount.Cnt++;
                            return;
                        }
                        else
                        {
                            // ミス許容全体はリセットカウント進める
                            if (QuizFileConfig.ErrorAllowAll)
                            {
                                ErrorResetCount.Cnt++;
                            }

                            // ミス許容リセットのときはミス数リセットする
                            if (QuizFileConfig.ErrorReset)
                            {
                                ErrorAllowCount.Cnt = 0;
                            }

                            IsFirstMistake = true;
                        }
                    }
                }
                else
                {
                    if (ErrorAllowCount.Cnt < QuizFileConfig.ErrorAllowCnt)
                    {
                        ErrorAllowCount.Cnt++;
                    }
                    else
                    {
                        ErrorResetCount.Cnt++;
                        ErrorAllowCount.Cnt = 0;
                    }

                    // 完答モードの時はやり直し
                    return;
                }
            }

            if (optionTSMI_progresoVisual.Checked)
            {
                label_progress[curProgress % 10].Text = check.isCorrect ? AppRom.ProgressStateCharacter_Correct : AppRom.ProgressStateCharacter_Incorrect;
                //label_progress[curProgress % 10].ForeColor = colorOffProgress;
                progress_state[UtilityFunction.Suelo(curProgress, 10)][curProgress % 10] = check.isCorrect ? AppRom.ProgressState.Correct : AppRom.ProgressState.Incorrect;
            }

            // 解答を保存
            respuestas.Add(txtAnswer.Text == "" ? "NONE" : txtAnswer.Text);
            txtAnswer.Text = "";

            QuizResult.Add(new QuizResult(QuizContents[curProgress].Quiz, QuizContents[curProgress].CorrectAnswer, txtAnswer.Text, QuizContents[curProgress].QuizNum, QuizContents[curProgress].Supplement, check.isCorrect));


            int endQuizNum = optionTSMI_progresoVisual.Checked ? QuizFileConfig.QuizNum - 1 : QuizFileConfig.MaxQuizNum - 1;

            // クイズ終了？
            if (curProgress == endQuizNum || curProgress == QuizCountMax - 1)
            {
                //tokenSource.Cancel();

                btnAnswer.Enabled = false;

                // 問題数と正解問題数が同じでpruebaモードのとき
                if ((endQuizNum + 1 == correctAnswerNum) && optionTSMI_prueba.Checked)
                {
                    DisplayResult("PERFECTO!", 5000);

                    // 綺麗な対処ではないが、のちのRefreshDisplayで++される使用のためここで調整
                    // PERFECTOしたあとは最終回数を表示していたい
                    PruebaChallengeCount--;

                    // チャプター数
                    int chapterNum = QuizFileConfig.MaxChapter - QuizFileConfig.MinChapter + 1;

                    // チャプター数と、それに対応するクイズ数が一致しているときは進捗を記録する
                    if (chapterNum * 10 == QuizFileConfig.QuizNum)
                    {
                        string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{CurrentQuizDB}_p.csv";

                        if (File.Exists(path) == false)
                        {
                            // 進捗ファイルがないときひな形を作成する
                            CreateNewProgressFile();
                        }

                        // 進捗ファイルに書き込む

                        // チャプター毎に進捗を更新する
                        for (int cnt = 0; cnt < chapterNum; cnt++)
                        {
                            string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                            string[] sp = lines[QuizFileConfig.MinChapter - 1 + cnt].Split(',');
                            string today = DateTime.Now.ToString("yyyy/MM/dd");

                            // 同日のPruebaは記録しない
                            // 日跨ぎのPruebaを重視するため(Ebbinghaus)
                            if (sp[0] != today)
                            {
                                sp[0] = today;
                                sp[1] = (int.Parse(sp[1]) + 1).ToString("D3");
                                lines[QuizFileConfig.MinChapter - 1 + cnt] = string.Join(",", sp);

                                File.WriteAllLines(path, lines);
                            }
                        }

                        // 練習が複数の章にわたるときは、どこからどこまでかを記録する
                        if (chapterNum > 1)
                        {
                            string path_i = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{CurrentQuizDB}_intercontinental.txt";
                            string write_text = $"{QuizFileConfig.MinChapter}~{QuizFileConfig.MaxChapter}";

                            if (File.Exists(path_i))
                            {
                                using (StreamWriter sw = File.AppendText(path_i))
                                {
                                    sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd")}:{write_text}");
                                }
                            }
                            else
                            {
                                using (StreamWriter sw = File.CreateText(path_i))
                                {
                                    sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd")}:{write_text}");
                                }
                            }
                        }
                    }

                    RefreshDisplay();
                }
                else
                {
                    DisplayResult("¡Buen trabajo!", 5000);
                }

                if (optionTSMI_resultados.Checked)
                {
                    if (resultForm.IsDisposed == false) resultForm.Dispose();

                    resultForm = new ResultForm(QuizResult, this)
                    {
                        ShowIcon = false
                    };

                    resultForm.Show();
                }

                return;
            }

            ShowQuestion();
        }

        // 正解を表示(respuesta)
        private void btnShowAnswer_Click(object sender, EventArgs e)
        {
            if (MessageForm_respuesta.IsDisposed == false) MessageForm_respuesta.Dispose();
            if (ExerRepo == null) return;

            List<string> processedAnswer = new List<string>();
            // 出力加工用
            Dictionary<string, List<string>> workAnswer = new Dictionary<string, List<string>>();

            // 答えをすべて集める
            foreach (KeyValuePair<string, List<Answer>> kvp in QuizContents[curProgress].CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    if (workAnswer.ContainsKey(kvp.Key))
                    {
                        workAnswer[kvp.Key] = workAnswer[kvp.Key].Concat(CoreProcess.ParseAnswer(ans.Sentence)).ToList();
                    }
                    else
                    {
                        workAnswer[kvp.Key] = CoreProcess.ParseAnswer(ans.Sentence);
                    }
                }
            }

            // 集めた答えの数に応じて出力形式を切り替える
            foreach (KeyValuePair<string, List<string>> ans in workAnswer)
            {
                int cnt = 1;

                foreach (string sentence in ans.Value)
                {
                    if (workAnswer.Keys.Count > 1)
                    {
                        if (ans.Value.Count > 1)
                        {
                            processedAnswer.Add($"{ans.Key}:{cnt++}:{sentence}");
                        }
                        else
                        {
                            processedAnswer.Add($"{ans.Key}:{sentence}");
                        }
                    }
                    else
                    {
                        if (ans.Value.Count > 1)
                        {
                            processedAnswer.Add($"{cnt++}:{sentence}");
                        }
                        else
                        {
                            processedAnswer.Add($"{sentence}");
                        }

                    }
                }
            }

            MessageForm_respuesta = new MessageForm(processedAnswer, "RESPUESTA", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            MessageForm_respuesta.Show();
        }

        #region TSMI

        #region Option

        // Setting
        private void optionTSMI_setting_Click(object sender, EventArgs e)
        {
            SettingForm s = new SettingForm(toolStripQuizFile.Text)
            {
                ShowInTaskbar = false,
                ShowIcon = false
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                ParseFile();
                //InitQuiz(true);
            }
        }

        private void optionTSMI_SettingLanguage_Click(object sender, EventArgs e)
        {
            SettingLanguageForm s = new SettingLanguageForm(LangType)
            {
                ShowInTaskbar = false,
                ShowIcon = false
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("ok");
            }
        }

        // QuizInfo
        private void optionTSMI_quizInfo_Click(object sender, EventArgs e)
        {
            if (MessageForm_quizInfo.IsDisposed == false) MessageForm_quizInfo.Dispose();

            if (ExerRepo == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<string> input_h = new List<string>() { "Quiz Number", "Quiz Title" };
            List<string> input_d = new List<string>() { QuizContents[curProgress].QuizNum.ToString(), QuizContents[curProgress].Section };
            List<string> quizInfo = new List<string>();

            string xml_s = UtilityFunction.GenerateXmlTable(input_h, input_d);

            quizInfo.AddRange(ParseXML.ConvertTextWithTable(xml_s).Split('\n'));

            MessageForm_quizInfo = new MessageForm(quizInfo, "QuizInfo", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            MessageForm_quizInfo.Show();
        }

        // Prueba
        private void optionTSMI_prueba_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
        }

        // Resultado
        private void optionTSMI_resultados_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
        }

        // Visual Progress
        private void optionTSMI_progresoVisual_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
        }

        // Dark Mode
        private void optionTSMI_DarkMode_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
        }

        #endregion

        #region Operation

        private void operationTSMI_start_Click(object sender, EventArgs e)
        {
            InitQuiz(true);
        }

        private void operationTSMI_siguiente_Click(object sender, EventArgs e)
        {
            MoveQuiz(true);
        }

        private void operationTSMI_anterior_Click(object sender, EventArgs e)
        {
            MoveQuiz(false);
        }

        // 進捗Undo
        private void UndoProgress()
        {
            if (QuizResult.Count == 0) return;

            if (optionTSMI_prueba.Checked)
            {
                if (QuizResult[QuizResult.Count - 1].Result == false)
                {
                    ErrorAllowCount.Cnt = 0;
                }
            }

            QuizResult.RemoveAt(QuizResult.Count - 1);
            respuestas.RemoveAt(respuestas.Count - 1);

            if (optionTSMI_progresoVisual.Checked)
            {
                // 現在のラベル★をニュートラル○にする
                label_progress[curProgress % 10].Text = AppRom.ProgressStateCharacter_Neutral;
                progress_state[UtilityFunction.Suelo(curProgress, 10)][curProgress % 10] = AppRom.ProgressState.Neutral;
            }

            // ShowQuestionで++されるからここでは-2する
            curProgress -= 2;

            ShowQuestion();
        }

        // Undo progress
        private void operationTSMI_Undo_p_Click(object sender, EventArgs e)
        {
            if (IsIdle) return;

            UndoProgress();
        }

        // ミスが確定した初回
        private bool IsFirstMistake;

        // Undo error
        private void operationTSMI_Undo_e_Click(object sender, EventArgs e)
        {
            if (IsIdle) return;

            if (QuizFileConfig.ErrorAllowCnt > 0)
            {
                // ミス許容が設定されているとき

                if (IsFirstMistake)
                {
                    // ミス確定初回の場合のUndoは進捗をUndoする
                    UndoProgress();

                    ErrorResetCount.Cnt--;
                    ErrorAllowCount.Cnt = QuizFileConfig.ErrorAllowCnt;
                }
                else
                {
                    if (QuizFileConfig.ErrorAllowAll)
                    {
                        // ミス許容全体

                        if (ErrorAllowCount.Cnt == 0)
                        {
                            // ミス数が0でミス許容リセットが1以上はミス許容リセットを-1
                            if (ErrorResetCount.Cnt > 0)
                            {
                                ErrorResetCount.Cnt--;
                            }
                        }
                        else
                        {
                            // ミス数が1以上はミス数を-1
                            ErrorAllowCount.Cnt--;
                        }
                    }
                    else
                    {
                        // ミス許容全体ではないときはミス数を-1するだけ
                        ErrorAllowCount.Cnt--;
                    }
                }

                IsFirstMistake = false;
            }
            else
            {
                // ミス許容が設定されていないときは無条件に進捗Undoする
                UndoProgress();
            }
        }

        #endregion

        #region Herramientas

        // 正解リストindex順表示
        private void toolTSMI_prueba_Order_Click(object sender, EventArgs e)
        {
            if (resultForm.IsDisposed == false) resultForm.Dispose();

            if (QuizContents.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            resultForm = new ResultForm(QuizContents, this, true)
            {
                Text = "Lista de Pruebas",
                ShowIcon = false
            };

            resultForm.Show();
        }

        // 正解リスト出題順表示
        private void toolTSMI_prueba_QuizOrder_Click(object sender, EventArgs e)
        {
            if (resultForm.IsDisposed == false) resultForm.Dispose();

            if (QuizContents.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            resultForm = new ResultForm(QuizContents, this, false)
            {
                Text = "Lista de Pruebas",
                ShowIcon = false
            };

            resultForm.Show();
        }

        // 正解リスト指定表示
        private void toolTSMI_prueba_Select_Click(object sender, EventArgs e)
        {
            // Pruebaリストの問題インデックスを指定して表示する

            if (QuizFileConfig == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cacheDesde == -1) cacheDesde = cacheIsIndex ? QuizFileConfig.MinChapterToIndex : QuizFileConfig.MinChapter;
            if (cacheHasta == -1) cacheHasta = cacheIsIndex ? QuizFileConfig.MaxChapterToIndex : QuizFileConfig.MaxChapter;

            InputDialog id = new InputDialog(cacheDesde, cacheHasta, QuizCountMax, cacheIsIndex);

            // 問題インデックスを入力する画面
            if (id.ShowDialog() == DialogResult.OK)
            {
                cacheDesde = id.Desde;
                cacheHasta = id.Hasta;
                cacheIsIndex = id.IsIndex;

                int desde = cacheIsIndex ? cacheDesde : cacheDesde * 10 - 9;
                int hasta = cacheIsIndex ? cacheHasta : cacheHasta * 10;
                hasta = hasta > QuizCountMax ? QuizCountMax : hasta;

                List<int> sequence = Enumerable.Range(desde, hasta - desde + 1).ToList();
                List<QuizContents> quizContents = CreateQuizContents(sequence);

                resultForm = new ResultForm(quizContents, this, true)
                {
                    Text = "Lista de Pruebas",
                    ShowIcon = false
                };

                resultForm.Show();
            }
        }

        // チャプターリスト表示
        private void toolTSMI_chapterList_Click(object sender, EventArgs e)
        {
            if (MessageForm_SectionList.IsDisposed == false) MessageForm_SectionList.Dispose();

            if (ExerRepo == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageForm_SectionList = new MessageForm(SectionList, "Lista de sección", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                ShowIcon = false
            };

            MessageForm_SectionList.Show();
        }

        // 翻訳機能
        private void toolTSMI_translate_Click(object sender, EventArgs e)
        {
            if (MessageForm_traducir.IsDisposed == false) MessageForm_traducir.Dispose();
            if (LangType == "" || txtAnswer.Text == "")
            {
                MessageBox.Show("Fallo en la traducción", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string traduccion = Translate.DoTransrate(txtAnswer.Text, LangType);

            List<string> mostrar = new List<string>();

            mostrar.Add(traduccion);

            MessageForm_traducir = new MessageForm(mostrar, "TRADUCCIÓN", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                ShowIcon = false
            };

            MessageForm_traducir.Show();
        }

        // 現在の問題を編集
        private void toolTSMI_EditQuiz_Click(object sender, EventArgs e)
        {
            if (QuizContents.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<int> quizSequence = QuizContents.Select(q => q.QuizNum).ToList();

            EditDBForm edb = new EditDBForm(CurrentQuizDBPath, QuizContents[curProgress].QuizNum, QuizFileConfig.PriorityRegion, quizSequence);

            if (!edb.IsDisposed) edb.ShowDialog();
        }

        // 一つ前の問題を編集
        private void toolTSMI_EditQuiz2_Click(object sender, EventArgs e)
        {
            if (QuizContents.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (curProgress - 1 >= 0)
            {
                List<int> quizSequence = QuizContents.Select(q => q.QuizNum).ToList();

                EditDBForm edb = new EditDBForm(CurrentQuizDBPath, QuizContents[curProgress - 1].QuizNum, QuizFileConfig.PriorityRegion, quizSequence);

                if (!edb.IsDisposed) edb.ShowDialog();
            }
        }

        #endregion

        #region DB

        // クイズDBを開く
        private void DBTSMI_QuizDB_Click(object sender, EventArgs e)
        {
            if (CurrentQuizDBPath == "")
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(CurrentQuizDBPath);

            string path = $"{PathManager.QuizDB}\\{fileName}.db";

            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                MessageBox.Show("El archivo de DB no existe.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        // 進捗を開く
        private void DBTSMI_Progress_Click(object sender, EventArgs e)
        {
            if (CurrentQuizDBPath == "")
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(CurrentQuizDBPath);

            string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{fileName}_p.csv";

            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                MessageBox.Show("El archivo de progreso no existe.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
