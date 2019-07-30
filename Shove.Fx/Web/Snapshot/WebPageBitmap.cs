namespace Shove._Web.Snapshot
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    internal class WebPageBitmap
    {
        private int Height;
        private WebBrowser MyBrowser;
        private string URL;
        private int Width;

        public WebPageBitmap(string url, int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.URL = url;
            this.MyBrowser = new WebBrowser();
            this.MyBrowser.ScrollBarsEnabled = false;
            this.MyBrowser.Size = new Size(this.Width, this.Height);
        }

        public Bitmap DrawBitmap(int theight, int twidth)
        {
            Bitmap bitmap2;
            Bitmap bitmap = new Bitmap(this.Width, this.Height);
            Rectangle targetBounds = new Rectangle(0, 0, this.Width, this.Height);
            this.MyBrowser.DrawToBitmap(bitmap, targetBounds);
            Image image = bitmap;
            Image image2 = new Bitmap(twidth, theight, image.PixelFormat);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            Rectangle rect = new Rectangle(0, 0, twidth, theight);
            graphics.DrawImage(image, rect);
            try
            {
                bitmap2 = (Bitmap) image2;
            }
            catch (Exception)
            {
                bitmap2 = null;
            }
            finally
            {
                image.Dispose();
                image = null;
                this.MyBrowser.Dispose();
                this.MyBrowser = null;
            }
            return bitmap2;
        }

        public void GetIt()
        {
            this.MyBrowser.Visible = true;
            this.MyBrowser.Navigate(this.URL);
            while (this.MyBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
    }
}

