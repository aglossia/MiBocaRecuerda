using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MiBocaRecuerda
{
    static class ParseXML
    {
        static public string ConvertTextWithTable(string inputText)
        {
            StringBuilder result = new StringBuilder();
            int currentIndex = 0;

            // `<table>` タグを検索
            while (true)
            {
                int tableStart = inputText.IndexOf("<table>", currentIndex, StringComparison.OrdinalIgnoreCase);
                if (tableStart == -1)
                {
                    // 残りのテキストを追加
                    result.Append(inputText.Substring(currentIndex));
                    break;
                }

                // `<table>` タグの前の部分を追加
                result.Append(inputText.Substring(currentIndex, tableStart - currentIndex));

                // `<table>` タグの終了を検索
                int tableEnd = inputText.IndexOf("</table>", tableStart, StringComparison.OrdinalIgnoreCase);
                if (tableEnd == -1)
                {
                    throw new Exception("Invalid input: missing </table> tag.");
                }

                // `<table>` 全体を抽出
                string tableXml = inputText.Substring(tableStart, tableEnd - tableStart + 8);

                // テーブルをテキスト形式に変換して追加
                result.Append(ConvertXmlToTextTable(tableXml));

                // 次の部分に進む
                currentIndex = tableEnd + 8;
            }

            return result.ToString();
        }

        static private string ConvertXmlToTextTable(string xmlInput)
        {
            // XMLのパース
            XDocument xmlDoc = XDocument.Parse(xmlInput);

            // ヘッダー取得
            var headers = xmlDoc.Descendants("thead")
                                .Descendants("tr")
                                .FirstOrDefault()?
                                .Elements()
                                .Select(e => e.Value)
                                .ToList() ?? new List<string>();

            // 行データ取得
            var rows = xmlDoc.Descendants("tbody")
                             .Descendants("tr")
                             .Select(tr => tr.Elements().Select(td => td.Value).ToList())
                             .ToList();

            // フッター取得
            var footer = xmlDoc.Descendants("tfoot")
                               .Descendants("tr")
                               .Select(tr => tr.Elements().Select(td => td.Value).ToList())
                               .FirstOrDefault();

            if (footer != null)
            {
                rows.Add(footer);
            }

            // 全ての列のデータを収集して、列幅を計算
            var allRows = new List<List<string>> { headers }.Concat(rows).ToList();
            var columnWidths = headers.Count > 0
                ? Enumerable.Range(0, headers.Count)
                            .Select(i => allRows.Max(row => row.Count > i ? GetStringDisplayWidth(row[i]) : 0))
                            .ToList()
                // headerがないときは行の最大要素数に合わせる
                : Enumerable.Range(0, rows.Select(l => l.Count()).Max())
                            .Select(i => allRows.Max(row => row.Count > i ? GetStringDisplayWidth(row[i]) : 0))
                            .ToList();

            // 表の作成
            var result = new StringBuilder();
            if (headers.Count > 0)
            {
                result.AppendLine(CreateRow(headers, columnWidths));
                result.AppendLine(new string('-', columnWidths.Sum() + (headers.Count - 1) * 3)); // 区切り線
            }

            foreach (var row in rows)
            {
                result.AppendLine(CreateRow(row, columnWidths));
            }

            // 最後にも\r\nがつくからそれを削除
            return result.ToString().Substring(0, result.Length - 2);
        }

        static private string CreateRow(List<string> row, List<int> columnWidths)
        {
            var formattedRow = row.Select((cell, index) =>
            {
                int width = columnWidths[index];
                return cell.PadRight(width - GetStringDisplayWidth(cell) + cell.Length);
            });
            return string.Join(" | ", formattedRow);
        }

        static private int GetStringDisplayWidth(string input)
        {
            // 全角文字は2、半角文字は1とみなす
            return input.Sum(c => IsFullWidth(c) ? 2 : 1);
        }

        static private bool IsFullWidth(char c)
        {
            // Unicodeの全角文字の範囲を判定
            return (c >= 0x1100 && c <= 0x115F) || // Hangul Jamo
                   (c >= 0x2E80 && c <= 0xA4CF && c != 0x303F) || // CJK Radicals Supplement and Kangxi Radicals
                   (c >= 0xAC00 && c <= 0xD7A3) || // Hangul Syllables
                   (c >= 0xF900 && c <= 0xFAFF) || // CJK Compatibility Ideographs
                   (c >= 0xFE10 && c <= 0xFE19) || // Vertical forms
                   (c >= 0xFE30 && c <= 0xFE6F) || // CJK Compatibility Forms
                   (c >= 0xFF00 && c <= 0xFF60) || // Fullwidth Forms
                   (c >= 0xFFE0 && c <= 0xFFE6);   // Fullwidth Symbol Forms
        }
    }
}
