using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class SettingLanguageBase : UserControl
    {
        [Browsable(true)]
        [Category("表示")]
        [Description("言語設定")]
        public string LanguageName { get; set; }

        public SettingLanguageBase()
        {
            InitializeComponent();

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
        }

        public void LoadConfig()
        {
            for (int i = 0; i < 9; i++)
            {
                if (SettingManager.LanguageConfigManager.ContainsKey(LanguageName))
                {
                    dgv.Rows[i].Cells[1].Value = SettingManager.LanguageConfigManager[LanguageName].InputSupport[i];
                }
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

        public List<string> GetAux()
        {
            int columnIndex = 1; // 0から始まるインデックス
            List<string> values = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    string value = row.Cells[columnIndex].Value?.ToString();
                    if (value == null) value = "";
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
