using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sharpen
{

    internal class olahCitra2
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

        public olahCitra2(ToolStripProgressBar prog)
        {
            this.prog = prog;
        }

        private int dilasi(int sumColor, int blok)
        {
            return sumColor == 0 ? 0 : 255;
        }

        private int erosi(int sumColor, int blok)
        {
            return sumColor == (255 * blok) ? 255 : 0;
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

        public Bitmap keSharpen(Bitmap bit)
        {
            kernel = new int[9] {
                0, -1, 0,
                -1, 5, -1,
                0, -1, 0
            };

            return filter3x3(bit, kernel);
        }
    }


}
