using System.Collections.Generic;
using System.Linq;

namespace MiBocaRecuerda
{
    public static class SettingManager
    {
        // ファイルごとの設定
        // Dic<Language name, Dic<File, Config>>
        public static Dictionary<string, Dictionary<string, CommonConfig>> CommonConfigManager = new Dictionary<string, Dictionary<string, CommonConfig>>();
        // 言語単位の設定
        public static Dictionary<string, LanguageConfig> LanguageConfigManager = new Dictionary<string, LanguageConfig>();
        public static InputCache InputCache = new InputCache();
        public static RomConfig RomConfig = new RomConfig();

        // 現在のクイズ言語
        public static string CurrentLangType { get; set; } = "";
        // 現在のクイズDB
        public static string CurrentQuizDB { get; set; }
        // 現在の読込ファイルの設定
        public static QuizFileConfig CurrentQuizFileConfig => CommonConfigManager[CurrentLangType][CurrentQuizDB].QuizFileConfig;

        public static QuizFileConfig GetQuizFileConfig(string langType, string quizDB)
        {
            return CommonConfigManager[langType][quizDB].QuizFileConfig;
        }

        public static IManageInput LangCtrl
        {
            get
            {
                if (AppRom.ManageLanguage_Dic.TryGetValue(CurrentLangType, out var lang))
                {
                    return lang;
                }
                return null;
            }
        }

        public static FileLenguaConfig currentLengua(string type)
        {
            return CommonConfigManager[type][CurrentQuizDB].LenguaConfig;
        }

        // ファイルごとのクイズ情報をすべて取得
        public static Dictionary<string, QuizFileConfig> GetAllQuizFileConfig(string language)
        {
            Dictionary<string, QuizFileConfig> ret = new Dictionary<string, QuizFileConfig>();

            foreach (var item in CommonConfigManager[language])
            {
                ret.Add(item.Key, item.Value.QuizFileConfig.Clone());
            }

            return ret;
        }
    }

    // 全体設定
    public class CommonConfig
    {
        public QuizFileConfig QuizFileConfig { get; set; }
        public FileLenguaConfig LenguaConfig { get; set; }

        public CommonConfig(QuizFileConfig qfc, FileLenguaConfig lc)
        {
            QuizFileConfig = qfc;
            LenguaConfig = lc;
        }
    }

    // ファイルごとのクイズ設定
    public class QuizFileConfig
    {
        // Chapter単位は10個を基準としている

        public int MinChapter { get; set; } = 1;
        public int MinChapterToIndex => MinChapter * 10 - 9;
        public int MaxChapter { get; set; } = 1;
        public int MaxChapterToIndex => MaxChapter * 10;
        public bool IsMaxChapter => UtilityFunction.Techo(MaxQuizNum, 10) <= MaxChapter;
        // 許容問題数
        public int PermitNum
        {
            get
            {
                int permitNum = (MaxChapter - MinChapter + 1) * 10;

                if (IsMaxChapter)
                {
                    // 最大章であれば溢れた分を差し引く
                    permitNum -= MaxChapter * 10 - MaxQuizNum;
                }

                return permitNum;
            }
        }

        private int _quizNum = 10;
        public int QuizNum
        {
            get
            {
                // 許容問題数を超過していれば許容問題数にする
                if (_quizNum > PermitNum)
                {
                    return PermitNum;
                }
                else
                {
                    return _quizNum;
                }
            }
            set
            {
                _quizNum = value;
            }
        }

        public int ErrorAllowCnt { get; set; } = 0;
        public bool ErrorAllowAll { get; set; } = false;
        // エラー数が満了したときにエラー数をリセットするか(ErrorAlloAllが有効のときに有効な設定)
        public bool ErrorReset { get; set; } = false;
        public string PriorityRegion { get; set; } = "";
        public int MaxQuizNum { get; set; } = -1;

        public void Copy(QuizFileConfig qfc)
        {
            MinChapter = qfc.MinChapter;
            MaxChapter = qfc.MaxChapter;
            QuizNum = qfc.QuizNum;
            ErrorAllowCnt = qfc.ErrorAllowCnt;
            ErrorAllowAll = qfc.ErrorAllowAll;
            ErrorReset = qfc.ErrorReset;
            PriorityRegion = qfc.PriorityRegion;
        }

        public QuizFileConfig Clone()
        {
            return new QuizFileConfig
            {
                MinChapter = MinChapter,
                MaxChapter = MaxChapter,
                QuizNum = QuizNum,
                ErrorAllowCnt = ErrorAllowCnt,
                ErrorAllowAll = ErrorAllowAll,
                ErrorReset = ErrorReset,
                PriorityRegion = PriorityRegion,
                MaxQuizNum = MaxQuizNum
            };
        }
    }

    // ファイルごとの言語設定
    public class FileLenguaConfig
    {
        public bool Capital { get; set; } = false;
        public bool ComaPunto { get; set; } = false;
    }

    // 言語単位の設定
    public class LanguageConfig
    {
        public List<string> InputSupport { get; set; }

        public LanguageConfig() { }

        public LanguageConfig(List<string> inputSupport)
        {
            InputSupport = inputSupport;
        }
    }

    public class InputCache
    {
        // 完答
        public bool Complete { get; set; } = false;
        // Ejercicioの種類
        public bool Exercise { get; set; } = false;
        // Resultado表示設定
        public bool Result { get; set; } = false;
        public string QuizFileName { get; set; } = string.Empty;
        public bool DarkMode { get; set; } = false;

        public InputCache() { }
    }

    public class RomConfig
    {
        public string ResourcePath { get; set; }

        public RomConfig() { }
    }

    public class QuizInfo
    {
        public string Quiz { get; set; }
        public Dictionary<string, List<Answer>> CorrectAnswer { get; set; }
        public int QuizNum { get; set; }
        public string Supplement { get; set; }

        public IEnumerable<Answer> Answers(string region)
        {
            if (!CorrectAnswer.TryGetValue(region, out var list))
            {
                list = CorrectAnswer.FirstOrDefault().Value;
            }

            foreach (Answer ans in list)
            {
                yield return ans;
            }
        }

        public IEnumerable<Answer> Answers()
        {
            foreach (KeyValuePair<string, List<Answer>> kvp in CorrectAnswer)
            {
                foreach (Answer ans in kvp.Value)
                {
                    yield return ans;
                }
            }
        }

        public QuizInfo(ExerciseDB edb)
        {
            Quiz = edb.Problem;
            CorrectAnswer = edb.Answer;
            QuizNum = edb.Num;
            Supplement = edb.Supplement;
        }
    }

    // クイズ内容
    public class QuizContents : QuizInfo
    {
        public string Section { get; set; }
        // 入力補助
        public List<string> AutoNombre { get; set; }
        // ユーザ入力
        public string Input { get; set; }
        // 答え合わせ
        public bool IsCorrect { get; set; }

        public QuizContents(ExerciseDB edb) : base(edb)
        {
            Section = edb.Section;
            AutoNombre = edb.Auxiliary;
        }
    }
}
