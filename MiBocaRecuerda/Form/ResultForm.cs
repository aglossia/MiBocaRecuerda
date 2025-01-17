using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ClosedXML.Excel;

namespace MiBocaRecuerda
{
    public partial class ResultForm : Form
    {
        public ResultForm() { }
        private Dictionary<string, string> Supplement = new Dictionary<string, string>();

        private ClassResize _form_resize;

        public ResultForm(List<QuizResult> qc, MainForm mf)
        {
            InitializeComponent();

            foreach (QuizResult r in qc)
            {
                Supplement.Add(r.QuizNum, r.Supplement);
            }

            dgv.Font = new Font("MeiryoKe_Console", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            DataGridViewTextBoxColumn col_num = new DataGridViewTextBoxColumn
            {
                Name = "num",
                HeaderText = "No",
                Width = 30,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn col_quiz = new DataGridViewTextBoxColumn
            {
                Name = "quiz",
                HeaderText = "Prueba",
                Width = dgv.Width / 2 - 1,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            DataGridViewTextBoxColumn col_correct = new DataGridViewTextBoxColumn
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

            for (int cnt = 0; cnt < qc.Count; cnt++)
            {
                dgv.Rows.Add();
                dgv.Rows[cnt].Cells["num"].Value = qc[cnt].QuizNum;
                dgv.Rows[cnt].Cells["quiz"].Value = qc[cnt].Quiz;
                dgv.Rows[cnt].Cells["correct"].Value = qc[cnt].CorrectAnswer;
                if(qc[cnt].Result == false)
                {
                    dgv.Rows[cnt].DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                // 補足があるやつは補足の目印をつける
                if(qc[cnt].Supplement != "")
                {
                    dgv.Rows[cnt].Cells["quiz"].Value += " *";
                }
            }

            dgv.SelectionChanged += (o, e) =>
            {
                dgv.Rows[dgv.CurrentCell.RowIndex].Selected = false;
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

                if(move_right < baseArea.MaxX)
                {
                    // 右に表示する余地があるとき
                    Location = new Point(move_right - Width, mf.Location.Y);
                }
                else if(move_left > baseArea.MinX)
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

                if (_form_resize != null) _form_resize._resize();
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

            bool isShiftPressed = false;

            KeyPreview = !KeyPreview;

            KeyDown += (o, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    isShiftPressed = true;
                }
            };

            KeyUp += (o, e) =>
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    isShiftPressed = false;
                }
            };

            Deactivate += (o, e) =>
            {
                isShiftPressed = false;
            };

            dgv.CellMouseClick += (o, e) =>
            {
                string cellValue = "";
                string quizNum = "";
                string japones = "";
                string correcto = "";

                // クリックされたセルが有効なセルかを確認
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // セルの値を取得
                    cellValue = dgv[e.ColumnIndex, e.RowIndex].Value?.ToString().Replace("*", "");
                    quizNum = dgv[0, e.RowIndex].Value?.ToString();
                    japones = dgv[1, e.RowIndex].Value?.ToString().Replace("*", "");
                    correcto = dgv[2, e.RowIndex].Value?.ToString();
                }
                else
                {
                    return;
                }

                switch (e.Button)
                {
                    case MouseButtons.Middle:

                        if (isShiftPressed)
                        {
                            try
                            {
                                string clip_str = Clipboard.GetText();

                                if(clip_str == "")
                                {
                                    MessageBox.Show("クリップボードに文字列がない");

                                    return;
                                }

                                using (FileStream fs = new FileStream(mf.currentFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                                {
                                    XLWorkbook workBook = new XLWorkbook(fs);
                                    IXLWorksheet ws = workBook.Worksheet(1);

                                    string existir = ws.Cell(int.Parse(quizNum), 6).Value.ToString();

                                    if (existir != "")
                                    {
                                        DialogResult result = MessageBox.Show("すでにSupplementがあるよ。上書きしますか？",
                                                                                "おっと",
                                                                                MessageBoxButtons.YesNo,
                                                                                MessageBoxIcon.Exclamation,
                                                                                MessageBoxDefaultButton.Button2);

                                        if (result == DialogResult.No)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            DialogResult result2 = MessageBox.Show("本当に上書きする？",
                                                                                "おっとっと",
                                                                                MessageBoxButtons.YesNo,
                                                                                MessageBoxIcon.Exclamation,
                                                                                MessageBoxDefaultButton.Button2);

                                            if (result == DialogResult.No)
                                            {
                                                return;
                                            }
                                        }
                                    }

                                    ws.Cell(int.Parse(quizNum), 6).Value = clip_str;
                                    workBook.Save();
                                    MessageBox.Show("Supplement書込完了");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            // セルの値をクリップボードにコピー
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                Clipboard.SetText(cellValue);
                            }
                        }

                        break;
                    case MouseButtons.Right:

                        // 補足がある場合
                        if(Supplement[quizNum] != "")
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

                        break;
                }
            };
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
    }
}
