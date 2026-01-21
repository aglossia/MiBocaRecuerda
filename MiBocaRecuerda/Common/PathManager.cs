namespace MiBocaRecuerda
{
    static class PathManager
    {
        public static string QuizDB => $"{SettingManager.RomConfig.ResourcePath}\\db";

        public static string QuizFileSettingCommon(string fileName)
        {
            return $"{SettingManager.RomConfig.ResourcePath}\\cache\\quiz\\common\\{fileName}_common.xml";
        }

        public static string QuizFileSettingLang(string fileName)
        {
            return $"{SettingManager.RomConfig.ResourcePath}\\cache\\quiz\\lang\\{fileName}_lang.xml";
        }

        public static string SettingLanguage(string langName)
        {
            return $"{SettingManager.RomConfig.ResourcePath}\\cache\\language\\{langName}.xml";
        }
    }
}
