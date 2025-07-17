using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    public class Spanish : IManageInput
    {
        public string Comparelize(string str)
        {
            string s2 = str.Replace("\r\n", "\n");
            s2 = s2.Replace("¿", "");
            s2 = s2.Replace("?", "");
            s2 = s2.Replace("!", "");
            s2 = s2.Replace("¡", "");
            s2 = s2.Replace(";", "");
            s2 = s2.Replace(":", "");
            s2 = s2.Replace("…", ",");

            s2 = s2.Replace("\n", " ");

            s2 = s2.Replace(".", ". ");
            s2 = s2.Replace(",", ", ");

            if (SettingManager.currentLengua("es").ComaPunto)
            {
                s2 = s2.Replace(".", "");
                s2 = s2.Replace(",", "");
            }

            s2 = UtilityFunction.ReplaceConsecutiveSpaces(s2);

            s2 = (new Regex(" $")).Replace(s2, "");

            // 先頭の空行をなくす
            s2 = (new Regex("^ +")).Replace(s2, "");

            return s2;
        }

        public string GetDistinction(string str1, string str2)
        {
            int maxLength = Math.Max(str1.Length, str2.Length);

            // 文字列の長さが異なる場合は、*で埋める
            str1 = str1.PadRight(maxLength, '*');
            str2 = str2.PadRight(maxLength, '*');

            // 差分を検出し、周辺数文字を表示
            for (int i = 0; i < maxLength; i++)
            {
                // string.Compareの第3引数をtrueにすると文字列の大小を区別しない
                if (string.Compare(str1[i].ToString(), str2[i].ToString(), SettingManager.currentLengua("es").Capital) != 0)
                {
                    Console.WriteLine($"差分が見つかりました: 位置 {i}, 前の文字列: {UtilityFunction.GetContext(str1, i)}, 後ろの文字列: {UtilityFunction.GetContext(str2, i)}");
                    return $"{i}: {UtilityFunction.GetContext(str1, i)} -> {UtilityFunction.GetContext(str2, i)}";
                }
            }

            return "";
        }

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

        // üとÜ以外は使わないだろうからアセントにした。
        // 特にSíと打つときにSのシフトが残ったまま"が打鍵されてしまってSïとなるのを防ぐ意味もある。この問題にスペイン語使用者も困ってんじゃないのか？
        private static readonly Dictionary<char, char> letra_dieresis = new Dictionary<char, char>()
        {
            //['a'] = 'ä',
            //['e'] = 'ë',
            //['i'] = 'ï',
            //['o'] = 'ö',
            ['a'] = 'á',
            ['e'] = 'é',
            ['i'] = 'í',
            ['o'] = 'ó',
            ['u'] = 'ü',
            //['A'] = 'Ä',
            //['E'] = 'Ë',
            //['I'] = 'Ï',
            //['O'] = 'Ö',
            ['A'] = 'Á',
            ['E'] = 'É',
            ['I'] = 'Í',
            ['O'] = 'Ó',
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
