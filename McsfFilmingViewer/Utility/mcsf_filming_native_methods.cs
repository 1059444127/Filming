//////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UIH.Mcsf.Filming.CardFE")]

namespace UIH.Mcsf.Filming.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point { public Int32 X; public Int32 Y; };

    internal class NativeMethods
    {
        internal static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        /// \brief  Copy memory from pointer to pointer
        /// 
        /// \param[in] pDest destination pointer
        /// \param[in] pSrc  source pointer
        /// \param[in] length length of buffer
        /// 
        /// \return void.
        [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
        public static extern unsafe void CopyMemory(
            IntPtr pDest,
            IntPtr pSrc,
            int length
            );

        [DllImport("msvcrt.dll", EntryPoint = "memset",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MemSet(IntPtr dest, int c, uint count);

        // Image Transform by intel ipp
        [DllImport("McsfViewerAlgoImageTransform.dll",
            EntryPoint = "ImageTransformEx",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void ImageTransformByIpp(int iRenderWidth, int iRenderHeight,
            int iViewportX, int iViewportY, int iViewportWidth, int iViewportHeight,
            IntPtr pOriginalPixelData, IntPtr pOriginalMaskData, IntPtr pRGBPixelData,
            int iPixelWidth, int iPixelHeight, int iSamplesPerPixel,
            int iBitsAllocated, int iPixelRepresentation,
            double dRescaleSlope, double dRescaleIntercept,
            double dAngleAntiClockwise, double dOffsetX, double dOffsetY, double dScaleX, double dScaleY,
            double dWindowCenter, double dWindowWidth, double dEnhancePara,
            byte[] pLUT, bool IsInversed, int iInterpMode, bool bIsFillNoneImage);

        // Image Transform by bilinear interpolation
        [DllImport("McsfViewerAlgoImageTransform.dll",
            EntryPoint = "ImageTransform",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern unsafe void ImageTransformOriginal(int iRenderWidth, int iRenderHeight,
            int iViewportX, int iViewportY, int iViewportWidth, int iViewportHeight,
            IntPtr pOriginalPixelData, IntPtr pOriginalMaskData, IntPtr pRGBPixelData,
            int iPixelWidth, int iPixelHeight, int iSamplesPerPixel,
            int iBitsAllocated, int iPixelRepresentation,
            double dRescaleSlope, double dRescaleIntercept,
            double dAngleAntiClockwise, double dOffsetX, double dOffsetY, double dScaleX, double dScaleY,
            double dWindowCenter, double dWindowWidth, double dEnhancePara,
            byte[] pLUT, bool IsInversed, int iInterpMode, bool bIsFillNoneImage);

        public static void ImageTransform(int iRenderWidth, int iRenderHeight,
            int iViewportX, int iViewportY, int iViewportWidth, int iViewportHeight,
            IntPtr pOriginalPixelData, IntPtr pOriginalMaskData, IntPtr pRGBPixelData,
            int iPixelWidth, int iPixelHeight, int iSamplesPerPixel,
            int iBitsAllocated, int iPixelRepresentation,
            double dRescaleSlope, double dRescaleIntercept,
            double dAngleAntiClockwise, double dOffsetX, double dOffsetY, double dScaleX, double dScaleY,
            double dWindowCenter, double dWindowWidth, double dEnhancePara,
            byte[] pLUT, bool IsInversed, int iInterpMode, bool bIsFillNoneImage)
        {
            // \NOTE: It seems the ipp algorithm is still slow for displaying images in a small cell.
            if ((iPixelWidth <= 2048 && iPixelHeight <= 2048 /*&& iRenderWidth >= 256 && iRenderHeight >= 256*/) || bIsFillNoneImage)
            {
                ImageTransformByIpp(iRenderWidth, iRenderHeight, iViewportX, iViewportY,
                    iViewportWidth, iViewportHeight, pOriginalPixelData, pOriginalMaskData, pRGBPixelData,
                    iPixelWidth, iPixelHeight, iSamplesPerPixel, iBitsAllocated, iPixelRepresentation,
                    dRescaleSlope, dRescaleIntercept, dAngleAntiClockwise, dOffsetX, dOffsetY,
                    dScaleX, dScaleY, dWindowCenter, dWindowWidth, dEnhancePara, pLUT, IsInversed, iInterpMode, bIsFillNoneImage);
            }
            else
            {
                ImageTransformOriginal(iRenderWidth, iRenderHeight, iViewportX, iViewportY,
                    iViewportWidth, iViewportHeight, pOriginalPixelData, pOriginalMaskData, pRGBPixelData,
                    iPixelWidth, iPixelHeight, iSamplesPerPixel, iBitsAllocated, iPixelRepresentation,
                    dRescaleSlope, dRescaleIntercept, dAngleAntiClockwise, dOffsetX, dOffsetY,
                    dScaleX, dScaleY, dWindowCenter, dWindowWidth, dEnhancePara, pLUT, IsInversed, iInterpMode, false);
            }
            //stopwatch.Stop();
            //Console.WriteLine("Total time outside PInvoke for image transform is " + stopwatch.ElapsedMilliseconds);
        }

    }
}