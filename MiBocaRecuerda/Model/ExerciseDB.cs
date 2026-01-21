using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MiBocaRecuerda
{
    public class Answer
    {
        // 問題番号-Region-通番
        public string ID { get; set; }
        public string Sentence { get; set; }

        // 問題番号-Region-通番を個別に取得
        public (int id1, string reg, int id2) ID_ind()
        {
            var match = Regex.Match(ID, @"^(\d+)-(.+)-(\d+)$");

            if (!match.Success)
                throw new ArgumentException("形式が正しくありません", nameof(ID));

            int id1 = int.Parse(match.Groups[1].Value);
            string reg = match.Groups[2].Value;
            int id2 = int.Parse(match.Groups[3].Value);

            return (id1, reg, id2);
        }

        public Answer(string id, string s)
        {
            ID = id;
            Sentence = s;
        }
    }

    // 編集状態入りの解答
    public class EditAnswer : Answer
    {
        // 操作bit bit0:Update, bit1:Insert, bit2:Delete
        public AppRom.SqlOperation SqlOperation;

        public EditAnswer(string id, string s, AppRom.SqlOperation so = 0) : base(id, s)
        {
            SqlOperation = so;
        }
    }

    public class ExerciseDB
    {
        public string Language { get; set; }
        public int Num { get; set; }
        public string Problem { get; set; }
        public string Section { get; set; }
        public string Supplement { get; set; }
        // region:answer
        public Dictionary<string, List<Answer>> Answer { get; set; } = new Dictionary<string, List<Answer>>();
        public List<string> Auxiliary { get; set; } = new List<string>();
        public List<string> Update { get; set; } = new List<string>();
    }
}
