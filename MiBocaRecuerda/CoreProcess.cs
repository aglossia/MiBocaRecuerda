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

            // ()のある文字列を分離させる
            List<string> abbreviation = ProcessString(correct_answer);

            List<string> _correct = new List<string>();

            // [^]のある文字列を分離させる
            foreach (string str in abbreviation)
            {
                _correct.AddRange(ParseString(str));
            }

            // 比較用に成形する
            List<string> correct = _correct.Select(s => Comparelize(s)).ToList();

            float max_sim = 1;
            float res = 0;

            adopt_str = _correct[0];

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

        // 正解を整形して出力
        public List<string> ShowAnswer(string correct_answer)
        {
            List<string> abbreviation = ProcessString(correct_answer);

            List<string> ans = new List<string>();

            foreach (string str in abbreviation)
            {
                ans.AddRange(ParseString(str));
            }

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

        private List<string> ProcessString(string input)
        {
            List<string> outputList = new List<string>();

            // 正規表現を使用して(***)の形式の部分を探す
            string pattern = @"\(([^)]+)\)";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string prefix = input.Substring(0, match.Index);
                    string suffix = input.Substring(match.Index + match.Length);
                    string part = match.Groups[1].Value;

                    // (***)の形式の部分を削除した文字列を出力リストに追加
                    outputList.Add(prefix + suffix);
                    // (***)の形式の部分を置き換えて出力リストに追加
                    outputList.Add(prefix + part + suffix);
                }
            }
            else
            {
                outputList.Add(input);
            }

            return outputList.Select(s => ReplaceConsecutiveSpaces(s)).ToList();
        }

        private List<string> ParseString(string input)
        {
            List<string> result = new List<string>();
            result.Add(input);
            Regex regex = new Regex(@"\[(.*?)\]");
            MatchCollection matches = regex.Matches(input);

            if(matches.Count != 0)
            {
                // [^]形式が見つかった
                foreach (Match ma in matches)
                {
                    string content = ma.Groups[1].Value;
                    string[] parts = content.Split('^');
                    List<string> tmp = new List<string>();

                    foreach (string part in parts)
                    {
                        foreach (string res in result)
                        {
                            tmp.Add(res.Replace("[" + content + "]", part));
                        }
                    }
                    // 現在の置換分を結果に入れておく[^]の形式が複数あるとき用
                    result = tmp;
                }
            }

            return result;
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
