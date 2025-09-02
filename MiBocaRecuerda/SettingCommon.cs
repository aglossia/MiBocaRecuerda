using System.Collections.Generic;

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

        public static FileLenguaConfig currentLengua(string type)
        {
            return CommonConfigManager[type][MainForm.currentQuizFile].LenguaConfig;
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

        private int _quizNum = 10;
        public int QuizNum
        {
            get
            {
                // クイズ数が最大クイズ数をこえていた場合は最大クイズ数にする
                // シフトで次の問題に移動した時に端に来た時に調整されてズレがでたときを想定
                if(_quizNum > MaxQuizNum)
                {
                    return MaxQuizNum;
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
        public int MaxQuizNum => (MaxChapter - MinChapter + 1) * 10;
        public bool Validation()
        {
            if (MinChapter > MaxChapter) return false;

            if ((MaxChapter - MinChapter + 1) * 10 < QuizNum) return false;

            return true;
        }

        public void Copy(QuizFileConfig qfc)
        {
            MinChapter = qfc.MinChapter;
            MaxChapter = qfc.MaxChapter;
            QuizNum = qfc.QuizNum;
            ErrorAllowCnt = qfc.ErrorAllowCnt;
            ErrorAllowAll = qfc.ErrorAllowAll;
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
        public bool Complete { get; set; }
        // Ejercicioの種類
        public bool Exercise { get; set; }
        // Resultado表示設定
        public bool Result { get; set; }
        public string QuizFileName { get; set; }
        public bool DarkMode { get; set; }

        public InputCache() { }
    }

    public class RomConfig
    {
        public string QuizFilePath { get; set; }

        public RomConfig() { }
    }

    // クイズ内容
    public class QuizContents
    {
        public string Quiz { get; set; }
        public string CorrectAnswer { get; set; }
        public int QuizNum { get; set; }
        public string ChapterTitle { get; set; }
        public string ChapterExample { get; set; }
        public string Supplement { get; set; }
        public List<string> AutoNombre { get; set; }

        public QuizContents(string quiz, string ca, int qn, string ct, string ce, string s, List<string> an)
        {
            Quiz = quiz;
            CorrectAnswer = ca;
            QuizNum = qn;
            ChapterTitle = ct;
            ChapterExample = ce;
            Supplement = s;
            AutoNombre = an;
        }
    }

    // クイズ結果
    public class QuizResult
    {
        public string Quiz { get; set; }
        public string CorrectAnswer { get; set; }
        public string Input { get; set; }
        public int QuizNum { get; set; }
        public string Supplement { get; set; }
        public bool Result { get; set; }

        public QuizResult(string quiz, string ca, string input, int quizNum, string s, bool result = false)
        {
            Quiz = quiz;
            CorrectAnswer = ca;
            Input = input;
            QuizNum = quizNum;
            Supplement = s;
            Result = result;
        }
    }
}
