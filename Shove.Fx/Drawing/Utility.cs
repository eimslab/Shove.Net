using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Shove.Drawing.PNGConvertCore;

namespace Shove.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="SourceImgFileName">源图片文件</param>
        /// <param name="TargetImgFileName">目标图片文件，可以同名</param>
        /// <param name="Scale">比例，1 表示 100%，如：0.2 表示 20%</param>
        public static void Thumbnail(string SourceImgFileName, string TargetImgFileName, double Scale)
        {
            Bitmap oldImage = new Bitmap(SourceImgFileName);
            double New_Width = oldImage.Width * Scale;
            double New_Height = oldImage.Height * Scale;
            Bitmap newImage = new Bitmap((int)New_Width, (int)New_Height);

            Graphics g = Graphics.FromImage(newImage);
            g.DrawImage(oldImage, new Rectangle(0, 0, (int)New_Width, (int)New_Height), new Rectangle(0, 0, oldImage.Width, oldImage.Height), GraphicsUnit.Pixel);
            oldImage.Dispose();
            newImage.Save(TargetImgFileName);
            g.Dispose();
            newImage.Dispose();
        }

        /// <summary>
        /// 将图片文件转换为 JPEG 格式
        /// </summary>
        /// <param name="sourceImageUrl"></param>
        /// <param name="targetImageUrl"></param>
        /// <param name="quality">质量，从 1-100，100 为最好</param>
        public static void ConvertImageToJPEG(string sourceImageUrl, string targetImageUrl, int quality)
        {
            if ((quality < 1) || (quality > 100))
            {
                throw new Exception("Quality 的值超出范围，有效值为 1 到 100 之间。100 为质量最好。");
            }

            Bitmap bitmap = new Bitmap(sourceImageUrl);
            ImageCodecInfo jpgEncoder = null;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpgEncoder = codec;

                    break;
                }
            }

            if (jpgEncoder == null)
            {
                throw new Exception("打开 JPEG 编码失败。");
            }

            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters encoderParameters = new EncoderParameters(1);

            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;

            FileStream ms = new FileStream(targetImageUrl, FileMode.Create, FileAccess.Write);
            bitmap.Save(ms, jpgEncoder, encoderParameters);

            ms.Flush();
            ms.Close();
        }

        /// <summary>
        /// 转换 PNG 格式的图片为 8 bit 格式的 PNG 图片，比改变图片的尺寸，但大量减少文件大小。质量有损失
        /// </summary>
        /// <param name="sourceImageUrl"></param>
        /// <param name="targetImageUrl"></param>
        public static void ConvertPngTo8Bit(string sourceImageUrl, string targetImageUrl)
        {
            var quantizer = new WuQuantizer();
            using (var bitmap = new Bitmap(sourceImageUrl))
            {
                using (var quantized = quantizer.QuantizeImage(bitmap))
                {
                    quantized.Save(targetImageUrl, ImageFormat.Png);
                }
            }
        }
    }
}
