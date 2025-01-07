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
        private bool showRowHeader = false;
        private Dictionary<string, float> FontTable = new Dictionary<string, float>();
        private Dictionary<string, Rectangle> ControlTable = new Dictionary<string, Rectangle>();

        private SizeF _formSize;
        private Form form;

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

            foreach (Control control in _controls)
            {
                FontTable.Add(control.Name, control.Font.Size);
                ControlTable.Add(control.Name, control.Bounds);
            }
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


        public void _resize()
        {
            double _form_ratio_width = (double)form.ClientSize.Width / (double)_formSize.Width;
            double _form_ratio_height = (double)form.ClientSize.Height / (double)_formSize.Height;
            var _controls = _get_all_controls(form);

            foreach (Control control in _controls)
            {
                if (ignoreControl.Contains(control.GetType())) continue;

                Size _controlSize = new Size((int)(ControlTable[control.Name].Width * _form_ratio_width), (int)(ControlTable[control.Name].Height * _form_ratio_height));
                Point _controlposition = new Point((int)(ControlTable[control.Name].X * _form_ratio_width), (int)(ControlTable[control.Name].Y * _form_ratio_height));
                control.Bounds = new Rectangle(_controlposition, _controlSize);
                if (control.GetType() == typeof(DataGridView))
                    _dgv_Column_Adjust((DataGridView)control, showRowHeader);
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
