using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public interface IManageInput
    {
        // 比較前処理
        string Comparelize(string str);

        // 差分抽出
        string GetDistinction(string str1, string str2);

        // 言語特有文字の入力補助
        void KeyPress(object o, KeyPressEventArgs e);
    }
}
