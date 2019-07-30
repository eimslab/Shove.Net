namespace Shove._Web.Snapshot
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Threading;

    /// <summary>
    /// WebPage 快照
    /// </summary>
    internal class _WebPageSnapshot : IDisposable
    {
        private int height = 0x300;
        private string url = "about:blank";
        private WebBrowser wb = new WebBrowser();
        private int width = 0x400;

        /// <summary>
        /// 构造
        /// </summary>
        public void Dispose()
        {
            this.wb.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitComobject()
        {
            try
            {
                this.wb.ScriptErrorsSuppressed = false;
                this.wb.ScrollBarsEnabled = false;
                this.wb.Size = new Size(Width, Height);
                this.wb.Navigate(this.url);
                while (this.wb.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                this.wb.Stop();
                if (this.wb.ActiveXInstance == null)
                {
                    throw new Exception("实例不能为空");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Bitmap TakeSnapshot()
        {
            Bitmap bitmap;
            try
            {
                this.InitComobject();
                bitmap = new Snapshot().TakeSnapshot(this.wb.ActiveXInstance, new Rectangle(0, 0, this.width, this.height));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw exception;
            }
            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
    }

    /// <summary>
    /// WebPage 快照
    /// </summary>
    public class WebPageSnapshot
    {
        string Url;
        int Width;
        int Height;
        string TargetFileFullName;
        System.Drawing.Imaging.ImageFormat imageFormat = System.Drawing.Imaging.ImageFormat.Png;

        System.Threading.Thread thread;

        string _ErrorDescription = "";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public WebPageSnapshot(string Url, int Width, int Height)
        {
            this.Url = Url;
            this.Width = Width;
            this.Height = Height;
        }
        
        /// <summary>
        /// 捕捉图像
        /// </summary>
        /// <param name="TargetFileFullName">带路径的完整目标文件名</param>
        /// <param name="ErrorDescription">如果方法返回 false, 则此变量包含错误描述</param>
        /// <returns></returns>
        public bool Capture(string TargetFileFullName, ref string ErrorDescription)
        {
            this.TargetFileFullName = TargetFileFullName;

            thread = new System.Threading.Thread(new System.Threading.ThreadStart(Do));
            thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;

            thread.Start();
            while (thread.ThreadState == ThreadState.Running) ;

            if (String.IsNullOrEmpty(_ErrorDescription))
            {
                return true;
            }
            else
            {
                ErrorDescription = _ErrorDescription;

                return false;
            }
        }

        /// <summary>
        /// 捕捉图像
        /// </summary>
        /// <param name="TargetFileFullName">带路径的完整目标文件名</param>
        /// <param name="imageFormat">要保存的图片的文件的图形格式，不指定此参数，则保存为 png 格式</param>
        /// <param name="ErrorDescription">如果方法返回 false, 则此变量包含错误描述</param>
        /// <returns></returns>
        public bool Capture(string TargetFileFullName, System.Drawing.Imaging.ImageFormat imageFormat, ref string ErrorDescription)
        {
            this.TargetFileFullName = TargetFileFullName;
            this.imageFormat = imageFormat;

            thread = new System.Threading.Thread(new System.Threading.ThreadStart(Do));
            thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;

            thread.Start();
            while (thread.ThreadState == ThreadState.Running) ;

            if (String.IsNullOrEmpty(_ErrorDescription))
            {
                return true;
            }
            else
            {
                ErrorDescription = _ErrorDescription;

                return false;
            }
        }

        void Do()
        {
            _WebPageSnapshot wps = new _WebPageSnapshot();
            wps.Height = Height;
            wps.Width = Width;
            wps.Url = Url;

            try
            {
                wps.TakeSnapshot().Save(TargetFileFullName, imageFormat);
            }
            catch (Exception e)
            {
                _ErrorDescription = e.Message;
            }

            wps.Dispose();
        }
    }
}

