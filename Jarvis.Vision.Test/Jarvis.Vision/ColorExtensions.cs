using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Vision
{
    public static class ColorExtensions
    {
        public static Color Blend(this Color color, Color other, float percentage)
        {
            var a = BlendChannel(color.A, other.A, percentage);
            var r = BlendChannel(color.R, other.R, percentage);
            var g = BlendChannel(color.G ,other.G ,percentage);
            var b = BlendChannel(color.B ,other.B ,percentage);
            return Color.FromArgb(a, r, g, b);
        }

        private static byte BlendChannel(byte channel1, byte channel2, float percentage)
        {
            return (byte)(channel1.Percentage(1-percentage) + channel2.Percentage(percentage)).Max(255);
        }

        private static byte Percentage(this byte i, float percentage) => (byte)(i * percentage);

        public static Bitmap MedianFilter(this Bitmap sourceBitmap,
                                         int matrixSize)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;

            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    neighbourPixels.Clear();

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            neighbourPixels.Add(BitConverter.ToInt32(
                                             pixelBuffer, calcOffset));
                        }
                    }

                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                                       neighbourPixels[filterOffset]);

                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);

            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }



        public static Bitmap ConvolutionFilter(this Bitmap sourceBitmap,
                                        double[,] xFilterMatrix,
                                        double[,] yFilterMatrix,
                                              double factor = 1,
                                                   int bias = 0,
                                         bool grayscale = false)
        {
            BitmapData sourceData =
                           sourceBitmap.LockBits(new Rectangle(0, 0,
                           sourceBitmap.Width, sourceBitmap.Height),
                                             ImageLockMode.ReadOnly,
                                        PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];


            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            if (grayscale == true)
            {
                float rgb = 0;


                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }


            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;


            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;


            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;


            int filterOffset = 1;
            int calcOffset = 0;


            int byteOffset = 0;


            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;


                    blueTotal = greenTotal = redTotal = 0.0;


                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);


                            blueX += (double)
                                      (pixelBuffer[calcOffset]) *
                                      xFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];


                            greenX += (double)
                                  (pixelBuffer[calcOffset + 1]) *
                                      xFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];


                            redX += (double)
                                  (pixelBuffer[calcOffset + 2]) *
                                      xFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];


                            blueY += (double)
                                      (pixelBuffer[calcOffset]) *
                                      yFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];


                            greenY += (double)
                                  (pixelBuffer[calcOffset + 1]) *
                                      yFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];


                            redY += (double)
                                  (pixelBuffer[calcOffset + 2]) *
                                      yFilterMatrix[filterY +
                                                    filterOffset,
                                                    filterX +
                                                    filterOffset];
                        }
                    }


                    blueTotal = Math.Sqrt((blueX * blueX) +
                                          (blueY * blueY));


                    greenTotal = Math.Sqrt((greenX * greenX) +
                                           (greenY * greenY));


                    redTotal = Math.Sqrt((redX * redX) +
                                         (redY * redY));


                    if (blueTotal > 255)
                    { blueTotal = 255; }
                    else if (blueTotal < 0)
                    { blueTotal = 0; }


                    if (greenTotal > 255)
                    { greenTotal = 255; }
                    else if (greenTotal < 0)
                    { greenTotal = 0; }


                    if (redTotal > 255)
                    { redTotal = 255; }
                    else if (redTotal < 0)
                    { redTotal = 0; }


                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);


            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);


            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }
    }
}
