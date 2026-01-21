using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class EditDBForm : Form
    {
        private int QuizNum;
        private ExerciseRepository ExerRepo;
        private string CurrentRegion;
        // DB取得時のDB
        private ExerciseDB ExerdbInit;
        // 入力されたDB(編集状態を記憶している)
        private Dictionary<string, List<EditAnswer>> InputAnswer = new Dictionary<string, List<EditAnswer>>();
        // 次のIDを生成する用(region:id通番)
        private Dictionary<string, List<int>> ID_list = new Dictionary<string, List<int>>();
        // regionごとの解答インデックス(region遷移用)(region:index)
        Dictionary<string, int> AnswerCache = new Dictionary<string, int>();
        // 起動時に表示する解答タブのreiogn設定用
        TabPage InitPage = null;

        // 現在の解答タブ
        private TextBox CurrentTB => tabAnswer.SelectedTab.Tag as TextBox;
        // 現在の解答インデックス
        private int CurrentIndex => cmbAnswer.Visible ? cmbAnswer.SelectedIndex : 0;
        // 現在編集中の解答
        private EditAnswer CurrentAnswer => InputAnswer[CurrentRegion][CurrentIndex];

        public EditDBForm(string currentFilePath, int quizNum, string prioridad_region)
        {
            InitializeComponent();

#if !DEBUG
            label4.Visible = false;
#endif

            //FormBorderStyle = FormBorderStyle.FixedSingle;
            //MaximizeBox = false;
            tabAnswer.SizeMode = TabSizeMode.Fixed;
            tabAnswer.ItemSize = new Size(40, tabAnswer.ItemSize.Height);
            cmbAnswer.Visible = false;

            Text = $"Edit - {quizNum}";

            QuizNum = quizNum;
            CurrentRegion = prioridad_region;

            #region dgvAuxiliary

            dgvAuxiliary.Font = new Font("MeiryoKe_Console", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            dgvAuxiliary.RowHeadersVisible = false;
            dgvAuxiliary.AllowUserToAddRows = false;
            dgvAuxiliary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAuxiliary.AllowUserToResizeColumns = false;
            dgvAuxiliary.AllowUserToResizeRows = false;

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
                Width = dgvAuxiliary.Width - 30,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            col_aux.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            col_aux.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvAuxiliary.Columns.Add(col_num);
            dgvAuxiliary.Columns.Add(col_aux);

            dgvAuxiliary.Columns[0].ReadOnly = true;

            for (int cnt = 0; cnt < 9; cnt++)
            {
                dgvAuxiliary.Rows.Add();
                dgvAuxiliary.Rows[cnt].Cells["num"].Value = (cnt + 1);
            }

            // 特定の列を選択できないように見せかける
            dgvAuxiliary.SelectionChanged += (o, e) =>
            {
                foreach (DataGridViewCell cell in dgvAuxiliary.SelectedCells)
                {
                    if (cell.ColumnIndex == 0)
                    {
                        cell.Selected = false;
                    }
                }
            };

            dgvAuxiliary.SizeChanged += (o, e) =>
            {
                AdjustRowHeightToFillGrid();
            };

            #endregion

            txtSupplement.TextChanged += (o, e) =>
            {
                btnPreview.Enabled = txtSupplement.Text != "";
            };

            AdjustRowHeightToFillGrid();

            ExerRepo = new ExerciseRepository($"Data Source={currentFilePath}");

            ExerdbInit = ExerRepo.GetByNum(QuizNum);

            txtProblem.Text = ExerdbInit.Problem;
            txtSupplement.Text = ExerdbInit.Supplement;

            // AnswerたちからEditAnswerに変換する(何かいい方法ない？)
            foreach (KeyValuePair<string, List<Answer>> kvp in ExerdbInit.Answer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    if (!InputAnswer.TryGetValue(kvp.Key, out var list))
                    {
                        list = new List<EditAnswer>();
                        InputAnswer[kvp.Key] = list;
                    }
                    list.Add(new EditAnswer(ans.ID, ans.Sentence));
                }
            }

            // AnswerからregionごとのID通番を取得する
            foreach (KeyValuePair<string, List<Answer>> kvp in ExerdbInit.Answer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    if (!ID_list.TryGetValue(kvp.Key, out var list))
                    {
                        list = new List<int>();
                        ID_list[kvp.Key] = list;
                    }
                    list.Add(ans.ID_ind().id2);
                }
            }

            // 優先regionがない場合は先頭のregionにする
            if (!ExerdbInit.Answer.ContainsKey(CurrentRegion))
            {
                CurrentRegion = ExerdbInit.Answer.FirstOrDefault().Key;
            }

            // 優先regionの解答が複数ある場合
            if (ExerdbInit.Answer[CurrentRegion].Count > 1)
            {
                cmbAnswer.Visible = true;
                AddNumbersToComboBox(ExerdbInit.Answer[CurrentRegion].Count);
                // POR HACER:起動時は0でいい？
                cmbAnswer.SelectedIndex = 0;
            }

            // regionに応じてタブページを作成
            foreach (KeyValuePair<string, List<Answer>> ans in ExerdbInit.Answer)
            {
                AddAnswerPage(ans.Key, ans.Value[0].Sentence, false);
            }

            // 解答regionタブ遷移イベント
            tabAnswer.SelectedIndexChanged += (o, e) =>
            {
                // タブを変える＝regionを変更
                CurrentRegion = tabAnswer.SelectedTab.Text;

                ChangeCmbAnswerState();
#if DEBUG
                label4.Text = InputAnswer[CurrentRegion][AnswerCache[CurrentRegion]].ID;
#endif
                // 削除対象判定
                if (CurrentAnswer.SqlOperation == AppRom.SqlOperation.Delete)
                {
                    CurrentTB.BackColor = Color.LightPink;
                }
                else
                {
                    CurrentTB.BackColor = SystemColors.Window;
                }
            };

            // 解答コンボボックス変更イベント
            cmbAnswer.SelectedIndexChanged += (o, e) =>
            {
                int index = (o as ComboBox).SelectedIndex;

                CurrentTB.TextChanged -= _TextChanged;
                // 別解を変えた時は編集中であればそれを保持したものに切り替える
                CurrentTB.Text = InputAnswer[CurrentRegion][index].Sentence;
                CurrentTB.TextChanged += _TextChanged;
#if DEBUG
                label4.Text = InputAnswer[CurrentRegion][index].ID;
#endif

                AnswerCache[CurrentRegion] = index;

                // 削除対象判定
                if (CurrentAnswer.SqlOperation == AppRom.SqlOperation.Delete)
                {
                    CurrentTB.BackColor = Color.LightPink;
                }
                else
                {
                    CurrentTB.BackColor = SystemColors.Window;
                }
            };

            // Auxiliaryに設定
            for (int cnt = 0; cnt < ExerdbInit.Auxiliary.Count; cnt++)
            {
                dgvAuxiliary.Rows[cnt].Cells[1].Value = ExerdbInit.Auxiliary[cnt];
            }

            Shown += (o, e) =>
            {
                if (InitPage != null)
                {
                    tabAnswer.SelectedTab = InitPage;
                    // ↑で遷移させるとなぜかテキストが全選択されているからフォーカスを外すようにした
                    //tabAnswer.Focus();
                }
                btnNO.Focus();
            };

#if DEBUG
            label4.Text = InputAnswer[CurrentRegion][0].ID;
#endif

            ActiveControl = null;
        }

        // コンボボックスに指定数までの連番を入れる
        private void AddNumbersToComboBox(int maxValue)
        {
            cmbAnswer.Items.Clear();

            for (int i = 1; i <= maxValue; i++)
            {
                cmbAnswer.Items.Add(i);
            }
        }

        // 解答タブにページ追加
        private void AddAnswerPage(string region, string sentence, bool isInserted)
        {
            TextBox textBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Text = sentence
            };

            textBox.TextChanged += _TextChanged;

            TabPage tabPage = new TabPage
            {
                Text = region,
                Name = $"tab{region}",
                Tag = textBox
            };

            if (tabPage.Text == MainForm.QuizFileConfig.PriorityRegion)
            {
                InitPage = tabPage;
            }

            tabPage.Controls.Add(textBox);
            tabAnswer.TabPages.Add(tabPage);

            // タブ状態の保持用
            AnswerCache[region] = 0;

            // 新規タブにページ追加ではなく、ページ追加の場合
            if (isInserted)
            {
                ID_list[region].Add(ID_list[region].Max() + 1);

                if(!InputAnswer.TryGetValue(region, out var list))
                {
                    list = new List<EditAnswer>();
                    InputAnswer[region] = list;
                }
                list.Add(new EditAnswer($"{QuizNum}-{region}-{ID_list[region].Max()}", "", AppRom.SqlOperation.Insert));
            }
        }

        private void _TextChanged(object o, EventArgs e)
        {
            int index = 0;

            if (cmbAnswer.Visible == true)
            {
                // コンボボックスが表示されているということは解答が複数あるので
                // どの解答を編集中かを取得する
                index = cmbAnswer.SelectedIndex;
            }

            // 入力したものを保持する
            InputAnswer[CurrentRegion][index].Sentence = (o as TextBox).Text;

            // 解答のSQLがINSERT出なかった場合(つまり既存)
            if (InputAnswer[CurrentRegion][index].SqlOperation != AppRom.SqlOperation.Insert)
            {
                // 元の文字列と差異があればSQLをUPDATEにする
                if (ExerdbInit.Answer[CurrentRegion][index].Sentence != InputAnswer[CurrentRegion][index].Sentence)
                {
                    InputAnswer[CurrentRegion][index].SqlOperation = AppRom.SqlOperation.Update;
                }
                else
                {
                    InputAnswer[CurrentRegion][index].SqlOperation = AppRom.SqlOperation.None;
                }
            }
        }

        private void ChangeCmbAnswerState()
        {
            if (InputAnswer[CurrentRegion].Count > 1)
            {
                // regionが複数あるときはコンボボックスを表示する
                cmbAnswer.Visible = true;
                // region分の連番をいれる
                AddNumbersToComboBox(InputAnswer[CurrentRegion].Count);
                // POR HACER:ここは前の状態を記憶した方がいい
                cmbAnswer.SelectedIndex = AnswerCache[CurrentRegion];
            }
            else
            {
                // regionが一つしかないときは表示しない
                cmbAnswer.Visible = false;
            }
        }

        private void AdjustRowHeightToFillGrid()
        {
            int visibleHeight = dgvAuxiliary.ClientSize.Height;
            int rowCount = dgvAuxiliary.RowCount;

            // ヘッダー分の高さを引いて調整（必要に応じて修正）
            int headerHeight = dgvAuxiliary.ColumnHeadersVisible ? dgvAuxiliary.ColumnHeadersHeight : 0;
            int availableHeight = visibleHeight - headerHeight;

            if (rowCount > 0)
            {
                int rowHeight = availableHeight / rowCount;

                foreach (DataGridViewRow row in dgvAuxiliary.Rows)
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

            foreach (DataGridViewRow row in dgvAuxiliary.Rows)
            {
                object cellValue = row.Cells[1].Value;

                if (cellValue != null)
                {
                    auxs.Add(cellValue.ToString());
                }
            }

            string problem = ExerdbInit.Problem != txtProblem.Text ? txtProblem.Text : null;
            string _supplement = ExerdbInit.Supplement != txtSupplement.Text ? txtSupplement.Text : null;
            List<string> _aux = !ExerdbInit.Auxiliary.SequenceEqual(auxs) ? auxs : null;

            string today = DateTime.Today.ToShortDateString();

            // 今日が入っていたら追記しない
            if (!ExerdbInit.Update.Contains(today))
            {
                // 編集日付を入れる
                today = null;
            }

            try
            {
                ExerRepo.EditDB(QuizNum, problem, _supplement, _aux, today, InputAnswer);

                MessageBox.Show("書込完了");
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

        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            string region = txtNewRegion.Text;

            if (region == "") return;

            if (InputAnswer.ContainsKey(region)) return;

            ID_list[region] = new List<int>() { 0 };

            // 新しいregionのページを追加
            AddAnswerPage(region, "", true);

            tabAnswer.SelectedIndex = tabAnswer.TabCount-1;
        }

        private void btnAddAlter_Click(object sender, EventArgs e)
        {
            ID_list[CurrentRegion].Add(ID_list[CurrentRegion].Max() + 1);

            // 入力中の解答に空を用意する
            InputAnswer[CurrentRegion].Add(new EditAnswer($"{QuizNum}-{CurrentRegion}-{ID_list[CurrentRegion].Max()}", "", AppRom.SqlOperation.Insert));

            // 別解が増えたからコンボボックスの状態を変える
            ChangeCmbAnswerState();

            // コンボボックスの選択を追加したものにする
            cmbAnswer.SelectedIndex = cmbAnswer.Items.Count - 1;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(CurrentAnswer.SqlOperation == AppRom.SqlOperation.Delete)
            {
                CurrentAnswer.SqlOperation = AppRom.SqlOperation.None;
                CurrentTB.BackColor = SystemColors.Window;
            }
            else
            {
                CurrentAnswer.SqlOperation = AppRom.SqlOperation.Delete;
                CurrentTB.BackColor = Color.LightPink;
            }
        }
    }
}
