using System.Collections.Generic;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public class Spanish : IManageInput
    {
        private static readonly Dictionary<char, char> letra_acento = new Dictionary<char, char>()
        {
            ['a'] = 'á',
            ['e'] = 'é',
            ['i'] = 'í',
            ['o'] = 'ó',
            ['u'] = 'ú',
            ['A'] = 'Á',
            ['E'] = 'É',
            ['I'] = 'Í',
            ['O'] = 'Ó',
            ['U'] = 'Ú',
        };

        private static readonly Dictionary<char, char> letra_dieresis = new Dictionary<char, char>()
        {
            ['u'] = 'ü',
            ['U'] = 'Ü',
        };

        private bool isAcento;
        private bool isDieresis;

        public void KeyPress(object o, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\'':
                    isAcento = true;
                    e.Handled = true;
                    break;
                case '"':
                    isDieresis = true;
                    e.Handled = true;
                    break;
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                case 'A':
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                    if (isAcento)
                    {
                        e.KeyChar = letra_acento[e.KeyChar];
                    }
                    else if (isDieresis)
                    {
                        e.KeyChar = letra_dieresis[e.KeyChar];
                    }
                    break;
                case ';':
                    e.KeyChar = 'ñ';
                    break;
                case ':':
                    e.KeyChar = 'Ñ';
                    break;
                case '<':
                    e.KeyChar = ';';
                    break;
                case '>':
                    e.KeyChar = ':';
                    break;
            }

            switch (e.KeyChar)
            {
                case '\'':
                case '"':
                    break;
                default:
                    isAcento = false;
                    isDieresis = false;
                    break;
            }
        }
    }
}
