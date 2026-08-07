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
        private ExerciseRepository _exerRepo;
        private List<Label> _labelProgress = new List<Label>();
        private List<Label> _labelBar = new List<Label>();
        private NumericUpDown _nudProgress;
        private List<List<AppRom.ProgressState>> _progressState = new List<List<AppRom.ProgressState>>();

        private Label _lblNumericProgress;

        private ResultForm _resultForm = new ResultForm();
        private MessageForm _messageForm_Respuesta = new MessageForm();
        private MessageForm _messageForm_Traducir = new MessageForm();
        private MessageForm _messageForm_QuizInfo = new MessageForm();
        private MessageForm _messageForm_SectionList = new MessageForm();

        // 前回のクイズ設定
        private int _preMinChapter;
        private int _preMaxChapter;
        // 現在の問題集(InitQuizで作成)
        private List<QuizContents> _workBook = new List<QuizContents>();
        // Regionの種類
        private List<string> _regionList => _workBook.SelectMany(q => q.CorrectAnswer.Keys).Distinct().ToList();
        // 問題集の一覧
        private SortedDictionary<int, (string quiz, Answer answer)> _handBook = new SortedDictionary<int, (string quiz, Answer answer)>();
        // セクションリスト(InitQuizで作成)
        private List<string> _sectionList = new List<string>();

        private bool _isError = false;
        // 初期状態かどうか
        private bool _isInit => SettingManager.CurrentQuizDB == null;
        // 待機中かどうかは解答ボタンのEnabledで判断
        private bool _isIdle => !btnAnswer.Enabled;

        // クイズファイルの最大行(設定オーバーを対応するため)
        private int _quizCountMax = 0;
        // 起動時のエラー情報
        private List<string> _initError = new List<string>();
        // 現在の問題のインデックス
        private int _curProgress = -1;
        // 前回の最後の問題インデックス(同じ問題集をするときに最後と最初が同じになることを防ぐ)
        private int _preLastQuiz = -1;

        private int _pruebaChallengeCount = -1;
        private Counter _errorAllowCount = new Counter(-1);
        private Counter _errorResetCount = new Counter(-1);

        // 答えの表を出すときの指定インデックス記憶用
        private int _cacheDesde = -1;
        private int _cacheHasta = -1;
        private bool _cacheIsIndex = false;

        // タイトルバーのベースとなる文字列
        private string _baseTitle = "";

        //public ClassResize _form_resize;

        // ダークモード制御用
        private Dictionary<string, Color> _preControlBackColor = new Dictionary<string, Color>();
        private Dictionary<string, Color> _preControlForeColor = new Dictionary<string, Color>();

        public class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base(new DarkColorTable()) { }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                // 矢印の色を白にする
                e.ArrowColor = Color.White;
                base.OnRenderArrow(e);
            }
        }

        public class DarkColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(60, 60, 60);
            public override Color MenuItemBorder => Color.FromArgb(80, 80, 80);
            public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientEnd => Color.FromArgb(45, 45, 45);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(70, 70, 70);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(70, 70, 70);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(50, 50, 50);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(50, 50, 50);
        }

        private ToolStripRenderer _defaultRenderer;

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

            _defaultRenderer = menuStrip1.Renderer;

            //menuStrip1.Renderer = new DarkRenderer();

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
                    Name = $"progress_group_label{i}",
                    //l.BorderStyle = BorderStyle.FixedSingle;
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = AppRom.ColorNeutral,
                    Visible = false
                };

                l.Click += Label_bar_Click;
                l.MouseHover += Label_hover;
                l.MouseLeave += Label_leave;

                Controls.Add(l);
                _labelBar.Add(l);
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
                    Name = $"progress_label{i}",
                    //l.BorderStyle = BorderStyle.FixedSingle;
                    TextAlign = ContentAlignment.MiddleCenter
                };

                l.Click += LabelClick;
                l.Visible = false;

                Controls.Add(l);
                _labelProgress.Add(l);
            }

            _lblNumericProgress = new Label
            {
                Location = new Point(txtAnswer.Location.X, txtAnswer.Location.Y + txtAnswer.Size.Height + 10),
                Text = "1000/1000",
                //Size = new Size(labelSize, labelSize),
                Font = new Font("MeiryoKe_Console", 9F, FontStyle.Regular, GraphicsUnit.Point, 128),
                Visible = false,
                Name = "NumericProgress"
            };

            Controls.Add(_lblNumericProgress);

            _nudProgress = new NumericUpDown
            {
                Location = new Point(_labelBar[9].Location.X + 50, _labelBar[9].Location.Y),
                Size = new Size(40, 20),
                Name = "hyper_group",
                Minimum = 0,
                Visible = false
            };

            _nudProgress.ValueChanged += nud_ValueChanged;

            Controls.Add(_nudProgress);

            _errorAllowCount.PropertyChanged += ErrorCountPropertyChanged;
            _errorResetCount.PropertyChanged += ErrorCountPropertyChanged;

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
            toolTSMI_ShowAnswer.ShortcutKeys = Keys.Control | Keys.R;
            toolTSMI_translate.ShortcutKeys = Keys.Control | Keys.F1;
            toolTSMI_Search.ShortcutKeys = Keys.Control | Keys.F;
            toolTSMI_EditQuiz_Current.ShortcutKeys = Keys.Control | Keys.E;

            DBTSMI_QuizDB.ShortcutKeys = Keys.Control | Keys.D;
            DBTSMI_Progress.ShortcutKeys = Keys.Control | Keys.G;

            _resultForm.Dispose();
            _messageForm_Respuesta.Dispose();
            _messageForm_Traducir.Dispose();

            txtAnswer.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtAnswer.KeyDown += TextAnswerKeyDown;
            txtQuiz.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtConsole.KeyDown += TextBoxKeyDown_AvoidBeep;

            optionTSMI_quizInfo.Enabled = false;

            operationTSMI_siguiente.Enabled = false;
            operationTSMI_anterior.Enabled = false;
            operationTSMI_Undo_p.Enabled = false;
            operationTSMI_Undo_e.Enabled = false;

            toolTSMI_pruebaLista.Enabled = false;
            toolTSMI_ShowAnswer.Enabled = false;
            toolTSMI_SectionList.Enabled = false;
            toolTSMI_translate.Enabled = false;
            toolTSMI_EditQuiz.Enabled = false;
            toolTSMI_CopyQuiz.Enabled = false;
            toolTSMI_Search.Enabled = false;

            DBTSMI_Progress.Enabled = false;

            #endregion

            //_form_resize = new ClassResize(this);

            // 各コントロールの現在の色を保持
            foreach (Control ctrl in Controls)
            {
                _preControlBackColor[ctrl.Name] = ctrl.BackColor;
                _preControlForeColor[ctrl.Name] = ctrl.ForeColor;

                if (ctrl.GetType() == typeof(Panel))
                {
                    foreach (Control ctrl2 in (ctrl as Panel).Controls)
                    {
                        _preControlBackColor[ctrl2.Name] = ctrl2.BackColor;
                        _preControlForeColor[ctrl2.Name] = ctrl2.ForeColor;
                    }
                }

                if (ctrl.GetType() == typeof(MenuStrip))
                {
                    foreach (Control ctrl2 in (ctrl as MenuStrip).Controls)
                    {
                        _preControlBackColor[ctrl2.Name] = ctrl2.BackColor;
                        _preControlForeColor[ctrl2.Name] = ctrl2.ForeColor;
                    }
                }
            }

            LoadConfig();

            if (!ParseFile())
            {
                txtConsole.Text = "続行不能なエラー\r\n設定ファイルやDBファイルの構成を確認してください";
                _isError = true;
            }
        }

        #region 内部処理

        // クイズファイルの読み込み
        private bool ParseFile()
        {
            string[] QuizFiles = Directory.GetFiles(PathManager.QuizDBDirectory, "*.db");

            QuizFiles = QuizFiles.Where(s => !Path.GetFileName(s).StartsWith('~'.ToString())).ToArray();

            ExerciseRepository exerRepo = null;
            string type = "";
            int exerciseCount = 0;

            foreach (string file in QuizFiles)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        exerRepo = new ExerciseRepository($"Data Source={file}");
                    }
                    else
                    {
                        continue;
                    }

                    type = exerRepo.GetLanguage();
                    exerciseCount = exerRepo.GetExerciseCount();

                    if (!SettingManager.CommonConfigManager.ContainsKey(type))
                    {
                        SettingManager.CommonConfigManager[type] = new Dictionary<string, CommonConfig>();
                    }
                }
                catch (Exception ex)
                {
                    _initError.Add($"{ex.GetType().Name};{ex.Message};{file}");
                    continue;
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
                        _initError.Add($"{ex.GetType().Name};{ex.Message};{cacheFile_common} or {cacheFile_lang}");
                        return false;
                    }
                }

                qfc.MaxQuizNum = exerciseCount;

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
                    _initError.Add($"{ex.GetType().Name};{ex.Message};cache");
                    return false;
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
                        _initError.Add($"{ex.GetType().Name};{ex.Message};{file};{lang}");
                        return false;
                    }
                }
            }

            return true;
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

                if (_resultForm.IsDisposed == false)
                {
                    _resultForm.Visible = false;
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

                if (_resultForm.IsDisposed == false) _resultForm.Visible = result;
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

            QuizFiles = Directory.GetFiles(PathManager.QuizDBDirectory, "*.db");

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

        // クイズ開始
        private void InitQuiz(bool isPrime)
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

            if (_isError)
            {
                MessageBox.Show("続行不能なエラーが発生しています。設定ファイルやDBファイルの構成を確認してください。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string currentQuizDB;

            // スタートボタンでのみDBを切り替える
            if (isPrime)
            {
                currentQuizDB = toolStripQuizFile.SelectedItem.ToString();
            }
            else
            {
                currentQuizDB = SettingManager.CurrentQuizDB;
            }

            ExerciseRepository exerRepo = null;
            string currentLangType = "";
            int quizCountMax = 0;
            List<string> sectionList = null;

            // 問題集DB読み込み

            try
            {
                if (File.Exists(PathManager.QuizDB(currentQuizDB)))
                {
                    // 問題集DBを読み込む
                    exerRepo = new ExerciseRepository($"Data Source={PathManager.QuizDB(currentQuizDB)};Mode=ReadOnly");
                }
                else
                {
                    MessageBox.Show($"{PathManager.QuizDB(currentQuizDB)}が見つかりません", "問題集DB読み込みエラー");
                    return;
                }

                currentLangType = exerRepo.GetLanguage();
                quizCountMax = exerRepo.GetExerciseCount();
                sectionList = exerRepo.GetAllSection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "問題集DB読み込みエラー");
                return;
            }

            QuizFileConfig qfc_tmp = SettingManager.GetQuizFileConfig(currentLangType, currentQuizDB);

            if (quizCountMax < qfc_tmp.MinChapterToIndex)
            {
                MessageBox.Show("問題最大数を超過しています", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 問題集DBからワークブック作成

            // nからmまでの整数のリストを作成
            List<int> numberList = new List<int>();
            for (int i = qfc_tmp.MinChapterToIndex; i <= qfc_tmp.MaxChapterToIndex; i++)
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
            while (randomSequence[0] == _preLastQuiz);

            List<QuizContents> workBook = null;
            SortedDictionary<int, (string, Answer)> handBook = null;

            try
            {
                workBook = CreateQuizContents(exerRepo, randomSequence);
                handBook = CoreProcess.GetHandBook(workBook);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQLの実行に失敗しました\n\n" + ex.Message, "SQLエラー");
                return;
            }

            // 問題集読み込み、ワークブック作成のトランザクション正常終了でコミット

            SettingManager.CurrentQuizDB = currentQuizDB;
            _exerRepo = exerRepo;
            SettingManager.CurrentLangType = currentLangType;
            _quizCountMax = quizCountMax;
            _sectionList = sectionList;
            _workBook = workBook;
            _handBook = handBook;

            txtConsole.Text = "";
            _curProgress = -1;
            _errorResetCount.Cnt = 0;
            // ErrorAllowCount,ErrorResetCountの表示に関わっているものはErrorAllowCountのプロパティを変化する前に変化させておく必要がある
            _errorAllowCount.Cnt = 0;

            // 前回の言語の補助入力を解除
            if (SettingManager.LangCtrl != null)
            {
                txtAnswer.KeyPress -= SettingManager.LangCtrl.KeyPress;
            }

            // 新しい言語の補助入力を登録
            txtAnswer.KeyPress += SettingManager.LangCtrl.KeyPress;

            txtAnswer.Focus();
            btnAnswer.Enabled = true;

            // 進捗表示作成
            CreateQuizProgress();

            InitDisplay();

            RefreshDisplay();

            ShowQuestion();

            // 今回のクイズ設定を保持
            _preMinChapter = SettingManager.CurrentQuizFileConfig.MinChapter;
            _preMaxChapter = SettingManager.CurrentQuizFileConfig.MaxChapter;
        }

        // インデックスリストから問題を取得する
        private List<QuizContents> CreateQuizContents(ExerciseRepository exerRepo, List<int> indexList)
        {
            List<QuizContents> quizContents = new List<QuizContents>();

            foreach (int index in indexList)
            {
                // DBが取得できなかった場合は設定しない
                if (exerRepo.GetByNum(index) is ExerciseDB edb)
                {
                    quizContents.Add(new QuizContents(edb));
                }
            }

            return quizContents;
        }

        // 初期起動状態から問題開始したときのディスプレイの更新
        private void InitDisplay()
        {
            bool isEnabled = _workBook.Count != 0;

            optionTSMI_quizInfo.Enabled = isEnabled;

            operationTSMI_siguiente.Enabled = isEnabled;
            operationTSMI_anterior.Enabled = isEnabled;
            operationTSMI_Undo_p.Enabled = isEnabled;
            operationTSMI_Undo_e.Enabled = isEnabled;

            toolTSMI_pruebaLista.Enabled = isEnabled;
            toolTSMI_ShowAnswer.Enabled = isEnabled;
            toolTSMI_SectionList.Enabled = isEnabled;
            toolTSMI_EditQuiz.Enabled = isEnabled;
            toolTSMI_CopyQuiz.Enabled = isEnabled;
            toolTSMI_Search.Enabled = isEnabled;
            toolTSMI_translate.Enabled = SettingManager.CurrentLangType != "";

            DBTSMI_Progress.Enabled = isEnabled;
        }

        private void RefreshDisplay()
        {
            // 前回とクイズ設定が違っていたらチャレンジ回数を初期化する
            if ((_preMinChapter != SettingManager.CurrentQuizFileConfig.MinChapter) ||
                (_preMaxChapter != SettingManager.CurrentQuizFileConfig.MaxChapter))
            {
                _pruebaChallengeCount = 0;
            }
            else
            {
                // pruebaモードの時だけ
                if (optionTSMI_prueba.Checked)
                {
                    _pruebaChallengeCount++;
                }
            }

            lbl_PruebaChallengeCount.Text = $"Try: {_pruebaChallengeCount}";
            lbl_PruebaChallengeCount.Visible = optionTSMI_prueba.Checked;

            // POR HACER:settingで切り替える
            //lbl_ErrorAllowCount.Visible = false;

            string baseTitle = $"MBR [{SettingManager.CurrentQuizFileConfig.MinChapterToIndex}~{SettingManager.CurrentQuizFileConfig.MaxChapterToIndex}]";

            // pruebaモードのとき
            if (optionTSMI_prueba.Checked)
            {
                if (SettingManager.CurrentQuizFileConfig.ErrorAllowCnt > 0)
                {
                    lbl_ErrorAllowCount.Visible = true;
                }

                // 練習が1章だけならPRUEBA回数を表示する
                if (SettingManager.CurrentQuizFileConfig.MinChapter == SettingManager.CurrentQuizFileConfig.MaxChapter)
                {
                    string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{SettingManager.CurrentQuizDB}_p.csv";

                    if (File.Exists(path))
                    {
                        string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                        // prueba回数
                        baseTitle += $" [PR {int.Parse(lines[SettingManager.CurrentQuizFileConfig.MinChapter - 1].Split(',')[1])}]";
                        // 最近のprueba日
                        baseTitle += $" {lines[SettingManager.CurrentQuizFileConfig.MinChapter - 1].Split(',')[0].Substring(2)}";
                    }
                }
            }

            _baseTitle = baseTitle;
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
            _curProgress++;

            // タイトル更新
            Text = $"{_baseTitle} {_workBook[_curProgress].Section}";

            _preLastQuiz = _workBook[_curProgress].QuizNum;

            if ((SettingManager.CurrentQuizFileConfig.ErrorAllowAll == false) && (SettingManager.CurrentQuizFileConfig.ErrorReset == true))
            {
                // ミス許容が全体ではないときに問題ごとのミスを初期化する
                _errorAllowCount.Cnt = 0;
            }

            // 進捗ビジュアルモード
            if (optionTSMI_progresoVisual.Checked)
            {
                _progressState[UtilityFunction.Suelo(_curProgress, 10)][_curProgress % 10] = AppRom.ProgressState.CurrentQuiz;

                RedrawProgress(_curProgress);
            }
            else
            {
                _ = SettingManager.CurrentQuizFileConfig.MaxQuizNum > _quizCountMax ? _quizCountMax : SettingManager.CurrentQuizFileConfig.MaxQuizNum;
                _lblNumericProgress.Text = $"{_curProgress + 1}/{SettingManager.CurrentQuizFileConfig.QuizNum}";
            }

            txtQuiz.Text = _workBook[_curProgress].Quiz;

            if (_messageForm_QuizInfo.Visible == true)
            {
                QuizInfoUpdate();
            }
        }

        private void QuizInfoUpdate()
        {
            // 同じ処理がTSMI_quizInfoにあるので冗長
            List<string> input_h = new List<string>() { "Quiz Number", "Quiz Title" };
            List<string> input_d = new List<string>() { _workBook[_curProgress].QuizNum.ToString(), _workBook[_curProgress].Section };
            List<string> quizInfo = new List<string>();

            Dictionary<string, List<string>> workAnswer = new Dictionary<string, List<string>>();

            // 答えをすべて集める
            foreach (KeyValuePair<string, List<Answer>> kvp in _workBook[_curProgress].CorrectAnswer)
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

            // Regionとその個数を追加
            foreach (string region in workAnswer.Keys)
            {
                input_h.Add(region);
                input_d.Add(workAnswer[region].Count.ToString());
            }

            string xml_s = UtilityFunction.GenerateXmlTable(input_h, input_d);

            quizInfo.AddRange(ParseXML.ConvertTextWithTable(xml_s).Split('\n'));

            _messageForm_QuizInfo.MessageUpdate(quizInfo);
        }

        // 進捗表示を作る
        private void CreateQuizProgress()
        {
            // 進捗ビジュアルモード
            if (optionTSMI_progresoVisual.Checked)
            {
                _lblNumericProgress.Visible = false;

                current_bar_index = 0;
                _labelBar[0].BackColor = AppRom.ColorCurrentGroup;

                int nudSize = UtilityFunction.Suelo(SettingManager.CurrentQuizFileConfig.QuizNum - 1, 100);

                _nudProgress.Maximum = nudSize;
                _nudProgress.Visible = nudSize != 0;

                _progressState = new List<List<AppRom.ProgressState>>(
                        new List<int>[UtilityFunction.Techo(SettingManager.CurrentQuizFileConfig.QuizNum, 10)]
                            .Select(_ => new List<AppRom.ProgressState>(new AppRom.ProgressState[10]))
                    );

                RedrawProgress(0);
            }
            else
            {
                _labelProgress.ForEach(l1 => l1.Visible = false);
                _labelBar.ForEach(l1 => l1.Visible = false);
                _nudProgress.Visible = false;
                _lblNumericProgress.Visible = true;
            }
        }

        // 進捗表示を更新する
        private void RedrawProgress(int progress_num)
        {
            int hyper_index = UtilityFunction.Suelo(progress_num, 100);

            _nudProgress.Value = hyper_index;

            current_bar_index = UtilityFunction.GetNDigit(progress_num, 2);

            // hyper group(100~)とbar index(10の位)の差をとって進捗ラベルをどこまで表示するか
            int progSize = SettingManager.CurrentQuizFileConfig.QuizNum - ((int)_nudProgress.Value * 100 + current_bar_index * 10);

            // hyper groupが最上位にいっているかを調べる
            int barSize = UtilityFunction.Techo(SettingManager.CurrentQuizFileConfig.QuizNum - ((int)_nudProgress.Value * 100), 10);

            // 進捗ラベルを指定箇所まで表示する
            _labelProgress.Select((label, index) => new { label, index })
                        .ToList()
                        .ForEach(item => item.label.Visible = item.index < progSize);

            // バーラベルを指定箇所まで表示する
            _labelBar.Select((label, index) => new { label, index })
                        .ToList()
                        .ForEach(item => item.label.Visible = item.index < barSize);

            // バーラベルを選択したやつは選択色に変えてそれ以外は未選択色
            _labelBar.Select((label, index) => new { label, index })
                      .ToList()
                      .ForEach(item => item.label.BackColor = (current_bar_index != item.index) ? Color.LightBlue : Color.Turquoise);

            string chara = "";

            for (int cnt = 0; cnt < 10; cnt++)
            {
                switch (_progressState[(int)_nudProgress.Value * 10 + current_bar_index][cnt])
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

                _labelProgress[cnt].Text = chara;
                //label_progress[cnt].ForeColor = chara == progressStateCharacter_CurrentQuiz ? colorOnProgress : colorOffProgress;
            }
        }

        // Siguiente制御
        private void MoveQuiz(bool isForward)
        {
            if (SettingManager.CurrentQuizFileConfig == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int diff = SettingManager.CurrentQuizFileConfig.MaxChapter - SettingManager.CurrentQuizFileConfig.MinChapter + 1;

            if (isForward)
            {
                // 最小章がMAX数を超えていたら何もすることがない
                if (SettingManager.CurrentQuizFileConfig.MinChapter + diff > _quizCountMax / 10)
                {
                    // 最小章が17だとて問題最大数が165だとしたら、最大出題は170まで対応してて
                    // 最大出題-問題最大数が1~10であれば対応できる
                    if(((SettingManager.CurrentQuizFileConfig.MinChapter + diff) * 10) - _quizCountMax > 10)
                    {
                        return;
                    }
                }

                if (SettingManager.CurrentQuizFileConfig.MaxChapter + diff > _quizCountMax / 10)
                {
                    if (((SettingManager.CurrentQuizFileConfig.MinChapter + diff) * 10) - _quizCountMax > 10)
                    {
                        // 最大章がMAX数を超えていたら最大章をMAX数にする
                        SettingManager.CurrentQuizFileConfig.MinChapter += diff;
                        SettingManager.CurrentQuizFileConfig.MaxChapter = _quizCountMax / 10;
                    }
                    else
                    {
                        // 基準の差分分を順シフトする
                        SettingManager.CurrentQuizFileConfig.MinChapter += diff;
                        SettingManager.CurrentQuizFileConfig.MaxChapter += diff;
                    }
                }
                else
                {
                    // 基準の差分分を順シフトする
                    SettingManager.CurrentQuizFileConfig.MinChapter += diff;
                    SettingManager.CurrentQuizFileConfig.MaxChapter += diff;
                }
            }
            else
            {
                // 最大章が0以下だと何もすることがない
                if (SettingManager.CurrentQuizFileConfig.MaxChapter - diff < 1) return;

                if (SettingManager.CurrentQuizFileConfig.MinChapter - diff < 1)
                {
                    // 最大章が0以下だと最小の1にする
                    SettingManager.CurrentQuizFileConfig.MinChapter = 1;
                    SettingManager.CurrentQuizFileConfig.MaxChapter -= diff;
                }
                else
                {
                    // 基準の差分分を逆シフトする
                    SettingManager.CurrentQuizFileConfig.MinChapter -= diff;
                    SettingManager.CurrentQuizFileConfig.MaxChapter -= diff;
                }
            }

            InitQuiz(false);
        }

        // 正誤表表示
        private void ShowFeDeErratas(int progNum)
        {
            if (progNum > 10) return;

            int bar_index = _labelBar.FindIndex(label => label.BackColor == AppRom.ColorCurrentGroup);
            int quizNum = (int)_nudProgress.Value * 100 + bar_index * 10 + progNum;

            if (_curProgress <= quizNum || quizNum < 0) return;

            List<string> tmp = new List<string>();

            string answer = "";

            foreach (KeyValuePair<string, List<Answer>> kvp in _workBook[quizNum].CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    answer += $"{kvp.Key}:{ans.Sentence}\n";
                }
            }

            tmp.Add(answer);
            tmp.Add("───────");
            tmp.Add(_workBook[quizNum].Input);

            MessageForm s = new MessageForm(tmp, "FE DE ERRATAS", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                Icon = Icon
            };

            s.Show();
        }

        // 進捗ファイルひな形作成
        private void CreateNewProgressFile()
        {
            string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{SettingManager.CurrentQuizDB}_p.csv";
            DateTime defaultDate = new DateTime(1970, 1, 1);

            // ファイル作成 & 書き込み
            using (StreamWriter writer = new StreamWriter(path, false)) // false = 上書き
            {
                foreach (string chapter in _sectionList)
                {
                    writer.WriteLine($"{defaultDate:yyyy/MM/dd},000,{chapter}");
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
                            insertText = SettingManager.LanguageConfigManager[SettingManager.CurrentLangType].InputSupport[num];
                        }
                        else
                        {
                            // ファイルごとの補助入力
                            if (_workBook[_curProgress].AutoNombre.Count <= num) return;

                            insertText = _workBook[_curProgress].AutoNombre[num];
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
            int bar_index = _labelBar.FindIndex(label => label.BackColor == AppRom.ColorCurrentGroup);
            int progress_index = _labelProgress.IndexOf(sender as Label);
            int quizNum = (int)_nudProgress.Value * 100 + bar_index * 10 + progress_index;

            if (_curProgress <= quizNum) return;

            List<string> tmp = new List<string>();

            string answer = "";

            foreach (KeyValuePair<string, List<Answer>> kvp in _workBook[quizNum].CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    answer += $"{kvp.Key}:{ans.Sentence}\n";
                }
            }

            tmp.Add(answer);
            tmp.Add("───────");
            tmp.Add(_workBook[quizNum].Input);

            MessageForm s = new MessageForm(tmp, "FE DE ERRATAS", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            s.Show();
        }

        private int current_bar_index = 0;

        private void Label_bar_Click(object sender, EventArgs e)
        {
            int bar_idx = _labelBar.IndexOf(sender as Label);

            // バーラベルを選択したやつは選択色に変えてそれ以外は未選択色
            _labelBar.Select((label, index) => new { label, index })
                      .ToList()
                      .ForEach(item => item.label.BackColor = (bar_idx != item.index) ? Color.LightBlue : Color.Turquoise);

            RedrawProgress((int)_nudProgress.Value * 100 + bar_idx * 10);
        }

        private void Label_hover(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = AppRom.ColorHover;
        }

        private void Label_leave(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = _labelBar[current_bar_index] == l ? AppRom.ColorCurrentGroup : AppRom.ColorNeutral;
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            _labelBar.ForEach(l => l.BackColor = Color.LightBlue);

            int hyper_group = (int)_nudProgress.Value * 100;

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
                lbl_ErrorAllowCount.Text = $"{_errorAllowCount.Cnt}/{SettingManager.CurrentQuizFileConfig.ErrorAllowCnt}";
                if (SettingManager.CurrentQuizFileConfig.ErrorAllowAll)
                {
                    lbl_ErrorAllowCount.Text = $"Todo[{_errorResetCount.Cnt}]: {lbl_ErrorAllowCount.Text}";
                }
            }
        }

        private void CopyQuiz_AllRegion(object o, EventArgs e)
        {
            string tagName = (o as ToolStripItem).Tag as string;

            int copyMode = tagName == "all" ? 0x01 : 0x00;

            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                copyMode |= 0x10;
            }

            List<string> contents = CoreProcess.GetHandBookContents_AllRegion(_handBook, copyMode);

            try
            {
                Clipboard.SetText(string.Join("\r\n", contents));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コピー失敗：{ex.Message}");
                return;
            }

            if (tagName == "all")
            {
                MessageBox.Show($"{(_regionList.Count > 1 ? "全てのRegionの" : "")}表全体をコピー");
            }
            else
            {
                MessageBox.Show($"{(_regionList.Count > 1 ? "全てのRegionの" : "")}答えをコピー");
            }
        }

        private void CopyQuiz_IndividualRegion(object o, EventArgs e)
        {
            string tagName = (o as ToolStripItem).Tag as string;

            int copyMode = tagName == "all" ? 0x01 : 0x00;

            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                copyMode |= 0x10;
            }

            List<string> contents = CoreProcess.GetHandBookContents_IndividualRegion(_handBook, SettingManager.CurrentQuizFileConfig.PriorityRegion, copyMode);

            try
            {
                Clipboard.SetText(string.Join("\r\n", contents));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コピー失敗：{ex.Message}");
                return;
            }

            if (tagName == "all")
            {
                MessageBox.Show("指定Regionの表全体をコピー");
            }
            else
            {
                MessageBox.Show("指定Regionの答えをコピー");
            }
        }

        private void RegisterEvent()
        {
            #region Form

            Load += (o, e) =>
            {
                if (_initError.Count != 0)
                {
                    MessageForm s = new MessageForm(_initError, "Load error", MessageForm.TipoDeUbicacion.CENTRO, this, true, true, true)
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

            #endregion

            #region TSMI

            optionTSMI_prueba.CheckedChanged += (o, e) =>
            {
                if (_isInit) return;
                _pruebaChallengeCount = -1;
                InitQuiz(false);
            };

            optionTSMI_progresoVisual.CheckedChanged += (o, e) =>
            {
                if (_isInit) return;
                InitQuiz(false);
            };

            optionTSMI_DarkMode.CheckedChanged += (o, e) =>
            {
                void SetMenuForeColorRecursive(ToolStripItem item, Color color)
                {
                    item.ForeColor = color;

                    if (item is ToolStripMenuItem menuItem)
                    {
                        foreach (ToolStripItem sub in menuItem.DropDownItems)
                        {
                            Console.WriteLine(sub.Text);
                            SetMenuForeColorRecursive(sub, color);
                        }
                    }
                }

                bool ch = (o as ToolStripMenuItem).Checked;

                Color baseColor = Color.FromArgb(80, 80, 80);
                Color textBackColor = Color.FromArgb(60, 60, 60);

                if (ch)
                {
                    // Dark
                    menuStrip1.Renderer = new DarkRenderer();

                    foreach (ToolStripItem item in menuStrip1.Items)
                    {
                        SetMenuForeColorRecursive(item, Color.White);
                    }

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
                    // Default
                    menuStrip1.Renderer = _defaultRenderer;

                    foreach (ToolStripItem item in menuStrip1.Items)
                    {
                        SetMenuForeColorRecursive(item, Color.Black);
                    }

                    txtAnswer.GotFocus -= _CaretWidthChange;
                    txtAnswer.FontChanged -= _CaretWidthChange;

                    BackColor = SystemColors.Control;

                    foreach (Control ctrl in Controls)
                    {
                        if (ctrl.GetType() == typeof(ToolStrip))
                        {

                        }
                        ctrl.BackColor = _preControlBackColor[ctrl.Name];
                        ctrl.ForeColor = _preControlForeColor[ctrl.Name];

                        if (ctrl.GetType() == typeof(Panel))
                        {
                            foreach (Control ctrl2 in (ctrl as Panel).Controls)
                            {
                                ctrl2.BackColor = _preControlBackColor[ctrl2.Name];
                                ctrl2.ForeColor = _preControlForeColor[ctrl2.Name];
                            }
                        }

                        if (ctrl.GetType() == typeof(MenuStrip))
                        {
                            foreach (Control ctrl2 in (ctrl as MenuStrip).Controls)
                            {
                                ctrl2.BackColor = _preControlBackColor[ctrl2.Name];
                                ctrl2.ForeColor = _preControlForeColor[ctrl2.Name];
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

            toolTSMI_CopyQuiz.DropDownOpening += (o, e) =>
            {
                toolTSMI_CopyQuiz_All.DropDownItems.Clear();
                toolTSMI_CopyQuiz_Answer.DropDownItems.Clear();

                toolTSMI_CopyQuiz_All.Click -= CopyQuiz_AllRegion;
                toolTSMI_CopyQuiz_Answer.Click -= CopyQuiz_AllRegion;

                if (_regionList.Count > 1 && SettingManager.CurrentQuizFileConfig.PriorityRegion != "")
                {
                    // 表全体をコピー
                    var a = toolTSMI_CopyQuiz_All.DropDownItems.Add("現在のRegion", null, CopyQuiz_IndividualRegion);
                    a.Tag = "all";
                    a.ForeColor = optionTSMI_DarkMode.Checked ? Color.White : Color.Black;
                    var b = toolTSMI_CopyQuiz_All.DropDownItems.Add("全てのRegion", null, CopyQuiz_AllRegion);
                    b.Tag = "all";
                    b.ForeColor = optionTSMI_DarkMode.Checked ? Color.White : Color.Black;
                    // 答え全体をコピー
                    var c = toolTSMI_CopyQuiz_Answer.DropDownItems.Add("現在のRegion", null, CopyQuiz_IndividualRegion);
                    c.Tag = "answer_all";
                    c.ForeColor = optionTSMI_DarkMode.Checked ? Color.White : Color.Black;
                    var d = toolTSMI_CopyQuiz_Answer.DropDownItems.Add("全てのRegion", null, CopyQuiz_AllRegion);
                    d.Tag = "answer_all";
                    d.ForeColor = optionTSMI_DarkMode.Checked ? Color.White : Color.Black;
                }
                else
                {
                    toolTSMI_CopyQuiz_All.Click += CopyQuiz_AllRegion;
                    toolTSMI_CopyQuiz_Answer.Click += CopyQuiz_AllRegion;
                }
            };

            toolTSMI_CopyQuiz_Quiz.Click += (o, e) =>
            {
                string quiz;
                List<string> ret = new List<string>();

                foreach (QuizContents qc in _workBook.OrderBy(x => x.QuizNum))
                {
                    quiz = System.Text.RegularExpressions.Regex.Replace(qc.Quiz, @"\r\n|\r|\n", "");

                    ret.Add($"{qc.QuizNum}\t{quiz}");
                }

                try
                {
                    Clipboard.SetText(string.Join("\r\n", ret));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"コピー失敗：{ex.Message}");
                    return;
                }

                MessageBox.Show("問題全体をコピー");
            };

            #endregion
        }

        #endregion

        #region イベント

        CancellationTokenSource cts = new CancellationTokenSource();
        static object lockObject = new object();

        // 解答ボタンクリック(responder)
        private void btnAnswer_Click(object sender, EventArgs e)
        {
            if (_exerRepo == null) return;

            cts.Cancel();

            txtConsole.Text = "";

            // POR HACER:20260106:region指定でやるモードも検討
            var (isCorrect, adopt_str) = CoreProcess.CheckAnswer(txtAnswer.Text, _workBook[_curProgress].Answers().ToList());

#if DEBUG
            if (chboxDebug.Checked) isCorrect = true;
#endif

            DisplayResult(isCorrect ? "¡Sí!" : "¡No!", 1000);

            txtConsole.Text = adopt_str;

            IsFirstMistake = false;

            if (!isCorrect)
            {
                // 不正解のとき

                if (optionTSMI_prueba.Checked)
                {
                    // pruebaモード

                    if (SettingManager.CurrentQuizFileConfig.ErrorAllowCnt > 0)
                    {
                        // ミス許容が設定されているとき

                        if (_errorAllowCount.Cnt < SettingManager.CurrentQuizFileConfig.ErrorAllowCnt)
                        {
                            // ミス許容未満のあいだはミス数を加算してやり直し
                            _errorAllowCount.Cnt++;
                            return;
                        }
                        else
                        {
                            // ミス許容全体はリセットカウント進める
                            if (SettingManager.CurrentQuizFileConfig.ErrorAllowAll)
                            {
                                _errorResetCount.Cnt++;
                            }

                            // ミス許容リセットのときはミス数リセットする
                            if (SettingManager.CurrentQuizFileConfig.ErrorReset)
                            {
                                _errorAllowCount.Cnt = 0;
                            }

                            IsFirstMistake = true;
                        }
                    }
                }
                else
                {
                    if (_errorAllowCount.Cnt < SettingManager.CurrentQuizFileConfig.ErrorAllowCnt)
                    {
                        _errorAllowCount.Cnt++;
                    }
                    else
                    {
                        _errorResetCount.Cnt++;
                        _errorAllowCount.Cnt = 0;
                    }

                    // 完答モードの時はやり直し
                    return;
                }
            }

            if (optionTSMI_progresoVisual.Checked)
            {
                _labelProgress[_curProgress % 10].Text = isCorrect ? AppRom.ProgressStateCharacter_Correct : AppRom.ProgressStateCharacter_Incorrect;
                //label_progress[curProgress % 10].ForeColor = colorOffProgress;
                _progressState[UtilityFunction.Suelo(_curProgress, 10)][_curProgress % 10] = isCorrect ? AppRom.ProgressState.Correct : AppRom.ProgressState.Incorrect;
            }

            // 解答と結果を保存
            _workBook[_curProgress].Input = txtAnswer.Text == "" ? "NONE" : txtAnswer.Text;
            _workBook[_curProgress].IsCorrect = isCorrect;

            txtAnswer.Text = "";

            int endQuizNum = optionTSMI_progresoVisual.Checked ? SettingManager.CurrentQuizFileConfig.QuizNum - 1 : SettingManager.CurrentQuizFileConfig.MaxQuizNum - 1;

            // クイズ終了？
            if (_curProgress == endQuizNum || _curProgress == _quizCountMax - 1)
            {
                //tokenSource.Cancel();

                btnAnswer.Enabled = false;

                // 問題集が全て正解でpruebaモードのとき
                if (_workBook.All(x => x.IsCorrect) && optionTSMI_prueba.Checked)
                {
                    DisplayResult("PERFECTO!", 5000);

                    // 綺麗な対処ではないが、のちのRefreshDisplayで++される仕様のためここで調整
                    // PERFECTOしたあとは最終回数を表示していたい
                    _pruebaChallengeCount--;

                    // チャプター数
                    int chapterNum = SettingManager.CurrentQuizFileConfig.MaxChapter - SettingManager.CurrentQuizFileConfig.MinChapter + 1;

                    // チャプター数に応じて最大問題数であるとき進捗を記録する
                    if (SettingManager.CurrentQuizFileConfig.PermitNum == SettingManager.CurrentQuizFileConfig.QuizNum)
                    {
                        string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{SettingManager.CurrentQuizDB}_p.csv";

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

                            string[] sp = lines[SettingManager.CurrentQuizFileConfig.MinChapter - 1 + cnt].Split(',');
                            string today = DateTime.Now.ToString("yyyy/MM/dd");

                            // 同日のPruebaは記録しない
                            // 日跨ぎのPruebaを重視するため(Ebbinghaus)
                            if (sp[0] != today)
                            {
                                sp[0] = today;
                                sp[1] = (int.Parse(sp[1]) + 1).ToString("D3");
                                lines[SettingManager.CurrentQuizFileConfig.MinChapter - 1 + cnt] = string.Join(",", sp);

                                File.WriteAllLines(path, lines);
                            }
                        }

                        // 練習が複数の章にわたるときは、どこからどこまでかを記録する
                        if (chapterNum > 1)
                        {
                            string path_i = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{SettingManager.CurrentQuizDB}_intercontinental.txt";
                            string write_text = $"{SettingManager.CurrentQuizFileConfig.MinChapter}~{SettingManager.CurrentQuizFileConfig.MaxChapter}";

                            if (File.Exists(path_i))
                            {
                                using (StreamWriter sw = File.AppendText(path_i))
                                {
                                    sw.WriteLine($"{DateTime.Now:yyyy/MM/dd}:{write_text}");
                                }
                            }
                            else
                            {
                                using (StreamWriter sw = File.CreateText(path_i))
                                {
                                    sw.WriteLine($"{DateTime.Now:yyyy/MM/dd}:{write_text}");
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
                    if (_resultForm.IsDisposed == false) _resultForm.Dispose();

                    _resultForm = new ResultForm(_workBook, this)
                    {
                        ShowIcon = false
                    };

                    _resultForm.Show();
                }

                return;
            }

            ShowQuestion();
        }

        // 正解を表示(respuesta)
        private void btnShowAnswer_Click(object sender, EventArgs e)
        {
            ShowAnswer();
        }

        private void ShowAnswer()
        {
            if (_messageForm_Respuesta.IsDisposed == false) _messageForm_Respuesta.Dispose();
            if (_exerRepo == null) return;

            List<string> processedAnswer = new List<string>();
            // 出力加工用
            Dictionary<string, List<string>> workAnswer = new Dictionary<string, List<string>>();

            // 答えをすべて集める
            foreach (KeyValuePair<string, List<Answer>> kvp in _workBook[_curProgress].CorrectAnswer)
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

            _messageForm_Respuesta = new MessageForm(processedAnswer, "RESPUESTA", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                Icon = Icon
            };

            _messageForm_Respuesta.Show();
        }

        #region TSMI

        #region Option

        // Setting
        private void optionTSMI_setting_Click(object sender, EventArgs e)
        {
            SettingForm s = new SettingForm()
            {
                Icon = Icon,
                ShowInTaskbar = false
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                if (!ParseFile())
                {
                    txtConsole.Text = "続行不能なエラー\r\n設定ファイルやDBファイルの構成を確認してください";
                    _isError = true;
                }
            }
        }

        private void optionTSMI_SettingLanguage_Click(object sender, EventArgs e)
        {
            SettingLanguageForm s = new SettingLanguageForm(SettingManager.CurrentLangType)
            {
                Icon = Icon,
                ShowInTaskbar = false
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("ok");
            }
        }

        // QuizInfo
        private void optionTSMI_quizInfo_Click(object sender, EventArgs e)
        {
            if (_messageForm_QuizInfo.IsDisposed == false) _messageForm_QuizInfo.Dispose();

            if (_exerRepo == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _messageForm_QuizInfo = new MessageForm(new List<string>(), "QuizInfo", MessageForm.TipoDeUbicacion.DERECHA, this, true)
            {
                Icon = Icon,
                FormBorderStyle = FormBorderStyle.SizableToolWindow
            };

            QuizInfoUpdate();

            _messageForm_QuizInfo.Show();
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
            if (_curProgress == 0) return;

            if (optionTSMI_prueba.Checked)
            {
                if (_workBook[_curProgress - 1].IsCorrect == false)
                {
                    _errorAllowCount.Cnt = 0;
                }
            }

            if (optionTSMI_progresoVisual.Checked)
            {
                // 現在のラベル★をニュートラル○にする
                _labelProgress[_curProgress % 10].Text = AppRom.ProgressStateCharacter_Neutral;
                _progressState[UtilityFunction.Suelo(_curProgress, 10)][_curProgress % 10] = AppRom.ProgressState.Neutral;
            }

            // ShowQuestionで++されるからここでは-2する
            _curProgress -= 2;

            ShowQuestion();
        }

        // Undo progress
        private void operationTSMI_Undo_p_Click(object sender, EventArgs e)
        {
            if (_isIdle) return;

            UndoProgress();
        }

        // ミスが確定した初回
        private bool IsFirstMistake;

        // Undo error
        private void operationTSMI_Undo_e_Click(object sender, EventArgs e)
        {
            if (_isIdle) return;

            if (SettingManager.CurrentQuizFileConfig.ErrorAllowCnt > 0)
            {
                // ミス許容が設定されているとき

                if (IsFirstMistake)
                {
                    // ミス確定初回の場合のUndoは進捗をUndoする
                    UndoProgress();

                    _errorResetCount.Cnt--;
                    _errorAllowCount.Cnt = SettingManager.CurrentQuizFileConfig.ErrorAllowCnt;
                }
                else
                {
                    if (SettingManager.CurrentQuizFileConfig.ErrorAllowAll)
                    {
                        // ミス許容全体

                        if (_errorAllowCount.Cnt == 0)
                        {
                            // ミス数が0でミス許容リセットが1以上はミス許容リセットを-1
                            if (_errorResetCount.Cnt > 0)
                            {
                                _errorResetCount.Cnt--;
                            }
                        }
                        else
                        {
                            // ミス数が1以上はミス数を-1
                            _errorAllowCount.Cnt--;
                        }
                    }
                    else
                    {
                        // ミス許容全体ではないときはミス数を-1するだけ
                        _errorAllowCount.Cnt--;
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
            if (_resultForm.IsDisposed == false) _resultForm.Dispose();

            if (_workBook.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _resultForm = new ResultForm(_workBook, this, true)
            {
                Text = "Lista de Pruebas",
                Icon = Icon
            };

            _resultForm.Show();
        }

        // 正解リスト出題順表示
        private void toolTSMI_prueba_QuizOrder_Click(object sender, EventArgs e)
        {
            if (_resultForm.IsDisposed == false) _resultForm.Dispose();

            if (_workBook.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _resultForm = new ResultForm(_workBook, this, false)
            {
                Text = "Lista de Pruebas",
                Icon = Icon
            };

            _resultForm.Show();
        }

        // 正解リスト指定表示
        private void toolTSMI_prueba_Select_Click(object sender, EventArgs e)
        {
            // Pruebaリストの問題インデックスを指定して表示する

            if (SettingManager.CurrentQuizFileConfig == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_cacheDesde == -1) _cacheDesde = _cacheIsIndex ? SettingManager.CurrentQuizFileConfig.MinChapterToIndex : SettingManager.CurrentQuizFileConfig.MinChapter;
            if (_cacheHasta == -1) _cacheHasta = _cacheIsIndex ? SettingManager.CurrentQuizFileConfig.MaxChapterToIndex : SettingManager.CurrentQuizFileConfig.MaxChapter;

            InputDialog id = new InputDialog(_cacheDesde, _cacheHasta, _quizCountMax, _cacheIsIndex);

            // 問題インデックスを入力する画面
            if (id.ShowDialog() == DialogResult.OK)
            {
                _cacheDesde = id.Desde;
                _cacheHasta = id.Hasta;
                _cacheIsIndex = id.IsIndex;

                int desde = _cacheIsIndex ? _cacheDesde : _cacheDesde * 10 - 9;
                int hasta = _cacheIsIndex ? _cacheHasta : _cacheHasta * 10;
                hasta = hasta > _quizCountMax ? _quizCountMax : hasta;

                List<int> sequence = Enumerable.Range(desde, hasta - desde + 1).ToList();
                List<QuizContents> quizContents = CreateQuizContents(_exerRepo, sequence);

                _resultForm = new ResultForm(quizContents, this, true)
                {
                    Text = "Lista de Pruebas",
                    Icon = Icon
                };

                _resultForm.Show();
            }
        }

        // 正解表示
        private void toolTSMI_ShowAnswer_Click(object sender, EventArgs e)
        {
            ShowAnswer();
        }

        // チャプターリスト表示
        private void toolTSMI_chapterList_Click(object sender, EventArgs e)
        {
            if (_messageForm_SectionList.IsDisposed == false) _messageForm_SectionList.Dispose();

            if (_exerRepo == null)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _messageForm_SectionList = new MessageForm(_sectionList, "Lista de sección", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                MaximizeBox = false,
                MinimizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                ShowInTaskbar = false
            };

            _messageForm_SectionList.ShowDialog();
        }

        // 翻訳機能
        private void toolTSMI_translate_Click(object sender, EventArgs e)
        {
            if (_messageForm_Traducir.IsDisposed == false) _messageForm_Traducir.Dispose();
            if (SettingManager.CurrentLangType == "" || txtAnswer.Text == "")
            {
                MessageBox.Show("Fallo en la traducción", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string traduccion = Translate.DoTransrate(txtAnswer.Text, SettingManager.CurrentLangType);

            List<string> mostrar = new List<string>();

            mostrar.Add(traduccion);

            _messageForm_Traducir = new MessageForm(mostrar, "TRADUCCIÓN", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                ShowIcon = false
            };

            _messageForm_Traducir.Show();
        }

        // 現在の問題を編集
        private void toolTSMI_EditQuiz_Current_Click(object sender, EventArgs e)
        {
            if (_workBook.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<int> quizSequence = _workBook.Select(q => q.QuizNum).ToList();

            EditDBForm edb = new EditDBForm(_workBook[_curProgress].QuizNum, quizSequence)
            {
                Icon = Icon,
            };

            if (!edb.IsDisposed) edb.Show(this);
        }

        // 一つ前の問題を編集
        private void toolTSMI_EditQuiz_Antes_Click(object sender, EventArgs e)
        {
            if (_workBook.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_curProgress - 1 >= 0)
            {
                List<int> quizSequence = _workBook.Select(q => q.QuizNum).ToList();

                EditDBForm edb = new EditDBForm(_workBook[_curProgress - 1].QuizNum, quizSequence)
                {
                    Icon = Icon,
                };

                if (!edb.IsDisposed) edb.Show(this);
            }
        }

        // 番号を指定して編集
        private void toolTSMI_EditQuiz_Number_Click(object sender, EventArgs e)
        {
            if (_workBook.Count == 0)
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int maxQuizNum = _exerRepo.GetExerciseCount();

            using (Form dialog = new Form())
            {
                dialog.Text = $"問題番号を入力(1~{maxQuizNum})";
                dialog.Font = Font = new Font("メイリオ", 9F);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(250, 100);
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ShowInTaskbar = false;

                Label label = new Label() { Left = 10, Top = 10, Text = "番号：", Width = 50, Font = new Font("メイリオ", 9F) };
                TextBox textBox = new TextBox() { Left = 60, Top = 8, Width = 150, Font = new Font("メイリオ", 9F) };

                Button okButton = new Button()
                {
                    Text = "OK",
                    Left = 60,
                    Width = 60,
                    Top = 40,
                    DialogResult = DialogResult.OK,
                    Font = new Font("メイリオ", 9F)
                };
                Button cancelButton = new Button()
                {
                    Text = "キャンセル",
                    Left = 130,
                    Width = 80,
                    Top = 40,
                    DialogResult = DialogResult.Cancel,
                    Font = new Font("メイリオ", 9F)
                };

                dialog.Controls.Add(label);
                dialog.Controls.Add(textBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);

                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!int.TryParse(textBox.Text, out int number))
                    {
                        MessageBox.Show("数値を入力してください");
                        return;
                    }

                    if((number > maxQuizNum) || (number < 1))
                    {
                        MessageBox.Show($"1~{maxQuizNum}で入力してください");
                        return;
                    }

                    List<int> quizSequence = Enumerable.Range(1, _quizCountMax).ToList();

                    EditDBForm edb = new EditDBForm(number, quizSequence)
                    {
                        Icon = Icon,
                    };

                    if (!edb.IsDisposed) edb.Show(this);
                }
            }
        }

        // 問題を検索
        private void toolTSMI_Search_Click(object sender, EventArgs e)
        {
            SearchDialog sd = new SearchDialog();

            if (sd.ShowDialog() == DialogResult.OK)
            {
                // 該当する問題インデックスを取得
                List<int> ret = _exerRepo.Buscar(sd.Input, sd.Mode).Distinct().ToList();

                if(ret.Count == 0)
                {
                    MessageBox.Show("該当なし");
                    return;
                }

                List<QuizContents> qc = CreateQuizContents(_exerRepo, ret);

                _resultForm = new ResultForm(qc, this);
                _resultForm.Show();
            }
        }

        #endregion

        #region DB

        // クイズDBを開く
        private void DBTSMI_QuizDB_Click(object sender, EventArgs e)
        {
            if (SettingManager.CurrentQuizDB == "")
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = PathManager.QuizDB(SettingManager.CurrentQuizDB);

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
            if (SettingManager.CurrentQuizDB == "")
            {
                MessageBox.Show("El archivo del Quiz no se ha cargado.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = $"{SettingManager.RomConfig.ResourcePath}\\progreso\\{SettingManager.CurrentQuizDB}_p.csv";

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
