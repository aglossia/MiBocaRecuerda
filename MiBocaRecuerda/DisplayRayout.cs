using System.Collections.Generic;
using System.Drawing;

namespace MiBocaRecuerda
{
    /// <summary>
    /// ベースエリア情報クラス
    /// </summary>
    public class BaseAreaInfo
    {
        public int MaxX { set; get; }   // 最大X座標
        public int MinX { set; get; }   // 最小X座標
        public int MaxY { set; get; }   // 最大Y座標
        public int MinY { set; get; }   // 最小Y座標

        // エリア幅
        public int Width => MaxX - MinX;

        // エリア高さ
        public int Height => MaxY - MinY;

        public BaseAreaInfo()
        {
            MaxX = 0;
            MinX = 0;
            MaxY = 0;
            MinY = 0;
        }

        /// <summary>
        /// 指定ディスプレイからスクリーン全体の最大座標を取得
        /// </summary>
        /// <param name="dispInfoList"></param>
        public BaseAreaInfo(List<DisplayInfo> dispInfoList)
        {
            // 指定ディスプレイからベースエリアを計算する
            foreach (DisplayInfo dispInfo in dispInfoList)
            {
                if (MaxX < dispInfo.X + dispInfo.Width)
                {
                    MaxX = dispInfo.X + dispInfo.Width;
                }

                if (MaxY < dispInfo.Y + dispInfo.Height)
                {
                    MaxY = dispInfo.Y + dispInfo.Height;
                }

                if (MinX > dispInfo.X)
                {
                    MinX = dispInfo.X;
                }

                if (MinY > dispInfo.Y)
                {
                    MinY = dispInfo.Y;
                }
            }
        }
    }

    /// <summary>
    /// 画面情報クラス
    /// </summary>
    public class DisplayInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Size Size { get; set; }

        private readonly int margin = 0;

        public int Width => Size.Width;

        public int Height => Size.Height;

        public DisplayInfo(Rectangle bounds)
        {
            X = bounds.X;
            Y = bounds.Y;
            Size = bounds.Size;
        }

        public void Scaling(float ratioX, float ratioY)
        {
            X = (int)((X - margin) * ratioX);
            Y = (int)((Y - margin) * ratioY);
            Size = new Size((int)((Size.Width - margin) * ratioX), (int)((Size.Height - margin) * ratioY));
        }
    }
}
