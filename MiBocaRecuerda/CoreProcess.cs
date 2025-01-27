using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiBocaRecuerda
{
    public class CoreProcess
    {
        public string adopt_str = "";

        public bool CheckAnswer(string user_input, string correct_answer)
        {
            string s1 = Comparelize(user_input);

            // _correctは成形前
            List<string> _correct = ParseAnswer(correct_answer);

            List<string> buffer = new List<string>();

            foreach (string str in _correct)
            {
                buffer.Add(ArabicSpanish.ConvertSpanishNumbers(str));
            }

            _correct = _correct.Union(buffer).ToList();

            // 比較用に成形する
            List<string> correct = _correct.Select(s => Comparelize(s)).ToList();

            float max_sim = 1;
            float res = 0;

            adopt_str = "";

            int index = 0;
            string _adopt_str = correct[0];

            foreach (string str in correct)
            {
                // 入力文字列と似てる方を比較として採用する
                // 0~1で0が完全一致、1がまったく違う
                res = CommonFunction.LevenshteinRate(s1, str);
                if (max_sim > res)
                {
                    _adopt_str = str;
                    adopt_str = _correct[index];
                    max_sim = res;
                }
                index++;
            }

            return DisplayDifferences(s1, _adopt_str);
        }

        // 解答DBの定義形式から解答群を抽出する ()とか[^]とかを使ってる時用
        public List<string> ParseAnswer(string s)
        {
            // ()のある文字列を分離させる
            List<string> abbreviation = ParseBrackets(s);

            List<string> ans = new List<string>();

            // [^]のある文字列を分離させる
            foreach (string str in abbreviation)
            {
                ans.AddRange(ExpandAlternatives(str));
            }

            ans.Sort();
            ans = ans.Distinct().ToList();

            ans.ForEach(a => ReplaceConsecutiveSpaces(a));

            return ans;
        }

        private string Comparelize(string str)
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

            if (MainForm.QuizFileConfig.ComaPunto)
            {
                s2 = s2.Replace(".", "");
                s2 = s2.Replace(",", "");
            }

            s2 = ReplaceConsecutiveSpaces(s2);

            s2 = (new Regex(" $")).Replace(s2, "");

            // 先頭の空行をなくす
            s2 = (new Regex("^ +")).Replace(s2, "");

            return s2;
        }

        // ()で囲まれた部分を任意文字列とする
        // a(b)c(d)e -> ace,abce,acde,abcde これを生成する
        // ネストには対応していない a(b(c)) こういうやつ
        static List<string> ParseBrackets(string cadena)
        {
            List<int> start = new List<int>();
            List<int> end = new List<int>();
            List<string> sp_res = new List<string>();
            List<int> must = new List<int>();
            int plane_idx = -1;

            for (int i = 0; i < cadena.Length; i++)
            {
                switch (cadena[i])
                {
                    case '(':

                        start.Add(i);

                        if(start.Count == 1 && plane_idx != -1)
                        {
                            // (が始まるときに、強制文字列があればそれを保存
                            must.Add(sp_res.Count);
                            sp_res.Add(cadena.Substring(plane_idx, i - plane_idx));
                        }

                        break;
                    case ')':

                        end.Add(i);

                        // ()形式が行儀よくあるときしか想定していないからこの条件式はたぶん役に立たない
                        if(start.Count == end.Count)
                        {
                            // ()の中身ほ保存、次に移るために作業スペースをクリア
                            sp_res.Add(cadena.Substring(start[0] + 1, i - (start[0] + 1)));
                            start.Clear();
                            end.Clear();
                            plane_idx = -1;
                        }

                        break;

                    default:
                        
                        // 強制文字列の始まり位置を保存
                        if(plane_idx == -1)
                        {
                            plane_idx = i;
                        }

                        break;
                }
            }

            // 強制文字列で終わっていたらそれを保存
            if(plane_idx != -1)
            {
                must.Add(sp_res.Count);
                sp_res.Add(cadena.Substring(plane_idx));
            }

            int rom = 0;
            List<string> result = new List<string>();

            // 強制文字列の位置をbitで表示
            foreach (int n in must)
            {
                rom |= (0x1 << n);
            }

            for (int n = 0; n < Math.Pow(2, sp_res.Count); n++)
            {
                // 強制文字列があるやつだけを対象にする
                if((n & rom) == rom)
                {
                    // nのbitが立っているところが表示する文字列
                    // (n & rom)でフィルタしてるから強制文字列は絶対に表示される

                    string s = "";

                    for (int m = 0; m < sp_res.Count; m++)
                    {
                        if((n & (0x1 << m)) != 0)
                        {
                            // onのbitに対応する文字列を追加していく
                            s += sp_res[m];
                        }
                    }

                    result.Add(s);
                }
            }

            return result;
        }

        static List<string> ExpandAlternatives(string input)
        {
            // 再帰的に文字列を展開するためのヘルパー関数
            List<string> Expand(string text)
            {
                // 正規表現で最も外側の [^] を検出
                var match = Regex.Match(text, @"\[(?<content>[^\[\]]+?)\]");
                if (!match.Success)
                {
                    // [^] がない場合、リストにそのまま返す
                    return new List<string> { text };
                }

                // マッチした部分を分解
                string before = text.Substring(0, match.Index); // マッチの前
                string after = text.Substring(match.Index + match.Length); // マッチの後
                string[] options = match.Groups["content"].Value.Split('^'); // [^]の中身を分解

                // 各選択肢を展開し、再帰的に結合
                var results = new List<string>();
                foreach (string option in options)
                {
                    foreach (string expanded in Expand(before + option + after))
                    {
                        if (!results.Contains(expanded))
                        {
                            results.Add(expanded);
                        }
                    }
                }

                results.Sort();

                return results;
            }

            // 展開処理の呼び出し
            return Expand(input);
        }

        private string ReplaceConsecutiveSpaces(string input)
        {
            // 連続したスペースを一つのスペースに置き換える
            string pattern = @"\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        private bool DisplayDifferences(string str1, string str2)
        {
            int maxLength = Math.Max(str1.Length, str2.Length);

            // 文字列の長さが異なる場合は、*で埋める
            str1 = str1.PadRight(maxLength, '*');
            str2 = str2.PadRight(maxLength, '*');

            // 差分を検出し、周辺数文字を表示
            for (int i = 0; i < maxLength; i++)
            {
                // string.Compareの第3引数をtrueにすると文字列の大小を区別しない
                if (string.Compare(str1[i].ToString(), str2[i].ToString(), MainForm.QuizFileConfig.Capital) != 0)
                {
                    Console.WriteLine($"差分が見つかりました: 位置 {i}, 前の文字列: {GetContext(str1, i)}, 後ろの文字列: {GetContext(str2, i)}");
                    adopt_str = $"{i}: {GetContext(str1, i)} -> {GetContext(str2, i)}";
                    return false;
                }
            }

            return true;
        }

        private string GetContext(string str, int index, int contextLength = 3)
        {
            int start = Math.Max(0, index - contextLength);

            int space_cnt = CountSpaces(str.Substring(start, Math.Abs(start - index)));

            if (space_cnt > 0)
            {
                start = Math.Max(0, index - (contextLength + space_cnt));
            }
            int end = Math.Min(str.Length - 1, index + contextLength);

            space_cnt = CountSpaces(str.Substring(index, end - index));

            if (space_cnt > 0)
            {
                end = Math.Min(str.Length - 1, index + (contextLength + space_cnt));
            }

            return str.Substring(start, end - start + 1);
        }

        private int CountSpaces(string str)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    count++;
                }
            }
            return count;
        }
    }
}
