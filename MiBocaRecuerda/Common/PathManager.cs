using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBocaRecuerda
{
    static class PathManager
    {
        public static string QuizFileSettingCommon(string fileName)
        {
            return $"{SettingManager.RomConfig.QuizFilePath}\\cache\\quiz\\common\\{fileName}_common.xml";
        }

        public static string QuizFileSettingLang(string fileName)
        {
            return $"{SettingManager.RomConfig.QuizFilePath}\\cache\\quiz\\lang\\{fileName}_lang.xml";
        }

        public static string SettingLanguage(string langName)
        {
            return $"{SettingManager.RomConfig.QuizFilePath}\\cache\\language\\{langName}.xml";
        }
    }
}
