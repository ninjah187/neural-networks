using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace Nintek.NeuralNetworks.Samples.Wpf.CopyImage
{
    public static class BitmapExtensions
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            IntPtr ip = source.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }
        }
    }
}
