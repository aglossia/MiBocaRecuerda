using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public class ResizableForm : Form
    {
        public ClassResize _form_resize;

        public ResizableForm()
        {
            Load += (o, e) =>
            {
                _form_resize = new ClassResize(this);
            };
        }
    }
}
