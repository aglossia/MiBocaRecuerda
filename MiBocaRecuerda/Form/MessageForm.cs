using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class MessageForm : Form
    {
        bool IsKeyDown = false;
        private ClassResize _form_resize;

        public enum TipoDeUbicacion
        {
            CENTRO,
            DERECHA,
            IZQUIERDA,
            PARENT_LINE
        }

        public MessageForm() { }

        public MessageForm(List<string> mensajes, string titulo, TipoDeUbicacion locationType, Form mf, bool readOnly = false)
        {
            InitializeComponent();

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

                int move_right = mf.Location.X + mf.Width + Width;
                int move_left = mf.Location.X - Width;

                switch (locationType)
                {
                    case TipoDeUbicacion.CENTRO:
                        Location = new Point(mf.Location.X + (mf.Width - Width) / 2, mf.Location.Y + (mf.Height - Height) / 2);
                        break;
                    case TipoDeUbicacion.DERECHA:
                        //Location = new Point(mf.Location.X + mf.Width, mf.Location.Y);
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
                        break;
                    case TipoDeUbicacion.PARENT_LINE:
                        Location = new Point(mf.Location.X, mf.Location.Y);
                        break;
                }
            };

            Shown += (o, e) =>
            {
                button1.Focus();

                _form_resize = new ClassResize(this);
            };

            SizeChanged += _Resize;

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
            if (_form_resize != null) _form_resize._resize();
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
