using System.Collections.Generic;
using System.Drawing;

namespace MiBocaRecuerda
{
    public static class AppRom
    {
        public enum ProgressState
        {
            Neutral,
            Correct,
            Incorrect,
            CurrentQuiz,
        }

        public enum DBColmun
        {
            Header = 1,
            Quiestion,
            Answer,
            ChapterTitle,
            ChapterExample,
            Supplement,
            Date,
            Auxiliary
        }

        public static Dictionary<string, int> LenguaIndex = new Dictionary<string, int> { { "es", 0 }, { "en", 1 } };
        public static Dictionary<string, string> EnglishToCode = new Dictionary<string, string> { { "Spanish", "es" }, { "English", "en" } };

        public static readonly string ProgressStateCharacter_Neutral = "○";
        public static readonly string ProgressStateCharacter_Correct = "■";
        public static readonly string ProgressStateCharacter_Incorrect = "×";
        public static readonly string ProgressStateCharacter_CurrentQuiz = "★";

        public static readonly Color ColorNeutral = Color.LightBlue;
        public static readonly Color ColorHover = Color.OrangeRed;
        public static readonly Color ColorCurrentGroup = Color.Turquoise;
        public static readonly Color ColorOnProgress = Color.Red;
        public static readonly Color ColorOffProgress = Color.Black;
    }
}
