using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MiBocaRecuerda
{
    static class UtilityFunction
    {
        public static string ReplaceConsecutiveSpaces(string input)
        {
            // 連続したスペースを一つのスペースに置き換える
            string pattern = @"\s+";
            string replacement = " ";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        public static string GetContext(string str, int index, int contextLength = 3)
        {
            int start = Math.Max(0, index - contextLength);

            int space_cnt = CountSpaces(str.Substring(start, Math.Abs(start - index)));

            if (space_cnt > 0)
            {
                start = Math.Max(0, index - (contextLength + space_cnt));
            }
            int end = Math.Min(str.Length - 1, index + contextLength);

            space_cnt = CountSpaces(str.Substring(index, end - index));

            if (space_cnt > 0)
            {
                end = Math.Min(str.Length - 1, index + (contextLength + space_cnt));
            }

            return str.Substring(start, end - start + 1);
        }

        public static int CountSpaces(string str)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    count++;
                }
            }
            return count;
        }

        public static int GetLastRowInColumn(IXLWorksheet worksheet, string columnName)
        {
            int lastRow = worksheet.LastRowUsed().RowNumber() + 1;

            // 特定の列の最終行を求める
            foreach (IXLRangeRow row in worksheet.Range($"{columnName}1:{columnName}{lastRow}").Rows())
            {
                //Console.WriteLine(row.Cell(1).Value);
                if (row.Cell(1).IsEmpty())
                {
                    return row.RowNumber() - 1;
                }
            }

            return 0; // 見つからない場合は0を返す
        }

        public static int Suelo(int n, int _base)
        {
            int t = (int)Math.Floor((decimal)n / _base);

            return t;
        }

        public static int Techo(int n, int _base)
        {
            int t = (int)Math.Ceiling((decimal)n / _base);

            return t;
        }

        // FLOOR(n/10^(d-1)) mod 10
        // numのdigit桁目を取得する
        public static int GetNDigit(int num, int digit)
        {
            double denominator = (int)Math.Pow(10, (double)digit - 1);

            // 指定桁にはなにもない
            if (num < denominator) return 0;

            return (int)Math.Floor(num / denominator) % 10;
        }

        // リストをシャッフルするメソッド
        public static List<int> ShuffleList(List<int> list)
        {
            Random rand = new Random();
            List<int> shuffledList = new List<int>(list);

            for (int i = shuffledList.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                int temp = shuffledList[i];
                shuffledList[i] = shuffledList[j];
                shuffledList[j] = temp;
            }

            return shuffledList;
        }

        public static string GenerateXmlTable(List<string> headers, List<string> data)
        {
            if (headers == null || data == null || headers.Count != data.Count)
            {
                throw new ArgumentException("Headers and data must be non-null and have the same number of elements.");
            }

            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.AppendLine("<table>");
            xmlBuilder.AppendLine("    <tbody>");

            for (int i = 0; i < headers.Count; i++)
            {
                xmlBuilder.AppendLine("        <tr>");
                xmlBuilder.AppendLine($"          <th>{System.Security.SecurityElement.Escape(headers[i])}</th>");
                xmlBuilder.AppendLine($"          <td>{System.Security.SecurityElement.Escape(data[i])}</td>");
                xmlBuilder.AppendLine("        </tr>");
            }

            xmlBuilder.AppendLine("    </tbody>");
            xmlBuilder.AppendLine("</table>");

            return xmlBuilder.ToString();
        }

        /// <summary>
        /// ディスプレイ全体を含む最小の領域を取得する
        /// </summary>
        /// <returns></returns>
        public static BaseAreaInfo GetBaseArea()
        {
            List<DisplayInfo> displayInfoList = Screen.AllScreens
                    // スクリーン情報からDisplayInfoリストを作成
                    .Select(sc => new DisplayInfo(sc.Bounds)).ToList();

            return new BaseAreaInfo(displayInfoList);
        }
    }
}
