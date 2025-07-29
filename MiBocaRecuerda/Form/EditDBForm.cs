using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class EditDBForm : Form
    {
        private string currentFilePath;
        private int quizNum;

        public EditDBForm(string _currentFilePath, int _quizNum)
        {
            InitializeComponent();

            //FormBorderStyle = FormBorderStyle.FixedSingle;
            //MaximizeBox = false;

            currentFilePath = _currentFilePath;
            quizNum = _quizNum;

            dgv.Font = new Font("MeiryoKe_Console", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;

            DataGridViewTextBoxColumn col_num = new DataGridViewTextBoxColumn
            {
                Name = "num",
                HeaderText = "No",
                Width = 30,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn col_aux = new DataGridViewTextBoxColumn
            {
                Name = "aux",
                HeaderText = "Auxiliary",
                Width = dgv.Width - 30,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            col_aux.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            col_aux.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgv.Columns.Add(col_num);
            dgv.Columns.Add(col_aux);

            dgv.Columns[0].ReadOnly = true;

            for (int cnt = 0; cnt < 9; cnt++)
            {
                dgv.Rows.Add();
                dgv.Rows[cnt].Cells["num"].Value = (cnt + 1);
            }

            // 特定の列を選択できないように見せかける
            dgv.SelectionChanged += (o, e) =>
            {
                foreach (DataGridViewCell cell in dgv.SelectedCells)
                {
                    if (cell.ColumnIndex == 0)
                    {
                        cell.Selected = false;
                    }
                }
            };

            dgv.SizeChanged += (o, e) =>
            {
                AdjustRowHeightToFillGrid();
            };

            AdjustRowHeightToFillGrid();

            Init();

            ActiveControl = null;
        }

        string question;
        string answer;
        string supplement;
        string[] auxiliary;

        private void Init()
        {
            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XLWorkbook workBook = new XLWorkbook(fs);
                    IXLWorksheet ws = workBook.Worksheet(1);

                    question = ws.Cell(quizNum, (int)AppRom.DBColmun.Quiestion).Value.ToString();
                    answer = ws.Cell(quizNum, (int)AppRom.DBColmun.Answer).Value.ToString();
                    supplement = ws.Cell(quizNum, (int)AppRom.DBColmun.Supplement).Value.ToString();
                    auxiliary = ws.Cell(quizNum, (int)AppRom.DBColmun.Auxiliary).Value.ToString().Split(',');

                    txtQuestion.Text = question;
                    txtAnswer.Text = answer;
                    txtSupplement.Text = supplement;

                    for (int cnt = 0; cnt < auxiliary.Length; cnt++)
                    {
                        dgv.Rows[cnt].Cells[1].Value = auxiliary[cnt];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void AdjustRowHeightToFillGrid()
        {
            int visibleHeight = dgv.ClientSize.Height;
            int rowCount = dgv.RowCount;

            // ヘッダー分の高さを引いて調整（必要に応じて修正）
            int headerHeight = dgv.ColumnHeadersVisible ? dgv.ColumnHeadersHeight : 0;
            int availableHeight = visibleHeight - headerHeight;

            if (rowCount > 0)
            {
                int rowHeight = availableHeight / rowCount;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = rowHeight;
                }
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            List<string> tmp = new List<string>();

            tmp.AddRange(ParseXML.ConvertTextWithTable(txtSupplement.Text).Split('\n'));

            MessageForm s = new MessageForm(tmp, "Supplement preview", MessageForm.TipoDeUbicacion.DERECHA, this)
            {
                ShowIcon = false
            };

            s.Show();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("書き込みますか？",
                                        "確認",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button2);

            if(result == DialogResult.No)
            {
                return;
            }

            // DGVからAUXを集める
            List<string> auxs = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                object cellValue = row.Cells[1].Value;

                if (cellValue != null)
                {
                    auxs.Add(cellValue.ToString());
                }
            }

            bool isDiff_Q = question != txtQuestion.Text;
            bool isDiff_A = answer != txtAnswer.Text;
            bool isDiff_S = supplement != txtSupplement.Text;
            bool isDiff_AUX = !auxiliary.SequenceEqual(auxs);

            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XLWorkbook workBook = new XLWorkbook(fs);
                    IXLWorksheet ws = workBook.Worksheet(1);

                    if (isDiff_Q)
                    {
                        ws.Cell(quizNum, (int)AppRom.DBColmun.Quiestion).Value = txtQuestion.Text;
                    }

                    if (isDiff_A)
                    {
                        ws.Cell(quizNum, (int)AppRom.DBColmun.Answer).Value = txtAnswer.Text;
                    }

                    if (isDiff_S)
                    {
                        ws.Cell(quizNum, (int)AppRom.DBColmun.Supplement).Value = txtSupplement.Text;
                    }

                    if (isDiff_AUX)
                    {
                        ws.Cell(quizNum, (int)AppRom.DBColmun.Auxiliary).Value = string.Join(",", auxs);
                    }

                    if (isDiff_Q | isDiff_A | isDiff_S | isDiff_AUX)
                    {
                        string date = ws.Cell(quizNum, (int)AppRom.DBColmun.Date).Value.ToString();

                        //List<string> dates = date.Split(',').Select(d => DateTime.Parse(d).ToShortDateString()).ToList();
                        List<string> dates = date
                                            .Split(',')
                                            .Select(d => {
                                                DateTime parsedDate;
                                                return DateTime.TryParse(d.Trim(), out parsedDate)
                                                    ? parsedDate.ToShortDateString()
                                                    : null;
                                            })
                                            .Where(s => s != null)
                                            .ToList();

                        string today = DateTime.Today.ToShortDateString();

                        // 今日が入っていたら追記しない
                        if (!dates.Contains(today))
                        {
                            dates.Add(today);
                            // 編集日付を入れる
                            ws.Cell(quizNum, (int)AppRom.DBColmun.Date).Value = string.Join(",", dates);
                        }

                        try
                        {
                            workBook.Save();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        MessageBox.Show("書込完了");
                    }
                    else
                    {
                        MessageBox.Show("書込対象がありませんでした");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNO_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
