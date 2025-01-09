using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class ResultForm : Form
    {
        public ResultForm() { }
        private Dictionary<string, string> Supplement = new Dictionary<string, string>();

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

            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
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
                Location = new Point(mf.Location.X + mf.Width, mf.Location.Y);
            };

            Shown += (o, e) =>
            {
                int width_num = AutoSizeColumnWidth(dgv, 0);
                int width_quiz = AutoSizeColumnWidth(dgv, 1);
                int width_correct = AutoSizeColumnWidth(dgv, 2);

                Console.WriteLine(width_quiz);
                Console.WriteLine(width_correct);

                Size = new Size(width_num + width_quiz + width_correct + 20, Size.Height);

                col_num.Width = width_num;
                col_quiz.Width = width_quiz;
                col_correct.Width = width_correct;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    //row.DefaultCellStyle.BackColor = Color.AliceBlue;
                }
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

                        // セルの値をクリップボードにコピー
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            Clipboard.SetText(cellValue);
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
