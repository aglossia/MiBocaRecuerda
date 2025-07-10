using System.Collections.Generic;

namespace MiBocaRecuerda
{
    public static class SettingManager
    {
        public static AppConfig AppConfig = new AppConfig();
        public static InputCache InputCache = new InputCache();
        public static RomConfig RomConfig = new RomConfig();
    }

    public class AppConfig
    {
        public QuizFileConfig quizFileConfig = new QuizFileConfig();
        public AppConfig() { }
    }

    // クイズファイルごとの設定
    public class QuizFileConfig
    {
        public int MinChapter { get; set; } = 1;
        public int MaxChapter { get; set; } = 1;
        public int QuizNum { get; set; } = 5;
        public bool Capital { get; set; } = false;
        public bool ComaPunto { get; set; } = false;
        public int ErrorAllow { get; set; } = 0;
        public int MaxQuizNum => (MaxChapter - MinChapter + 1) * 10;
        public bool Validation()
        {
            if (MinChapter > MaxChapter) return false;

            if ((MaxChapter - MinChapter + 1) * 10 < QuizNum) return false;

            return true;
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
        public bool QuizNum { get; set; }
        public bool ChapterTitle { get; set; }
        public bool ChapterExample { get; set; }
        public int QuizFilePathIndex { get; set; } = 0;
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
        public string QuizNum { get; set; }
        public string ChapterTitle { get; set; }
        public string ChapterExample { get; set; }
        public string Supplement { get; set; }
        public List<string> AutoNombre { get; set; }

        public QuizContents(string quiz, string ca, string qn, string ct, string ce, string s, List<string> an)
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
        public string QuizNum { get; set; }
        public string Supplement { get; set; }
        public bool Result { get; set; }

        public QuizResult(string quiz, string ca, string input, string quizNum, string s, bool result = false)
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
