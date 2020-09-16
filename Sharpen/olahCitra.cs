using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;

namespace Form1
{
    internal class olahCitra
    {
        private Bitmap copy;
        private bool b01, b02, b03, b04, b05, b06, b07, b08, b09, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20, b21, b22, b23, b24, b25;
        private Color c, c1, c2;
        private int r, g, b, h, w, blok;
        private ToolStripProgressBar prog;
        private List<int> listR = new List<int>();
        private List<int> listG = new List<int>();
        private List<int> listB = new List<int>();
        private Dictionary<int, double> histoR = new Dictionary<int, double>();
        private Dictionary<int, double> histoG = new Dictionary<int, double>();
        private Dictionary<int, double> histoB = new Dictionary<int, double>();
        private int[] kernel;
        private float sumR, sumG, sumB;

        public olahCitra(ToolStripProgressBar prog)
        {
            this.prog = prog;
        }

        public Bitmap keAverageDenoising(List<Image> bit)
        {
            Bitmap copy1 = new Bitmap(bit[0]);
            Bitmap copy2 = new Bitmap(bit[0]);

            prog.Visible = true;

            //nilai 50 berikut menunjukkan jumlah citra, yang diproses adalah 50 citra
            int jumGambar = 50;
            for (int i = 0; i < copy1.Width; i++)
            {
                for (int j = 0; j < copy1.Height; j++)
                {
                    int newR = r = 0;
                    int newG = g = 0;
                    int newB = b = 0;

                    for (int k = 0; k < jumGambar - 1; k++)
                    {
                        copy1 = (Bitmap)bit[k];
                        c = copy1.GetPixel(i, j);

                        newR += c.R;
                        newG += c.G;
                        newB += c.B;
                    }

                    r = newR / jumGambar;
                    g = newG / jumGambar;
                    b = newB / jumGambar;

                    copy2.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
                prog.Value = Convert.ToInt16(100 * (i + 1) / copy2.Width);
            }

            prog.Visible = false;

            return copy2;
        }

        public Bitmap keBitDepth(Bitmap bit, double bitDepth)
        {
            double level = 255 / (Math.Pow(2, bitDepth) - 1);

            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = Convert.ToInt16(Math.Round(c.R / level) * level);
                    g = Convert.ToInt16(Math.Round(c.G / level) * level);
                    b = Convert.ToInt16(Math.Round(c.B / level) * level);

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keBrightness(Bitmap bit, int bright)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = c.R + bright;
                    g = c.G + bright;
                    b = c.B + bright;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keContrast(Bitmap bit, int contrast)
        {
            int con = (259 * (contrast + 255)) / (255 * (259 - contrast));

            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = con * (c.R - 128) + 128;
                    g = con * (c.G - 128) + 128;
                    b = con * (c.B - 128) + 128;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keFloydAndSteinbergErrorDiffusion(Bitmap bit)
        {
            int[] baru = new int[3];
            double errorR, errorG, errorB;
            prog.Visible = true;

            for (int i = 0; i <= bit.Width - 2; i++)
            {
                for (int j = 0; j <= bit.Height - 2; j++)
                {
                    c = bit.GetPixel(i, j);

                    baru = warnaTerdekat(c);

                    errorR = c.R - baru[0];
                    errorG = c.G - baru[1];
                    errorB = c.B - baru[2];

                    bit.SetPixel(i, j, Color.FromArgb(baru[0], baru[1], baru[2]));

                    // (x + 1, y)
                    if (i < bit.Width)
                    {
                        r = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j).R + (errorR * 7 / 16)));
                        g = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j).G + (errorG * 7 / 16)));
                        b = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j).B + (errorB * 7 / 16)));
                        bit.SetPixel(i + 1, j, Color.FromArgb(r, g, b));
                    }

                    // (x - 1, y + 1)
                    if (i > 0 && j < bit.Height)
                    {
                        r = truncate(Convert.ToInt16(bit.GetPixel(i - 1, j + 1).R + (errorR * 3 / 16)));
                        g = truncate(Convert.ToInt16(bit.GetPixel(i - 1, j + 1).G + (errorG * 3 / 16)));
                        b = truncate(Convert.ToInt16(bit.GetPixel(i - 1, j + 1).B + (errorB * 3 / 16)));
                        bit.SetPixel(i - 1, j + 1, Color.FromArgb(r, g, b));
                    }

                    // (x, y + 1)
                    if (j < bit.Height)
                    {
                        r = truncate(Convert.ToInt16(bit.GetPixel(i, j + 1).R + (errorR * 5 / 16)));
                        g = truncate(Convert.ToInt16(bit.GetPixel(i, j + 1).G + (errorG * 5 / 16)));
                        b = truncate(Convert.ToInt16(bit.GetPixel(i, j + 1).B + (errorB * 5 / 16)));
                        bit.SetPixel(i, j + 1, Color.FromArgb(r, g, b));
                    }

                    // (x + 1, y + 1)
                    if (i < bit.Width && j < bit.Height)
                    {
                        r = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j + 1).R + (errorR * 1 / 16)));
                        g = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j + 1).G + (errorG * 1 / 16)));
                        b = truncate(Convert.ToInt16(bit.GetPixel(i + 1, j + 1).B + (errorB * 1 / 16)));
                        bit.SetPixel(i + 1, j + 1, Color.FromArgb(r, g, b));
                    }
                }
                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keFuzzyGrayscale(Bitmap bit)
        {
            setDictionary(bit);

            prog.Visible = true;
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = (c.R + c.G + c.B) / 3;

                    r = fuzzifikasi(r);

                    bit.SetPixel(i, j, Color.FromArgb(r, r, r));
                }
                prog.Value = Convert.ToInt32(Math.Floor((double)(100 * (i + 1) / bit.Width)));
            }
            prog.Visible = false;

            return bit;
        }

        public Bitmap keFuzzyRGB(Bitmap bit)
        {
            setDictionary(bit);

            prog.Visible = true;
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = fuzzifikasi(c.R);
                    g = fuzzifikasi(c.G);
                    b = fuzzifikasi(c.B);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
                prog.Value = Convert.ToInt32(Math.Floor((double)(100 * (i + 1) / bit.Width)));
            }
            prog.Visible = false;

            return bit;
        }

        public Bitmap keGammaTransform(Bitmap bit, double gammaTrans)
        {
            bool gray = false;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = c.R;
                    g = c.G;
                    b = c.B;

                    if (r.Equals(g).Equals(b))
                    { gray = true; }

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    double dr = c.R;
                    double dg = c.G;
                    double db = c.B;

                    if (gray)
                    {
                        int col = Convert.ToInt16(255 * Math.Pow(dr / 255, 1 / gammaTrans));

                        bit.SetPixel(i, j, Color.FromArgb(col, col, col));
                    }
                    else
                    {
                        r = Convert.ToInt16(255 * Math.Pow(dr / 255, 1 / gammaTrans));
                        g = Convert.ToInt16(255 * Math.Pow(dg / 255, 1 / gammaTrans));
                        b = Convert.ToInt16(255 * Math.Pow(db / 255, 1 / gammaTrans));

                        bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                    }
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keGrayscaleAverage(Bitmap bit)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    int average = (c.R + c.G + c.B) / 3;

                    average = truncate(average);

                    bit.SetPixel(i, j, Color.FromArgb(average, average, average));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keGrayscaleLightness(Bitmap bit)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    int[] rgb = new int[3] { c.R, c.G, c.B };

                    int lightness = (rgb.Max() + rgb.Min()) / 2;

                    lightness = truncate(lightness);

                    bit.SetPixel(i, j, Color.FromArgb(lightness, lightness, lightness));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keGrayscaleLuminance(Bitmap bit)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    int luminance = Convert.ToInt16((0.21 * c.R) + (0.72 * c.G) + (0.07 * c.B));

                    luminance = truncate(luminance);

                    bit.SetPixel(i, j, Color.FromArgb(luminance, luminance, luminance));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keHE(Bitmap bit)
        {
            setDictionary(bit);

            //Proses menghitung nilai transform function,
            double[] transformR = new double[256];
            double[] transformG = new double[256];
            double[] transformB = new double[256];
            double jumlahR, jumlahG, jumlahB;
            jumlahR = jumlahG = jumlahB = 0;

            foreach (int i in histoR.Keys.ToList())
            {
                jumlahR += 255 * (histoR[i] / (bit.Width * bit.Height));
                transformR[i] = jumlahR;

                jumlahG += 255 * (histoG[i] / (bit.Width * bit.Height));
                transformG[i] = jumlahG;

                jumlahB += 255 * (histoB[i] / (bit.Width * bit.Height));
                transformB[i] = jumlahB;
            }

            //Proses mengubah nilai pixel ke nilai baru sesuai transform function
            prog.Visible = true;
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = Convert.ToInt16(transformR[c.R]);
                    g = Convert.ToInt16(transformG[c.G]);
                    b = Convert.ToInt16(transformB[c.B]);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt32(Math.Floor((double)(100 * (i + 1) /
                bit.Width)));
            }
            prog.Visible = false;

            return bit;
        }

     
        public Bitmap keInverse(Bitmap bit)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = 255 - c.R;
                    g = 255 - c.G;
                    b = 255 - c.B;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keLogBrightness(Bitmap bit, int logBright)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = Convert.ToInt16(logBright * Math.Log10(1 + Math.Abs(c.R)));
                    g = Convert.ToInt16(logBright * Math.Log10(1 + Math.Abs(c.G)));
                    b = Convert.ToInt16(logBright * Math.Log10(1 + Math.Abs(c.B)));

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        public Bitmap keNearest8Color(Bitmap bit)
        {
            int[] baru = new int[3];

            prog.Visible = true;
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);
                    baru = warnaTerdekat(c);
                    bit.SetPixel(i, j, Color.FromArgb(baru[0], baru[1], baru[2]));
                }
                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;
            return bit;
        }

        public Bitmap keRGB(Bitmap bit, int red, int green, int blue)
        {
            prog.Visible = true;

            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    r = c.R + red;
                    g = c.G + green;
                    b = c.B + blue;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = 100 * (i + 1) / bit.Width;
            }

            prog.Visible = false;

            return bit;
        }

        /// 

        private int fuzzifikasi(int color)
        {
            double batas1, batas2, v1, v2, hasilFuzzy1, hasilFuzzy2, col;
            batas1 = batas2 = v1 = v2 = hasilFuzzy1 = hasilFuzzy2 = col = 0;

            //Proses Fuzifikasi
            if (color >= 0 && color <= 63)
            {
                batas1 = 0;
                batas2 = 63;
                v1 = 0;
                v2 = 0;
            }
            else if (color >= 64 && color <= 126)
            {
                batas1 = 63;
                batas2 = 127;
                v1 = 0;
                v2 = 127;
            }
            else if (color >= 128 && color <= 190)
            {
                batas1 = 127;
                batas2 = 191;
                v1 = 127;
                v2 = 255;
            }
            else if (color >= 191 && color <= 255)
            {
                batas1 = 191;
                batas2 = 255;
                v1 = 255;
                v2 = 255;
            }

            //Proses Fuzzification untuk menghasilkan fuzzy input
            hasilFuzzy1 = (color - batas1) / (batas2 - batas1);
            hasilFuzzy2 = -(color - batas2) / (batas2 - batas1);

            //Proses Defuzzifikasi
            if (color >= 0 && color <= 63)
            {
                col = 0;
            }
            else if (color >= 64 && color <= 126)
            {
                col = ((hasilFuzzy1 * v2) + (hasilFuzzy2 * v1)) / (hasilFuzzy1 + hasilFuzzy2);
            }
            else if (color == 127)
            {
                col = 127;
            }
            else if (color >= 128 && color <= 190)
            {
                col = ((hasilFuzzy1 * v2) + (hasilFuzzy2 * v1)) / (hasilFuzzy1 + hasilFuzzy2);
            }
            else if (color >= 191 && color <= 255)
            {
                col = 255;
            }

            return Convert.ToInt16(Math.Round(col));
        }

        private void setDictionary(Bitmap bit)
        {
            //int size = bit.Width * bit.Height;
            //Proses inisiasi nilai awal pixel 0 - 255 diset bernilai 0
            for (int counter = 0; counter <= 255; counter++)
            {
                histoR[counter] = 0;
                histoG[counter] = 0;
                histoB[counter] = 0;
            }

            prog.Visible = true;
            //Untuk tiap baris dan kolom citra, nilai histogram ditambahkan
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    c = bit.GetPixel(i, j);

                    histoR[c.R] += 1;
                    histoG[c.G] += 1;
                    histoB[c.B] += 1; //kerja histogram
                }
                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;
        }

        private int truncate(int c)
        {
            if (c < 0)
            {
                c = 0;
            }

            if (c > 255)
            {
                c = 255;
            }

            return c;
        }

        private int[] warnaTerdekat(Color col)
        {
            double minDistance = (255 * 255) + (255 * 255) + (255 * 255);

            int palColor, rDiff, gDiff, bDiff;
            int[] pValue = new int[3];
            double distance;

            //set warna pallete: hitam, merah, hijau, kuning, biru, cyan, magenta, putih
            int[,] palletteColor = new int[,] {
                { 0, 0, 0 }, { 255, 0, 0 }, { 0, 255, 0 }, { 255, 255, 0 }, { 0, 0, 255 }, { 0, 255, 255 }, { 255, 0, 255 }, { 255, 255, 255 }
            };

            for (palColor = 0; palColor <= palletteColor.GetLength(0) - 1; palColor++)
            {
                rDiff = col.R - palletteColor[palColor, 0];
                gDiff = col.G - palletteColor[palColor, 1];
                bDiff = col.B - palletteColor[palColor, 2];

                distance = (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    pValue[0] = palletteColor[palColor, 0];
                    pValue[1] = palletteColor[palColor, 1];
                    pValue[2] = palletteColor[palColor, 2];
                }
            }

            return pValue;
        }

        //

        /*private Bitmap filter3x3(Bitmap bit, int[] kernel)
        {
            Bitmap copy = new Bitmap(bit);

            bool b1, b2, b3, b4, b5, b6, b7, b8, b9;
            Color c1, c2, c3, c4, c5, c6, c7, c8, c9;
            float sumR, sumG, sumB;

            int w = bit.Width - 1;
            int h = bit.Height - 1;

            int blok = 0;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b1 = b2 = b3 = b4 = b5 = b6 = b7 = b8 = b9 = false;
                    sumR = sumG = sumB = 0;
                    blok = 0;

                    // top
                    if (j == 0)
                    {
                        // top left
                        if (i == 0)
                        {
                            b5 = b6 = true;
                            b8 = b9 = true;
                        }

                        // top center
                        else if (0 < i && i < w)
                        {
                            b4 = b5 = b6 = true;
                            b7 = b8 = b9 = true;
                        }

                        // top right
                        else if (i == w)
                        {
                            b4 = b5 = true;
                            b7 = b8 = true;
                        }
                    }

                    // middle
                    else if (0 < j && j < h)
                    {
                        // middle left
                        if (i == 0)
                        {
                            b2 = b3 = true;
                            b5 = b6 = true;
                            b8 = b9 = true;
                        }

                        // middle center
                        else if (0 < i && i < w)
                        {
                            b1 = b2 = b3 = true;
                            b4 = b5 = b6 = true;
                            b7 = b8 = b9 = true;
                        }

                        // middle right
                        else if (i == w)
                        {
                            b1 = b2 = true;
                            b4 = b5 = true;
                            b7 = b8 = true;
                        }
                    }

                    // bottom
                    else if (j == h)
                    {
                        // bottom left
                        if (i == 0)
                        {
                            b2 = b3 = true;
                            b5 = b6 = true;
                        }

                        // bottom center
                        else if (0 < i && i < w)
                        {
                            b1 = b2 = b3 = true;
                            b4 = b5 = b6 = true;
                        }

                        // bottom right
                        else if (i == w)
                        {
                            b1 = b2 = true;
                            b4 = b5 = true;
                        }
                    }

                    if (b1)
                    {
                        c1 = bit.GetPixel(i - 1, j - 1);
                        sumR += (c1.R * kernel[0]);
                        sumG += (c1.G * kernel[0]);
                        sumB += (c1.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b2)
                    {
                        c2 = bit.GetPixel(i, j - 1);
                        sumR += (c2.R * kernel[1]);
                        sumG += (c2.G * kernel[1]);
                        sumB += (c2.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b3)
                    {
                        c3 = bit.GetPixel(i + 1, j - 1);
                        sumR += (c3.R * kernel[2]);
                        sumG += (c3.G * kernel[2]);
                        sumB += (c3.B * kernel[2]);
                        blok += kernel[2];
                    }

                    if (b4)
                    {
                        c4 = bit.GetPixel(i - 1, j);
                        sumR += (c4.R * kernel[3]);
                        sumG += (c4.G * kernel[3]);
                        sumB += (c4.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b5)
                    {
                        c5 = bit.GetPixel(i, j);
                        sumR += (c5.R * kernel[4]);
                        sumG += (c5.G * kernel[4]);
                        sumB += (c5.B * kernel[4]);
                        blok += kernel[4];
                    }

                    if (b6)
                    {
                        c6 = bit.GetPixel(i + 1, j);
                        sumR += (c6.R * kernel[5]);
                        sumG += (c6.G * kernel[5]);
                        sumB += (c6.B * kernel[5]);
                        blok += kernel[5];
                    }

                    if (b7)
                    {
                        c7 = bit.GetPixel(i - 1, j + 1);
                        sumR += (c7.R * kernel[6]);
                        sumG += (c7.G * kernel[6]);
                        sumB += (c7.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b8)
                    {
                        c8 = bit.GetPixel(i, j + 1);
                        sumR += (c8.R * kernel[7]);
                        sumG += (c8.G * kernel[7]);
                        sumB += (c8.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b9)
                    {
                        c9 = bit.GetPixel(i + 1, j + 1);
                        sumR += (c9.R * kernel[8]);
                        sumG += (c9.G * kernel[8]);
                        sumB += (c9.B * kernel[8]);
                        blok += kernel[8];
                    }

                    r = blok != 0 ? (int)sumR / blok : (int)sumR;
                    g = blok != 0 ? (int)sumG / blok : (int)sumG;
                    b = blok != 0 ? (int)sumB / blok : (int)sumB;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;
        }*/


        private Bitmap filter3x3(Bitmap bit, int[] kernel, string str = "")
        {
            copy = new Bitmap(bit);

            w = bit.Width - 1;
            h = bit.Height - 1;

            blok = 0;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b01 = b02 = b03 = false;
                    b04 = b05 = b06 = false;
                    b07 = b08 = b09 = false;
                    sumR = sumG = sumB = 0;
                    blok = 0;
                    listR.Clear();
                    listG.Clear();
                    listB.Clear();

                    // top
                    if (j == 0)
                    {
                        // top left
                        if (i == 0)
                        {
                            b05 = b06 = true;
                            b08 = b09 = true;
                        }

                        // top center
                        else if (0 < i && i < w)
                        {
                            b04 = b05 = b06 = true;
                            b07 = b08 = b09 = true;
                        }

                        // top right
                        else if (i == w)
                        {
                            b04 = b05 = true;
                            b07 = b08 = true;
                        }
                    }

                    // middle
                    else if (0 < j && j < h)
                    {
                        // middle left
                        if (i == 0)
                        {
                            b02 = b03 = true;
                            b05 = b06 = true;
                            b08 = b09 = true;
                        }

                        // middle center
                        else if (0 < i && i < w)
                        {
                            b01 = b02 = b03 = true;
                            b04 = b05 = b06 = true;
                            b07 = b08 = b09 = true;
                        }

                        // middle right
                        else if (i == w)
                        {
                            b01 = b02 = true;
                            b04 = b05 = true;
                            b07 = b08 = true;
                        }
                    }

                    // bottom
                    else if (j == h)
                    {
                        // bottom left
                        if (i == 0)
                        {
                            b02 = b03 = true;
                            b05 = b06 = true;
                        }

                        // bottom center
                        else if (0 < i && i < w)
                        {
                            b01 = b02 = b03 = true;
                            b04 = b05 = b06 = true;
                        }

                        // bottom right
                        else if (i == w)
                        {
                            b01 = b02 = true;
                            b04 = b05 = true;
                        }
                    }

                    if (b01)
                    {
                        c = bit.GetPixel(i - 1, j - 1);
                        listR.Add(c.R * kernel[0]);
                        listG.Add(c.G * kernel[0]);
                        listB.Add(c.B * kernel[0]);
                        sumR += (c.R * kernel[0]);
                        sumG += (c.G * kernel[0]);
                        sumB += (c.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b02)
                    {
                        c = bit.GetPixel(i, j - 1);
                        listR.Add(c.R * kernel[1]);
                        listG.Add(c.G * kernel[1]);
                        listB.Add(c.B * kernel[1]);
                        sumR += (c.R * kernel[1]);
                        sumG += (c.G * kernel[1]);
                        sumB += (c.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b03)
                    {
                        c = bit.GetPixel(i + 1, j - 1);
                        listR.Add(c.R * kernel[2]);
                        listG.Add(c.G * kernel[2]);
                        listB.Add(c.B * kernel[2]);
                        sumR += (c.R * kernel[2]);
                        sumG += (c.G * kernel[2]);
                        sumB += (c.B * kernel[2]);
                        blok += kernel[2];
                    }

                    ///

                    if (b04)
                    {
                        c = bit.GetPixel(i - 1, j);
                        listR.Add(c.R * kernel[3]);
                        listG.Add(c.G * kernel[3]);
                        listB.Add(c.B * kernel[3]);
                        sumR += (c.R * kernel[3]);
                        sumG += (c.G * kernel[3]);
                        sumB += (c.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b05)
                    {
                        c = bit.GetPixel(i, j);
                        listR.Add(c.R * kernel[4]);
                        listG.Add(c.G * kernel[4]);
                        listB.Add(c.B * kernel[4]);
                        sumR += (c.R * kernel[4]);
                        sumG += (c.G * kernel[4]);
                        sumB += (c.B * kernel[4]);
                        blok += kernel[4];
                    }

                    if (b06)
                    {
                        c = bit.GetPixel(i + 1, j);
                        listR.Add(c.R * kernel[5]);
                        listG.Add(c.G * kernel[5]);
                        listB.Add(c.B * kernel[5]);
                        sumR += (c.R * kernel[5]);
                        sumG += (c.G * kernel[5]);
                        sumB += (c.B * kernel[5]);
                        blok += kernel[5];
                    }

                    ///

                    if (b07)
                    {
                        c = bit.GetPixel(i - 1, j + 1);
                        listR.Add(c.R * kernel[6]);
                        listG.Add(c.G * kernel[6]);
                        listB.Add(c.B * kernel[6]);
                        sumR += (c.R * kernel[6]);
                        sumG += (c.G * kernel[6]);
                        sumB += (c.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b08)
                    {
                        c = bit.GetPixel(i, j + 1);
                        listR.Add(c.R * kernel[7]);
                        listG.Add(c.G * kernel[7]);
                        listB.Add(c.B * kernel[7]);
                        sumR += (c.R * kernel[7]);
                        sumG += (c.G * kernel[7]);
                        sumB += (c.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b09)
                    {
                        c = bit.GetPixel(i + 1, j + 1);
                        listR.Add(c.R * kernel[8]);
                        listG.Add(c.G * kernel[8]);
                        listB.Add(c.B * kernel[8]);
                        sumR += (c.R * kernel[8]);
                        sumG += (c.G * kernel[8]);
                        sumB += (c.B * kernel[8]);
                        blok += kernel[8];
                    }

                    Console.WriteLine("sum " + sumR + " : list " + listR.Sum());

                    if (str.Equals(""))
                    {
                        r = blok != 0 ? (int)sumR / blok : (int)sumR;
                        g = blok != 0 ? (int)sumG / blok : (int)sumG;
                        b = blok != 0 ? (int)sumB / blok : (int)sumB;
                    }
                    else if (str.Equals("dilasi"))
                    {
                        r = dilasi((int)sumR, blok);
                        g = dilasi((int)sumG, blok);
                        b = dilasi((int)sumB, blok);
                    }
                    else if (str.Equals("erosi"))
                    {
                        r = erosi((int)sumR, blok);
                        g = erosi((int)sumG, blok);
                        b = erosi((int)sumB, blok);
                    }
                    else if (str.Equals("grayscale_dilasi"))
                    {
                        r = listR.Max();
                        g = listG.Max();
                        b = listB.Max();
                    }
                    else if (str.Equals("grayscale_erosi"))
                    {
                        r = listR.Min();
                        g = listG.Min();
                        b = listB.Min();
                    }

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;
        }


        /*private Bitmap filter5x5(Bitmap bit, int[] kernel)
        {
            copy = new Bitmap(bit);

            w = bit.Width - 1;
            h = bit.Height - 1;

            blok = 0;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b1 = b2 = b3 = b4 = b5 = false;
                    b6 = b7 = b8 = b9 = b10 = false;
                    b11 = b12 = b13 = b14 = b15 = false;
                    b16 = b17 = b18 = b19 = b20 = false;
                    b21 = b22 = b23 = b24 = b25 = false;

                    sumR = sumG = sumB = 0;
                    blok = 0;

                    // row 1
                    if (j == 0)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 2
                    else if (j == 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 3
                    else if (1 < j && j < h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 4
                    else if (j == h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                        }
                    }

                    // row 5
                    else if (j == h)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                        }
                    }

                    if (b1)
                    {
                        c1 = bit.GetPixel(i - 2, j - 2);
                        sumR += (c1.R * kernel[0]);
                        sumG += (c1.G * kernel[0]);
                        sumB += (c1.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b2)
                    {
                        c2 = bit.GetPixel(i - 1, j - 2);
                        sumR += (c2.R * kernel[1]);
                        sumG += (c2.G * kernel[1]);
                        sumB += (c2.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b3)
                    {
                        c3 = bit.GetPixel(i, j - 2);
                        sumR += (c3.R * kernel[2]);
                        sumG += (c3.G * kernel[2]);
                        sumB += (c3.B * kernel[2]);
                        blok += kernel[2];
                    }

                    if (b4)
                    {
                        c4 = bit.GetPixel(i + 1, j - 2);
                        sumR += (c4.R * kernel[3]);
                        sumG += (c4.G * kernel[3]);
                        sumB += (c4.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b5)
                    {
                        c5 = bit.GetPixel(i + 2, j - 2);
                        sumR += (c5.R * kernel[4]);
                        sumG += (c5.G * kernel[4]);
                        sumB += (c5.B * kernel[4]);
                        blok += kernel[4];
                    }

                    if (b6)
                    {
                        c6 = bit.GetPixel(i - 2, j - 1);
                        sumR += (c6.R * kernel[5]);
                        sumG += (c6.G * kernel[5]);
                        sumB += (c6.B * kernel[5]);
                        blok += kernel[5];
                    }

                    if (b7)
                    {
                        c7 = bit.GetPixel(i - 1, j - 1);
                        sumR += (c7.R * kernel[6]);
                        sumG += (c7.G * kernel[6]);
                        sumB += (c7.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b8)
                    {
                        c8 = bit.GetPixel(i, j - 1);
                        sumR += (c8.R * kernel[7]);
                        sumG += (c8.G * kernel[7]);
                        sumB += (c8.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b9)
                    {
                        c9 = bit.GetPixel(i + 1, j - 1);
                        sumR += (c9.R * kernel[8]);
                        sumG += (c9.G * kernel[8]);
                        sumB += (c9.B * kernel[8]);
                        blok += kernel[8];
                    }

                    if (b10)
                    {
                        c10 = bit.GetPixel(i + 2, j - 1);
                        sumR += (c10.R * kernel[9]);
                        sumG += (c10.G * kernel[9]);
                        sumB += (c10.B * kernel[9]);
                        blok += kernel[9];
                    }

                    if (b11)
                    {
                        c11 = bit.GetPixel(i - 2, j);
                        sumR += (c11.R * kernel[10]);
                        sumG += (c11.G * kernel[10]);
                        sumB += (c11.B * kernel[10]);
                        blok += kernel[10];
                    }

                    if (b12)
                    {
                        c12 = bit.GetPixel(i - 1, j);
                        sumR += (c12.R * kernel[11]);
                        sumG += (c12.G * kernel[11]);
                        sumB += (c12.B * kernel[11]);
                        blok += kernel[11];
                    }

                    if (b13)
                    {
                        c13 = bit.GetPixel(i, j);
                        sumR += (c13.R * kernel[12]);
                        sumG += (c13.G * kernel[12]);
                        sumB += (c13.B * kernel[12]);
                        blok += kernel[12];
                    }

                    if (b14)
                    {
                        c14 = bit.GetPixel(i + 1, j);
                        sumR += (c14.R * kernel[13]);
                        sumG += (c14.G * kernel[13]);
                        sumB += (c14.B * kernel[13]);
                        blok += kernel[13];
                    }

                    if (b15)
                    {
                        c15 = bit.GetPixel(i + 2, j);
                        sumR += (c15.R * kernel[14]);
                        sumG += (c15.G * kernel[14]);
                        sumB += (c15.B * kernel[14]);
                        blok += kernel[14];
                    }

                    if (b16)
                    {
                        c16 = bit.GetPixel(i - 2, j + 1);
                        sumR += (c16.R * kernel[15]);
                        sumG += (c16.G * kernel[15]);
                        sumB += (c16.B * kernel[15]);
                        blok += kernel[15];
                    }

                    if (b17)
                    {
                        c17 = bit.GetPixel(i - 1, j + 1);
                        sumR += (c17.R * kernel[16]);
                        sumG += (c17.G * kernel[16]);
                        sumB += (c17.B * kernel[16]);
                        blok += kernel[16];
                    }

                    if (b18)
                    {
                        c18 = bit.GetPixel(i, j + 1);
                        sumR += (c18.R * kernel[17]);
                        sumG += (c18.G * kernel[17]);
                        sumB += (c18.B * kernel[17]);
                        blok += kernel[17];
                    }

                    if (b19)
                    {
                        c19 = bit.GetPixel(i + 1, j + 1);
                        sumR += (c19.R * kernel[18]);
                        sumG += (c19.G * kernel[18]);
                        sumB += (c19.B * kernel[18]);
                        blok += kernel[18];
                    }

                    if (b20)
                    {
                        c20 = bit.GetPixel(i + 2, j + 1);
                        sumR += (c20.R * kernel[19]);
                        sumG += (c20.G * kernel[19]);
                        sumB += (c20.B * kernel[19]);
                        blok += kernel[19];
                    }

                    if (b21)
                    {
                        c21 = bit.GetPixel(i - 2, j + 2);
                        sumR += (c21.R * kernel[20]);
                        sumG += (c21.G * kernel[20]);
                        sumB += (c21.B * kernel[20]);
                        blok += kernel[20];
                    }

                    if (b22)
                    {
                        c22 = bit.GetPixel(i - 1, j + 2);
                        sumR += (c22.R * kernel[21]);
                        sumG += (c22.G * kernel[21]);
                        sumB += (c22.B * kernel[21]);
                        blok += kernel[21];
                    }

                    if (b23)
                    {
                        c23 = bit.GetPixel(i, j + 2);
                        sumR += (c23.R * kernel[22]);
                        sumG += (c23.G * kernel[22]);
                        sumB += (c23.B * kernel[22]);
                        blok += kernel[22];
                    }

                    if (b24)
                    {
                        c24 = bit.GetPixel(i + 1, j + 2);
                        sumR += (c24.R * kernel[23]);
                        sumG += (c24.G * kernel[23]);
                        sumB += (c24.B * kernel[23]);
                        blok += kernel[23];
                    }

                    if (b25)
                    {
                        c25 = bit.GetPixel(i + 2, j + 2);
                        sumR += (c25.R * kernel[24]);
                        sumG += (c25.G * kernel[24]);
                        sumB += (c25.B * kernel[24]);
                        blok += kernel[24];
                    }
                    Console.WriteLine(blok);
                    r = blok != 0 ? (int)sumR / blok : (int)sumR;
                    g = blok != 0 ? (int)sumG / blok : (int)sumG;
                    b = blok != 0 ? (int)sumB / blok : (int)sumB;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;
        }*/


        private Bitmap filter5x5(Bitmap bit, int[] kernel, string str = "")
        {
            copy = new Bitmap(bit);

            w = bit.Width - 1;
            h = bit.Height - 1;

            blok = 0;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b01 = b02 = b03 = b04 = b05 = false;
                    b06 = b07 = b08 = b09 = b10 = false;
                    b11 = b12 = b13 = b14 = b15 = false;
                    b16 = b17 = b18 = b19 = b20 = false;
                    b21 = b22 = b23 = b24 = b25 = false;

                    sumR = sumG = sumB = 0;
                    blok = 0;

                    // row 1
                    if (j == 0)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 2
                    else if (j == 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b08 = b09 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b07 = b08 = b09 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b06 = b07 = b08 = b09 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b06 = b07 = b08 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 3
                    else if (1 < j && j < h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b03 = b04 = b05 = true;
                            b08 = b09 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b02 = b03 = b04 = b05 = true;
                            b07 = b08 = b09 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b06 = b07 = b08 = b09 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = true;
                            b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b01 = b02 = b03 = true;
                            b06 = b07 = b08 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 4
                    else if (j == h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b03 = b04 = b05 = true;
                            b08 = b09 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b02 = b03 = b04 = b05 = true;
                            b07 = b08 = b09 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b06 = b07 = b08 = b09 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = true;
                            b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b01 = b02 = b03 = true;
                            b06 = b07 = b08 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                        }
                    }

                    // row 5
                    else if (j == h)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b03 = b04 = b05 = true;
                            b08 = b09 = b10 = true;
                            b13 = b14 = b15 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b02 = b03 = b04 = b05 = true;
                            b07 = b08 = b09 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b06 = b07 = b08 = b09 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = true;
                            b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b01 = b02 = b03 = true;
                            b06 = b07 = b08 = true;
                            b11 = b12 = b13 = true;
                        }
                    }

                    if (b01)
                    {
                        c = bit.GetPixel(i - 2, j - 2);
                        sumR += (c.R * kernel[0]);
                        sumG += (c.G * kernel[0]);
                        sumB += (c.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b02)
                    {
                        c = bit.GetPixel(i - 1, j - 2);
                        sumR += (c.R * kernel[1]);
                        sumG += (c.G * kernel[1]);
                        sumB += (c.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b03)
                    {
                        c = bit.GetPixel(i, j - 2);
                        sumR += (c.R * kernel[2]);
                        sumG += (c.G * kernel[2]);
                        sumB += (c.B * kernel[2]);
                        blok += kernel[2];
                    }

                    if (b04)
                    {
                        c = bit.GetPixel(i + 1, j - 2);
                        sumR += (c.R * kernel[3]);
                        sumG += (c.G * kernel[3]);
                        sumB += (c.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b05)
                    {
                        c = bit.GetPixel(i + 2, j - 2);
                        sumR += (c.R * kernel[4]);
                        sumG += (c.G * kernel[4]);
                        sumB += (c.B * kernel[4]);
                        blok += kernel[4];
                    }

                    ///

                    if (b06)
                    {
                        c = bit.GetPixel(i - 2, j - 1);
                        sumR += (c.R * kernel[5]);
                        sumG += (c.G * kernel[5]);
                        sumB += (c.B * kernel[5]);
                        blok += kernel[5];
                    }

                    if (b07)
                    {
                        c = bit.GetPixel(i - 1, j - 1);
                        sumR += (c.R * kernel[6]);
                        sumG += (c.G * kernel[6]);
                        sumB += (c.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b08)
                    {
                        c = bit.GetPixel(i, j - 1);
                        sumR += (c.R * kernel[7]);
                        sumG += (c.G * kernel[7]);
                        sumB += (c.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b09)
                    {
                        c = bit.GetPixel(i + 1, j - 1);
                        sumR += (c.R * kernel[8]);
                        sumG += (c.G * kernel[8]);
                        sumB += (c.B * kernel[8]);
                        blok += kernel[8];
                    }

                    if (b10)
                    {
                        c = bit.GetPixel(i + 2, j - 1);
                        sumR += (c.R * kernel[9]);
                        sumG += (c.G * kernel[9]);
                        sumB += (c.B * kernel[9]);
                        blok += kernel[9];
                    }

                    ///

                    if (b11)
                    {
                        c = bit.GetPixel(i - 2, j);
                        sumR += (c.R * kernel[10]);
                        sumG += (c.G * kernel[10]);
                        sumB += (c.B * kernel[10]);
                        blok += kernel[10];
                    }

                    if (b12)
                    {
                        c = bit.GetPixel(i - 1, j);
                        sumR += (c.R * kernel[11]);
                        sumG += (c.G * kernel[11]);
                        sumB += (c.B * kernel[11]);
                        blok += kernel[11];
                    }

                    if (b13)
                    {
                        c = bit.GetPixel(i, j);
                        sumR += (c.R * kernel[12]);
                        sumG += (c.G * kernel[12]);
                        sumB += (c.B * kernel[12]);
                        blok += kernel[12];
                    }

                    if (b14)
                    {
                        c = bit.GetPixel(i + 1, j);
                        sumR += (c.R * kernel[13]);
                        sumG += (c.G * kernel[13]);
                        sumB += (c.B * kernel[13]);
                        blok += kernel[13];
                    }

                    if (b15)
                    {
                        c = bit.GetPixel(i + 2, j);
                        sumR += (c.R * kernel[14]);
                        sumG += (c.G * kernel[14]);
                        sumB += (c.B * kernel[14]);
                        blok += kernel[14];
                    }

                    ///

                    if (b16)
                    {
                        c = bit.GetPixel(i - 2, j + 1);
                        sumR += (c.R * kernel[15]);
                        sumG += (c.G * kernel[15]);
                        sumB += (c.B * kernel[15]);
                        blok += kernel[15];
                    }

                    if (b17)
                    {
                        c = bit.GetPixel(i - 1, j + 1);
                        sumR += (c.R * kernel[16]);
                        sumG += (c.G * kernel[16]);
                        sumB += (c.B * kernel[16]);
                        blok += kernel[16];
                    }

                    if (b18)
                    {
                        c = bit.GetPixel(i, j + 1);
                        sumR += (c.R * kernel[17]);
                        sumG += (c.G * kernel[17]);
                        sumB += (c.B * kernel[17]);
                        blok += kernel[17];
                    }

                    if (b19)
                    {
                        c = bit.GetPixel(i + 1, j + 1);
                        sumR += (c.R * kernel[18]);
                        sumG += (c.G * kernel[18]);
                        sumB += (c.B * kernel[18]);
                        blok += kernel[18];
                    }

                    if (b20)
                    {
                        c = bit.GetPixel(i + 2, j + 1);
                        sumR += (c.R * kernel[19]);
                        sumG += (c.G * kernel[19]);
                        sumB += (c.B * kernel[19]);
                        blok += kernel[19];
                    }

                    ///

                    if (b21)
                    {
                        c = bit.GetPixel(i - 2, j + 2);
                        sumR += (c.R * kernel[20]);
                        sumG += (c.G * kernel[20]);
                        sumB += (c.B * kernel[20]);
                        blok += kernel[20];
                    }

                    if (b22)
                    {
                        c = bit.GetPixel(i - 1, j + 2);
                        sumR += (c.R * kernel[21]);
                        sumG += (c.G * kernel[21]);
                        sumB += (c.B * kernel[21]);
                        blok += kernel[21];
                    }

                    if (b23)
                    {
                        c = bit.GetPixel(i, j + 2);
                        sumR += (c.R * kernel[22]);
                        sumG += (c.G * kernel[22]);
                        sumB += (c.B * kernel[22]);
                        blok += kernel[22];
                    }

                    if (b24)
                    {
                        c = bit.GetPixel(i + 1, j + 2);
                        sumR += (c.R * kernel[23]);
                        sumG += (c.G * kernel[23]);
                        sumB += (c.B * kernel[23]);
                        blok += kernel[23];
                    }

                    if (b25)
                    {
                        c = bit.GetPixel(i + 2, j + 2);
                        sumR += (c.R * kernel[24]);
                        sumG += (c.G * kernel[24]);
                        sumB += (c.B * kernel[24]);
                        blok += kernel[24];
                    }

                    if (str.Equals(""))
                    {
                        r = blok != 0 ? (int)sumR / blok : (int)sumR;
                        g = blok != 0 ? (int)sumG / blok : (int)sumG;
                        b = blok != 0 ? (int)sumB / blok : (int)sumB;
                    }
                    else
                    {
                        if (str.Equals("erosi"))
                        {
                            r = erosi((int)sumR, blok);
                            g = erosi((int)sumG, blok);
                            b = erosi((int)sumB, blok);
                        }

                        if (str.Equals("dilasi"))
                        {
                            r = dilasi((int)sumR, blok);
                            g = dilasi((int)sumG, blok);
                            b = dilasi((int)sumB, blok);
                        }
                    }

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;
        }

        private Bitmap filter9x9(Bitmap bit, int[] kernel, string str = "")
        {
            copy = new Bitmap(bit);

            w = bit.Width - 1;
            h = bit.Height - 1;

            blok = 0;

            bool b26, b27;
            bool b28, b29, b30, b31, b32, b33, b34, b35, b36;
            bool b37, b38, b39, b40, b41, b42, b43, b44, b45;
            bool b46, b47, b48, b49, b50, b51, b52, b53, b54;
            bool b55, b56, b57, b58, b59, b60, b61, b62, b63;
            bool b64, b65, b66, b67, b68, b69, b70, b71, b72;
            bool b73, b74, b75, b76, b77, b78, b79, b80, b81;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = false;
                    b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = false;
                    b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = false;
                    b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = false;
                    b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = false;
                    b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = false;
                    b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = false;
                    b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = false;
                    b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = false;

                    sumR = sumG = sumB = 0;
                    blok = 0;

                    // row 1
                    if (j == 0)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                            b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                            b73 = b74 = b75 = b76 = b77 = true;
                        }
                    }

                    // row 2
                    else if (j == 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                            b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                            b73 = b74 = b75 = b76 = b77 = true;
                        }
                    }

                    // row 3
                    else if (j == 2)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                            b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                            b73 = b74 = b75 = b76 = b77 = true;
                        }
                    }

                    // row 3
                    else if (j == 3)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                            b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                            b73 = b74 = b75 = b76 = b77 = true;
                        }
                    }

                    // row 5
                    else if (3 < j && j < h - 3)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b05 = b06 = b07 = b08 = b09 = true;
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                            b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = b81 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = b80 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = b79 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                            b73 = b74 = b75 = b76 = b77 = b78 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                            b73 = b74 = b75 = b76 = b77 = true;
                        }
                    }

                    // row 6
                    else if (j == h - 3)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b05 = b06 = b07 = b08 = b09 = true;
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                            b68 = b69 = b70 = b71 = b72 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b67 = b68 = b69 = b70 = b71 = b72 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = b72 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = b71 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = b70 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                            b64 = b65 = b66 = b67 = b68 = b69 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                            b64 = b65 = b66 = b67 = b68 = true;
                        }
                    }

                    // row 7
                    else if (j == h - 2)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b05 = b06 = b07 = b08 = b09 = true;
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                            b59 = b60 = b61 = b62 = b63 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b58 = b59 = b60 = b61 = b62 = b63 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = b63 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = b62 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = b61 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                            b55 = b56 = b57 = b58 = b59 = b60 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                            b55 = b56 = b57 = b58 = b59 = true;
                        }
                    }

                    // row 8
                    else if (j == h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b05 = b06 = b07 = b08 = b09 = true;
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                            b50 = b51 = b52 = b53 = b54 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b49 = b50 = b51 = b52 = b53 = b54 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = b54 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = b53 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = b52 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                            b46 = b47 = b48 = b49 = b50 = b51 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                            b46 = b47 = b48 = b49 = b50 = true;
                        }
                    }

                    // row 9
                    else if (j == h)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b05 = b06 = b07 = b08 = b09 = true;
                            b14 = b15 = b16 = b17 = b18 = true;
                            b23 = b24 = b25 = b26 = b27 = true;
                            b32 = b33 = b34 = b35 = b36 = true;
                            b41 = b42 = b43 = b44 = b45 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b40 = b41 = b42 = b43 = b44 = b45 = true;
                        }

                        // column 3
                        else if (i == 2)
                        {
                            b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                        }

                        // column 4
                        else if (i == 3)
                        {
                            b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                        }

                        // column 5
                        else if (3 < i && i < w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = b09 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = b18 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = b27 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = b36 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = b45 = true;
                        }

                        // column 6
                        else if (i == w - 3)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = b08 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = b17 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = b26 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = b35 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = b44 = true;
                        }

                        // column 7
                        else if (i == w - 2)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = b07 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = b16 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = b25 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = b34 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = b43 = true;
                        }

                        // column 8
                        else if (i == w - 1)
                        {
                            b01 = b02 = b03 = b04 = b05 = b06 = true;
                            b10 = b11 = b12 = b13 = b14 = b15 = true;
                            b19 = b20 = b21 = b22 = b23 = b24 = true;
                            b28 = b29 = b30 = b31 = b32 = b33 = true;
                            b37 = b38 = b39 = b40 = b41 = b42 = true;
                        }

                        // column 9
                        else if (i == w)
                        {
                            b01 = b02 = b03 = b04 = b05 = true;
                            b10 = b11 = b12 = b13 = b14 = true;
                            b19 = b20 = b21 = b22 = b23 = true;
                            b28 = b29 = b30 = b31 = b32 = true;
                            b37 = b38 = b39 = b40 = b41 = true;
                        }
                    }

                    if (b01)
                    {
                        c = bit.GetPixel(i - 4, j - 4);
                        sumR += (c.R * kernel[0]);
                        sumG += (c.G * kernel[0]);
                        sumB += (c.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b02)
                    {
                        c = bit.GetPixel(i - 3, j - 4);
                        sumR += (c.R * kernel[1]);
                        sumG += (c.G * kernel[1]);
                        sumB += (c.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b03)
                    {
                        c = bit.GetPixel(i - 2, j - 4);
                        sumR += (c.R * kernel[2]);
                        sumG += (c.G * kernel[2]);
                        sumB += (c.B * kernel[2]);
                        blok += kernel[2];
                    }

                    if (b04)
                    {
                        c = bit.GetPixel(i - 1, j - 4);
                        sumR += (c.R * kernel[3]);
                        sumG += (c.G * kernel[3]);
                        sumB += (c.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b05)
                    {
                        c = bit.GetPixel(i, j - 4);
                        sumR += (c.R * kernel[4]);
                        sumG += (c.G * kernel[4]);
                        sumB += (c.B * kernel[4]);
                        blok += kernel[4];
                    }

                    if (b06)
                    {
                        c = bit.GetPixel(i + 1, j - 4);
                        sumR += (c.R * kernel[5]);
                        sumG += (c.G * kernel[5]);
                        sumB += (c.B * kernel[5]);
                        blok += kernel[5];
                    }

                    if (b07)
                    {
                        c = bit.GetPixel(i + 2, j - 4);
                        sumR += (c.R * kernel[6]);
                        sumG += (c.G * kernel[6]);
                        sumB += (c.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b08)
                    {
                        c = bit.GetPixel(i + 3, j - 4);
                        sumR += (c.R * kernel[7]);
                        sumG += (c.G * kernel[7]);
                        sumB += (c.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b09)
                    {
                        c = bit.GetPixel(i + 4, j - 4);
                        sumR += (c.R * kernel[8]);
                        sumG += (c.G * kernel[8]);
                        sumB += (c.B * kernel[8]);
                        blok += kernel[8];
                    }

                    ///

                    if (b10)
                    {
                        c = bit.GetPixel(i - 4, j - 3);
                        sumR += (c.R * kernel[9]);
                        sumG += (c.G * kernel[9]);
                        sumB += (c.B * kernel[9]);
                        blok += kernel[9];
                    }

                    if (b11)
                    {
                        c = bit.GetPixel(i - 3, j - 3);
                        sumR += (c.R * kernel[10]);
                        sumG += (c.G * kernel[10]);
                        sumB += (c.B * kernel[10]);
                        blok += kernel[10];
                    }

                    if (b12)
                    {
                        c = bit.GetPixel(i - 2, j - 3);
                        sumR += (c.R * kernel[11]);
                        sumG += (c.G * kernel[11]);
                        sumB += (c.B * kernel[11]);
                        blok += kernel[11];
                    }

                    if (b13)
                    {
                        c = bit.GetPixel(i - 1, j - 3);
                        sumR += (c.R * kernel[12]);
                        sumG += (c.G * kernel[12]);
                        sumB += (c.B * kernel[12]);
                        blok += kernel[12];
                    }

                    if (b14)
                    {
                        c = bit.GetPixel(i, j - 3);
                        sumR += (c.R * kernel[13]);
                        sumG += (c.G * kernel[13]);
                        sumB += (c.B * kernel[13]);
                        blok += kernel[13];
                    }

                    if (b15)
                    {
                        c = bit.GetPixel(i + 1, j - 3);
                        sumR += (c.R * kernel[14]);
                        sumG += (c.G * kernel[14]);
                        sumB += (c.B * kernel[14]);
                        blok += kernel[14];
                    }

                    if (b16)
                    {
                        c = bit.GetPixel(i + 2, j - 3);
                        sumR += (c.R * kernel[15]);
                        sumG += (c.G * kernel[15]);
                        sumB += (c.B * kernel[15]);
                        blok += kernel[15];
                    }

                    if (b17)
                    {
                        c = bit.GetPixel(i + 3, j - 3);
                        sumR += (c.R * kernel[16]);
                        sumG += (c.G * kernel[16]);
                        sumB += (c.B * kernel[16]);
                        blok += kernel[16];
                    }

                    if (b18)
                    {
                        c = bit.GetPixel(i + 4, j - 3);
                        sumR += (c.R * kernel[17]);
                        sumG += (c.G * kernel[17]);
                        sumB += (c.B * kernel[17]);
                        blok += kernel[17];
                    }

                    ///

                    if (b19)
                    {
                        c = bit.GetPixel(i - 4, j - 2);
                        sumR += (c.R * kernel[18]);
                        sumG += (c.G * kernel[18]);
                        sumB += (c.B * kernel[18]);
                        blok += kernel[18];
                    }

                    if (b20)
                    {
                        c = bit.GetPixel(i - 3, j - 2);
                        sumR += (c.R * kernel[19]);
                        sumG += (c.G * kernel[19]);
                        sumB += (c.B * kernel[19]);
                        blok += kernel[19];
                    }

                    if (b21)
                    {
                        c = bit.GetPixel(i - 2, j - 2);
                        sumR += (c.R * kernel[20]);
                        sumG += (c.G * kernel[20]);
                        sumB += (c.B * kernel[20]);
                        blok += kernel[20];
                    }

                    if (b22)
                    {
                        c = bit.GetPixel(i - 1, j - 2);
                        sumR += (c.R * kernel[21]);
                        sumG += (c.G * kernel[21]);
                        sumB += (c.B * kernel[21]);
                        blok += kernel[21];
                    }

                    if (b23)
                    {
                        c = bit.GetPixel(i, j - 2);
                        sumR += (c.R * kernel[22]);
                        sumG += (c.G * kernel[22]);
                        sumB += (c.B * kernel[22]);
                        blok += kernel[22];
                    }

                    if (b24)
                    {
                        c = bit.GetPixel(i + 1, j - 2);
                        sumR += (c.R * kernel[23]);
                        sumG += (c.G * kernel[23]);
                        sumB += (c.B * kernel[23]);
                        blok += kernel[23];
                    }

                    if (b25)
                    {
                        c = bit.GetPixel(i + 2, j - 2);
                        sumR += (c.R * kernel[24]);
                        sumG += (c.G * kernel[24]);
                        sumB += (c.B * kernel[24]);
                        blok += kernel[24];
                    }

                    if (b26)
                    {
                        c = bit.GetPixel(i + 3, j - 2);
                        sumR += (c.R * kernel[25]);
                        sumG += (c.G * kernel[25]);
                        sumB += (c.B * kernel[25]);
                        blok += kernel[25];
                    }

                    if (b27)
                    {
                        c = bit.GetPixel(i + 4, j - 2);
                        sumR += (c.R * kernel[26]);
                        sumG += (c.G * kernel[26]);
                        sumB += (c.B * kernel[26]);
                        blok += kernel[26];
                    }

                    ///

                    if (b28)
                    {
                        c = bit.GetPixel(i - 4, j - 1);
                        sumR += (c.R * kernel[27]);
                        sumG += (c.G * kernel[27]);
                        sumB += (c.B * kernel[27]);
                        blok += kernel[27];
                    }

                    if (b29)
                    {
                        c = bit.GetPixel(i - 3, j - 1);
                        sumR += (c.R * kernel[28]);
                        sumG += (c.G * kernel[28]);
                        sumB += (c.B * kernel[28]);
                        blok += kernel[28];
                    }

                    if (b30)
                    {
                        c = bit.GetPixel(i - 2, j - 1);
                        sumR += (c.R * kernel[29]);
                        sumG += (c.G * kernel[29]);
                        sumB += (c.B * kernel[29]);
                        blok += kernel[29];
                    }

                    if (b31)
                    {
                        c = bit.GetPixel(i - 1, j - 1);
                        sumR += (c.R * kernel[30]);
                        sumG += (c.G * kernel[30]);
                        sumB += (c.B * kernel[30]);
                        blok += kernel[30];
                    }

                    if (b32)
                    {
                        c = bit.GetPixel(i, j - 1);
                        sumR += (c.R * kernel[31]);
                        sumG += (c.G * kernel[31]);
                        sumB += (c.B * kernel[31]);
                        blok += kernel[31];
                    }

                    if (b33)
                    {
                        c = bit.GetPixel(i + 1, j - 1);
                        sumR += (c.R * kernel[32]);
                        sumG += (c.G * kernel[32]);
                        sumB += (c.B * kernel[32]);
                        blok += kernel[32];
                    }

                    if (b34)
                    {
                        c = bit.GetPixel(i + 2, j - 1);
                        sumR += (c.R * kernel[33]);
                        sumG += (c.G * kernel[33]);
                        sumB += (c.B * kernel[33]);
                        blok += kernel[33];
                    }

                    if (b35)
                    {
                        c = bit.GetPixel(i + 3, j - 1);
                        sumR += (c.R * kernel[34]);
                        sumG += (c.G * kernel[34]);
                        sumB += (c.B * kernel[34]);
                        blok += kernel[34];
                    }

                    if (b36)
                    {
                        c = bit.GetPixel(i + 4, j - 1);
                        sumR += (c.R * kernel[35]);
                        sumG += (c.G * kernel[35]);
                        sumB += (c.B * kernel[35]);
                        blok += kernel[35];
                    }

                    ///

                    if (b37)
                    {
                        c = bit.GetPixel(i - 4, j);
                        sumR += (c.R * kernel[36]);
                        sumG += (c.G * kernel[36]);
                        sumB += (c.B * kernel[36]);
                        blok += kernel[36];
                    }

                    if (b38)
                    {
                        c = bit.GetPixel(i - 3, j);
                        sumR += (c.R * kernel[37]);
                        sumG += (c.G * kernel[37]);
                        sumB += (c.B * kernel[37]);
                        blok += kernel[37];
                    }

                    if (b39)
                    {
                        c = bit.GetPixel(i - 2, j);
                        sumR += (c.R * kernel[38]);
                        sumG += (c.G * kernel[38]);
                        sumB += (c.B * kernel[38]);
                        blok += kernel[38];
                    }

                    if (b40)
                    {
                        c = bit.GetPixel(i - 1, j);
                        sumR += (c.R * kernel[39]);
                        sumG += (c.G * kernel[39]);
                        sumB += (c.B * kernel[39]);
                        blok += kernel[39];
                    }

                    if (b41)
                    {
                        c = bit.GetPixel(i, j);
                        sumR += (c.R * kernel[40]);
                        sumG += (c.G * kernel[40]);
                        sumB += (c.B * kernel[40]);
                        blok += kernel[40];
                    }

                    if (b42)
                    {
                        c = bit.GetPixel(i + 1, j);
                        sumR += (c.R * kernel[41]);
                        sumG += (c.G * kernel[41]);
                        sumB += (c.B * kernel[41]);
                        blok += kernel[41];
                    }

                    if (b43)
                    {
                        c = bit.GetPixel(i + 2, j);
                        sumR += (c.R * kernel[42]);
                        sumG += (c.G * kernel[42]);
                        sumB += (c.B * kernel[42]);
                        blok += kernel[42];
                    }

                    if (b44)
                    {
                        c = bit.GetPixel(i + 3, j);
                        sumR += (c.R * kernel[43]);
                        sumG += (c.G * kernel[43]);
                        sumB += (c.B * kernel[43]);
                        blok += kernel[43];
                    }

                    if (b45)
                    {
                        c = bit.GetPixel(i + 4, j);
                        sumR += (c.R * kernel[44]);
                        sumG += (c.G * kernel[44]);
                        sumB += (c.B * kernel[44]);
                        blok += kernel[44];
                    }

                    ///

                    if (b46)
                    {
                        c = bit.GetPixel(i - 4, j + 1);
                        sumR += (c.R * kernel[45]);
                        sumG += (c.G * kernel[45]);
                        sumB += (c.B * kernel[45]);
                        blok += kernel[45];
                    }

                    if (b47)
                    {
                        c = bit.GetPixel(i - 3, j + 1);
                        sumR += (c.R * kernel[46]);
                        sumG += (c.G * kernel[46]);
                        sumB += (c.B * kernel[46]);
                        blok += kernel[46];
                    }

                    if (b48)
                    {
                        c = bit.GetPixel(i - 2, j + 1);
                        sumR += (c.R * kernel[47]);
                        sumG += (c.G * kernel[47]);
                        sumB += (c.B * kernel[47]);
                        blok += kernel[47];
                    }

                    if (b49)
                    {
                        c = bit.GetPixel(i - 1, j + 1);
                        sumR += (c.R * kernel[48]);
                        sumG += (c.G * kernel[48]);
                        sumB += (c.B * kernel[48]);
                        blok += kernel[48];
                    }

                    if (b50)
                    {
                        c = bit.GetPixel(i, j + 1);
                        sumR += (c.R * kernel[49]);
                        sumG += (c.G * kernel[49]);
                        sumB += (c.B * kernel[49]);
                        blok += kernel[49];
                    }

                    if (b51)
                    {
                        c = bit.GetPixel(i + 1, j + 1);
                        sumR += (c.R * kernel[50]);
                        sumG += (c.G * kernel[50]);
                        sumB += (c.B * kernel[50]);
                        blok += kernel[50];
                    }

                    if (b52)
                    {
                        c = bit.GetPixel(i + 2, j + 1);
                        sumR += (c.R * kernel[51]);
                        sumG += (c.G * kernel[51]);
                        sumB += (c.B * kernel[51]);
                        blok += kernel[51];
                    }

                    if (b53)
                    {
                        c = bit.GetPixel(i + 3, j + 1);
                        sumR += (c.R * kernel[52]);
                        sumG += (c.G * kernel[52]);
                        sumB += (c.B * kernel[52]);
                        blok += kernel[52];
                    }

                    if (b54)
                    {
                        c = bit.GetPixel(i + 4, j + 1);
                        sumR += (c.R * kernel[53]);
                        sumG += (c.G * kernel[53]);
                        sumB += (c.B * kernel[53]);
                        blok += kernel[53];
                    }

                    ///

                    if (b55)
                    {
                        c = bit.GetPixel(i - 4, j + 2);
                        sumR += (c.R * kernel[54]);
                        sumG += (c.G * kernel[54]);
                        sumB += (c.B * kernel[54]);
                        blok += kernel[54];
                    }

                    if (b56)
                    {
                        c = bit.GetPixel(i - 3, j + 2);
                        sumR += (c.R * kernel[55]);
                        sumG += (c.G * kernel[55]);
                        sumB += (c.B * kernel[55]);
                        blok += kernel[55];
                    }

                    if (b57)
                    {
                        c = bit.GetPixel(i - 2, j + 2);
                        sumR += (c.R * kernel[56]);
                        sumG += (c.G * kernel[56]);
                        sumB += (c.B * kernel[56]);
                        blok += kernel[56];
                    }

                    if (b58)
                    {
                        c = bit.GetPixel(i - 1, j + 2);
                        sumR += (c.R * kernel[57]);
                        sumG += (c.G * kernel[57]);
                        sumB += (c.B * kernel[57]);
                        blok += kernel[57];
                    }

                    if (b59)
                    {
                        c = bit.GetPixel(i, j + 2);
                        sumR += (c.R * kernel[58]);
                        sumG += (c.G * kernel[58]);
                        sumB += (c.B * kernel[58]);
                        blok += kernel[58];
                    }

                    if (b60)
                    {
                        c = bit.GetPixel(i + 1, j + 2);
                        sumR += (c.R * kernel[59]);
                        sumG += (c.G * kernel[59]);
                        sumB += (c.B * kernel[59]);
                        blok += kernel[59];
                    }

                    if (b61)
                    {
                        c = bit.GetPixel(i + 2, j + 2);
                        sumR += (c.R * kernel[60]);
                        sumG += (c.G * kernel[60]);
                        sumB += (c.B * kernel[60]);
                        blok += kernel[60];
                    }

                    if (b62)
                    {
                        c = bit.GetPixel(i + 3, j + 2);
                        sumR += (c.R * kernel[61]);
                        sumG += (c.G * kernel[61]);
                        sumB += (c.B * kernel[61]);
                        blok += kernel[61];
                    }

                    if (b63)
                    {
                        c = bit.GetPixel(i + 4, j + 2);
                        sumR += (c.R * kernel[62]);
                        sumG += (c.G * kernel[62]);
                        sumB += (c.B * kernel[62]);
                        blok += kernel[62];
                    }

                    ///

                    if (b64)
                    {
                        c = bit.GetPixel(i - 4, j + 3);
                        sumR += (c.R * kernel[63]);
                        sumG += (c.G * kernel[63]);
                        sumB += (c.B * kernel[63]);
                        blok += kernel[63];
                    }

                    if (b65)
                    {
                        c = bit.GetPixel(i - 3, j + 3);
                        sumR += (c.R * kernel[64]);
                        sumG += (c.G * kernel[64]);
                        sumB += (c.B * kernel[64]);
                        blok += kernel[64];
                    }

                    if (b66)
                    {
                        c = bit.GetPixel(i - 2, j + 3);
                        sumR += (c.R * kernel[65]);
                        sumG += (c.G * kernel[65]);
                        sumB += (c.B * kernel[65]);
                        blok += kernel[65];
                    }

                    if (b67)
                    {
                        c = bit.GetPixel(i - 1, j + 3);
                        sumR += (c.R * kernel[66]);
                        sumG += (c.G * kernel[66]);
                        sumB += (c.B * kernel[66]);
                        blok += kernel[66];
                    }

                    if (b68)
                    {
                        c = bit.GetPixel(i, j + 3);
                        sumR += (c.R * kernel[67]);
                        sumG += (c.G * kernel[67]);
                        sumB += (c.B * kernel[67]);
                        blok += kernel[67];
                    }

                    if (b69)
                    {
                        c = bit.GetPixel(i + 1, j + 3);
                        sumR += (c.R * kernel[68]);
                        sumG += (c.G * kernel[68]);
                        sumB += (c.B * kernel[68]);
                        blok += kernel[68];
                    }

                    if (b70)
                    {
                        c = bit.GetPixel(i + 2, j + 3);
                        sumR += (c.R * kernel[69]);
                        sumG += (c.G * kernel[69]);
                        sumB += (c.B * kernel[69]);
                        blok += kernel[69];
                    }

                    if (b71)
                    {
                        c = bit.GetPixel(i + 3, j + 3);
                        sumR += (c.R * kernel[70]);
                        sumG += (c.G * kernel[70]);
                        sumB += (c.B * kernel[70]);
                        blok += kernel[70];
                    }

                    if (b72)
                    {
                        c = bit.GetPixel(i + 4, j + 3);
                        sumR += (c.R * kernel[71]);
                        sumG += (c.G * kernel[71]);
                        sumB += (c.B * kernel[71]);
                        blok += kernel[71];
                    }

                    ///

                    if (b73)
                    {
                        c = bit.GetPixel(i - 4, j + 4);
                        sumR += (c.R * kernel[72]);
                        sumG += (c.G * kernel[72]);
                        sumB += (c.B * kernel[72]);
                        blok += kernel[72];
                    }

                    if (b74)
                    {
                        c = bit.GetPixel(i - 3, j + 4);
                        sumR += (c.R * kernel[73]);
                        sumG += (c.G * kernel[73]);
                        sumB += (c.B * kernel[73]);
                        blok += kernel[73];
                    }

                    if (b75)
                    {
                        c = bit.GetPixel(i - 2, j + 4);
                        sumR += (c.R * kernel[74]);
                        sumG += (c.G * kernel[74]);
                        sumB += (c.B * kernel[74]);
                        blok += kernel[74];
                    }

                    if (b76)
                    {
                        c = bit.GetPixel(i - 1, j + 4);
                        sumR += (c.R * kernel[75]);
                        sumG += (c.G * kernel[75]);
                        sumB += (c.B * kernel[75]);
                        blok += kernel[75];
                    }

                    if (b77)
                    {
                        c = bit.GetPixel(i, j + 4);
                        sumR += (c.R * kernel[76]);
                        sumG += (c.G * kernel[76]);
                        sumB += (c.B * kernel[76]);
                        blok += kernel[76];
                    }

                    if (b78)
                    {
                        c = bit.GetPixel(i + 1, j + 4);
                        sumR += (c.R * kernel[77]);
                        sumG += (c.G * kernel[77]);
                        sumB += (c.B * kernel[77]);
                        blok += kernel[77];
                    }

                    if (b79)
                    {
                        c = bit.GetPixel(i + 2, j + 4);
                        sumR += (c.R * kernel[78]);
                        sumG += (c.G * kernel[78]);
                        sumB += (c.B * kernel[78]);
                        blok += kernel[78];
                    }

                    if (b80)
                    {
                        c = bit.GetPixel(i + 3, j + 4);
                        sumR += (c.R * kernel[79]);
                        sumG += (c.G * kernel[79]);
                        sumB += (c.B * kernel[79]);
                        blok += kernel[79];
                    }

                    if (b81)
                    {
                        c = bit.GetPixel(i + 4, j + 4);
                        sumR += (c.R * kernel[80]);
                        sumG += (c.G * kernel[80]);
                        sumB += (c.B * kernel[80]);
                        blok += kernel[80];
                    }

                    if (str.Equals(""))
                    {
                        r = blok != 0 ? (int)sumR / blok : (int)sumR;
                        g = blok != 0 ? (int)sumG / blok : (int)sumG;
                        b = blok != 0 ? (int)sumB / blok : (int)sumB;
                    }
                    else
                    {
                        if (str.Equals("erosi"))
                        {
                            r = erosi((int)sumR, blok);
                            g = erosi((int)sumG, blok);
                            b = erosi((int)sumB, blok);
                        }

                        if (str.Equals("dilasi"))
                        {
                            r = dilasi((int)sumR, blok);
                            g = dilasi((int)sumG, blok);
                            b = dilasi((int)sumB, blok);
                        }
                    }

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;
        }

        private int dilasi(int sumColor, int blok)
        {
            return sumColor == 0 ? 0 : 255;
        }

        private int erosi(int sumColor, int blok)
        {
            return sumColor == (255 * blok) ? 255 : 0;
        }

        //

        public Bitmap keAverageFilter5x5(Bitmap bit)
        {
            Bitmap copy = new Bitmap(bit);

            bool b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20, b21, b22, b23, b24, b25;
            Color c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20, c21, c22, c23, c24, c25;
            float sumR, sumG, sumB;

            int w = bit.Width - 1;
            int h = bit.Height - 1;

            kernel = new int[25] {
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
            };

            int blok = 0;

            prog.Visible = true;
            for (int i = 0; i <= w; i++)
            {
                for (int j = 0; j <= h; j++)
                {
                    b1 = b2 = b3 = b4 = b5 = false;
                    b6 = b7 = b8 = b9 = b10 = false;
                    b11 = b12 = b13 = b14 = b15 = false;
                    b16 = b17 = b18 = b19 = b20 = false;
                    b21 = b22 = b23 = b24 = b25 = false;

                    sumR = sumG = sumB = 0;
                    blok = 0;

                    // row 1
                    if (j == 0)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 2
                    else if (j == 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 3
                    else if (1 < j && j < h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                            b23 = b24 = b25 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                            b22 = b23 = b24 = b25 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                            b21 = b22 = b23 = b24 = b25 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                            b21 = b22 = b23 = b24 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                            b21 = b22 = b23 = true;
                        }
                    }

                    // row 4
                    else if (j == h - 1)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                            b18 = b19 = b20 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                            b17 = b18 = b19 = b20 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                            b16 = b17 = b18 = b19 = b20 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                            b16 = b17 = b18 = b19 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                            b16 = b17 = b18 = true;
                        }
                    }

                    // row 5
                    else if (j == h)
                    {
                        // column 1
                        if (i == 0)
                        {
                            b3 = b4 = b5 = true;
                            b8 = b9 = b10 = true;
                            b13 = b14 = b15 = true;
                        }

                        // column 2
                        else if (i == 1)
                        {
                            b2 = b3 = b4 = b5 = true;
                            b7 = b8 = b9 = b10 = true;
                            b12 = b13 = b14 = b15 = true;
                        }

                        // column 3
                        else if (1 < i && i < w - 1)
                        {
                            b1 = b2 = b3 = b4 = b5 = true;
                            b6 = b7 = b8 = b9 = b10 = true;
                            b11 = b12 = b13 = b14 = b15 = true;
                        }

                        // column 4
                        else if (i == w - 1)
                        {
                            b1 = b2 = b3 = b4 = true;
                            b6 = b7 = b8 = b9 = true;
                            b11 = b12 = b13 = b14 = true;
                        }

                        // column 5
                        else if (i == w)
                        {
                            b1 = b2 = b3 = true;
                            b6 = b7 = b8 = true;
                            b11 = b12 = b13 = true;
                        }
                    }

                    if (b1)
                    {
                        c1 = bit.GetPixel(i - 2, j - 2);
                        sumR += (c1.R * kernel[0]);
                        sumG += (c1.G * kernel[0]);
                        sumB += (c1.B * kernel[0]);
                        blok += kernel[0];
                    }

                    if (b2)
                    {
                        c2 = bit.GetPixel(i - 1, j - 2);
                        sumR += (c2.R * kernel[1]);
                        sumG += (c2.G * kernel[1]);
                        sumB += (c2.B * kernel[1]);
                        blok += kernel[1];
                    }

                    if (b3)
                    {
                        c3 = bit.GetPixel(i, j - 2);
                        sumR += (c3.R * kernel[2]);
                        sumG += (c3.G * kernel[2]);
                        sumB += (c3.B * kernel[2]);
                        blok += kernel[2];
                    }

                    if (b4)
                    {
                        c4 = bit.GetPixel(i + 1, j - 2);
                        sumR += (c4.R * kernel[3]);
                        sumG += (c4.G * kernel[3]);
                        sumB += (c4.B * kernel[3]);
                        blok += kernel[3];
                    }

                    if (b5)
                    {
                        c5 = bit.GetPixel(i + 2, j - 2);
                        sumR += (c5.R * kernel[4]);
                        sumG += (c5.G * kernel[4]);
                        sumB += (c5.B * kernel[4]);
                        blok += kernel[4];
                    }

                    if (b6)
                    {
                        c6 = bit.GetPixel(i - 2, j - 1);
                        sumR += (c6.R * kernel[5]);
                        sumG += (c6.G * kernel[5]);
                        sumB += (c6.B * kernel[5]);
                        blok += kernel[5];
                    }

                    if (b7)
                    {
                        c7 = bit.GetPixel(i - 1, j - 1);
                        sumR += (c7.R * kernel[6]);
                        sumG += (c7.G * kernel[6]);
                        sumB += (c7.B * kernel[6]);
                        blok += kernel[6];
                    }

                    if (b8)
                    {
                        c8 = bit.GetPixel(i, j - 1);
                        sumR += (c8.R * kernel[7]);
                        sumG += (c8.G * kernel[7]);
                        sumB += (c8.B * kernel[7]);
                        blok += kernel[7];
                    }

                    if (b9)
                    {
                        c9 = bit.GetPixel(i + 1, j - 1);
                        sumR += (c9.R * kernel[8]);
                        sumG += (c9.G * kernel[8]);
                        sumB += (c9.B * kernel[8]);
                        blok += kernel[8];
                    }

                    if (b10)
                    {
                        c10 = bit.GetPixel(i + 2, j - 1);
                        sumR += (c10.R * kernel[9]);
                        sumG += (c10.G * kernel[9]);
                        sumB += (c10.B * kernel[9]);
                        blok += kernel[9];
                    }

                    if (b11)
                    {
                        c11 = bit.GetPixel(i - 2, j);
                        sumR += (c11.R * kernel[10]);
                        sumG += (c11.G * kernel[10]);
                        sumB += (c11.B * kernel[10]);
                        blok += kernel[10];
                    }

                    if (b12)
                    {
                        c12 = bit.GetPixel(i - 1, j);
                        sumR += (c12.R * kernel[11]);
                        sumG += (c12.G * kernel[11]);
                        sumB += (c12.B * kernel[11]);
                        blok += kernel[11];
                    }

                    if (b13)
                    {
                        c13 = bit.GetPixel(i, j);
                        sumR += (c13.R * kernel[12]);
                        sumG += (c13.G * kernel[12]);
                        sumB += (c13.B * kernel[12]);
                        blok += kernel[12];
                    }

                    if (b14)
                    {
                        c14 = bit.GetPixel(i + 1, j);
                        sumR += (c14.R * kernel[13]);
                        sumG += (c14.G * kernel[13]);
                        sumB += (c14.B * kernel[13]);
                        blok += kernel[13];
                    }

                    if (b15)
                    {
                        c15 = bit.GetPixel(i + 2, j);
                        sumR += (c15.R * kernel[14]);
                        sumG += (c15.G * kernel[14]);
                        sumB += (c15.B * kernel[14]);
                        blok += kernel[14];
                    }

                    if (b16)
                    {
                        c16 = bit.GetPixel(i - 2, j + 1);
                        sumR += (c16.R * kernel[15]);
                        sumG += (c16.G * kernel[15]);
                        sumB += (c16.B * kernel[15]);
                        blok += kernel[15];
                    }

                    if (b17)
                    {
                        c17 = bit.GetPixel(i - 1, j + 1);
                        sumR += (c17.R * kernel[16]);
                        sumG += (c17.G * kernel[16]);
                        sumB += (c17.B * kernel[16]);
                        blok += kernel[16];
                    }

                    if (b18)
                    {
                        c18 = bit.GetPixel(i, j + 1);
                        sumR += (c18.R * kernel[17]);
                        sumG += (c18.G * kernel[17]);
                        sumB += (c18.B * kernel[17]);
                        blok += kernel[17];
                    }

                    if (b19)
                    {
                        c19 = bit.GetPixel(i + 1, j + 1);
                        sumR += (c19.R * kernel[18]);
                        sumG += (c19.G * kernel[18]);
                        sumB += (c19.B * kernel[18]);
                        blok += kernel[18];
                    }

                    if (b20)
                    {
                        c20 = bit.GetPixel(i + 2, j + 1);
                        sumR += (c20.R * kernel[19]);
                        sumG += (c20.G * kernel[19]);
                        sumB += (c20.B * kernel[19]);
                        blok += kernel[19];
                    }

                    if (b21)
                    {
                        c21 = bit.GetPixel(i - 2, j + 2);
                        sumR += (c21.R * kernel[20]);
                        sumG += (c21.G * kernel[20]);
                        sumB += (c21.B * kernel[20]);
                        blok += kernel[20];
                    }

                    if (b22)
                    {
                        c22 = bit.GetPixel(i - 1, j + 2);
                        sumR += (c22.R * kernel[21]);
                        sumG += (c22.G * kernel[21]);
                        sumB += (c22.B * kernel[21]);
                        blok += kernel[21];
                    }

                    if (b23)
                    {
                        c23 = bit.GetPixel(i, j + 2);
                        sumR += (c23.R * kernel[22]);
                        sumG += (c23.G * kernel[22]);
                        sumB += (c23.B * kernel[22]);
                        blok += kernel[22];
                    }

                    if (b24)
                    {
                        c24 = bit.GetPixel(i + 1, j + 2);
                        sumR += (c24.R * kernel[23]);
                        sumG += (c24.G * kernel[23]);
                        sumB += (c24.B * kernel[23]);
                        blok += kernel[23];
                    }

                    if (b25)
                    {
                        c25 = bit.GetPixel(i + 2, j + 2);
                        sumR += (c25.R * kernel[24]);
                        sumG += (c25.G * kernel[24]);
                        sumB += (c25.B * kernel[24]);
                        blok += kernel[24];
                    }

                    r = blok != 0 ? (int)sumR / blok : (int)sumR;
                    g = blok != 0 ? (int)sumG / blok : (int)sumG;
                    b = blok != 0 ? (int)sumB / blok : (int)sumB;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / bit.Width);
            }
            prog.Visible = false;

            return copy;

        }

        public Bitmap kePenambahan(Bitmap bit1, Bitmap bit2)
        {

            copy = new Bitmap(bit1);

            prog.Visible = true;
            for (int i = 0; i < copy.Width; i++)
            {
                for (int j = 0; j < copy.Height; j++)
                {
                    c1 = bit1.GetPixel(i, j);
                    c2 = bit2.GetPixel(i, j);

                    r = c1.R + c2.R;
                    g = c1.G + c2.G;
                    b = c1.B + c2.B;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / copy.Width);
            }
            prog.Visible = false;

            return copy;
        }

        public Bitmap kePengurangan(Bitmap bit1, Bitmap bit2)
        {
            copy = new Bitmap(bit1);

            prog.Visible = true;
            for (int i = 0; i < copy.Width; i++)
            {
                for (int j = 0; j < copy.Height; j++)
                {
                    c1 = bit1.GetPixel(i, j);
                    c2 = bit2.GetPixel(i, j);

                    r = c1.R - c2.R;
                    g = c1.G - c2.G;
                    b = c1.B - c2.B;

                    r = truncate(r);
                    g = truncate(g);
                    b = truncate(b);

                    copy.SetPixel(i, j, Color.FromArgb(r, g, b));
                }

                prog.Value = Convert.ToInt16(100 * (i + 1) / copy.Width);
            }
            prog.Visible = false;

            return copy;
        }

        //

        public Bitmap keAverageFilter3x3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLowPassFilter(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 4, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keHighPassFilter(Bitmap bit)
        {
            kernel = new int[9] {
                -1, 0, 1,
                -1, 0, 3,
                -3, 0, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keIdentity(Bitmap bit)
        {
            kernel = new int[9] {
                0, 0, 0,
                0, 1, 0,
                0, 0, 0
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLowEdgeDetection(Bitmap bit)
        {
            kernel = new int[9] {
                1, 0, -1,
                0, 0, 0,
                -1, 0, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keNormalEdgeDetection(Bitmap bit)
        {
            kernel = new int[9] {
                0, 1, 0,
                1, -4, 1,
                0, 1, 0
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keHighEdgeDetection(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -1, -1,
                -1, 8, -1,
                -1, -1, -1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keSharpen(Bitmap bit)
        {
            kernel = new int[9] {
                0, -1, 0,
                -1, 5, -1,
                0, -1, 0
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keGaussianBlur3x3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 2, 1,
                2, 4, 2,
                1, 2, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keGaussianBlur5x5(Bitmap bit)
        {
            kernel = new int[25] {
                1, 4, 6, 4, 1,
                4, 16, 24, 16, 4,
                6, 24, 36, 24, 6,
                4, 16, 24, 16, 4,
                1, 4, 6, 4, 1
            };

            return filter5x5(bit, kernel);
        }

        public Bitmap keUnsharpMasking5x5(Bitmap bit)
        {
            kernel = new int[25] {
                1, 4, 6, 4, 1,
                4, 16, 24, 16, 4,
                6, 24, -476, 24, 6,
                4, 16, 24, 16, 4,
                1, 4, 6, 4, 1
            };

            return filter5x5(bit, kernel);
        }

        public Bitmap keErosionCross3(Bitmap bit)
        {
            kernel = new int[9] {
                0, 1, 0,
                1, 1, 1,
                0, 1, 0
            };

            return filter3x3(bit, kernel, "erosi");
        }

        public Bitmap keErosionSquare3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel, "erosi");
        }

        public Bitmap keErosionSquare5(Bitmap bit)
        {
            kernel = new int[25] {
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
            };

            return filter5x5(bit, kernel, "erosi");
        }

        public Bitmap keDilationCross3(Bitmap bit)
        {
            kernel = new int[9] {
                0, 1, 0,
                1, 1, 1,
                0, 1, 0
            };

            return filter3x3(bit, kernel, "dilasi");
        }

        public Bitmap keDilationSquare3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel, "dilasi");
        }

        public Bitmap keDilationSquare5(Bitmap bit)
        {
            kernel = new int[25] {
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
                1, 1, 1, 1, 1,
            };

            return filter5x5(bit, kernel, "dilasi");
        }

        public Bitmap keErosionSquare9(Bitmap bit)
        {
            kernel = new int[81] {
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
            };

            return filter9x9(bit, kernel, "erosi");
        }

        public Bitmap keDilationSquare9(Bitmap bit)
        {
            kernel = new int[81] {
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
            };

            return filter9x9(bit, kernel, "dilasi");
        }

        public Bitmap keGrayscaleDilationSquare3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel, "grayscale_dilasi");
        }

        public Bitmap keGrayscaleErosionSquare3(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel, "grayscale_erosi");
        }

        public Bitmap kePointDetection(Bitmap bit)
        {
            kernel = new int[9] {
                1, 1, 1,
                1, -8, 1,
                1, 1, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLineDetectionPlus45(Bitmap bit)
        {
            kernel = new int[9] {
                2, -1, -1,
                -1, 2, -1,
                -1, -1, 2
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLineDetectionMinus45(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -1, 2,
                -1, 2, -1,
                2, -1, -1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLineDetectionHorizontal(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -1, -1,
                2, 2, 2,
                -1, -1, -1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keLineDetectionVertical(Bitmap bit)
        {
            kernel = new int[9] {
                -1, 2, -1,
                -1, 2, -1,
                -1, 2, -1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap kePrewittHorizontal(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -1, -1,
                0, 0, 0,
                1, 1, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap kePrewittVertical(Bitmap bit)
        {
            kernel = new int[9] {
                -1, 0, 1,
                -1, 0, 1,
                -1, 0, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap kePrewittMinus45(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -1, 0,
                -1, 0, 1,
                0, 1, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap kePrewittPlus45(Bitmap bit)
        {
            kernel = new int[9] {
                0, 1, 1,
                -1, 0, 1,
                -1, -1, 0
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keSobelHorizontal(Bitmap bit)
        {
            kernel = new int[9] {
                -1, -2, -1,
                0, 0, 0,
                1, 2, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keSobelVertical(Bitmap bit)
        {
            kernel = new int[9] {
                -1, 0, 1,
                -2, 0, 2,
                -1, 0, 1
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keSobelMinus45(Bitmap bit)
        {
            kernel = new int[9] {
                -2, -1, 0,
                -1, 0, 1,
                0, 1, 2
            };

            return filter3x3(bit, kernel);
        }

        public Bitmap keSobelPlus45(Bitmap bit)
        {
            kernel = new int[9] {
                0, 1, 2,
                -1, 0, 1,
                -2, -1, 0
            };

            return filter3x3(bit, kernel);
        }
    }
}
