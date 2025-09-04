using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public class ClassResize
    {
        //private List<Rectangle> _arr_control_storage = new List<Rectangle>();
        private Dictionary<string, float> FontTable = new Dictionary<string, float>();
        private Dictionary<string, Rectangle> ControlTable = new Dictionary<string, Rectangle>();
        private Dictionary<string, List<int>> ControlTable_dgv_col = new Dictionary<string, List<int>>();
        private Dictionary<string, List<int>> ControlTable_dgv_row = new Dictionary<string, List<int>>();

        private SizeF _formSize;
        private Form form;

        public double WidthRate => form.ClientSize.Width / (double) _formSize.Width;
        public double HeightRate => form.ClientSize.Height / (double)_formSize.Height;

        public ClassResize(Form _form_)
        {
            form = _form_;
            _formSize = _form_.ClientSize;

            GetControlTable(_form_);
        }

        public void GetControlTable(Form _form_)
        {
            var _controls = _get_all_controls(form);

            //_get_initial_size();
            _formSize = _form_.ClientSize;

            FontTable.Clear();
            ControlTable.Clear();
            ControlTable_dgv_col.Clear();
            ControlTable_dgv_row.Clear();

            foreach (Control control in _controls)
            {
                FontTable.Add(control.Name, control.Font.Size);
                ControlTable.Add(control.Name, control.Bounds);

                if (control.GetType() == typeof(DataGridView))
                {
                    //List<int> col_widths = new List<int>();

                    //DataGridView dgv = control as DataGridView;

                    List<int> col_widths = (control as DataGridView).Columns.Cast<DataGridViewColumn>().Select(col => col.Width).ToList();

                    ControlTable_dgv_col.Add(control.Name, col_widths);

                    List<int> row_heights = (control as DataGridView).Rows.Cast<DataGridViewRow>().Select(row => row.Height).ToList();

                    ControlTable_dgv_row.Add(control.Name, row_heights);
                }
            }
        }

        public void UpdateFormSize(Form fm)
        {
            _formSize = fm.ClientSize;
            //
            GetControlTable(fm);
        }

        //public void _get_initial_size()
        //{
        //    var _controls = _get_all_controls(form);

        //    _arr_control_storage.Clear();

        //    foreach (Control control in _controls)
        //    {
        //        _arr_control_storage.Add(control.Bounds);
        //        if (control.GetType() == typeof(DataGridView))
        //            _dgv_Column_Adjust((DataGridView)control, showRowHeader);
        //    }
        //}

        List<Type> ignoreControl = new List<Type>() { typeof(MenuStrip) };


        public void _resize(bool manual, double width_rate = 1, double height_rate = 1)
        {
            double _form_ratio_width;
            double _form_ratio_height;

            if (manual == false)
            {
                _form_ratio_width = WidthRate;
                _form_ratio_height = HeightRate;
            }
            else
            {
                _form_ratio_width = width_rate;
                _form_ratio_height = height_rate;

                form.Width = (int)(form.Width * width_rate);
                form.Height = (int)(form.Height * height_rate);
            }

            if ((_form_ratio_width == 0) || (_form_ratio_height == 0)) return;

            var _controls = _get_all_controls(form);

            foreach (Control control in _controls)
            {
                if (ignoreControl.Contains(control.GetType())) continue;

                Size _controlSize = new Size((int)(ControlTable[control.Name].Width * _form_ratio_width), (int)(ControlTable[control.Name].Height * _form_ratio_height));
                Point _controlposition = new Point((int)(ControlTable[control.Name].X * _form_ratio_width), (int)(ControlTable[control.Name].Y * _form_ratio_height));
                control.Bounds = new Rectangle(_controlposition, _controlSize);
                if (control.GetType() == typeof(DataGridView))
                {
                    DataGridView dgv = control as DataGridView;

                    //dgv.Columns.Cast<DataGridViewColumn>().ToList().ForEach(col => col.Width =(int)( * _form_ratio_width));

                    // 列幅の調整
                    foreach (var (item1, item2) in dgv.Columns.Cast<DataGridViewColumn>().Zip((ControlTable_dgv_col[control.Name] as List<int>), (x, y) => (x, y)))
                    {
                        item1.Width = (int)(item2 * _form_ratio_width);
                    }

                    // 行高さの調整
                    foreach (var (item1, item2) in dgv.Rows.Cast<DataGridViewRow>().Zip((ControlTable_dgv_row[control.Name] as List<int>), (x, y) => (x, y)))
                    {
                        item1.Height = (int)(item2 * _form_ratio_height);
                    }
                }
                //_dgv_Column_Adjust((DataGridView)control, showRowHeader);
                control.Font = new Font(form.Font.FontFamily, (float)(((FontTable[control.Name] * _form_ratio_width) / 2) + ((FontTable[control.Name] * _form_ratio_height) / 2)));
            }
        }

        private void _dgv_Column_Adjust(DataGridView dgv, bool _showRowHeader)
        {
            int intRowHeader = 0;
            const int Hscrollbarwidth = 5;

            if (_showRowHeader)
            {
                intRowHeader = dgv.RowHeadersWidth;
            }
            else
            {
                dgv.RowHeadersVisible = false;
            }

            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (dgv.Dock == DockStyle.Fill)
                {
                    dgv.Columns[i].Width = (dgv.Width - intRowHeader) / dgv.ColumnCount;
                }
                else
                {
                    dgv.Columns[i].Width = (dgv.Width - intRowHeader - Hscrollbarwidth) / dgv.ColumnCount;
                }
            }
        }

        private static IEnumerable<Control> _get_all_controls(Control c)
        {
            return c.Controls.Cast<Control>().SelectMany(item => _get_all_controls(item)).Concat(c.Controls.Cast<Control>()).Where(control => control.Name != string.Empty);
        }
    }
}
