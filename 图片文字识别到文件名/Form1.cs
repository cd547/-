using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        }

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
                if (!str.Contains(extension))
                {
                    MessageBox.Show("仅能上传jpg,png格式的图片！");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //识别图片文字
            var img = new Bitmap(fn);
            var ocr = new TesseractEngine("./tessdata", "chi_sim",EngineMode.Default);
            var page = ocr.Process(Cut(img, 100, 584, 3687, 713));
             
            //using (var page = ocr.Process(Cut(img, 1869, 561, 1865, 661)))
            {
                this.textBox1.Text = page.GetText().Replace(" ", "");
                page.Dispose();
                ocr.Dispose();
            }

            //查找订单编号
            
            int start=this.textBox1.Text.IndexOf("订单编号");
            if (start < 0)
            {
                this.label1.Text = "无";
            }
            else
            {
                this.label1.Text = "订单编号：" + this.textBox1.Text.Substring(start + 5, 10);
            }
            
           
          


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

      

        private void button3_Click(object sender, EventArgs e)
        {
            if (fn!=null)
            {
                Bitmap bmp = new Bitmap(fn);

                this.pictureBox2.Image = Cut(bmp, 100, 584, 3687, 713);
                this.pictureBox2.Height = (int) 438 * 713 / 3687;
            }
           
        }

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
            }
        }


        private string code;//识别码
        private string code1;//识别码
        private int num = 0;
        public void pic_orc()
        {
            int n = files.Count();
            for (int i = 0; i < n; i++)
            {
                string ans = files[i];
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                //识别图片文字
                var img = new Bitmap(files[i]);
                var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.Default);

                var page = ocr.Process(Cut(img, 100, 584, 3687, 713));//100, 584, 3687, 713
                //var page = ocr.Process(Cut(img, 1875, 579, 1917, 693));
                string ocr_text = page.GetText().Replace(" ", "");
                page.Dispose();
                img.Dispose();
                ocr.Dispose();
                //查找订单编号

                int start = ocr_text.IndexOf("订单编号");
                if (start < 0)
                {
                    ans += " >>无，订单编号：" + code1 + "_" + num.ToString();
                    //list.Add(filename + "-" + code);
                    code = code1 + "_" + num.ToString();
                    num++;
                }
                else
                {
                    code = ocr_text.Substring(start + 5, 10);//更新
                    code1 = code;
                    ans += " >>订单编号：" + code;
                    num = 0;
                    num++;
                    //list.Add(filename+"-"+code);
                }
                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;

                string dettime = (ts2.TotalMilliseconds / 1000.0).ToString("0.00") + "秒";
                ans += " 耗时：" + dettime + "\r\n";
                ocr_text = null;
               
             
                if (this.textBox2.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.textBox2.Text += ans; }));
                 }
                if (this.label4.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.label4.Text = (i).ToString()+"/"+n.ToString(); }));
                }
                if (this.progressBar1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.progressBar1.Value += this.progressBar1.Step ; }));
                }
                dt.Rows.Add(files[i], code, dettime.ToString());
            }
            
           
            if (this.button5.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button5.Enabled = true; }));
            }
            if (this.button6.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button6.Enabled = true; }));
            }

            //MessageBox.Show("识别完毕");

            if (!Directory.Exists(path + "/new/"))
            {
                Directory.CreateDirectory(path + "/new/");

            }
            //复制，修改名称
            for (int i = 0; i < n; i++)
            {
                System.IO.File.Copy(dt.Rows[i]["url"].ToString(), path + "/new/" + dt.Rows[i]["code"].ToString() + ".jpg", true);
                File.AppendAllText(path + "/new/log.txt", "\r\n" + "复制图片" + dt.Rows[i]["url"].ToString().Replace(path,"") + "到/new/" + dt.Rows[i]["code"].ToString() + ".jpg");
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
                this.button6.Enabled = false;
                Task  task = new Task (() => pic_orc());
                task.Start();
               // this.textBox2.Text += task.Result;
               
            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(path + "/new/"))
            {
                Directory.CreateDirectory(path + "/new/");

            }
            int n = files.Count();
            //复制，修改名称
            for (int i = 0; i < n; i++)
            {
                System.IO.File.Copy(dt.Rows[i]["url"].ToString(), path + "/new/" + dt.Rows[i]["code"].ToString() + ".jpg", true);
                File.AppendAllText(path + "/new/log.txt", "\r\n" + "复制图片" + dt.Rows[i]["url"].ToString() + "到new/" + dt.Rows[i]["code"].ToString() + ".jpg");
            }
            MessageBox.Show("复制完毕");
        }
    }
}
