using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        List<List<Label>> labels = new List<List<Label>>();
        List<Label> labels_bar = new List<Label>();
        List<Tuple<List<string>, string>> label_info = new List<Tuple<List<string>, string>>();

        Label lblQuizNum;
        TextBox txtChapterTitle;
        TextBox txtChapterExample;
        Label lblExercise;

        ResultForm resultForm = new ResultForm();
        MessageForm MessageForm_respuesta = new MessageForm();
        MessageForm MessageForm_traducir = new MessageForm();

        // Resultadoに表示する為に蓄積するやつ
        List<QuizResult> QuizResult = new List<QuizResult>();
        // 現在の読込ファイルの設定
        public static QuizFileConfig QuizFileConfig;
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

        public MainForm()
        {
            InitializeComponent();

#if DEBUG
            Text += " [debug]";
#endif

            chboxComplete.Checked = true;

            lblResult.Visible = false;
            label1.Visible = false;
            btnAnswer.Enabled = false;

            resultForm.Dispose();
            MessageForm_respuesta.Dispose();
            MessageForm_traducir.Dispose();

            txtAnswer.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtQuiz.KeyDown += TextBoxKeyDown_AvoidBeep;
            txtConsole.KeyDown += TextBoxKeyDown_AvoidBeep;

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

                // CTRLキーとエンターキーが同時に押されたかを確認
                if (ctrlPressed && enterPressed)
                {
                    e.Handled = true;
                    btnTranslate.PerformClick();
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
                        case Keys.Q:
                            // Empezar
                            IsKeyDown = true;
                            InitQuiz(true);
                            break;
                        case Keys.N:
                            if (shiftPressed) MoveQuiz(true);
                            break;
                        case Keys.B:
                            if (shiftPressed)  MoveQuiz(false);
                            break;
                    }
                }
                else
                {
                    if(e.KeyCode == Keys.F1)
                    {
                        btnTranslate.PerformClick();
                    }
                }
            };

            KeyUp += (o, e) =>
            {
                IsKeyDown = false;
            };

            lblQuizNum = new Label
            {
                Location = new Point(btnShowAnswer.Location.X, btnShowAnswer.Location.Y + btnShowAnswer.Size.Height),
                //Text = "30",
                //Size = new Size(20, 20),
                AutoSize = true,
                Visible = false,
                Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128)
            };

            txtChapterTitle = new TextBox
            {
                Location = new Point(btnShowAnswer.Location.X, btnShowAnswer.Location.Y + btnShowAnswer.Size.Height + 20),
                //Text = "12345678910111213141516171819201234567891011121314151617181920",
                Size = new Size(300, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false,
                Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128)
            };

            txtChapterExample = new TextBox
            {
                Location = new Point(btnShowAnswer.Location.X, btnShowAnswer.Location.Y + btnShowAnswer.Size.Height + 50),
                //Text = "12345678910111213141516171819201234567891011121314151617181920",
                Size = new Size(300, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false,
                Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128)
            };

            Controls.Add(lblQuizNum);
            Controls.Add(txtChapterTitle);
            Controls.Add(txtChapterExample);

            chboxQuizNum.CheckedChanged += CheckedChanged;
            chboxChapterTitle.CheckedChanged += CheckedChanged;
            chboxChapterExample.CheckedChanged += CheckedChanged;

            chboxQuizNum.CheckedChanged += (o, e) =>
            {
                lblQuizNum.Visible = (o as CheckBox).Checked;
            };
            chboxChapterTitle.CheckedChanged += (o, e) =>
            {
                txtChapterTitle.Visible = (o as CheckBox).Checked;
            };
            chboxChapterExample.CheckedChanged += (o, e) =>
            {
                txtChapterExample.Visible = (o as CheckBox).Checked;
            };

            chboxComplete.CheckedChanged += (o, e) =>
            {
                if (!IsLoaded) return;
                InitQuiz(true);
            };

            chboxExercise.CheckedChanged += (o, e) =>
            {
                if (!IsLoaded) return;
                InitQuiz(true);
            };

            btnShowAnswer.MouseDown += (o, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    InitQuiz(true);
                }
            };

            startToolStripMenuItem.MouseDown += (o, e) =>
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

            siguienteToolStripMenuItem.MouseDown += (o, e) =>
            {
                if (ws == null) return;

                switch (e.Button)
                {
                    case MouseButtons.Middle:
                        int chapter = ws.LastRowUsed().RowNumber() / 10;

                        string str = "";

                        for (int i = 0; i < chapter; i++)
                        {
                            str += $"{i + 1}:{ws.Cell(i * 10 + 1, 4).Value.ToString()}\r\n";
                        }

                        MessageBox.Show(str);

                        break;
                    case MouseButtons.Right:
                        MoveQuiz(false);
                        break;
                }
            };

            chboxResult.MouseDown += (o, e) =>
            {
                switch (e.Button)
                {
                    // Resultadoフォームの表示
                    case MouseButtons.Middle:
                    case MouseButtons.Right:

                        if (resultForm.IsDisposed == false) resultForm.Dispose();
                        if (QuizContents.Count == 0) return;

                        List<QuizResult> tmp = new List<QuizResult>();

                        foreach (QuizContents qc in QuizContents)
                        {
                            tmp.Add(new QuizResult(qc.Quiz, string.Join("\n", CoreProcess.ParseAnswer(qc.CorrectAnswer)), "", qc.QuizNum, qc.Supplement));
                        }

                        if(e.Button == MouseButtons.Right)
                        {
                            tmp = tmp.OrderBy(q => int.Parse(q.QuizNum)).ToList();
                        }

                        resultForm = new ResultForm(tmp, this)
                        {
                            ShowIcon = false
                        };

                        resultForm.Show();

                        break;
                }
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

                tt.SetToolTip(chboxComplete, "完答");
                tt.SetToolTip(chboxExercise, "エクササイズの種類");
                tt.SetToolTip(chboxResult, "Resultadoを表示するか");
                tt.SetToolTip(chboxQuizNum, "クイズ番号");
                tt.SetToolTip(chboxChapterTitle, "章タイトル");
                tt.SetToolTip(chboxChapterExample, "章の例文");
                tt.SetToolTip(btnTranslate, "traducción");
            };

            FormClosing += (o, e) =>
            {
                SettingManager.InputCache.Complete = chboxComplete.Checked;
                SettingManager.InputCache.Exercise = chboxExercise.Checked;
                SettingManager.InputCache.Result = chboxResult.Checked;
                SettingManager.InputCache.QuizNum = chboxQuizNum.Checked;
                SettingManager.InputCache.ChapterTitle = chboxChapterTitle.Checked;
                SettingManager.InputCache.ChapterExample = chboxChapterExample.Checked;
                SettingManager.InputCache.QuizFilePathIndex = toolStripQuizFile.SelectedIndex;

                CommonFunction.XmlWrite(SettingManager.InputCache, "cache.xml");
            };

            Point loc = chboxComplete.Location;
            int cw = 20;

            chboxExercise.Location = new Point(loc.X + cw, loc.Y);
            chboxResult.Location = new Point(loc.X + cw * 2, loc.Y);
            chboxQuizNum.Location = new Point(loc.X + cw * 3, loc.Y);
            chboxChapterTitle.Location = new Point(loc.X + cw * 4, loc.Y);
            chboxChapterExample.Location = new Point(loc.X + cw * 5, loc.Y);
            btnTranslate.Location = new Point(loc.X + cw * 6, loc.Y);

            LoadConfig();

            ParseFile();

            QuizFileConfig = SettingManager.AppConfig.quizFileConfig;
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
        bool check1 = false, check2 = false, check3 = false, result = false;
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

                check1 = chboxChapterTitle.Checked;
                check2 = chboxChapterExample.Checked;

                chboxChapterTitle.Checked = false;
                chboxChapterExample.Checked = false;
                chboxQuizNum.Checked = false;

                ba = btnAnswer.Enabled;
                btnAnswer.Enabled = false;
                btnShowAnswer.Enabled = false;
                chboxComplete.Enabled = false;
                chboxExercise.Enabled = false;
                chboxResult.Enabled = false;
                chboxQuizNum.Enabled = false;
                chboxChapterTitle.Enabled = false;
                chboxChapterExample.Enabled = false;
                btnTranslate.Enabled = false;

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
                chboxComplete.Enabled = true;
                chboxExercise.Enabled = true;
                chboxResult.Enabled = true;
                chboxQuizNum.Enabled = true;
                chboxChapterTitle.Enabled = true;
                chboxChapterExample.Enabled = true;
                btnTranslate.Enabled = true;

                chboxChapterTitle.Checked = check1;
                chboxChapterExample.Checked = check2;
                chboxQuizNum.Checked = check3;

                if (resultForm.IsDisposed == false) resultForm.Visible = result;
                txtAnswer.Focus();
            }

            txtQuiz.ReadOnly = !txtQuiz.ReadOnly;
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

            chboxComplete.Checked = SettingManager.InputCache.Complete;
            chboxExercise.Checked = SettingManager.InputCache.Exercise;
            chboxResult.Checked = SettingManager.InputCache.Result;
            chboxQuizNum.Checked = SettingManager.InputCache.QuizNum;
            chboxChapterTitle.Checked = SettingManager.InputCache.ChapterTitle;
            chboxChapterExample.Checked = SettingManager.InputCache.ChapterExample;
            if (toolStripQuizFile.Items.Count > SettingManager.InputCache.QuizFilePathIndex)
            {
                toolStripQuizFile.SelectedIndex = SettingManager.InputCache.QuizFilePathIndex;
            }

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

        // クイズ開始
        private void InitQuiz(bool manual)
        {
            // 非表示中はクイズを始めない
            if (label1.Visible)
            {
                MessageBox.Show("No se puede continuar con la prueba mientras está oculto");
                return;
            }

            btnAnswer.Enabled = true;

            currentQuizFile = toolStripQuizFile.SelectedItem.ToString();
            string filePath = $"{SettingManager.RomConfig.QuizFilePath}\\{toolStripQuizFile.SelectedItem.ToString()}.xlsx";

            OpenExcel(filePath);
            if (manual) txtConsole.Text = "";
            curProgress = -1;
            correctAnswerNum = 0;
            current_label_group = 0;
            QuizResult.Clear();
            QuizContents.Clear();

            // 進捗表示作成
            CreateQuizProgress();

            labels.ForEach(l1 => l1.ForEach(l2 => l2.Text = "○"));

            Text = $"MBR [{QuizFileConfig.MinChapter * 10 - 9}~{QuizFileConfig.MaxChapter * 10}]";

            // 完答モードでないとき
            if (!chboxComplete.Checked)
            {
                Text += " prueba ";

                // 練習が1章だけならPRUEBA回数を表示する
                if(QuizFileConfig.MinChapter == QuizFileConfig.MaxChapter)
                {
                    string path = $"{SettingManager.RomConfig.QuizFilePath}\\progreso\\{currentQuizFile}_p.csv";

                    if (File.Exists(path))
                    {
                        string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));

                        Text += int.Parse(lines[QuizFileConfig.MinChapter - 1].Split(',')[1]).ToString();
                    }
                }
            } 

            // nからmまでの整数のリストを作成
            List<int> numberList = new List<int>();
            for (int i = QuizFileConfig.MinChapter * 10 - 9; i <= QuizFileConfig.MaxChapter * 10; i++)
            {
                numberList.Add(i);
            }

            // リストをシャッフルしてランダムな数列を作成
            List<int> randomSequence = UtilityFunction.ShuffleList(numberList);

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

            ShowQuestion();
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

            // 現在の問題のインデックスから進捗ラベルのどこのグループに属するかを算出
            current_label_group = UtilityFunction.Suelo(curProgress);

            if (chboxExercise.Checked)
            {
                int totalNum = QuizFileConfig.MaxQuizNum > MaxRow ? MaxRow : QuizFileConfig.MaxQuizNum;
                lblExercise.Text = $"{curProgress + 1}/{totalNum}";
            }
            else
            {
                labels[current_label_group][curProgress % 10].ForeColor = Color.Red;
                labels[current_label_group][curProgress % 10].Text = "★";

                ProgressRedrow();
            }

            txtQuiz.Text = QuizContents[curProgress].Quiz;
            lblQuizNum.Text = QuizContents[curProgress].QuizNum;
            txtChapterTitle.Text = QuizContents[curProgress].ChapterTitle;
            txtChapterExample.Text = QuizContents[curProgress].ChapterExample;
        }

        // 進捗表示を作る
        private void CreateQuizProgress()
        {
            labels.ForEach(l1 => l1.ForEach(l2 => { if (Controls.Contains(l2)) Controls.Remove(l2); }));
            labels_bar.ForEach(l1 => { if (Controls.Contains(l1)) Controls.Remove(l1); });

            labels.Clear();
            labels_bar.Clear();
            label_info.Clear();

            if (Controls.Contains(lblExercise))
            {
                Controls.Remove(lblExercise);
            }

            if (chboxExercise.Checked == false)
            {
                int labelSize = 15;

                for(int i = 0; i <= UtilityFunction.Suelo(QuizFileConfig.QuizNum - 1); i++)
                {
                    labels.Add(new List<Label>());

                    Label l = new Label
                    {
                        Location = new Point(txtAnswer.Location.X + (i % 10) * labelSize, txtAnswer.Location.Y + txtAnswer.Size.Height),
                        Text = "―",
                        Size = new Size(labelSize, labelSize - 5),
                        Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128)
                    };

                    l.Click += Label_bar_Click;
                    l.MouseHover += Label_hover;
                    l.MouseLeave += Label_leave;

                    Controls.Add(l);
                    labels_bar.Add(l);
                }

                int group_num = 0;

                for (int i = 0; i < QuizFileConfig.QuizNum; i++)
                {
                    group_num = UtilityFunction.Suelo(i);

                    Label l = new Label
                    {
                        Location = new Point(txtAnswer.Location.X + (i % 10) * labelSize, txtAnswer.Location.Y + txtAnswer.Size.Height + 10),
                        Text = "○",
                        Size = new Size(labelSize, labelSize),
                        Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128)
                    };

                    l.Click += LabelClick;
                    l.Visible = false;

                    Controls.Add(l);
                    labels[group_num].Add(l);
                }

                labels[0].ForEach(l => l.Visible = true);
                labels_bar[0].ForeColor = Color.Red;
            }
            else
            {
                lblExercise = new Label
                {
                    Location = new Point(txtAnswer.Location.X, txtAnswer.Location.Y + txtAnswer.Size.Height + 10),
                    Text = "100/100",
                    //Size = new Size(labelSize, labelSize),
                    Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128)
                };

                Controls.Add(lblExercise);
            }
        }

        // 進捗表示を更新する
        private void ProgressRedrow()
        {
            labels_bar.ForEach(l => l.ForeColor = Color.Black);

            labels_bar[current_label_group].ForeColor = Color.Red;

            labels.ForEach(l1 => l1.ForEach(l2 => l2.Visible = false));
            labels[current_label_group].ForEach(l => l.Visible = true);
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

        // 補足情報を表示するときにウィンドウサイズを変える
        private void CheckedChanged(object o, EventArgs e)
        {
            List<CheckBox> cs = new List<CheckBox> { chboxQuizNum, chboxChapterTitle, chboxChapterExample };
            int cnt = cs.Count(c => c.Checked == true);

            if ((o as CheckBox).Checked)
            {
                if (cnt == 1)
                {
                    Size = new Size(Size.Width, Size.Height + 70);
                }
                return;
            }

            if (cnt == 0)
            {
                Size = new Size(Size.Width, Size.Height - 70);
            }
        }

        int current_label_group = 0;

        private void LabelClick(object sender, EventArgs e)
        {
            int index = labels[current_label_group].IndexOf(sender as Label) + (10 * current_label_group);

            if (label_info.Count <= index) return;

            List<string> tmp = new List<string>();

            tmp = tmp.Concat(label_info[index].Item1).Concat(new List<string> { "───────", label_info[index].Item2 }).ToList();

            MessageForm s = new MessageForm(tmp, "FE DE ERRATAS", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            s.Show();
        }

        private void Label_bar_Click(object sender, EventArgs e)
        {
            int index = labels_bar.IndexOf(sender as Label);

            current_label_group = index;

            ProgressRedrow();
        }

        private void Label_hover(object o, EventArgs e)
        {
            Label l = o as Label;

            //l.Font = new Font("メイリオ", 10F, FontStyle.Bold, GraphicsUnit.Point, 128);
            l.Text = "凸";
        }

        private void Label_leave(object o, EventArgs e)
        {
            Label l = o as Label;

            //l.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            l.Text = "―";
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

            bool res = CoreProcess.CheckAnswer(txtAnswer.Text, QuizContents[curProgress].CorrectAnswer);
            DisplayResult(res ? "¡Sí!" : "¡No!", 1000);

            txtConsole.Text = CoreProcess.adopt_str;

            if (res)
            {
                if (!chboxExercise.Checked) labels[current_label_group][curProgress % 10].Text = "■";
                correctAnswerNum++;
            }
            else
            {
                if (chboxComplete.Checked) return;
                if (!chboxExercise.Checked) labels[current_label_group][curProgress % 10].Text = "×";
            }

            // 進捗ラベルに紐づく解答を追加
            // ShowAnswerで複数の表現があるやつを分離する
            label_info.Add(Tuple.Create(CoreProcess.ParseAnswer(QuizContents[curProgress].CorrectAnswer), txtAnswer.Text == "" ? "NONE" : txtAnswer.Text));
            txtAnswer.Text = "";

            QuizResult.Add(new QuizResult(QuizContents[curProgress].Quiz, QuizContents[curProgress].CorrectAnswer, txtAnswer.Text, QuizContents[curProgress].QuizNum, QuizContents[curProgress].Supplement, res));

            if (!chboxExercise.Checked) labels[current_label_group][curProgress % 10].ForeColor = Color.Black;

            int endQuizNum = chboxExercise.Checked ? QuizFileConfig.MaxQuizNum - 1 : QuizFileConfig.QuizNum - 1;

            if (curProgress == endQuizNum || curProgress == MaxRow - 1)
            {
                //tokenSource.Cancel();

                btnAnswer.Enabled = false;

                // 問題数と正解問題数が同じで完答モードでないとき
                if ((endQuizNum + 1 == correctAnswerNum) && !chboxComplete.Checked)
                //if(true)
                {
                    DisplayResult("PERFECTO!", 5000);

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
                }
                else
                {
                    DisplayResult("¡Buen trabajo!", 5000);
                }

                if (chboxResult.Checked)
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

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
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

        // Traducir
        private void btnTranslate_Click(object sender, EventArgs e)
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

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitQuiz(true);
        }

        private void siguienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveQuiz(true);
        }

        #endregion
    }
}
