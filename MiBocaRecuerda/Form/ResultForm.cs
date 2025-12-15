using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiBocaRecuerda
{
    public partial class ResultForm : Form
    {
        public ResultForm() { }
        private Dictionary<int, string> Supplement = new Dictionary<int, string>();

        private ClassResize _form_resize;

        private List<QuizResult> qr = new List<QuizResult>();
        private MainForm mf;

        private DataGridViewTextBoxColumn col_num;
        private DataGridViewTextBoxColumn col_quiz;
        private DataGridViewTextBoxColumn col_correct;

        private bool IsAuto = false;

        // 答えを含むコピー用
        private Dictionary<int, (string quiz, string answer)> RespuestaCopy = new Dictionary<int, (string quiz, string answer)>();

        public ResultForm(List<QuizResult> _qr, MainForm _mf)
        {
            InitializeComponent();

            qr = _qr;
            mf = _mf;

            foreach (QuizResult r in qr)
            {
                Supplement.Add(r.QuizNum, r.Supplement);
            }

            CreateControls();

            RegisterEvent();
        }

        // QuizContentsから表を作る用(つまり結果ではなく、答えの一覧)
        public ResultForm(List<QuizContents> qc, MainForm _mf, bool isOrder)
        {
            InitializeComponent();

            mf = _mf;

            int cnt = 0;
            List<string> parseAnswer = new List<string>();

            if (isOrder) qc = qc.OrderBy(q => q.QuizNum).ToList();

            foreach (QuizContents c in qc)
            {
                cnt++;

                parseAnswer = CoreProcess.ParseAnswer(c.CorrectAnswer);

                // 答え全体コピー用を生成する(「答え」は複数パターンある場合があるのでDGVの表示をそのまま使えない)
                if(parseAnswer.Count == 1)
                {
                    // 解答パターンが複数ない場合
                    RespuestaCopy[cnt] = (c.Quiz, parseAnswer[0]);
                }
                else
                {
                    // 解答パターンが複数
                    for (int i = 0; i < parseAnswer.Count; i++)
                    {
                        // 下位16ビットは問題番号として17ビット以降を解答パターン通番にする
                        RespuestaCopy[cnt | ((i + 1) << 16)] = (c.Quiz, parseAnswer[i]);
                    }
                }

                qr.Add(new QuizResult(c.Quiz, string.Join("\n", parseAnswer), "", c.QuizNum, c.Supplement));
            }

            foreach (QuizResult r in qr)
            {
                Supplement.Add(r.QuizNum, r.Supplement);
            }

            CreateControls();

            RegisterEvent();
        }

        private void CreateControls()
        {
            dgv.Font = new Font("MeiryoKe_Console", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            //dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            col_num = new DataGridViewTextBoxColumn
            {
                Name = "num",
                HeaderText = "No",
                Width = 30,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = true
            };

            col_quiz = new DataGridViewTextBoxColumn
            {
                Name = "quiz",
                HeaderText = "Prueba",
                Width = dgv.Width / 2 - 1,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            col_correct = new DataGridViewTextBoxColumn
            {
                Name = "correct",
                HeaderText = "Respuesta Correcta",
                Width = dgv.Width / 2 - 1,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            col_correct.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            col_quiz.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            col_correct.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgv.Columns.Add(col_num);
            dgv.Columns.Add(col_quiz);
            dgv.Columns.Add(col_correct);

            for (int cnt = 0; cnt < qr.Count; cnt++)
            {
                dgv.Rows.Add();
                dgv.Rows[cnt].Cells["num"].Value = qr[cnt].QuizNum;
                dgv.Rows[cnt].Cells["quiz"].Value = qr[cnt].Quiz;
                dgv.Rows[cnt].Cells["correct"].Value = qr[cnt].CorrectAnswer;
                if (qr[cnt].Result == false)
                {
                    dgv.Rows[cnt].DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                // 補足があるやつは補足の目印をつける
                if (qr[cnt].Supplement != "")
                {
                    dgv.Rows[cnt].Cells["quiz"].Value += " *";
                }
            }
        }

        private void RegisterEvent()
        {
            Load += (o, e) =>
            {
                int width_num = AutoSizeColumnWidth(dgv, 0);
                int width_quiz = AutoSizeColumnWidth(dgv, 1);
                int width_correct = AutoSizeColumnWidth(dgv, 2);

                Size = new Size(width_num + width_quiz + width_correct + 20, Size.Height);

                col_num.Width = width_num;
                col_quiz.Width = width_quiz;
                col_correct.Width = width_correct;

                BaseAreaInfo baseArea = UtilityFunction.GetBaseArea();

                int move_right = mf.Location.X + mf.Width + Width;
                int move_left = mf.Location.X - Width;

                Console.WriteLine($"{baseArea.MaxX}, {mf.Location.X + mf.Width + Width}");

                if (move_right < baseArea.MaxX)
                {
                    // 右に表示する余地があるとき
                    Location = new Point(move_right - Width, mf.Location.Y);
                }
                else if (move_left > baseArea.MinX)
                {
                    // 左に表示する余地があるとき
                    Location = new Point(move_left, mf.Location.Y);
                }
                // 右にも左にも表示できないときはデフォルト位置
            };

            Shown += (o, e) =>
            {
                _form_resize = new ClassResize(this);

                AdjustRowHeight();
            };

            SizeChanged += (o, e) =>
            {
                if (IsAuto) return;

                if (_form_resize != null) _form_resize._resize(false);

                AdjustRowHeight();
            };

            KeyPreview = !KeyPreview;

            KeyDown += (o, e) =>
            {
                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

                if (ctrlPressed)
                {
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

                            // KeyCodeをToStringすると"Dn"がでてくるから2文字目を取ってcharからstringにして
                            // 9+してmod10したら1+9 mod 10 =0だし0+9 mod 10 = 9になる
                            int num = (int.Parse(e.KeyCode.ToString()[1].ToString()) + 9) % 10;

                            ShowSupplement(num);

                            break;
                        case Keys.Q:
                            Close();
                            break;
                    }
                }
            };
        }

        private string cellValue = "";
        private string quizNum = "";
        private int ColumnIndex;

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // クリックされたセルが有効なセルかを確認
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgv.ClearSelection();

                // セルの値を取得
                cellValue = dgv[e.ColumnIndex, e.RowIndex].Value?.ToString().Replace("*", "");
                ColumnIndex = e.ColumnIndex;
                quizNum = dgv[0, e.RowIndex].Value?.ToString();
                dgv.Rows[e.RowIndex].Selected = true;

            }
            else
            {
                return;
            }

            switch (e.Button)
            {
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:

                    if (dgv.SelectedRows.Count == 0) return;

                    contextMenuStrip1.Show(Cursor.Position);

                    // 補足の有無を末尾の*で判断
                    CMS_supl.Enabled = dgv[1, dgv.SelectedRows[0].Index].Value.ToString().EndsWith("*");

                    break;
            }
        }

        private void ShowSupplement(int RowIndex)
        {
            int quizNum = int.Parse(dgv[0, RowIndex].Value?.ToString());
            string japones = dgv[1, RowIndex].Value?.ToString().Replace("*", "");
            string correcto = dgv[2, RowIndex].Value?.ToString();

            // 補足がある場合
            if (Supplement[quizNum] != "")
            {
                List<string> tmp = new List<string>
                            {
                                japones,
                                correcto,
                                "───────"
                            };
                tmp.AddRange(ParseXML.ConvertTextWithTable(Supplement[quizNum]).Split('\n'));

                MessageForm s = new MessageForm(tmp, $"Suplemento - {quizNum}", MessageForm.TipoDeUbicacion.PARENT_LINE, this)
                {
                    ShowIcon = false
                };

                s.Show();
            }
        }

        private void AdjustRowHeight()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Height += 10;
            }
        }

        private int AutoSizeColumnWidth(DataGridView grid, int column)
        {
            //DataGridのGraphicsを取得
            Graphics g = Graphics.FromHwnd(grid.Handle);

            //すべてのセルを調べて、一番広い幅を取得
            StringFormat sf =
                new StringFormat(StringFormat.GenericTypographic);
            DataTable dt = ((DataTable)grid.DataSource);
            int rowsCount = grid.Rows.Count;
            float maxWidth = 0;
            for (int i = 0; i < rowsCount; i++)
            {
                string text = grid[column, i].Value.ToString();
                //text = text.Replace("\n", "");
                //maxWidth = Math.Max(g.MeasureString(text, grid.Font, 10000, sf).Width, maxWidth);
                maxWidth = Math.Max(TextRenderer.MeasureText(text, grid.Font).Width, maxWidth);
            }

            //破棄
            g.Dispose();

            //幅の変更
            return (int)maxWidth + 16;
        }

        #region CMS

        // 補足を表示
        private void CMS_supl_Click(object sender, EventArgs e)
        {
            int index = dgv.SelectedRows[0].Index;

            ShowSupplement(index);
        }

        // 指定箇所をコピー
        private void CMS_copy_designate_Click(object sender, EventArgs e)
        {
            // セルの値をクリップボードにコピー
            if (!string.IsNullOrEmpty(cellValue))
            {
                Clipboard.SetText(cellValue);

                MessageBox.Show($"{(ColumnIndex == 1 ? "問題" : ColumnIndex == 2 ? "答え" : "???")}をコピー");
            }
        }

        // 表全体をコピー
        private void CMS_copy_all_Click(object sender, EventArgs e)
        {
            string quiz, answer;
            List<string> ret = new List<string>();

            foreach (var rc in RespuestaCopy)
            {
                quiz = Regex.Replace(rc.Value.quiz, @"\r\n|\r|\n", "");
                answer = Regex.Replace(rc.Value.answer, @"\r\n|\r|\n", "");

                // 0xffff0000の部分にビットがある場合は、解答パターンが複数あるとき
                if ((rc.Key & 0xffff0000) != 0)
                {
                    ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}\t{quiz}\t{answer}");
                }
                else
                {
                    ret.Add($"{rc.Key}\t{quiz}\t{answer}");
                }
            }

            Clipboard.SetText(string.Join("\r\n", ret));

            MessageBox.Show("表全体をコピー");
        }

        // 問題全体をコピー
        private void CMS_copy_quiz_all_Click(object sender, EventArgs e)
        {
            string quiz;
            List<string> ret = new List<string>();
            int cnt = 1;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                quiz = row.Cells[1].Value.ToString();

                quiz = quiz.TrimEnd('*');
                quiz = Regex.Replace(quiz, @"\r\n|\r|\n", "");

                ret.Add($"{cnt++}\t{quiz}");
            }

            Clipboard.SetText(string.Join("\r\n", ret));

            MessageBox.Show("問題全体をコピー");
        }

        // 答え全体をコピー
        private void CMS_copy_answer_all_Click(object sender, EventArgs e)
        {
            string answer;
            List<string> ret = new List<string>();

            foreach (var rc in RespuestaCopy)
            {
                answer = Regex.Replace(rc.Value.answer, @"\r\n|\r|\n", "");

                // 0xffff0000の部分にビットがある場合は、解答パターンが複数あるとき
                if ((rc.Key & 0xffff0000) != 0)
                {
                    ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}\t{answer}");
                }
                else
                {
                    ret.Add($"{rc.Key}\t{answer}");
                }
            }

            Clipboard.SetText(string.Join("\r\n", ret));

            MessageBox.Show("答え全体をコピー");
        }

        // 編集
        private void CMS_edit_Click(object sender, EventArgs e)
        {
            EditDBForm edb = new EditDBForm(mf.currentFilePath, int.Parse(quizNum));

            if(!edb.IsDisposed) edb.ShowDialog();
        }

        // クイズ非表示
        private void CMS_quiz_hide_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            // IsAutoをONにしてサイズ変更しないと想定外にフォントサイズが変更されてしまう
            IsAuto = true;
            ToggleColumnVisibility("quiz", item.Checked);
            IsAuto = false;
            _form_resize.UpdateFormSize(this);

            item.Checked = !item.Checked;
        }

        private void ToggleColumnVisibility(string columnName, bool visible)
        {
            // 対象列の表示・非表示を切り替え
            dgv.Columns[columnName].Visible = visible;

            int adjustWidth = (visible ? 1 : -1) * dgv.Columns[columnName].Width;

            // 表示(非表示)した分を調整する
            Size = new Size(Size.Width + adjustWidth, Size.Height);
        }

        #endregion
    }
}
