using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public interface IManageInput
    {
        string Comparelize(string str);

        string GetDistinction(string str1, string str2);

        void KeyPress(object o, KeyPressEventArgs e);
    }
}
