using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MiBocaRecuerda
{
    static class CommonFunction
    {
        /// <summary>
        /// XML読み込み
        /// </summary>
        /// <typeparam name="DeserializeObject">デシリアライズするオブジェクト型</typeparam>
        /// <param name="filePath">XMLファイルパス</param>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public static DeserializeObject XmlRead<DeserializeObject>(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DeserializeObject));

                using (StreamReader sr = new StreamReader(filePath, new UTF8Encoding(false)))
                {
                    return (DeserializeObject)serializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// XML書き込み
        /// </summary>
        /// <typeparam name="SerializeObject">シリアライズするオブジェクト型</typeparam>
        /// <param name="serializeObject">シリアライズするオブジェクト</param>
        /// <param name="filePath">XMLファイルパス</param>
        public static void XmlWrite<SerializeObject>(SerializeObject serializeObject, string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializeObject));

                using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, serializeObject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static float LevenshteinRate(string str1, string str2)
        {
            int len1 = (str1 != null) ? str1.Length : 0;
            int len2 = (str2 != null) ? str2.Length : 0;

            if (len1 > len2)
            {
                int tmp = len1;
                len1 = len2;
                len2 = tmp;
            }

            if (len1 == 0)
            {
                return (len2 == 0) ? 0.0f : 1.0f;
            }

            return LevenshteinDistance(str1, str2) / (float)len2;
        }

        private static int LevenshteinDistance(string str1, string str2)
        {
            int n1 = 0;
            int n2 = str2.Length + 2;
            int[] d = new int[n2 << 1];

            for (int i = 0; i < n2; i++)
            {
                d[i] = i;
            }

            d[n2 - 1]++;
            d[d.Length - 1] = 0;

            for (int i = 0; i < str1.Length; i++)
            {
                d[n2] = i + 1;

                for (int j = 0; j < str2.Length; j++)
                {
                    int v = d[n1++];

                    if (str1[i] == str2[j])
                    {
                        v--;
                    }

                    v = (v < d[n1]) ? v : d[n1];
                    v = (v < d[n2]) ? v : d[n2];

                    d[++n2] = ++v;
                }

                n1 = d[n1 + 1];
                n2 = d[n2 + 1];
            }

            return d[d.Length - n2 - 2];
        }

        public static int GetMaxStringWidth(string text, Font font)
        {
            // フォントとBitmapを準備
            using (Bitmap bmp = new Bitmap(1, 1)) // 仮のBitmap
            using (Graphics g = Graphics.FromImage(bmp))
            {
                int maxWidth = 0;

                // 改行で文字列を分割して各行を測定
                string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    SizeF size = g.MeasureString(line, font);
                    if (size.Width > maxWidth)
                    {
                        maxWidth = (int)size.Width;
                    }
                }

                return maxWidth;
            }
        }
    }
}
