using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiBocaRecuerda
{
    // ChatGPT-4oに作ってもらった。いろいろ修正をお願いしてそれなりに動くようにはなったが
    // 怪しい動きはいっぱいだし実装方法もかなり厳しい
    public static class ArabicSpanish
    {
        // 辞書: ラテン文字表記からアラビア数字への対応
        static readonly private Dictionary<string, int> TextToNumber = new Dictionary<string, int>
        {
            { "cero", 0},{"uno", 1}, {"dos", 2}, {"tres", 3}, {"cuatro", 4}, {"cinco", 5},
            {"seis", 6}, {"siete", 7}, {"ocho", 8}, {"nueve", 9}, {"diez", 10},
            {"once", 11}, {"doce", 12}, {"trece", 13}, {"catorce", 14}, {"quince", 15},
            {"dieciséis", 16}, {"diecisiete", 17}, {"dieciocho", 18}, {"diecinueve", 19}, {"veinte", 20},
            {"veintiuno", 21}, {"veintidós", 22}, {"veintitrés", 23}, {"veinticuatro", 24}, {"veinticinco", 25},
            {"veintiséis", 26}, {"veintisiete", 27}, {"veintiocho", 28}, {"veintinueve", 29},
            {"treinta", 30}, {"cuarenta", 40}, {"cincuenta", 50}, {"sesenta", 60}, {"setenta", 70},
            {"ochenta", 80}, {"noventa", 90}, {"cien", 100}, {"ciento", 100}, {"doscientos", 200},
            {"trescientos", 300}, {"cuatrocientos", 400}, {"quinientos", 500}, {"seiscientos", 600},
            {"setecientos", 700}, {"ochocientos", 800}, {"novecientos", 900}, {"mil", 1000},
            {"millón", 1000000}, {"millones", 1000000}
        };

        // 辞書: アラビア数字からラテン文字表記への対応
        static readonly private Dictionary<int, string> NumberToText = new Dictionary<int, string>
        {
            { 0, "cero"}, {1, "uno"}, {2, "dos"}, {3, "tres"}, {4, "cuatro"}, {5, "cinco"},
            {6, "seis"}, {7, "siete"}, {8, "ocho"}, {9, "nueve"}, {10, "diez"},
            {11, "once"}, {12, "doce"}, {13, "trece"}, {14, "catorce"}, {15, "quince"},
            {16, "dieciséis"}, {17, "diecisiete"}, {18, "dieciocho"}, {19, "diecinueve"}, {20, "veinte"},
            {21, "veintiuno"}, {22, "veintidós"}, {23, "veintitrés"}, {24, "veinticuatro"}, {25, "veinticinco"},
            {26, "veintiséis"}, {27, "veintisiete"}, {28, "veintiocho"}, {29, "veintinueve"},
            {30, "treinta"}, {40, "cuarenta"}, {50, "cincuenta"}, {60, "sesenta"}, {70, "setenta"},
            {80, "ochenta"}, {90, "noventa"}, {100, "cien"}, {200, "doscientos"}, {300, "trescientos"},
            {400, "cuatrocientos"}, {500, "quinientos"}, {600, "seiscientos"}, {700, "setecientos"},
            {800, "ochocientos"}, {900, "novecientos"}, {1000, "mil"}, {1000000, "millón"}, {2000000, "millones"}
        };

        public static string ConvertSpanishNumbers(string input)
        {
            if (Regex.IsMatch(input, "\\d+"))
            {
                input = Regex.Replace(input, "\\d+", match => ArabicToSpanish(long.Parse(match.Value)));
            }
            else if (Regex.IsMatch(input, "\\b(?:(?:(?:un|dos|tres|cuatro|cinco|seis|siete|ocho|nueve|diez|veinte|treinta|cuarenta|cincuenta|sesenta|setenta|ochenta|noventa|cien|mil|millón|millones)(?:\\s|\\b))*)\\b", RegexOptions.IgnoreCase))
            {
                input = ReplaceSpanishWordsWithNumbers(input);
            }

            return input;
        }

        private static string[] ProcessString(string cadena)
        {
            string[] allowedChars = new string[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "ñ", "Ñ", "á", "é", "í", "ó", "ú", "Á", "É", "Í", "Ó", "Ú", "ü", "Ü"
            };

            int[] res = GetFirstAndLastIndex(cadena, allowedChars);

            if (res == null)
            {
                return new string[] { "", cadena, "" };
            }

            return SplitString(cadena, res[0], res[1]);

            // 入力文字列から、許可された文字列だけを抽出
            //return new string(cadena.Where(c => allowedChars.Contains(c.ToString())).ToArray());
        }

        static string[] SplitString(string input, int start, int end)
        {
            // start インデックスと end インデックスで文字列を分割
            string part1 = input.Substring(0, start + 1);
            string part2 = input.Substring(start + 1, end - start - 1);
            string part3 = input.Substring(end);

            return new string[] { part1, part2, part3 };
        }

        static int[] GetFirstAndLastIndex(string input, string[] allowedChars)
        {
            int? firstIndex = null;
            int? lastIndex = null;
            bool is_other = false;

            // 文字列内をチェック
            for (int i = 0; i < input.Length; i++)
            {
                string s = input[i].ToString();

                if (!allowedChars.Contains(s))
                {
                    // 指定文字以外のとき

                    is_other = true;

                    // 「始まりの終わり」が入っていて「終わりの始まり」がないときにのみ
                    // 「終わりの始まり」を設定する
                    if (firstIndex != null && lastIndex == null)
                    {
                        lastIndex = i;
                    }
                }
                else if (firstIndex == null)
                {
                    if (i == 0)
                    {
                        // インデクス0のときにいきなりここに来たら、前半はなしとみなして-1
                        firstIndex = -1;
                    }

                    if (is_other)
                    {
                        // otherの終わりを検知
                        firstIndex = i - 1;
                    }
                }
            }

            //if (firstIndex == lastIndex) lastIndex = input.Length;
            if (lastIndex == null) lastIndex = input.Length;

            // 結果として最初と最後のインデックスを返す
            return firstIndex.HasValue && lastIndex.HasValue ? new int[] { firstIndex.Value, lastIndex.Value } : null;
        }

        static string ReplaceSpanishWordsWithNumbers(string input)
        {
            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            var buffer = new List<string>();
            bool is_un = false;
            string[] p_word;
            string buf_first = "";
            string buf_last = "";

            foreach (var word in words)
            {
                // ??cien??みたいなのを["??", ""cien", "??""]に分解する
                // ??unに対応してない
                p_word = ProcessString(word);

                if (TextToNumber.ContainsKey(p_word[1].ToLower()))
                {
                    // 最初がなかったら最初をいれる
                    if (buf_first == "")
                    {
                        buf_first = p_word[0];
                    }

                    // 最後がなかったら最後をいれる
                    if (buf_last == "")
                    {
                        buf_last = p_word[2];
                    }

                    // unの次に数詞が来たのでunフラグを落とす
                    if (is_un) is_un = false;

                    buffer.Add(p_word[1].ToLower());
                }
                else if (word.ToLower() == "y" && buffer.Count > 0)
                {
                    buffer.Add(word.ToLower());
                }
                else
                {
                    // これはunのあとに数詞以外が来た時、正式にunを追加する
                    if (is_un) result.Add("un");

                    // 次に数詞が来たときはunは付けないのでフラグだけを立てておく
                    is_un = (word.ToLower() == "un");

                    if (buffer.Count > 0)
                    {
                        result.Add(buf_first + SpanishToArabic(string.Join(" ", buffer)).ToString() + buf_last);
                        buffer.Clear();
                        buf_first = "";
                        buf_last = "";

                    }

                    if (!is_un) result.Add(word);
                }
            }

            if (buffer.Count > 0)
            {
                result.Add(buf_first + SpanishToArabic(string.Join(" ", buffer)).ToString() + buf_last);
            }

            return string.Join(" ", result);
        }

        private static string ArabicToSpanish(long number)
        {
            if (NumberToText.ContainsKey((int)number)) return NumberToText[(int)number];

            if (number < 30)
            {
                return NumberToText[(int)number];
            }
            else if (number < 100)
            {
                int tens = (int)(number / 10) * 10;
                int units = (int)(number % 10);
                return units == 0 ? NumberToText[tens] : $"{NumberToText[tens]} y {NumberToText[units]}";
            }
            else if (number < 1000)
            {
                int hundreds = (int)(number / 100) * 100;
                long remainder = number % 100;
                return remainder == 0 ? NumberToText[hundreds] : $"{NumberToText[hundreds]} {ArabicToSpanish(remainder)}";
            }
            else if (number < 1000000)
            {
                long thousands = number / 1000;
                long remainder = number % 1000;
                string thousandsText = thousands == 1 ? "mil" : $"{ArabicToSpanish(thousands)} mil";
                return remainder == 0 ? thousandsText : $"{thousandsText} {ArabicToSpanish(remainder)}";
            }
            else if (number < 10000000)
            {
                long millions = number / 1000000;
                long remainder = number % 1000000;
                return remainder == 0 ? $"{ArabicToSpanish(millions)} millón" : $"{ArabicToSpanish(millions)} millón {ArabicToSpanish(remainder)}";
            }

            return number.ToString();
        }

        private static long SpanishToArabic(string input)
        {
            long total = 0;
            long current = 0;

            foreach (var word in input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (TextToNumber.TryGetValue(word.ToLower(), out int value))
                {
                    if (value == 1000 || value == 1000000)
                    {
                        current = (current == 0 ? 1 : current) * value;
                        total += current;
                        current = 0;
                    }
                    else if (value % 100 == 0)
                    {
                        current += value;
                    }
                    else
                    {
                        current += value;
                    }
                }
            }

            return total + current;
        }
    }
}
