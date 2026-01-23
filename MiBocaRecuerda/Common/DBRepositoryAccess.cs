using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MiBocaRecuerda
{
    public interface IDBRepository
    {
        ExerciseDB GetByNum(int num);
        IEnumerable<ExerciseDB> GetAll();
    }

    public class ExerciseRepository : IDBRepository
    {
        private readonly string _connectionString;

        public ExerciseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Exercise数を取得
        public int GetExerciseCount()
        {
            int count = 0;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM exercise";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt16(0);
                    }
                }
            }

            return count;
        }

        // 言語コードを取得
        public string GetLanguage()
        {
            string lang = "";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM language";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lang = reader.GetString(0);
                    }
                }
            }

            return lang;
        }

        public List<string> GetAllSection()
        {
            List<string> sections = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "select str from section";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sections.Add(reader.GetString(0));
                    }
                }
            }

            return sections;
        }

        // ExerciseDBを取得
        public ExerciseDB GetByNum(int num)
        {
            ExerciseDB exerciseDB = new ExerciseDB();
            exerciseDB.Num = num;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM language";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exerciseDB.Language = reader.GetString(0);
                    }
                }

                sql = @"
                        select a.id, a.num, problem, s.str, region, a.sentence, supplement
                        from exercise as e
                        inner join answer as a
                        inner join section as s
                        on e.num = a.num and e.section = s.num
                        where e.num = @num
                        ";

                string id;
                string problem;
                string section;
                string region;
                string sentence;

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.StepCount < 1) return null;

                        while (reader.Read())
                        {
                            id = reader.GetString(0);
                            problem = reader.GetString(2);
                            section = reader.GetString(3);
                            region = reader.GetString(4);
                            sentence = reader.GetString(5);

                            exerciseDB.Problem = problem;
                            exerciseDB.Section = section;

                            if (!exerciseDB.Answer.TryGetValue(region, out var list))
                            {
                                list = new List<Answer>();
                                exerciseDB.Answer[region] = list;
                            }
                            list.Add(new Answer(id, sentence));

                            exerciseDB.Supplement = reader.GetString(6);
                        }
                    }
                }

                sql = @"
                        select str
                        from auxiliary
                        where id like @pattern
                        ";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@pattern", $"{num}-%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exerciseDB.Auxiliary.Add(reader.GetString(0));
                        }
                    }
                }

                sql = @"
                        select a.num, up.date
                        from answer as a
                        inner join _update as up
                        on a.num = up.num
                        where a.num = @num
                        ";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exerciseDB.Update.Add(reader.GetString(1));
                        }
                    }
                }
            }

            return exerciseDB;
        }

        public IEnumerable<ExerciseDB> GetAll()
        {
            var list = new List<ExerciseDB>();

            //using (var connection = new SQLiteConnection(_connectionString))
            //{
            //    connection.Open();

            //    const string sql = "SELECT id, name FROM users";

            //    using (var command = new SQLiteCommand(sql, connection))
            //    using (var reader = command.ExecuteReader())
            //    {

            //        while (reader.Read())
            //        {
            //            list.Add(new ExerciseDB
            //            {
            //                Id = reader.GetInt32(0),
            //                Name = reader.GetString(1)
            //            });
            //        }
            //    }
            //}

            return list;
        }

        public void EditDB(int num, string problem, string supplement, List<string> auxs, string date, Dictionary<string, List<EditAnswer>> answers)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            if (problem != null)
                            {
                                cmd.CommandText = "UPDATE exercise SET problem = @problem WHERE num = @num";
                                cmd.Parameters.AddWithValue("@num", num);
                                cmd.Parameters.AddWithValue("@problem", problem);
                                cmd.ExecuteNonQuery();
                            }

                            if (supplement != null)
                            {
                                cmd.CommandText = "UPDATE exercise SET supplement = @supplement WHERE num = @num";
                                cmd.Parameters.AddWithValue("@num", num);
                                cmd.Parameters.AddWithValue("@supplement", supplement);
                                cmd.ExecuteNonQuery();
                            }

                            if (auxs != null)
                            {
                                int cnt = 1;

                                foreach (string aux in auxs)
                                {
                                    cmd.CommandText = @"INSERT INTO auxiliary VALUES(@id, @str)
                                                        ON CONFLICT(id)
                                                        DO UPDATE SET str = @str";
                                    cmd.Parameters.AddWithValue("@id", $"{num}-{cnt++}");
                                    cmd.Parameters.AddWithValue("@str", aux);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            if (date != null)
                            {
                                cmd.CommandText = "INSERT INTO _update VALUES(@num, @date)";
                                cmd.Parameters.AddWithValue("@num", num);
                                cmd.Parameters.AddWithValue("@date", date);
                                cmd.ExecuteNonQuery();
                            }

                            foreach (KeyValuePair<string, List<EditAnswer>> kvp in answers)
                            {
                                foreach (EditAnswer ans in kvp.Value)
                                {
                                    switch (ans.SqlOperation)
                                    {
                                        case AppRom.SqlOperation.Update:
                                            cmd.CommandText = "UPDATE answer SET sentence = @sentence WHERE id = @id";
                                            cmd.Parameters.AddWithValue("@id", ans.ID);
                                            cmd.Parameters.AddWithValue("@sentence", ans.Sentence);
                                            cmd.ExecuteNonQuery();
                                            break;
                                        case AppRom.SqlOperation.Insert:
                                            cmd.CommandText = "INSERT INTO answer VALUES(@id, @num, @region, @sentence)";
                                            cmd.Parameters.AddWithValue("@id", ans.ID);
                                            cmd.Parameters.AddWithValue("@num", num);
                                            cmd.Parameters.AddWithValue("@region", kvp.Key);
                                            cmd.Parameters.AddWithValue("@sentence", ans.Sentence);
                                            cmd.ExecuteNonQuery();
                                            break;
                                        case AppRom.SqlOperation.Delete:
                                            cmd.CommandText = "DELETE FROM answer WHERE id = @id AND num = @num AND region = @region";
                                            cmd.Parameters.AddWithValue("@id", ans.ID);
                                            cmd.Parameters.AddWithValue("@num", num);
                                            cmd.Parameters.AddWithValue("@region", kvp.Key);
                                            cmd.Parameters.AddWithValue("@sentence", ans.Sentence);
                                            cmd.ExecuteNonQuery();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        // 問題なければ確定
                        tran.Commit();
                    }
                    catch
                    {
                        // 何かあったら全部取り消し
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // problemをUPDATEする
        public void UpdateProblem(int num, string problem)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE exercise SET problem = @problem WHERE num = @num";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@problem", problem);

                    command.ExecuteNonQuery();
                }
            }
        }

        // answerをUPDATEする
        public void UpdateAnswer(int id, int num, string region, string sentence)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE answer SET sentence = @sentence WHERE id = @id AND num = @num AND region = @region";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@region", region);
                    command.Parameters.AddWithValue("@sentence", sentence);

                    command.ExecuteNonQuery();
                }
            }
        }

        // answerをINSERTする
        public void InsertAnswer(int id, int num, string region, string sentence)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO answer VALUES(@id, @num, @region, @sentence)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@region", region);
                    command.Parameters.AddWithValue("@sentence", sentence);

                    command.ExecuteNonQuery();
                }
            }
        }

        // supplementをUPDATEする
        public void UpdateSupplement(int num, string supplement)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE exercise SET supplement = @supplement WHERE num = @num";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@supplement", supplement);

                    command.ExecuteNonQuery();
                }
            }
        }

        // auxiliaryをUPDATEする
        public void UpdateAuxiliary(int num, string aux)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "UPDATE auxiliary SET aux = @aux WHERE num = @num";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@aux", aux);

                    command.ExecuteNonQuery();
                }
            }
        }

        // auxiliaryをINSERTする
        public void InsertAuxiliary(int num, string aux)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO auxiliary VALUES(@num, @aux)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@aux", aux);

                    command.ExecuteNonQuery();
                }
            }
        }

        // dateをINSERTする
        public void InsertDate(int num, string date)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO _update VALUES(@num, @date)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@date", date);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
