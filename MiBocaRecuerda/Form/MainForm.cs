using ClosedXML.Excel;
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
    public partial class MainForm : Form
    {
        private IXLWorksheet ws;
        private CoreProcess CoreProcess = new CoreProcess();
        List<Label> label_progress = new List<Label>();
        List<Label> label_bar = new List<Label>();
        NumericUpDown nudProgress;
        List<List<int>> progress_state = new List<List<int>>();
        List<string> respuestas = new List<string>();

        Color colorNeutral = Color.LightBlue;
        Color colorHover = Color.OrangeRed;
        Color colorCurrentGroup = Color.Turquoise;
        Color colorOnProgress = Color.Red;
        Color colorOffProgress = Color.Black;

        string progressStateCharacter_Neutral = "○";
        string progressStateCharacter_Correct = "■";
        string progressStateCharacter_Incorrect = "×";
        string progressStateCharacter_CurrentQuiz = "★";

        Label lblNumericProgress;

        ResultForm resultForm = new ResultForm();
        MessageForm MessageForm_respuesta = new MessageForm();
        MessageForm MessageForm_traducir = new MessageForm();
        MessageForm MessageForm_quizInfo = new MessageForm();
        MessageForm MessageForm_chapterList = new MessageForm();

        // Resultadoに表示する為に蓄積するやつ
        List<QuizResult> QuizResult = new List<QuizResult>();
        // 現在の読込ファイルの設定
        public static QuizFileConfig QuizFileConfig;
        // 前回のクイズ設定
        private int preMinChapter;
        private int preMaxChapter;
        // 現在の問題集(InitQuizで作成)
        private List<QuizContents> QuizContents = new List<QuizContents>();

        private bool IsLoaded = false;

        // 現在のクイズファイル
        string currentQuizFile;
        // クイズファイルの最大行(設定オーバーを対応するため)
        int MaxRow = 0;
        // 現在のクイズ言語
        string langType = "";
        // 現在の問題のインデックス
        int curProgress = -1;

        int PruebaChallengeCount = -1;

        bool isAcento = false;
        bool isDieresis = false;

        private static readonly Dictionary<char, char> letra_acento = new Dictionary<char, char>()
        {
            ['a'] = 'á',
            ['e'] = 'é',
            ['i'] = 'í',
            ['o'] = 'ó',
            ['u'] = 'ú',
            ['A'] = 'Á',
            ['E'] = 'É',
            ['I'] = 'Í',
            ['O'] = 'Ó',
            ['U'] = 'Ú',
        };

        private static readonly Dictionary<char, char> letra_dieresis = new Dictionary<char, char>()
        {
            ['u'] = 'ü',
            ['U'] = 'Ü',
        };

        Dictionary<string, Dictionary<string, QuizFileConfig>> ArchivosDeLengua = new Dictionary<string, Dictionary<string, QuizFileConfig>>();

        bool IsKeyDown = false;

        private ClassResize _form_resize;

        Dictionary<string, Color> preControlBackColor = new Dictionary<string, Color>();
        Dictionary<string, Color> preControlForeColor = new Dictionary<string, Color>();

        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool DestroyCaret();

        public MainForm()
        {
            InitializeComponent();

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
                l.BackColor = colorNeutral;
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

            #endregion

            #region デザイナを使わないコントロールプロパティ設定

#if DEBUG
            Text += " [debug]";
#endif

            lblResult.Visible = false;
            label1.Visible = false;
            btnAnswer.Enabled = false;
            lbl_PruebaChallengeCount.Visible = false;
            txtQuiz.ReadOnly = true;
            txtQuiz.BackColor = Color.White;

            optionTSMI_quizInfo.ShortcutKeys = Keys.Control | Keys.I;
            optionTSMI_prueba.ShortcutKeys = Keys.Control | Keys.P;
            optionTSMI_resultados.ShortcutKeys = Keys.Control | Keys.T;
            optionTSMI_progresoVisual.ShortcutKeys = Keys.Control | Keys.O;

            operationTSMI_start.ShortcutKeys = Keys.Control | Keys.Q;
            operationTSMI_siguiente.ShortcutKeys = Keys.Control | Keys.Shift | Keys.N;
            operationTSMI_anterior.ShortcutKeys = Keys.Control | Keys.Shift | Keys.B;

            toolTSMI_pruebaLista.ShortcutKeys = Keys.Control | Keys.L;
            toolTSMI_translate.ShortcutKeys = Keys.Control | Keys.F1;

            resultForm.Dispose();
            MessageForm_respuesta.Dispose();
            MessageForm_traducir.Dispose();

            txtAnswer.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtQuiz.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtConsole.KeyDown += TextBoxKeyDown_AvoidBeep;

            #endregion

            _form_resize = new ClassResize(this);

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

            QuizFileConfig = SettingManager.AppConfig.quizFileConfig;

            // 最初のクイズ設定を保持
            preMinChapter = QuizFileConfig.MinChapter;
            preMaxChapter = QuizFileConfig.MaxChapter;
        }

        #region 内部処理

        // クイズファイルの読み込み
        private void ParseFile()
        {
            string[] QuizFiles = Directory.GetFiles(SettingManager.RomConfig.QuizFilePath, "*.xlsx");

            QuizFiles = QuizFiles.Where(s => !Path.GetFileName(s).StartsWith('~'.ToString())).ToArray();

            FileStream fs;
            string type;

            foreach (string file in QuizFiles)
            {
                using (fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    type = new XLWorkbook(fs).Worksheet(1).Cell(1, 1).Value.ToString();
                    if (!ArchivosDeLengua.ContainsKey(type))
                    {
                        ArchivosDeLengua[type] = new Dictionary<string, QuizFileConfig>();
                    }
                }

                string cacheFile = $"{SettingManager.RomConfig.QuizFilePath}\\cache\\{Path.GetFileNameWithoutExtension(file)}.xml";

                QuizFileConfig lang = new QuizFileConfig();

                if (File.Exists(cacheFile))
                {
                    lang = CommonFunction.XmlRead<QuizFileConfig>(cacheFile);
                }

                ArchivosDeLengua[type][file] = lang;
            }
        }

        // 非表示記憶用
        string tmp1 = "", tmp2 = "", tmp3 = "";
        bool result = false;
        bool ba = false;

        // 表示されてる文字を非表示にする
        private void HideText()
        {
            if (!label1.Visible)
            {
                tmp1 = txtQuiz.Text;
                txtQuiz.Text = "";

                tmp2 = txtAnswer.Text;
                txtAnswer.Text = "";

                tmp3 = txtConsole.Text;
                txtConsole.Text = "";

                ba = btnAnswer.Enabled;
                btnAnswer.Enabled = false;
                btnShowAnswer.Enabled = false;
                optionTSMI_prueba.Enabled = false;
                optionTSMI_progresoVisual.Enabled = false;
                optionTSMI_resultados.Enabled = false;

                if (resultForm.IsDisposed == false)
                {
                    resultForm.Visible = false;
                    result = true;
                }
            }
            else
            {
                txtQuiz.Text = tmp1;
                txtAnswer.Text = tmp2;
                txtAnswer.Select(txtAnswer.Text.Length, 0);
                txtConsole.Text = tmp3;

                btnAnswer.Enabled = ba;
                btnShowAnswer.Enabled = true;
                optionTSMI_prueba.Enabled = true;
                optionTSMI_progresoVisual.Enabled = true;
                optionTSMI_resultados.Enabled = true;

                if (resultForm.IsDisposed == false) resultForm.Visible = result;
                txtAnswer.Focus();
            }

            txtAnswer.ReadOnly = !txtAnswer.ReadOnly;
            txtConsole.ReadOnly = !txtConsole.ReadOnly;

            label1.Visible = !label1.Visible;
        }

        // MBRの初期設定
        private void LoadConfig()
        {
            string[] QuizFiles;

            if (File.Exists("rom.config"))
            {
                SettingManager.RomConfig = CommonFunction.XmlRead<RomConfig>("rom.config");
            }

            QuizFiles = Directory.GetFiles(SettingManager.RomConfig.QuizFilePath, "*.xlsx");

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
            if (toolStripQuizFile.Items.Count > SettingManager.InputCache.QuizFilePathIndex)
            {
                toolStripQuizFile.SelectedIndex = SettingManager.InputCache.QuizFilePathIndex;
            }
            optionTSMI_DarkMode.Checked = SettingManager.InputCache.DarkMode;

            if (File.Exists("MBR.config"))
            {
                SettingManager.AppConfig = CommonFunction.XmlRead<AppConfig>("MBR.config");
            }
        }

        // 問題集excelを開く
        private void OpenExcel(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            XLWorkbook workBook = new XLWorkbook(fs);
            ws = workBook.Worksheet(1);
            fs.Close();

            MaxRow = UtilityFunction.GetLastRowInColumn(ws, "B");

            langType = ws.Cell(1, 1).Value.ToString();

            QuizFileConfig = ArchivosDeLengua[langType][filePath];
        }

        int preLastQuiz = -1;

        // クイズ開始
        private void InitQuiz(bool manual)
        {
            // 非表示中はクイズを始めない
            if (label1.Visible)
            {
                MessageBox.Show("No se puede continuar con la prueba mientras está oculto");
                return;
            }

            txtAnswer.Focus();

            btnAnswer.Enabled = true;

            currentQuizFile = toolStripQuizFile.SelectedItem.ToString();
            string filePath = $"{SettingManager.RomConfig.QuizFilePath}\\{toolStripQuizFile.SelectedItem.ToString()}.xlsx";

            OpenExcel(filePath);
            if (manual) txtConsole.Text = "";
            curProgress = -1;
            correctAnswerNum = 0;
            QuizResult.Clear();
            QuizContents.Clear();
            respuestas.Clear();

            // 進捗表示作成
            CreateQuizProgress();

            RefreshDisplay();

            // nからmまでの整数のリストを作成
            List<int> numberList = new List<int>();
            for (int i = QuizFileConfig.MinChapter * 10 - 9; i <= QuizFileConfig.MaxChapter * 10; i++)
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

            string quizTxt = "";
            string correctAnswer = "";
            string quizNum = "";
            string chapterTitle = "";
            string chapterExample = "";
            string supplement = "";

            foreach (int index in randomSequence)
            {
                quizTxt = ws.Cell(index, 2).Value.ToString();
                correctAnswer = ws.Cell(index, 3).Value.ToString();
                quizNum = index.ToString();
                chapterTitle = ws.Cell((int)Math.Floor((decimal)((index - 1) / 10)) * 10 + 1, 4).Value.ToString();
                chapterExample = ws.Cell((int)Math.Floor((decimal)((index - 1) / 10)) * 10 + 1, 5).Value.ToString();
                supplement = ws.Cell(index, 6).Value.ToString();

                QuizContents.Add(new QuizContents(quizTxt, correctAnswer, quizNum, chapterTitle, chapterExample, supplement));
            }

            // 今回のクイズ設定を保持
            preMinChapter = QuizFileConfig.MinChapter;
            preMaxChapter = QuizFileConfig.MaxChapter;

            ShowQuestion();
        }

        private void RefreshDisplay()
        {
            // 前回とクイズ設定が違っていたらチャレンジ回数を初期化する
            if((preMinChapter != QuizFileConfig.MinChapter) ||
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

            lbl_PruebaChallengeCount.Text = PruebaChallengeCount.ToString();
            lbl_PruebaChallengeCount.Visible = optionTSMI_prueba.Checked;

            Text = $"MBR [{QuizFileConfig.MinChapter * 10 - 9}~{QuizFileConfig.MaxChapter * 10}]";

            // pruebaモードのとき
            if (optionTSMI_prueba.Checked)
            {
                // 練習が1章だけならPRUEBA回数を表示する
                if (QuizFileConfig.MinChapter == QuizFileConfig.MaxChapter)
                {
                    string path = $"{SettingManager.RomConfig.QuizFilePath}\\progreso\\{currentQuizFile}_p.csv";

                    if (File.Exists(path))
                    {
                        string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                        // prueba回数
                        Text += $" [PRUEBA {int.Parse(lines[QuizFileConfig.MinChapter - 1].Split(',')[1]).ToString()}]";
                        // 最近のprueba日
                        Text += $" {lines[QuizFileConfig.MinChapter - 1].Split(',')[0]}";
                    }
                }
            }
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

            preLastQuiz = int.Parse(QuizContents[curProgress].QuizNum);

            // 進捗ビジュアルモード
            if (optionTSMI_progresoVisual.Checked)
            {
                progress_state[UtilityFunction.Suelo(curProgress, 10)][curProgress % 10] = 3;

                ProgressRedrow(UtilityFunction.GetNDigit(curProgress, 2));
            }
            else
            {
                int totalNum = QuizFileConfig.MaxQuizNum > MaxRow ? MaxRow : QuizFileConfig.MaxQuizNum;
                lblNumericProgress.Text = $"{curProgress + 1}/{QuizFileConfig.QuizNum}";
            }

            txtQuiz.Text = QuizContents[curProgress].Quiz;

            if (MessageForm_quizInfo.Visible == true)
            {
                // 同じ処理がTSMI_quizInfoにあるので冗長
                List<string> input_h = new List<string>() { "Quiz Number", "Quiz Title" };
                List<string> input_d = new List<string>() { QuizContents[curProgress].QuizNum, QuizContents[curProgress].ChapterTitle };
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
                label_bar[0].BackColor = colorCurrentGroup;

                int nudSize = UtilityFunction.Suelo(QuizFileConfig.QuizNum - 1, 100);

                nudProgress.Maximum = nudSize;
                nudProgress.Visible = nudSize == 0 ? false : true;

                progress_state = new List<List<int>>(
                        new List<int>[UtilityFunction.Techo(QuizFileConfig.QuizNum, 10)]
                            .Select(_ => new List<int>(new int[10]))
                    );

                ProgressRedrow(0);
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
        private void ProgressRedrow(int bar_index)
        {
            current_bar_index = bar_index;

            // hyper group(100~)とbar index(10の位)の差をとって進捗ラベルをどこまで表示するか
            int progSize = QuizFileConfig.QuizNum - ((int)nudProgress.Value * 100 + bar_index * 10);

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
                    case 0:
                        chara = progressStateCharacter_Neutral;
                        break;
                    case 1:
                        chara = progressStateCharacter_Correct;
                        break;
                    case 2:
                        chara = progressStateCharacter_Incorrect;
                        break;
                    case 3:
                        chara = progressStateCharacter_CurrentQuiz;
                        break;
                }

                label_progress[cnt].Text = chara;
                //label_progress[cnt].ForeColor = chara == progressStateCharacter_CurrentQuiz ? colorOnProgress : colorOffProgress;
            }
        }

        // Siguiente制御
        private void MoveQuiz(bool isForward)
        {
            if (isForward)
            {
                if ((QuizFileConfig.MinChapter + 1 > (MaxRow / 10)) || (QuizFileConfig.MaxChapter + 1 > (MaxRow / 10))) return;

                QuizFileConfig.MinChapter++;
                QuizFileConfig.MaxChapter++;
                InitQuiz(true);
            }
            else
            {
                if ((QuizFileConfig.MinChapter - 1 <= 0) || (QuizFileConfig.MaxChapter - 1 <= 0)) return;

                QuizFileConfig.MinChapter--;
                QuizFileConfig.MaxChapter--;
                InitQuiz(true);
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

        private void LabelClick(object sender, EventArgs e)
        {
            int bar_index = label_bar.FindIndex(label => label.BackColor == colorCurrentGroup);
            int progress_index = label_progress.IndexOf(sender as Label);
            int quizNum = (int)nudProgress.Value * 100 + bar_index * 10 + progress_index;

            if (respuestas.Count <= quizNum) return;

            List<string> tmp = new List<string>();

            tmp.Add(QuizContents[quizNum].CorrectAnswer);
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
            int idx = label_bar.IndexOf(sender as Label);

            current_bar_index = idx;

            // バーラベルを選択したやつは選択色に変えてそれ以外は未選択色
            label_bar.Select((label, index) => new { label, index })
                      .ToList()
                      .ForEach(item => item.label.BackColor = (idx != item.index) ? Color.LightBlue : Color.Turquoise);

            ProgressRedrow(idx);
        }

        private void Label_hover(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = colorHover;
        }

        private void Label_leave(object o, EventArgs e)
        {
            Label l = o as Label;

            l.BackColor = label_bar[current_bar_index] == l ? colorCurrentGroup : colorNeutral;
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            label_bar.ForEach(l => l.BackColor = Color.LightBlue);

            ProgressRedrow(0);
        }

        private void RegisterEvent()
        {
            #region Form

            Load += (o, e) =>
            {
            };

            SizeChanged += (o, e) =>
            {
                _form_resize._resize();
            };

            KeyDown += (o, e) =>
            {
                if (IsKeyDown) return;

                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                bool shiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;

                if (ctrlPressed)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.R:
                            // Respuesta
                            IsKeyDown = true;
                            btnShowAnswer.PerformClick();
                            break;
                        case Keys.N:
                            //if (shiftPressed) MoveQuiz(true);
                            break;
                        case Keys.B:
                            //if (shiftPressed) MoveQuiz(false);
                            break;
                        case Keys.L:
                            //ShowLista();
                            break;
                    }
                }
                else
                {
                    //if (e.KeyCode == Keys.F1)
                    //{
                    //    btnTranslate.PerformClick();
                    //}
                }
            };

            KeyUp += (o, e) =>
            {
                IsKeyDown = false;
            };

            Shown += (o, e) =>
            {
                IsLoaded = true;
            };

            Load += (o, e) =>
            {
                KeyPreview = true;

                //ToolTipを作成する
                ToolTip tt = new ToolTip
                {
                    //ToolTipが表示されるまでの時間
                    InitialDelay = 10,
                    //ToolTipが表示されている時に、別のToolTipを表示するまでの時間
                    ReshowDelay = 10,
                    //ToolTipを表示する時間
                    AutoPopDelay = 10000,
                    //フォームがアクティブでない時でもToolTipを表示する
                    ShowAlways = true
                };

                //tt.SetToolTip(btnTranslate, "traducción");
            };

            FormClosing += (o, e) =>
            {
                SettingManager.InputCache.Complete = optionTSMI_prueba.Checked;
                SettingManager.InputCache.Exercise = optionTSMI_progresoVisual.Checked;
                SettingManager.InputCache.Result = optionTSMI_resultados.Checked;
                SettingManager.InputCache.QuizFilePathIndex = toolStripQuizFile.SelectedIndex;
                SettingManager.InputCache.DarkMode = optionTSMI_DarkMode.Checked;

                CommonFunction.XmlWrite(SettingManager.InputCache, "cache.xml");
            };

            #endregion

            #region OtherControl

            txtAnswer.KeyPress += (o, e) =>
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

                switch (e.KeyChar)
                {
                    case '\'':
                        isAcento = true;
                        e.Handled = true;
                        break;
                    case '"':
                        isDieresis = true;
                        e.Handled = true;
                        break;
                    case 'a':
                    case 'e':
                    case 'i':
                    case 'o':
                    case 'u':
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                        if (isAcento)
                        {
                            e.KeyChar = letra_acento[e.KeyChar];
                        }
                        else if (isDieresis)
                        {
                            e.KeyChar = letra_dieresis[e.KeyChar];
                        }
                        break;
                    case ';':
                        e.KeyChar = 'ñ';
                        break;
                    case ':':
                        e.KeyChar = 'Ñ';
                        break;
                    case '<':
                        e.KeyChar = ';';
                        break;
                    case '>':
                        e.KeyChar = ':';
                        break;
                }

                switch (e.KeyChar)
                {
                    case '\'':
                    case '"':
                        break;
                    default:
                        isAcento = false;
                        isDieresis = false;
                        break;
                }
            };

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

            toolStripQuizFile.MouseDown += (o, e) =>
            {
                string file = (o as ToolStripComboBox).SelectedItem.ToString();
                string path = "";

                switch (e.Button)
                {
                    case MouseButtons.Right:
                        // 進捗
                        path = $"{SettingManager.RomConfig.QuizFilePath}\\progreso\\{file}_p.csv";

                        break;
                    case MouseButtons.Middle:
                        // DB
                        path = $"{SettingManager.RomConfig.QuizFilePath}\\{file}.xlsx";

                        break;
                }

                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
            };

            toolTSMI_pruebaLista.MouseDown += (o, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:

                        if (resultForm.IsDisposed == false) resultForm.Dispose();
                        if (QuizContents.Count == 0) return;

                        List<QuizResult> tmp = new List<QuizResult>();

                        foreach (QuizContents qc in QuizContents)
                        {
                            tmp.Add(new QuizResult(qc.Quiz, string.Join("\n", CoreProcess.ParseAnswer(qc.CorrectAnswer)), "", qc.QuizNum, qc.Supplement));
                        }

                        resultForm = new ResultForm(tmp, this)
                        {
                            Text = "Lista de Pruebas",
                            ShowIcon = false
                        };

                        resultForm.Show();

                        break;
                }
            };

            #endregion
        }

        #endregion

        #region イベント

        // 正解数
        int correctAnswerNum = 0;

        CancellationTokenSource cts = new CancellationTokenSource();
        static object lockObject = new object();

        // 解答ボタンクリック(responder)
        private void btnAnswer_Click(object sender, EventArgs e)
        {
            if (ws == null) return;

            cts.Cancel();

            txtConsole.Text = "";

            bool isCorrect = CoreProcess.CheckAnswer(txtAnswer.Text, QuizContents[curProgress].CorrectAnswer);
            DisplayResult(isCorrect ? "¡Sí!" : "¡No!", 1000);

            txtConsole.Text = CoreProcess.adopt_str;

            if (isCorrect)
            {
                correctAnswerNum++;
            }
            else
            {
                // 完答モードの時はやり直し
                if (optionTSMI_prueba.Checked == false) return;
            }

            if (optionTSMI_progresoVisual.Checked)
            {
                label_progress[curProgress % 10].Text = isCorrect ? progressStateCharacter_Correct : progressStateCharacter_Incorrect;
                //label_progress[curProgress % 10].ForeColor = colorOffProgress;
                progress_state[UtilityFunction.Suelo(curProgress, 10)][curProgress % 10] = isCorrect ? 1 : 2;
            }

            // 解答を保存
            respuestas.Add(txtAnswer.Text == "" ? "NONE" : txtAnswer.Text);
            txtAnswer.Text = "";

            QuizResult.Add(new QuizResult(QuizContents[curProgress].Quiz, QuizContents[curProgress].CorrectAnswer, txtAnswer.Text, QuizContents[curProgress].QuizNum, QuizContents[curProgress].Supplement, isCorrect));


            int endQuizNum = optionTSMI_progresoVisual.Checked ? QuizFileConfig.QuizNum - 1 : QuizFileConfig.MaxQuizNum - 1;

            // クイズ終了？
            if (curProgress == endQuizNum || curProgress == MaxRow - 1)
            {
                //tokenSource.Cancel();

                btnAnswer.Enabled = false;

                // 問題数と正解問題数が同じでpruebaモードのとき
                if ((endQuizNum + 1 == correctAnswerNum) && optionTSMI_prueba.Checked)
                //if(true)
                {
                    DisplayResult("PERFECTO!", 5000);

                    // 綺麗な対処ではないが、のちのRefreshDisplayで++される使用のためここで調整
                    // PERFECTOしたあとは最終回数を表示していたい
                    PruebaChallengeCount--;

                    // 練習が1章だけならPRUEBA達成を記録する
                    if (QuizFileConfig.MinChapter == QuizFileConfig.MaxChapter)
                    {
                        string path = $"{SettingManager.RomConfig.QuizFilePath}\\progreso\\{currentQuizFile}_p.csv";

                        // 進捗ファイルに書き込む
                        if (File.Exists(path))
                        {
                            string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                            string[] sp = lines[QuizFileConfig.MinChapter - 1].Split(',');
                            string today = DateTime.Now.ToString("yyyy/MM/dd");

                            // 同日のPruebaは記録しない
                            // 日跨ぎのPruebaを重視するため(Ebbinghaus)
                            if (sp[0] != today)
                            {
                                sp[0] = today;
                                sp[1] = (int.Parse(sp[1]) + 1).ToString("D3");
                                lines[QuizFileConfig.MinChapter - 1] = string.Join(",", sp);

                                File.WriteAllLines(path, lines);
                            }
                        }
                    }
                    else
                    {
                        // 練習が複数の章にわたるときは、どこからどこまでかを記録する

                        string path = $"{SettingManager.RomConfig.QuizFilePath}\\progreso\\{currentQuizFile}_intercontinental.txt";
                        string write_text = $"{QuizFileConfig.MinChapter}~{QuizFileConfig.MaxChapter}";

                        if (File.Exists(path))
                        {
                            using(StreamWriter sw = File.AppendText(path))
                            {
                                sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd")}:{write_text}");
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = File.CreateText(path))
                            {
                                sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd")}:{write_text}");
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
            if (ws == null) return;

            List<string> processedAnswer = CoreProcess.ParseAnswer(QuizContents[curProgress].CorrectAnswer);

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
            SettingForm s = new SettingForm(ArchivosDeLengua, toolStripQuizFile.Text)
            {
                ShowInTaskbar = false,
                ShowIcon = false
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                ParseFile();
                InitQuiz(true);
            }
        }

        // QuizInfo
        private void optionTSMI_quizInfo_Click(object sender, EventArgs e)
        {
            if (MessageForm_quizInfo.IsDisposed == false) MessageForm_quizInfo.Dispose();
            if (ws == null) return;

            List<string> input_h = new List<string>() { "Quiz Number", "Quiz Title" };
            List<string> input_d = new List<string>() { QuizContents[curProgress].QuizNum, QuizContents[curProgress].ChapterTitle };
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

        #endregion

        #region Herramientas

        private void toolTSMI_pruebaLista_Click(object sender, EventArgs e)
        {
            if (resultForm.IsDisposed == false) resultForm.Dispose();
            if (QuizContents.Count == 0) return;

            List<QuizResult> tmp = new List<QuizResult>();

            foreach (QuizContents qc in QuizContents)
            {
                tmp.Add(new QuizResult(qc.Quiz, string.Join("\n", CoreProcess.ParseAnswer(qc.CorrectAnswer)), "", qc.QuizNum, qc.Supplement));
            }

            tmp = tmp.OrderBy(q => int.Parse(q.QuizNum)).ToList();

            resultForm = new ResultForm(tmp, this)
            {
                Text = "Lista de Pruebas",
                ShowIcon = false
            };

            resultForm.Show();
        }

        private void toolTSMI_chapterList_Click(object sender, EventArgs e)
        {
            if (MessageForm_chapterList.IsDisposed == false) MessageForm_chapterList.Dispose();
            if (ws == null) return;

            int chapter = ws.LastRowUsed().RowNumber() / 10;

            List<string> chapter_list = new List<string>();

            for (int i = 0; i < chapter; i++)
            {
                chapter_list.Add($"{i + 1}:{ws.Cell(i * 10 + 1, 4).Value.ToString()}");
            }

            MessageForm_chapterList = new MessageForm(chapter_list, "Lista de capítulos", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                ShowIcon = false
            };

            MessageForm_chapterList.Show();
        }

        private void toolTSMI_translate_Click(object sender, EventArgs e)
        {
            if (MessageForm_traducir.IsDisposed == false) MessageForm_traducir.Dispose();
            if (langType == "") return;
            if (txtAnswer.Text == "") return;

            string traduccion = Translate.DoTransrate(txtAnswer.Text, langType);

            List<string> mostrar = new List<string>();

            mostrar.Add(traduccion);

            MessageForm_traducir = new MessageForm(mostrar, "TRADUCCIÓN", MessageForm.TipoDeUbicacion.CENTRO, this)
            {
                ShowIcon = false
            };

            MessageForm_traducir.Show();
        }

        #endregion

        #endregion

        #endregion
    }
}
