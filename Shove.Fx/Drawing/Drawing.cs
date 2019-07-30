using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Shove
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("类即将停用，此类下的方法已经移动到 Shove.Drawing.Utility 类。")]
    public class _Drawing
    {
        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="SourceImgFileName">源图片文件</param>
        /// <param name="TargetImgFileName">目标图片文件，可以同名</param>
        /// <param name="Scale">比例，1 表示 100%，如：0.2 表示 20%</param>
        [Obsolete("方法已经否决，请使用 Shove.Drawing.Utility.Thumbnail 方法。")]
        public static void Thumbnail(string SourceImgFileName, string TargetImgFileName, double Scale)
        {
            Shove.Drawing.Utility.Thumbnail(SourceImgFileName, TargetImgFileName, Scale);
        }

        /// <summary>
        /// 将图片文件转换为 JPEG 格式
        /// </summary>
        /// <param name="sourceImageUrl"></param>
        /// <param name="targetImageUrl"></param>
        /// <param name="quality">质量，从 1-100，100 为最好</param>
        [Obsolete("方法已经否决，请使用 Shove.Drawing.Utility.ConvertImageToJPEG 方法。")]
        public static void ConvertImageToJPEG(string sourceImageUrl, string targetImageUrl, int quality)
        {
            Shove.Drawing.Utility.ConvertImageToJPEG(sourceImageUrl, targetImageUrl, quality);
        }
    }
}