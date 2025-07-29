using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace MiBocaRecuerda
{
    public partial class ResultForm : Form
    {
        public ResultForm() { }
        private Dictionary<string, string> Supplement = new Dictionary<string, string>();

        private ClassResize _form_resize;

        private List<QuizResult> qr = new List<QuizResult>();
        private MainForm mf;

        private DataGridViewTextBoxColumn col_num;
        private DataGridViewTextBoxColumn col_quiz;
        private DataGridViewTextBoxColumn col_correct;

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

            foreach (QuizContents c in qc)
            {
                qr.Add(new QuizResult(c.Quiz, string.Join("\n", CoreProcess.ParseAnswer(c.CorrectAnswer)), "", c.QuizNum, c.Supplement));
            }

            if(isOrder) qr = qr.OrderBy(q => int.Parse(q.QuizNum)).ToList();

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
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
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
            dgv.SelectionChanged += (o, e) =>
            {
                //dgv.Rows[dgv.CurrentCell.RowIndex].Selected = false;
            };

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
                AdjustRowHeight();

                _form_resize = new ClassResize(this);
            };

            SizeChanged += (o, e) =>
            {
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

                if (_form_resize != null) _form_resize._resize(false);
            };

            dgv.FontChanged += (o, e) =>
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    // 最大化のときはDataGridViewAutoSizeRowsMode.DisplayedCellsに任せるしかないか…
                    return;
                }

                AdjustRowHeight();
            };

            //bool isShiftPressed = false;

            KeyPreview = !KeyPreview;

            KeyDown += (o, e) =>
            {
                //if (e.KeyCode == Keys.ShiftKey)
                //{
                //    isShiftPressed = true;
                //}

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

            //KeyUp += (o, e) =>
            //{
            //    if (e.KeyCode == Keys.ShiftKey)
            //    {
            //        isShiftPressed = false;
            //    }
            //};

            //Deactivate += (o, e) =>
            //{
            //    isShiftPressed = false;
            //};
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

                    break;
            }
        }

        private void ShowSupplement(int RowIndex)
        {
            string quizNum = dgv[0, RowIndex].Value?.ToString();
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
            // dgv.AutoSizeRowsModeの設定によって最適高さにされた高さを取得する
            List<int> row_heights = new List<int>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row_heights.Add(row.Height);
            }

            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // AutoSizeモードを無効にして最適高さに固定マージンを入れた高さにする
            foreach (var (item1, item2) in dgv.Rows.Cast<DataGridViewRow>().Zip(row_heights, (x, y) => (x, y)))
            {
                item1.Height = item2 + 10;
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

        private void CMS_supl_Click(object sender, EventArgs e)
        {
            int index = dgv.SelectedRows[0].Index;

            ShowSupplement(index);
        }

        private void CMS_copy_Click(object sender, EventArgs e)
        {
            // セルの値をクリップボードにコピー
            if (!string.IsNullOrEmpty(cellValue))
            {
                Clipboard.SetText(cellValue);

                MessageBox.Show($"{(ColumnIndex == 1 ? "問題" : ColumnIndex  == 2 ? "答え" : "???")}をコピー");
            }
        }

        private void CMS_edit_Click(object sender, EventArgs e)
        {
            EditDBForm edb = new EditDBForm(mf.currentFilePath, int.Parse(quizNum));

            if(!edb.IsDisposed) edb.ShowDialog();
        }
    }
}
