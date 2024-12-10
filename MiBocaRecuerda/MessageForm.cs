using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public partial class MessageForm : Form
    {
        bool IsKeyDown = false;

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

            txtMensaje.Text = string.Join("\r\n", mensajes);
            txtMensaje.SelectionStart = 0;
            txtMensaje.ReadOnly = readOnly;

            Load += (o, e) =>
            {
                // maxWidthは文字列のピッタリサイズのはずなので、余白分をたす
                int maxWidth = CommonFunction.GetMaxStringWidth(txtMensaje.Text, txtMensaje.Font) + 22;

                int newLineCount = txtMensaje.Text.Where(c => c == '\n').Count() + 1;

                // 41行までは伸ばして表示、それ以上はスクロルバ
                if(newLineCount <= 41)
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

                Text = titulo;

                switch (locationType)
                {
                    case TipoDeUbicacion.CENTRO:
                        Location = new Point(mf.Location.X + (mf.Width - Width) / 2, mf.Location.Y + (mf.Height - Height) / 2);
                        break;
                    case TipoDeUbicacion.DERECHA:
                        Location = new Point(mf.Location.X + mf.Width, mf.Location.Y);
                        break;
                    case TipoDeUbicacion.PARENT_LINE:
                        Location = new Point(mf.Location.X, mf.Location.Y);
                        break;
                }
            };

            Shown += (o, e) =>
            {
                button1.Focus();
            };

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
    }
}
