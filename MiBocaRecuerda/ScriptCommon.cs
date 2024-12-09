using System;
using System.Diagnostics;
using System.Text;

namespace MiBocaRecuerda
{
    /// <summary>
    /// Python終了コード
    /// </summary>
    public enum PythonExitType
    {
        Success,
        ScriptError,
        Argument,
        NotFoundFile,
        JsonOpen,
        GetColor,
    }

    /// <summary>
    /// スクリプトタイプ
    /// </summary>
    public enum ScriptType
    {
        Python,
    }

    /// <summary>
    /// スクリプト実行メイン
    /// </summary>
    public static class ScriptMain
    {
        public static ScriptProcess DoScript(ScriptType st, string arguments)
        {
            switch (st)
            {
                case ScriptType.Python:
                    return new PythonProcess(arguments);
            }

            return null;
        }
    }

    /// <summary>
    /// スクリプトプロセスインターフェイス
    /// </summary>
    interface IScriptProcess
    {
        // スクリプト実行結果取得
        ScriptResult GetResult();
    }

    /// <summary>
    /// スクリプト実行結果
    /// </summary>
    public class ScriptResult
    {
        // 戻り値
        public string Output { get; set; } = null;
        // エラーメッセージ
        public string Error { get; set; } = null;
        // 終了コード
        public int ExitCode { get; set; } = 0;
        //public virtual string ConsoleText { get; set; } = "None";

        public bool Success => ExitCode == 0 ? true : false;

        public ScriptResult() { }

        public ScriptResult(string output, string error, int exitCode)
        {
            Output = output;
            Error = error;
            ExitCode = exitCode;
        }
    }

    /// <summary>
    /// スクリプトプロセス基底クラス
    /// </summary>
    public abstract class ScriptProcess : IScriptProcess
    {
        private string ScriptName = "";
        private string Arguments = "";
        // 生のスクリプト実行結果
        protected ScriptResult scriptResult = new ScriptResult();
        // 以下、生実行結果からスクリプト別に結果を整形するための抽象メソッド
        protected abstract string GetOutput();
        protected abstract string GetError();
        protected abstract int GetExitCode();

        // 整形後実行結果取得
        public ScriptResult GetResult()
        {
            ScriptResult s = new ScriptResult(GetOutput(), GetError(), GetExitCode());

            return new ScriptResult(GetOutput(), GetError(), GetExitCode());
        }

        public ScriptProcess(string script, string arguments)
        {
            ScriptName = script;
            Arguments = arguments;
        }

        // スクリプト実行
        public void Start()
        {
            try
            {
                using (Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo(ScriptName)
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Arguments = Arguments,
                        StandardOutputEncoding = Encoding.UTF8,
                    }
                })
                {
                    process.Start();

                    scriptResult.Error = process.StandardError.ReadToEnd();
                    scriptResult.Output = process.StandardOutput.ReadLine();
                    scriptResult.ExitCode = process.ExitCode;

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                // scriptコマンド自体が認識できないときとか
                scriptResult.Error = string.Format(
                    "script name:{0}\r\narguments:{1}\r\nerror message:{2}",
                    ScriptName, Arguments, ex.Message);
                scriptResult.ExitCode = 1;
            }
        }
    }

    /// <summary>
    /// Pythonプロセス
    /// </summary>
    public class PythonProcess : ScriptProcess
    {
        // Pythonがスクリプトエラーがなく終了したとき
        // 終了コード,メッセージor戻り値
        // の形式で取得される

        protected override string GetOutput()
        {
            // script異常終了で戻り値がnullとなる
            if (scriptResult.Output != null)
            {
                //return scriptResult.Output.Split(',')[1];
                return scriptResult.Output.Substring(2);
            }
            else
            {
                return "";
            }
        }

        protected override string GetError()
        {
            if (scriptResult.ExitCode != 0)
            {
                return "script error:\r\n" + scriptResult.Error;
            }
            else
            {
                // スクリプト正常終了で例外発生の場合、Outputに異常メッセージが入る
                return "script exception:" + GetOutput();
            }
        }

        protected override int GetExitCode()
        {
            if (scriptResult.ExitCode != 0)
            {
                return (int)PythonExitType.ScriptError;
            }
            else
            {
                if (scriptResult.Output != null)
                {
                    return int.Parse(scriptResult.Output.Split('¶')[0]);
                }
                else
                {
                    // OutputがnullのときはExitCodeが0になるのでこのパターンになることはないはず
                    return (int)PythonExitType.ScriptError;
                }
            }
        }

        public PythonProcess(string arguments) : base("python.exe", arguments)
        {
            Start();
        }
    }
}
