using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiBocaRecuerda
{
    // POR HACER:20250717:CoreProcessは言語に依存してはならない設計なのだが一部、言語に依存してるので要修正

    public static class CoreProcess
    {
        public static (bool isCorrect, string adopt_str) CheckAnswer(string user_input, List<Answer> correct_answer)
        {
            // 入力文字列(比較用)
            string s1 = MainForm.LangCtrl.Comparelize(user_input);
            
            // [^]、()形式の解答を分解する
            List<Answer> parsedAnswer = new List<Answer>();
            List<Answer> tmp = new List<Answer>();
            foreach (Answer ans in correct_answer)
            {
                tmp = new List<Answer>();

                foreach (string s in ParseAnswer(ans.Sentence))
                {
                    tmp.Add(new Answer(ans.ID, s));
                }

                parsedAnswer = parsedAnswer.Concat(tmp).ToList();
            }

            tmp.Clear();

            // 数の表現を追加
            foreach (Answer ans in parsedAnswer)
            {
                tmp.Add(new Answer(ans.ID, ArabicSpanish.ConvertSpanishNumbers(ans.Sentence)));
            }

            parsedAnswer = parsedAnswer.Union(tmp).ToList();

            // 正式用に比較用文字列整形前のものを置いておく
            List<Answer> parsedAnswer_raw = parsedAnswer.Select(x => (Answer)x.Clone()).ToList();

            // 比較用に成形する
            parsedAnswer.ForEach(s => s.Sentence = MainForm.LangCtrl.Comparelize(s.Sentence));

            float sim_rate_max = 1;
            float sim_rate = 0;
             
            int index = 0;

            // 比較用の採用文字列
            string _adopt_str = parsedAnswer[0].Sentence;
            // 正式の採用文字列
            string adopt_str = "";

            foreach (Answer str in parsedAnswer)
            {
                // 入力文字列と似てる方を比較として採用する
                // 0~1で0が完全一致、1がまったく違う
                sim_rate = CommonFunction.LevenshteinRate(s1, str.Sentence);
                if (sim_rate_max > sim_rate)
                {
                    _adopt_str = str.Sentence;
                    adopt_str = parsedAnswer_raw[index].Sentence;
                    sim_rate_max = sim_rate;
                }
                index++;
            }

            // 各候補の部分一致をみる
            // 入力部分まですべて一致していたらその中で優先Regionのものを抜き出す
            var imperfect = parsedAnswer
                .Select(candidate => new
                {
                    answer = candidate,
                    partially_correct = AtLeastInputCorrect(s1, candidate.Sentence),
                })
                // 部分一致しているもの
                .Where(s => s.partially_correct == true)
                // 優先Regionのもの
                .Where(s => s.answer.ID_ind().reg == MainForm.QuizFileConfig.PriorityRegion)
                .ToList();

            // 入力部分まですべて一致していたらLevenstein距離では対応しきれないので優先Regionの方を採用する
            // 完答も含めてしまうはずだがその場合でも問題ないはず
            if (imperfect.Count != 0)
            {
                _adopt_str = imperfect.FirstOrDefault().answer.Sentence;
            }

            // 相違確認
            string distinction = MainForm.LangCtrl.GetDistinction(s1, _adopt_str);

            if (distinction != "")
            {
                // 相違があった場合は、相違の箇所を設定する
                adopt_str = distinction;

                return (false, adopt_str);
            }

            // 入力と一致すれば採用したパターンを設定
            return (true, adopt_str);
        }

        private static bool AtLeastInputCorrect(string input, string candidate)
        {
            // 入力の方が大きいときは確認しない
            if (input.Length > candidate.Length) return false;

            int score = 0;

            for (int cnt = 0; cnt < input.Length; cnt++)
            {
                if (input[cnt] == candidate[cnt])
                {
                    score++;
                }
            }

            // 入力が部分一致していない場合
            if (score != input.Length) return false;

            return true;
        }

        // 解答DBの定義形式から解答群を抽出する ()とか[^]とかを使ってる時用
        public static List<string> ParseAnswer(string s)
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

            ans.ForEach(a => UtilityFunction.ReplaceConsecutiveSpaces(a));

            return ans;
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

                        if (start.Count == 1 && plane_idx != -1)
                        {
                            // (が始まるときに、強制文字列があればそれを保存
                            must.Add(sp_res.Count);
                            sp_res.Add(cadena.Substring(plane_idx, i - plane_idx));
                        }

                        break;
                    case ')':

                        end.Add(i);

                        // ()形式が行儀よくあるときしか想定していないからこの条件式はたぶん役に立たない
                        if (start.Count == end.Count)
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
                        if (plane_idx == -1)
                        {
                            plane_idx = i;
                        }

                        break;
                }
            }

            // 強制文字列で終わっていたらそれを保存
            if (plane_idx != -1)
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
                if ((n & rom) == rom)
                {
                    // nのbitが立っているところが表示する文字列
                    // (n & rom)でフィルタしてるから強制文字列は絶対に表示される

                    string s = "";

                    for (int m = 0; m < sp_res.Count; m++)
                    {
                        if ((n & (0x1 << m)) != 0)
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
    }
}
