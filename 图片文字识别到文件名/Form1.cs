﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace 图片文字识别到文件名
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DataTable dt;
        private void Form1_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            cX = Convert.ToInt32(this.textX.Text);
            cY = Convert.ToInt32(this.textY.Text);
            cW = Convert.ToInt32(this.textW.Text);
            cH = Convert.ToInt32(this.textH.Text);
           
        }

        public int cX, cY, cW, cH;
        string fn;
        string[] files = null;
        private string path = null;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog=new OpenFileDialog();
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fn = openFileDialog.FileName;
                this.label2.Text = fn;
                //获取用户选择文件的后缀名 
                string extension = Path.GetExtension(fn);
                var img = new Bitmap(fn);
                //声明允许的后缀名 
                string[] str = new string[] { ".jpg", ".png" };
                this.pictureBox1.Load(fn);



            this.pictureBox2.Image = BlodBitmap(Cut(img, cX, cY, cW, cH));
                this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;
                
            }
        }

        public void showPic(string fn)
        {
            this.pictureBox2.Image = null;
            this.pictureBox1.Image = null;
            var img = new Bitmap(fn);
            this.pictureBox1.Load(fn);
            this.pictureBox2.Image = (Cut(img, cX, cY, cW, cH));
            this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;
            img.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            orconce(fn);

        }


        /// <summary>  
        /// 剪裁 -- 用GDI+   
        /// </summary>  
        /// <param name="b">原始Bitmap</param>  
        /// <param name="StartX">开始坐标X</param>  
        /// <param name="StartY">开始坐标Y</param>  
        /// <param name="iWidth">宽度</param>  
        /// <param name="iHeight">高度</param>  
        /// <returns>剪裁后的Bitmap</returns>  
        public  Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 平均值法获取二值化的阈值
        /// </summary>
        /// <param name="image">灰度图像</param>
        /// <returns></returns>
        private Bitmap GetBinaryzationImage1(Bitmap image)
        {
            Bitmap result = image.Clone() as Bitmap;




            Color color = new Color();
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    color = result.GetPixel(i, j);
                    if ((color.R + color.G + color.B) / 3 > 100)
                    {
                        result.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        result.SetPixel(i, j, Color.Black);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 柔化图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Bitmap BlodBitmap(Bitmap image)
        {
            int Height = image.Height;
            int Width = image.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Bitmap MyBitmap = image;
            Color pixel;
            //高斯模板
            int[] Gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            for (int x = 1; x < Width - 1; x++)
            for (int y = 1; y < Height - 1; y++)
            {
                int r = 0, g = 0, b = 0;
                int Index = 0;
                for (int col = -1; col <= 1; col++)
                for (int row = -1; row <= 1; row++)
                {
                    pixel = MyBitmap.GetPixel(x + row, y + col);
                    r += pixel.R * Gauss[Index];
                    g += pixel.G * Gauss[Index];
                    b += pixel.B * Gauss[Index];
                    Index++;
                }
                r /= 16;
                g /= 16;
                b /= 16;
                //处理颜色值溢出
                r = r > 255 ? 255 : r;
                r = r < 0 ? 0 : r;
                g = g > 255 ? 255 : g;
                g = g < 0 ? 0 : g;
                b = b > 255 ? 255 : b;
                b = b < 0 ? 0 : b;
                bitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
            }
            return bitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cX = Convert.ToInt32(this.textX.Text);
            cY = Convert.ToInt32(this.textY.Text);
            cW = Convert.ToInt32(this.textW.Text);
            cH = Convert.ToInt32(this.textH.Text);

            if (fn!=null)
            {
                Bitmap bmp = new Bitmap(fn);

                this.pictureBox2.Image = GetBinaryzationImage1(Cut(bmp, cX, cY, cW, cH));
                this.pictureBox2.Height = (int) this.pictureBox2.Width * cH / cW;
            }
           
        }

        private int currentnum;
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //定义打开的默认文件夹位置
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                files = null;
                label3.Text = "0";
                path = openFileDialog1.FileName.Replace(openFileDialog1.SafeFileName, "");
                files = Directory.GetFiles(path, "*.jpg");
                int filescount = files.Count();
                this.label3.Text = filescount.ToString();
                this.textBox2.Text += path + "  "+"共"+ this.label3.Text+"个jpg文件\r\n";
                this.label4.Text = "0/" + filescount.ToString();
                this.progressBar1.Maximum = filescount;//设置最大长度值
                this.progressBar1.Value = 0;//设置当前值
                this.progressBar1.Step = 1;//设置没次增长多少
                currentnum = 0;
                showPic(files[currentnum]);
                orconce(files[currentnum]);
                this.label6.Text = "1";
            }
        }

        public void orconce(string filename)
        {
            //识别图片文字
            var img = new Bitmap(filename);
            var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.Default);
            ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
            var page = ocr.Process(BlodBitmap(Cut(img, cX, cY, cW, cH)));

            //using (var page = ocr.Process(Cut(img, 1869, 561, 1865, 661)))
            {
                this.textBox1.Text = page.GetText().Replace(" ", "").Replace("\n","");
                page.Dispose();
                ocr.Dispose();
            }
            int l = this.textBox1.Text.Length;
            if (l >= 10)
            {
                int start = this.textBox1.Text.IndexOf("4");
                if (start < 0)
                {
                    this.label1.Text = "无";
                }
                else
                {
                    this.label1.Text = "订单编号：" + this.textBox1.Text.Substring(start, 10);
                }
            }
            else
            {
                this.label1.Text = "无";
            }

            /*
            //查找订单编号

            int start = this.textBox1.Text.IndexOf("订单编号");
            if (start < 0)
            {
                this.label1.Text = "无";
            }
            else
            {
                this.label1.Text = "订单编号：" + this.textBox1.Text.Substring(start + 5, 10).Replace("〇", "0");
            }
            */
        }

        private string code;//识别码
        private string code1;//识别码temp
        private int num = 0;//计数
        //last
        private void button7_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                if (--currentnum < 0)
                {
                    currentnum = 0;
                    MessageBox.Show("到头了");
                    return;
                }

                this.label6.Text = (currentnum + 1).ToString();
                showPic(files[currentnum]);
                orconce(files[currentnum]);
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
           
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (files!=null)
            {
                if (++currentnum > files.Count()-1)
                {
                    currentnum = files.Count();
                    MessageBox.Show("到底了");
                    return;
                }

                this.label6.Text = (currentnum+1).ToString();
                showPic(files[currentnum]);
                orconce(files[currentnum]);
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
        }

        public void pic_orc()
        {
            if (!Directory.Exists(path + "/new/"))
            {
                Directory.CreateDirectory(path + "/new/");

            }
            int n = files.Count();
          
            System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
            sw1.Start();
            for (int i = 0; i < n; i++)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                string ans = files[i].Replace(path,"");
               
                sw.Start();
                //识别图片文字
                var img = new Bitmap(files[i]);
                //显示
                if (this.pictureBox2.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.pictureBox2.Image = null; this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW; this.pictureBox2.Image = (Cut(img, cX, cY, cW, cH));

                    }));
                }

                if (this.pictureBox1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.pictureBox1.Image = null; this.pictureBox1.Load(files[i]);

                    }));
                }

                var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.TesseractOnly);
                ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
                var page = ocr.Process(BlodBitmap(Cut(img, cX, cY, cW, cH)));//100, 584, 3687, 713
                //var page = ocr.Process(Cut(img, 1875, 579, 1917, 693));
                string ocr_text = page.GetText().Replace(" ", "");
                page.Dispose();
                img.Dispose();
                ocr.Dispose();

                int l = ocr_text.Length;
                if (l >= 10)
                {
                    int start = ocr_text.IndexOf("4");
                    if (start < 0)
                    {
                        ans += " >?，编号：" + code1 + "_" + num.ToString();
                        //list.Add(filename + "-" + code);
                        code = code1 + "_" + num.ToString();
                        num++;
                    }
                    else
                    {
                        code = ocr_text.Substring(start, 10); //更新
                        code1 = code;
                        ans += " >编号：" + code;
                        num = 0;
                        num++;
                       
                    }
                }
                else
                {
                    ans += " >?，编号：" + code1 + "_" + num.ToString();
                    //list.Add(filename + "-" + code);
                    code = code1 + "_" + num.ToString();
                    num++;
                }

                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;

                string dettime = (ts2.TotalMilliseconds / 1000.0).ToString("0.00") + "秒";
                ans += " 耗时：" + dettime + "\r\n";
                ocr_text = null;
               
             
                if (this.textBox2.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.textBox2.Text += ans; this.textBox2.SelectionStart = this.textBox2.TextLength; this.textBox2.ScrollToCaret(); }));
                 }
                if (this.label4.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.label4.Text = (i+1).ToString()+"/"+n.ToString(); }));
                }
                if (this.progressBar1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.progressBar1.Value += this.progressBar1.Step ; }));
                }
                dt.Rows.Add(files[i], code, dettime.ToString());

                System.IO.File.Copy(dt.Rows[i]["url"].ToString(), path + "/new/" + dt.Rows[i]["code"].ToString() + ".jpg", true);
                File.AppendAllText(path + "/new/log.txt", "\r\n" + "复制" + dt.Rows[i]["url"].ToString().Replace(path,"") + " 到 " + dt.Rows[i]["code"].ToString() + ".jpg");
            }
            
           
            if (this.button5.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button5.Enabled = true; }));
            }

            //MessageBox.Show("识别完毕");
            sw1.Stop();
            TimeSpan ts3 = sw1.Elapsed;
            if (this.textBox2.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.textBox2.Text += "共耗时：" + (ts3.TotalMilliseconds / 1000.0).ToString("0.00") + "秒"; ; this.textBox2.SelectionStart = this.textBox2.TextLength; this.textBox2.ScrollToCaret(); }));
            }

            MessageBox.Show("复制完毕");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dt.Dispose();
            dt = new DataTable();
            dt.Columns.Add("url", Type.GetType("System.String"));
            dt.Columns.Add("code", Type.GetType("System.String"));
            dt.Columns.Add("time", Type.GetType("System.String"));

            
            // var ocr = new TesseractEngine("./tessdata", "chi_sim");
            //list = new List<string>();
           // for (int i = 0; i < n; i++)
           {
                this.button5.Enabled = false;
             
                Task  task = new Task (() => pic_orc());
                task.Start();
               // this.textBox2.Text += task.Result;
               
            }


        }

    }
}
