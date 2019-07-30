namespace Shove._Web.Snapshot
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public class COMRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public COMRECT()
            {
            }

            public COMRECT(Rectangle r)
            {
                this.left = r.X;
                this.top = r.Y;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public COMRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static NativeMethods.COMRECT FromXYWH(int x, int y, int width, int height)
            {
                return new NativeMethods.COMRECT(x, y, x + width, y + height);
            }

            public override string ToString()
            {
                return string.Concat(new object[] { "Left = ", this.left, " Top ", this.top, " Right = ", this.right, " Bottom = ", this.bottom });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagDVTARGETDEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int tdSize;
            [MarshalAs(UnmanagedType.U2)]
            public short tdDriverNameOffset;
            [MarshalAs(UnmanagedType.U2)]
            public short tdDeviceNameOffset;
            [MarshalAs(UnmanagedType.U2)]
            public short tdPortNameOffset;
            [MarshalAs(UnmanagedType.U2)]
            public short tdExtDevmodeOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagLOGPALETTE
        {
            [MarshalAs(UnmanagedType.U2)]
            public short palVersion;
            [MarshalAs(UnmanagedType.U2)]
            public short palNumEntries;
        }
    }
}

