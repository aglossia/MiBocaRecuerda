using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class MessageForm : ResizableForm
    {
        bool IsKeyDown = false;
        //private ClassResize _form_resize;
        private Form _form;

        public enum TipoDeUbicacion
        {
            CENTRO,
            DERECHA,
            IZQUIERDA,
            PARENT_LINE
        }

        public MessageForm() { }

        public MessageForm(List<string> mensajes, string titulo, TipoDeUbicacion locationType, Form form, bool readOnly = false)
        {
            InitializeComponent();

            _form = form;

            // テキストボックスが\nだと改行されないんだが？
            // mensajes.ForEach(s => s.Replace("\n", "\r\n")がきかないのはなぜだ？
            txtMensaje.Text = string.Join("\r\n", mensajes.Select(s => s.Replace("\n", "\r\n")));
            txtMensaje.SelectionStart = 0;
            txtMensaje.ReadOnly = readOnly;

            Load += (o, e) =>
            {
                // maxWidthは文字列のピッタリサイズのはずなので、余白分をたす
                int maxWidth = CommonFunction.GetMaxStringWidth(txtMensaje.Text, txtMensaje.Font) + 30;

                int newLineCount = txtMensaje.Text.Where(c => c == '\n').Count() + 1;

                // 41行までは伸ばして表示、それ以上はスクロルバ
                if(newLineCount <= 41)
                {
                    // barra de título:40 + 行数*16 + 余白:10
                    Size = new Size(maxWidth, 50 + newLineCount * 25 + 10);
                }
                else
                {
                    // 50 + newLineCount * 32 - 2
                    // スクロルバ分maxWidthを伸ばす
                    Size = new Size(maxWidth + 20, 784);
                    txtMensaje.ScrollBars = ScrollBars.Vertical;
                }

                Text = titulo;

                BaseAreaInfo baseArea = UtilityFunction.GetBaseArea();

                //_form_resize = new ClassResize(this);

                if(_form is ResizableForm)
                {
                    _form_resize._resize(true, (_form as ResizableForm)._form_resize.WidthRate, (_form as ResizableForm)._form_resize.HeightRate);
                }

                int move_right = form.Location.X + form.Width + Width;
                int move_left = form.Location.X - Width;

                switch (locationType)
                {
                    case TipoDeUbicacion.CENTRO:
                        Location = new Point(form.Location.X + (form.Width - Width) / 2, form.Location.Y + (form.Height - Height) / 2);
                        break;
                    case TipoDeUbicacion.DERECHA:
                        //Location = new Point(mf.Location.X + mf.Width, mf.Location.Y);
                        if (move_right < baseArea.MaxX)
                        {
                            // 右に表示する余地があるとき
                            Location = new Point(move_right - Width, form.Location.Y);
                        }
                        else if (move_left > baseArea.MinX)
                        {
                            // 左に表示する余地があるとき
                            Location = new Point(move_left, form.Location.Y);
                        }
                        break;
                    case TipoDeUbicacion.PARENT_LINE:
                        Location = new Point(form.Location.X, form.Location.Y);
                        break;
                }
            };

            Shown += (o, e) =>
            {
                button1.Focus();

                //_form_resize = new ClassResize(this);

                //_form_resize._resize(true, (_mf as MainForm)._form_resize.WidthRate, (_mf as MainForm)._form_resize.HeightRate);
            };

            SizeChanged += _Resize;

            KeyPreview = !KeyPreview;

            KeyDown += (o, e) =>
            {
                if (IsKeyDown) return;

                bool ctrlPressed = (ModifierKeys & (Keys.Control | Keys.Alt)) != 0;
                bool designatedKeyPressed = (e.KeyCode & Keys.C) == Keys.C;

                if(ctrlPressed)
                {
                    IsKeyDown = true;
                    return;
                }

                Close();
            };

            KeyUp += (o, e) =>
            {
                IsKeyDown = false;
            };
        }

        private void _Resize(object o, EventArgs e)
        {
            if (_form_resize != null) _form_resize._resize(false);
        }

        public void MessageUpdate(List<string> mensajes)
        {
            txtMensaje.Text = string.Join("\r\n", mensajes.Select(s => s.Replace("\n", "\r\n")));
            txtMensaje.SelectionStart = 0;

            // これからフォームサイズを変えるのでフォントがリサイズされないようにする
            SizeChanged -= _Resize;

            // maxWidthは文字列のピッタリサイズのはずなので、余白分をたす
            int maxWidth = CommonFunction.GetMaxStringWidth(txtMensaje.Text, txtMensaje.Font) + 30;

            int newLineCount = txtMensaje.Text.Where(c => c == '\n').Count() + 1;

            // 41行までは伸ばして表示、それ以上はスクロルバ
            if (newLineCount <= 41)
            {
                // barra de título:40 + 行数*16 + 余白:10
                Size = new Size(maxWidth, 40 + newLineCount * 16 + 10);
            }
            else
            {
                // 50 + newLineCount * 32 - 2
                // スクロルバ分maxWidthを伸ばす
                Size = new Size(maxWidth + 20, 784);
                txtMensaje.ScrollBars = ScrollBars.Vertical;
            }

            // これを新しい基準フォームサイズとする
            _form_resize = new ClassResize(this);

            SizeChanged += _Resize;
        }
    }
}
