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
            string input = SettingManager.LangCtrl?.Comparelize(user_input);
            
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
            parsedAnswer.ForEach(s => s.Sentence = SettingManager.LangCtrl?.Comparelize(s.Sentence));

            float sim_rate_max = 1;
            float sim_rate = 0;
             
            int index = 0;

            // 比較用の採用文字列
            string _adopt_str = parsedAnswer[0].Sentence;
            // 正式の採用文字列
            string adopt_str = "";

            foreach (Answer str in parsedAnswer)
            {
                if (str.Sentence.StartsWith(input))
                {
                    // 空入力もここ
                    sim_rate = 0;
                }
                else
                {
                    // 入力文字列と似てる方を比較として採用する
                    // 0~1で0が完全一致、1がまったく違う
                    sim_rate = CommonFunction.LevenshteinRate(input, str.Sentence);
                }

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
                    partially_correct = AtLeastInputCorrect(input, candidate.Sentence),
                })
                // 部分一致しているもの
                .Where(s => s.partially_correct == true)
                // 優先Regionのもの
                .Where(s => s.answer.ID_ind().reg == SettingManager.CurrentQuizFileConfig.PriorityRegion)
                .ToList();

            // 入力部分まですべて一致していたらLevenstein距離では対応しきれないので優先Regionの方を採用する
            // 完答も含めてしまうはずだがその場合でも問題ないはず
            if (imperfect.Count != 0)
            {
                _adopt_str = imperfect.FirstOrDefault().answer.Sentence;
            }

            // 相違確認
            string distinction = SettingManager.LangCtrl?.GetDistinction(input, _adopt_str);

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

        public static SortedDictionary<int, (string quiz, Answer answer)> GetHandBook(List<QuizContents> workBook)
        {
            // Dic<ID, (quiz, answer)>
            // ＜IDの説明＞
            // IDを32bitとして、下位16bitを問題番号、上位16bitを解答パターン
            // 例えば、問題番号1として解答パターンが3個あるときは
            // quiz1:0x00010001, quiz2:0x00020001, quiz3:0x00030001 となる
            // 解答パターンが1個のときは上位16bitにbitはたたない→ 0x00000001となる

            List<Answer> parsedAnswer = new List<Answer>();

            Comparer<int> comparer = Comparer<int>.Create((x, y) =>
            {
                // 下位16bitでソートされるようにする
                int result = (x & 0xFFFF).CompareTo(y & 0xFFFF);

                if (result != 0) return result;

                // resultが0のとき、下位16bitが一致で全体一致とされてしまうのでその場合は改めて全体で比較する
                // 例えば、0x00010001と0x00000001がキー重複と判断されるのをさける
                return x.CompareTo(y);
            });

            SortedDictionary<int, (string quiz, Answer answer)> handBook = new SortedDictionary<int, (string quiz, Answer answer)>(comparer);

            foreach (QuizContents quizContents in workBook)
            {
                parsedAnswer.Clear();

                // パース後のanswerを作る
                foreach (KeyValuePair<string, List<Answer>> kvp in quizContents.CorrectAnswer)
                {
                    List<Answer> tmp = new List<Answer>();

                    foreach (Answer ans in kvp.Value)
                    {
                        // ここまではDic<region, answers>の数通りだが、ここで分裂する可能性がある
                        foreach (string a in ParseAnswer(ans.Sentence))
                        {
                            tmp.Add(new Answer(ans.ID, a));
                        }
                    }

                    parsedAnswer = parsedAnswer.Concat(tmp).ToList();
                }

                // 答え全体コピー用を生成する(「答え」は複数パターンある場合があるのでDGVの表示をそのまま使えない)
                if (parsedAnswer.Count == 1)
                {
                    // 解答パターンが複数ない場合
                    handBook[quizContents.QuizNum] = (quizContents.Quiz, parsedAnswer[0]);
                }
                else
                {
                    // 解答パターンが複数
                    for (int i = 0; i < parsedAnswer.Count; i++)
                    {
                        // 下位16ビットは問題番号として17ビット以降を解答パターン通番にする
                        handBook[quizContents.QuizNum | ((i + 1) << 16)] = (quizContents.Quiz, parsedAnswer[i]);
                    }
                }
            }

            return handBook;
        }

        // 指定リージョン
        public static List<string> GetHandBookContents_IndividualRegion(SortedDictionary<int, (string quiz, Answer answer)> handBook,string selectedRegion, bool isListAll)
        {
            string quiz = "", answer;
            List<string> ret = new List<string>();

            foreach (var rc in handBook)
            {
                // ハンドブックの問題番号が一致するやつのregion種類を数える
                List<string> regions = handBook.Where(r => (r.Key & 0xffff) == (rc.Key & 0xffff)).Select(v => v.Value.answer.ID_ind().reg).ToList();
                int reg_cnt = regions.Distinct().Count();

                // 表全ての時のみ問題をつける
                if (isListAll == true)
                {
                    quiz = Regex.Replace(rc.Value.quiz, @"\r\n|\r|\n", "") + "\t";
                }
                answer = Regex.Replace(rc.Value.answer.Sentence, @"\r\n|\r|\n", "");

                // 0xffff0000の部分にビットがある場合は、解答パターンが複数あるとき
                if ((rc.Key & 0xffff0000) != 0)
                {
                    if (reg_cnt > 1)
                    {
                        if (!regions.Contains(selectedRegion))
                        {
                            // Regionが複数あるけど指定Regionの表現が存在しないときはすべて出す
                            ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}:({rc.Value.answer.ID_ind().reg})\t{quiz}{answer}");
                        }
                        else
                        {
                            // 指定Regionが存在する場合はそれだけを出す
                            if (rc.Value.answer.ID_ind().reg == selectedRegion)
                            {
                                ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}\t{quiz}{answer}");
                            }
                        }
                    }
                    else
                    {
                        // Regionが一つだけのときは無条件に出す
                        ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}\t{quiz}{answer}");
                    }
                }
                else
                {
                    // 解答パターンが複数ないとき
                    ret.Add($"{rc.Key}\t{quiz}{answer}");
                }
            }

            return ret;
        }

        // 全てのregion
        public static List<string> GetHandBookContents_AllRegion(SortedDictionary<int, (string quiz, Answer answer)> handBook, bool isListAll)
        {
            string quiz = "", answer;
            List<string> ret = new List<string>();

            foreach (var rc in handBook)
            {
                // ハンドブックの問題番号が一致するやつのregion種類を数える
                int reg_cnt = handBook.Where(r => (r.Key & 0xffff) == (rc.Key & 0xffff)).Select(v => v.Value.answer.ID_ind().reg).Distinct().Count();

                string region = "";

                if (reg_cnt > 1)
                {
                    region = $":({rc.Value.answer.ID_ind().reg})";
                }

                // 表全ての時のみ問題をつける
                if (isListAll == true)
                {
                    quiz = Regex.Replace(rc.Value.quiz, @"\r\n|\r|\n", "") + "\t";
                }
                answer = Regex.Replace(rc.Value.answer.Sentence, @"\r\n|\r|\n", "");

                // 0xffff0000の部分にビットがある場合は、解答パターンが複数あるとき
                if ((rc.Key & 0xffff0000) != 0)
                {
                    ret.Add($"{rc.Key & 0xffff}-{(rc.Key >> 16)}{region}\t{quiz}{answer}");
                }
                else
                {
                    ret.Add($"{rc.Key}{region}\t{quiz}{answer}");
                }
            }

            return ret;
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
