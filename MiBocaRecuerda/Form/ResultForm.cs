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
        private Dictionary<int, string> _supplement = new Dictionary<int, string>();

        private ClassResize _form_resize;

        private List<QuizContents> _workBook;
        private Dictionary<int, (string quiz, Answer answer)> _handBook;

        private Point _parentLocation;
        private Size _parentSize;

        private DataGridViewTextBoxColumn _col_num;
        private DataGridViewTextBoxColumn _col_quiz;
        private DataGridViewTextBoxColumn _col_correct;

        private bool _isAuto = false;

        private string _prioridadRegion;
        private List<string> _regionList = new List<string>();

        private void Init()
        {
            dgv.RowPrePaint += dataGridView1_RowPrePaint;

            //dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgv.Rows[e.RowIndex];
            int preferred = row.GetPreferredHeight(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells, true);

            // 余白を追加
            int padding = 10;

            if (row.Height != preferred + padding)
            {
                row.Height = preferred + padding;
            }
        }

        public ResultForm() { }

        public ResultForm(List<QuizContents> workBook, MainForm mainForm, bool isOrder = true)
        {
            InitializeComponent();

            Init();

            _prioridadRegion = SettingManager.CurrentQuizFileConfig.PriorityRegion;

            _workBook = workBook;
            _handBook = CoreProcess.GetHandBook(workBook);

            _parentLocation = mainForm.Location;
            _parentSize = mainForm.Size;

            List<Answer> parseAnswer = new List<Answer>();

            if (isOrder) _workBook = _workBook.OrderBy(q => q.QuizNum).ToList();

            // regionの種類を集める
            _regionList = _workBook.SelectMany(q => q.CorrectAnswer.Keys).Distinct().ToList();

            if (_regionList.Count == 1)
            {
                // regionが一つしかないときは表示しない
                Controls.Remove(menuStrip1);
            }
            else
            {
                // regionが複数あるときはregionを列挙する
                TS_cmbRegion.Items.AddRange(_regionList.ToArray());
                TS_cmbRegion.SelectedItem = _prioridadRegion;
            }

            TS_cmbRegion.SelectedIndexChanged += (o, e) =>
            {
                // 優先を選択したregionに変えてデータをセットしなおす
                _prioridadRegion = (o as ToolStripComboBox).SelectedItem.ToString();
                SetTableData();
            };

            foreach (QuizContents c in _workBook)
            {
                _supplement.Add(c.QuizNum, c.Supplement);
            }

            CreateControls();
            SetTableData();

            RegisterEvent();
        }

        private void CreateControls()
        {
            dgv.Font = new Font("MeiryoKe_Console", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            //dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            _col_num = new DataGridViewTextBoxColumn
            {
                Name = "num",
                HeaderText = "No",
                Width = 30,
                SortMode = DataGridViewColumnSortMode.Automatic,
                ReadOnly = true
            };

            _col_quiz = new DataGridViewTextBoxColumn
            {
                Name = "quiz",
                HeaderText = "Prueba",
                Width = dgv.Width / 2 - 1,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            _col_correct = new DataGridViewTextBoxColumn
            {
                Name = "correct",
                HeaderText = "Respuesta Correcta",
                Width = dgv.Width / 2 - 1,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            _col_correct.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            _col_quiz.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _col_correct.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgv.Columns.Add(_col_num);
            dgv.Columns.Add(_col_quiz);
            dgv.Columns.Add(_col_correct);

            for (int cnt = 0; cnt < _workBook.Count; cnt++)
            {
                dgv.Rows.Add();
            }
        }

        private void SetTableData()
        {
            // DGVにデータを設定する
            for (int cnt = 0; cnt < _workBook.Count; cnt++)
            {
                dgv.Rows[cnt].Cells["num"].Value = _workBook[cnt].QuizNum;
                dgv.Rows[cnt].Cells["quiz"].Value = _workBook[cnt].Quiz;

                List<string> parsedAnswers = new List<string>();

                // パースした解答を集める
                foreach (Answer ans in _workBook[cnt].Answers(_prioridadRegion))
                {
                    parsedAnswers = parsedAnswers.Concat(CoreProcess.ParseAnswer(ans.Sentence)).ToList();
                }

                // 集めた解答が複数あれば連番をつける
                if (parsedAnswers.Count > 1)
                {
                    parsedAnswers = parsedAnswers
                        .Select((value, index) => $"{index + 1}:{value}")
                        .ToList();
                }

                dgv.Rows[cnt].Cells["correct"].Value = string.Join("\n", parsedAnswers);

                if (_workBook[cnt].IsCorrect == false)
                {
                    dgv.Rows[cnt].DefaultCellStyle.BackColor = Color.AliceBlue;
                }

                // 補足があるやつは補足の目印をつける
                if (_workBook[cnt].Supplement != "")
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

                _col_num.Width = width_num;
                _col_quiz.Width = width_quiz;
                _col_correct.Width = width_correct;

                BaseAreaInfo baseArea = UtilityFunction.GetBaseArea();

                int move_right = _parentLocation.X + _parentSize.Width + Width;
                int move_left = _parentLocation.X - Width;

                Console.WriteLine($"{baseArea.MaxX}, {_parentLocation.X + _parentSize.Width + Width}");

                if (move_right < baseArea.MaxX)
                {
                    // 右に表示する余地があるとき
                    Location = new Point(move_right - Width, _parentLocation.Y);
                }
                else if (move_left > baseArea.MinX)
                {
                    // 左に表示する余地があるとき
                    Location = new Point(move_left, _parentLocation.Y);
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
                if (_isAuto) return;

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

            // コピーのコンテキストメニューを開くとき
            CMS_copy.DropDownOpening += (o, e) =>
            {
                CMS_copy_all.DropDownItems.Clear();
                CMS_copy_answer_all.DropDownItems.Clear();

                CMS_copy_all.Click -= AllCopy_Region_all;
                CMS_copy_answer_all.Click -= AllCopy_Region_all;

                if (_regionList.Count > 1)
                {
                    // 表全体をコピー
                    var a = CMS_copy_all.DropDownItems.Add("現在のRegion", null, AllCopy_Region_selected);
                    a.Tag = "all";
                    var b = CMS_copy_all.DropDownItems.Add("全てのRegion", null, AllCopy_Region_all);
                    b.Tag = "all";
                    // 答え全体をコピー
                    var c = CMS_copy_answer_all.DropDownItems.Add("現在のRegion", null, AllCopy_Region_selected);
                    c.Tag = "answer_all";
                    var d = CMS_copy_answer_all.DropDownItems.Add("全てのRegion", null, AllCopy_Region_all);
                    d.Tag = "answer_all";
                }
                else
                {
                    CMS_copy_all.Click += AllCopy_Region_all;
                    CMS_copy_answer_all.Click += AllCopy_Region_all;
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
            if (_supplement[quizNum] != "")
            {
                List<string> tmp = new List<string>
                            {
                                japones,
                                correcto,
                                "───────"
                            };
                tmp.AddRange(ParseXML.ConvertTextWithTable(_supplement[quizNum]).Split('\n'));

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
                //row.Height += 10;
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
                string text = grid[column, i].Value?.ToString();
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
                try
                {
                    Clipboard.SetText(cellValue);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"コピー失敗：{ex.Message}");
                    return;
                }

                MessageBox.Show($"{(ColumnIndex == 1 ? "問題" : ColumnIndex == 2 ? "答え" : "???")}をコピー");
            }
        }

        // 指定リージョンをコピー
        private void AllCopy_Region_selected(object o, EventArgs e)
        {
            string tagName = (o as ToolStripItem).Tag as string;

            List<string> contents = CoreProcess.GetHandBookContents_IndividualRegion(_handBook, tagName == "all");

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

        // 全てのregionをコピー
        private void AllCopy_Region_all(object o, EventArgs e)
        {
            string tagName = (o as ToolStripItem).Tag as string;

            List<string> contents = CoreProcess.GetHandBookContents_AllRegion(_handBook, tagName == "all");

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
        }

        // 編集
        private void CMS_edit_Click(object sender, EventArgs e)
        {
            List<int> quizSequence = dgv.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r => Convert.ToInt32(r.Cells[0].Value))
                .ToList();

            EditDBForm edb = new EditDBForm(int.Parse(quizNum), quizSequence);

            if (!edb.IsDisposed) edb.Show(this);
        }

        // クイズ非表示
        private void CMS_quiz_hide_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            // _isAutoをONにしてサイズ変更しないと想定外にフォントサイズが変更されてしまう
            _isAuto = true;
            ToggleColumnVisibility("quiz", item.Checked);
            _isAuto = false;
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
