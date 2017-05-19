using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxIm;
using nom.tam.fits;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DataKeeper
{
    public class FITSHandler
    {
        private static Image<Gray, UInt16> LastestImageCV = null;

        public static void CreateMaximObject()
        {
        }

        public static Byte[] Convert16To8Bit(UInt16[][] Image16)
        {
            List<Byte> Image8 = new List<Byte>();

            for (int i = 0; i < Image16.Length; i++)
            {
                for (int j = 0; j < Image16.GetLength(0); j++)
                {
                    int value16 = Image16[i][j];
                    value16 = value16 + value16 << 8;
                    Image8.Add((byte)(value16 / 257.0 + 0.5));
                }
            }

            return Image8.ToArray();
        }

        public static Image<Gray, UInt16> ResizeImage(Image<Gray, UInt16> ImgU16Bit, int Width, int Height)
        {
            return ImgU16Bit.Resize(Width, Height, Emgu.CV.CvEnum.Inter.Linear);
        }

        public static Image<Gray, UInt16> StretchImage(Object CVImageData, UInt16 MinVal, UInt16 MaxVal)
        {
            Image<Gray, UInt16> img = (Image<Gray, UInt16>)CVImageData;

            UInt16 Devider = Convert.ToUInt16(Math.Abs(MaxVal - MinVal));
            UInt16 Mulipler = 0;

            if (Devider > 0)
                Mulipler = (UInt16)(65535 / Devider);
            else
                return null;

            Image<Gray, UInt16> NewImg = ((img - MinVal) * Mulipler);
            return NewImg;
        }

        //public static void CalculatePercentile(Image<Gray, UInt16> LastestImageCV, out UInt16 MinIntensity, out UInt16 MaxIntensity)
        //{
        //    try
        //    {
        //        UInt16[] TempSortedImageData = null;
        //        LastestImageCV.DAta

        //        if (SortedImageData != null)
        //        {
        //            TempSortedImageData = new ushort[SortedImageData.Length];
        //            SortedImageData.CopyTo(TempSortedImageData, 0);
        //            Array.Sort(TempSortedImageData);
        //        }
        //        else
        //        {
        //            MaxIntensity = MinIntensity = 0;
        //            return;
        //        }

        //        int MinIndex = 0;
        //        int MaxIndex = 0;

        //        Double MinPercentile = 0, MaxPercentile = 0;
        //        GetPercentileProfile(out MinPercentile, out MaxPercentile);

        //        MinIndex = (int)((TempSortedImageData.Count() * MinPercentile) / 100);
        //        MaxIndex = (int)((TempSortedImageData.Count() * MaxPercentile) / 100);

        //        if (MinIndex > 0)
        //        {
        //            MinIntensity = TempSortedImageData[MinIndex - 1];
        //        }
        //        else
        //        {
        //            MinIntensity = TempSortedImageData[0];
        //        }

        //        if (MaxIndex > 0)
        //        {
        //            MaxIntensity = TempSortedImageData[MaxIndex - 1];
        //        }
        //        else
        //        {
        //            MaxIntensity = TempSortedImageData[0];
        //        }
        //    }
        //    catch
        //    {
        //        MinIntensity = 0;
        //        MaxIntensity = 0;
        //    }
        //}

        public static void GetPercentileProfile(out Double MinPercentile, out Double MaxPercentile)
        {
            switch (Properties.Settings.Default.StretchProfile)
            {
                case 0: //low
                    MaxPercentile = 99.71;
                    MinPercentile = 5.6;
                    break;

                case 1: //medium
                    MaxPercentile = 99.54;
                    MinPercentile = 26.74;
                    break;

                case 2: //hight
                    MaxPercentile = 99.25;
                    MinPercentile = 50.00;
                    break;

                case 3: //moon
                    MaxPercentile = 99.87;
                    MinPercentile = 95.04;
                    break;

                case 4://planet
                    MaxPercentile = 99.92;
                    MinPercentile = 99.16;
                    break;

                case 5: // Max Value
                    MaxPercentile = 100.00;
                    MinPercentile = 0;
                    break;

                default:
                    MaxPercentile = 99.54;
                    MinPercentile = 26.74;
                    break;
            }
        }

        public static void SaveImage(Image<Gray, UInt16> ImgU16Bit, String ImageName)
        {
            ImgU16Bit.Save(@"C:\inetpub\wwwroot\Champ\TRTV2\" + ImageName + ".jpg");
        }

        public static Image<Gray, UInt16> ReadFITSFile(String ImagePath)
        {
            try
            {
                Fits f = new Fits(ImagePath);
                ImageHDU h = (ImageHDU)f.ReadHDU();

                System.Array[] img = (System.Array[])h.Kernel;
                f.Close();
                int Width = img.Count();
                int Height = img.GetLength(0);

                UInt16[][] ImgConverted = new UInt16[Width][];
                Image<Gray, UInt16> LastestImageCV = new Image<Gray, UInt16>(Width, Height);
                Int16 MaxNumber = Int16.MaxValue;

                for (int i = 0; i < Width; i++)
                {
                    ImgConverted[i] = new UInt16[Height];
                    for (int j = 0; j < Height; j++)
                    {
                        int Data = MaxNumber + (Int16)img[i].GetValue(j) + 1;
                        ImgConverted[i][j] = (UInt16)Data;
                        LastestImageCV.Data[i, j,0] = (UInt16)Data;
                    }
                }
                
                return LastestImageCV;
            }
            catch
            {
                return null;
            }
        }
    }
}
