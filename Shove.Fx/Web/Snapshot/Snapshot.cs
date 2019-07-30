namespace Shove._Web.Snapshot
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Web 快照
    /// </summary>
    internal class Snapshot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pUnknown"></param>
        /// <param name="bmpRect"></param>
        /// <returns></returns>
        public Bitmap TakeSnapshot(object pUnknown, Rectangle bmpRect)
        {
            if (pUnknown == null)
            {
                return null;
            }
            if (!Marshal.IsComObject(pUnknown))
            {
                return null;
            }
            IntPtr zero = IntPtr.Zero;
            Bitmap image = new Bitmap(bmpRect.Width, bmpRect.Height);
            Graphics graphics = Graphics.FromImage(image);
            Marshal.QueryInterface(Marshal.GetIUnknownForObject(pUnknown), ref UnsafeNativeMethods.IID_IViewObject, out zero);
            try
            {
                (Marshal.GetTypedObjectForIUnknown(zero, typeof(UnsafeNativeMethods.IViewObject)) as UnsafeNativeMethods.IViewObject).Draw(1, -1, IntPtr.Zero, null, IntPtr.Zero, graphics.GetHdc(), new NativeMethods.COMRECT(bmpRect), null, IntPtr.Zero, 0);
                Marshal.Release(zero);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            graphics.Dispose();
            return image;
        }
    }
}

