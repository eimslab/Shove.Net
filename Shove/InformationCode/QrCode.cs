using System;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace Shove.InformationCode
{
    /// <summary>
    /// 基于 com.google.zxing 的二维码封装
    /// </summary>
    public class QrCode
    {
        #region Create

        /// <summary>
        /// 生成二维码，保存为图片文件
        /// </summary>
        /// <param name="input">输入的需要编码的信息字符串</param>
        /// <param name="fromat">二维码格式</param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="canvasHeight">画布高度</param>
        /// <param name="outputFileName">保存图片的文件名</param>
        /// <param name="imageFormat">图像格式</param>
        public static void CreateCode(string input, BarcodeFormat fromat, int canvasWidth, int canvasHeight, string outputFileName, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            CreateCode(input, fromat, canvasWidth, canvasHeight, outputFileName, imageFormat, "");
        }

        /// <summary>
        /// 生成二维码，保存为图片文件
        /// </summary>
        /// <param name="input">输入的需要编码的信息字符串</param>
        /// <param name="fromat">二维码格式</param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="canvasHeight">画布高度</param>
        /// <param name="outputFileName">保存图片的文件名</param>
        /// <param name="imageFormat">图像格式</param>
        /// <param name="logoImageFileName">中间嵌入的 Logo 图片</param>
        public static void CreateCode(string input, BarcodeFormat fromat, int canvasWidth, int canvasHeight, string outputFileName, System.Drawing.Imaging.ImageFormat imageFormat, string logoImageFileName)
        {
            Bitmap bmap = CreateCode(input, fromat, canvasWidth, canvasHeight, imageFormat, logoImageFileName);
            bmap.Save(outputFileName, imageFormat);
        }

        /// <summary>
        /// 生成二维码，返回位图
        /// </summary>
        /// <param name="input">输入的需要编码的信息字符串</param>
        /// <param name="fromat">二维码格式</param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="canvasHeight">画布高度</param>
        /// <param name="imageFormat">图像格式</param>
        /// <returns></returns>
        public static Bitmap CreateCode(string input, BarcodeFormat fromat, int canvasWidth, int canvasHeight, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            return CreateCode(input, fromat, canvasWidth, canvasHeight, imageFormat, "");
        }

        /// <summary>
        /// 生成二维码，返回位图
        /// </summary>
        /// <param name="input">输入的需要编码的信息字符串</param>
        /// <param name="fromat">二维码格式</param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="canvasHeight">画布高度</param>
        /// <param name="imageFormat">图像格式</param>
        /// <param name="logoImageFileName">中间嵌入的 Logo 图片</param>
        /// <returns></returns>
        public static Bitmap CreateCode(string input, BarcodeFormat fromat, int canvasWidth, int canvasHeight, System.Drawing.Imaging.ImageFormat imageFormat, string logoImageFileName)
        {
            var barcodeWriter = new BarcodeWriter<Bitmap>
            {
                Format = fromat,
                Options = new EncodingOptions
                {
                    Height = canvasHeight,
                    Width = canvasWidth,
                    Margin = 0
                }
            };

            barcodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            var bmap = barcodeWriter.Write(input);

            if (string.IsNullOrEmpty(logoImageFileName.Trim()) || (!System.IO.File.Exists(logoImageFileName)))
            {
                return bmap;
            }

            // 加图片水印
            System.Drawing.Image logo = System.Drawing.Image.FromFile(logoImageFileName);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmap);

            // 设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            // 设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // 清空画布并以透明背景色填充   
            //g.Clear(System.Drawing.Color.Transparent);

            // 计算 Logo 的范围
            int x = (canvasWidth - logo.Width) / 2;
            int y = (canvasHeight - logo.Height) / 2;

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(logo, new System.Drawing.Rectangle(x, y, logo.Width, logo.Height), 0, 0, logo.Width, logo.Height, System.Drawing.GraphicsUnit.Pixel);

            logo.Dispose();
            g.Dispose();

            return bmap;
        }

        #endregion

        #region Read

        /// <summary>
        /// 从二维码图片读取二维码信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadCode(string fileName)
        {
            Image img = Image.FromFile(fileName);

            return ReadCode(img);
        }

        /// <summary>
        /// 从二维码图片读取二维码信息
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ReadCode(Image img)
        {
            Bitmap bmap = new Bitmap(img);

            return ReadCode(bmap);
        }

        /// <summary>
        /// 从二维码图片读取二维码信息
        /// </summary>
        /// <param name="bmap"></param>
        /// <returns></returns>
        public static string ReadCode(Bitmap bmap)
        {
            if (bmap == null)
            {
                throw new Exception("Invalid code pictures.");
            }

            LuminanceSource source = null;//[shove] new RGBLuminanceSource(bmap, bmap.Width, bmap.Height);
            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
            Result result;

            result = new MultiFormatReader().decode(bitmap);
            // 如果要捕获异常，用 catch (ReaderException re)， re.ToString();

            return result.Text;
        }

        #endregion
    }
}
