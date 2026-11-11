using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace CosmoTweest {
// Present the opaque virtual framebuffer through Windows' fast DIB scaler.
// GDI+ alpha-aware image scaling was the measured bottleneck on older CPUs.
static class Presentation {
 [StructLayout(LayoutKind.Sequential)] struct Info {
  public uint size; public int width,height; public ushort planes,bits;
  public uint compression,imageSize; public int xppm,yppm;public uint used,important;
 }
 [DllImport("gdi32.dll")]static extern int StretchDIBits(IntPtr dc,int x,int y,int w,int h,int sx,int sy,int sw,int sh,IntPtr pixels,ref Info info,uint usage,uint operation);
 [DllImport("gdi32.dll")]static extern int SetStretchBltMode(IntPtr dc,int mode);
 public static void Draw(Graphics target,Bitmap frame,Rectangle destination){
  BitmapData bits=frame.LockBits(new Rectangle(0,0,frame.Width,frame.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppPArgb);
  IntPtr dc=IntPtr.Zero;
  try{dc=target.GetHdc();int old=SetStretchBltMode(dc,3);Info info=new Info{size=40,width=frame.Width,height=-frame.Height,planes=1,bits=32};StretchDIBits(dc,destination.X,destination.Y,destination.Width,destination.Height,0,0,frame.Width,frame.Height,bits.Scan0,ref info,0,0x00CC0020);SetStretchBltMode(dc,old);}
  finally{if(dc!=IntPtr.Zero)target.ReleaseHdc(dc);frame.UnlockBits(bits);}
 }
}
}
