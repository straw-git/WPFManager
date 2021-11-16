
using BarcodeLib;
using System.Drawing;
using System.Windows.Controls;

namespace Common.Utils
{
    public class QRCodeCommon
    {
        public static System.Drawing.Image CreateBarCode(string content)
        {
            using (var barcode = new Barcode()
            {
                //true显示content，false反之
                IncludeLabel = true,

                //content的位置
                Alignment = AlignmentPositions.CENTER,

                //条形码的宽高
                Width = 200,
                Height = 60,

                //类型
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,

                //颜色
                BackColor = Color.White,
                ForeColor = Color.Black,
            })
            {
                return barcode.Encode(TYPE.CODE128B, content);
            }
        }
    }
}
