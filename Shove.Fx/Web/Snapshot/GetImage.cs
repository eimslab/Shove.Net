namespace Shove._Web.Snapshot
{
    using System;
    using System.Drawing;

    /// <summary>
    /// 根据 Url 获取页面快照
    /// </summary>
    internal class GetImage
    {
        private int F_Height;
        private int F_Width;
        private string MyURL;
        private int S_Height;
        private int S_Width;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="WebSite"></param>
        /// <param name="ScreenWidth"></param>
        /// <param name="ScreenHeight"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        public GetImage(string WebSite, int ScreenWidth, int ScreenHeight, int ImageWidth, int ImageHeight)
        {
            this.WebSite = WebSite;
            this.ScreenWidth = ScreenWidth;
            this.ScreenHeight = ScreenHeight;
            this.ImageHeight = ImageHeight;
            this.ImageWidth = ImageWidth;
        }

        /// <summary>
        /// 获取快照
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            WebPageBitmap bitmap = new WebPageBitmap(this.WebSite, this.ScreenWidth, this.ScreenHeight);
            bitmap.GetIt();
            return bitmap.DrawBitmap(this.ImageHeight, this.ImageWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        public int ImageHeight
        {
            get
            {
                return this.F_Height;
            }
            set
            {
                this.F_Height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ImageWidth
        {
            get
            {
                return this.F_Width;
            }
            set
            {
                this.F_Width = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ScreenHeight
        {
            get
            {
                return this.S_Height;
            }
            set
            {
                this.S_Height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ScreenWidth
        {
            get
            {
                return this.S_Width;
            }
            set
            {
                this.S_Width = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WebSite
        {
            get
            {
                return this.MyURL;
            }
            set
            {
                this.MyURL = value;
            }
        }
    }
}

