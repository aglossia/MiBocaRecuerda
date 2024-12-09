namespace MiBocaRecuerda
{
    public static class Translate
    {
        public static string DoTransrate(string text, string langType)
        {
            int src_langIndex = 0;
            int dst_langIndex = 0;

            switch (langType)
            {
                case "es":
                    src_langIndex = 2;
                    break;
                case "en":
                    src_langIndex = 1;
                    break;
                case "jp":
                    src_langIndex = 0;
                    break;
            }

            string arguments = $"trans.py \"{text}\" {src_langIndex} {dst_langIndex}";

            IScriptProcess python = ScriptMain.DoScript(ScriptType.Python, arguments);

            ScriptResult result = python.GetResult();

            string[] res = result.Output.Split('¶');

            if (res.Length >= 2)
            {
                return res[1];
            }
            else
            {
                return "";
            }
        }
    }
}
