using ClosedXML.Excel;
using System;
using System.Collections.Generic;

namespace MiBocaRecuerda
{
    static class UtilityFunction
    {

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

        public static int Suelo(int n)
        {
            int t = (int)Math.Floor((decimal)n / 10);

            return t;
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
    }
}
