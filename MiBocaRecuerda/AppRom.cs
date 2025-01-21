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
