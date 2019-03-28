using System;
//using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
//using System.Runtime.InteropServices;
//using System.Text;
using System.Threading;
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
        int panel2w;
        private void Form1_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
          //  cX = Convert.ToInt32(this.textX.Text);
           // cY = Convert.ToInt32(this.textY.Text);
           // cW = Convert.ToInt32(this.textW.Text);
          //  cH = Convert.ToInt32(this.textH.Text);
           // codestart = Convert.ToInt32(this.textCodeStart.Text);
            panel2w = this.panel2.Width;
            InitListView();
        }
        private void InitListView()
        {
            this.listView1.View = View.Details;
            this.listView1.FullRowSelect = true;
            this.listView1.Columns.Add("序号", 40, HorizontalAlignment.Left);
            this.listView1.Columns.Add("原始文件", 220, HorizontalAlignment.Left);
            this.listView1.Columns.Add("识别码", 100, HorizontalAlignment.Left);
            this.listView1.Columns.Add("耗时", 60, HorizontalAlignment.Left);
            this.listView1.Columns.Add("准确度", 60, HorizontalAlignment.Left);
        }
        public int cX, cY, cW, cH;
        string fn;
        string[] files = null;
        private string path = null;
        int codestart;
        bool needstop = false;
        Bitmap bigbitmap;
        //等比例缩放图片
        private Bitmap ZoomImage(Bitmap bitmap, int destHeight, int destWidth)
        {
            try
            {
                System.Drawing.Image sourImage = bitmap;
                int width = 0, height = 0;
                //按比例缩放           
                int sourWidth = sourImage.Width;
                int sourHeight = sourImage.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if ((sourWidth * destHeight) > (sourHeight * destWidth))
                    {
                        width = destWidth;
                        height = (destWidth * sourHeight) / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = (sourWidth * destHeight) / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                Bitmap destBitmap = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);
                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量     
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            catch
            {
                return bitmap;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fn = openFileDialog.FileName;
                this.label2.Text = fn;
                //获取用户选择文件的后缀名 
                string extension = Path.GetExtension(fn);
                if (extension == ".jpg")
                {
                    pictureBox1.Refresh();
                    pictureBox2.Refresh();
                    Thread thread1 = new Thread(() => orconce(fn));
                    thread1.Start();
                    Bitmap img = new Bitmap(fn);
                    //bigbitmap = img;
                    this.pictureBox2.Image = BlodBitmap(Cut(img, cX, cY, cW, cH));
                    this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;

                    //this.pictureBox1.Image=ZoomImage(img, this.pictureBox1.Height, this.pictureBox1.Width);


                    this.pictureBox1.Load(fn) ;

                    // Graphics g = pictureBox1.CreateGraphics();



                    // System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                    //Rectangle rect = new Rectangle();
                    //rect.Width = cW;
                    //rect.Height = cH;
                    //rect.X = cX;
                    //rect.Y = cY;

                    //gp.AddRectangle(rect);
                    //g.DrawPath(Pens.Blue, gp);
                    // g.DrawImage(ZoomImage(img,this.pictureBox1.Height,this.pictureBox1.Width), 0, 0);
                    // g.FillRectangle(Brushes.Red, new Rectangle(0, 10, 100, 100));

                    // this.pictureBox1.Load(fn) ;
                    // g.Dispose();

                    img.Dispose();
                    
                }
               
            }
        }
        public void showPic(string fn)
        {
            if (this.button6.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => {
                    this.button6.Enabled = false;
                    this.button7.Enabled = false;
                    this.button8.Enabled = false;
                    this.button9.Enabled = false;
                }));
            }
            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }
            this.pictureBox1.Load(fn);
            if (this.pictureBox2.Image != null)
            {
                this.pictureBox2.Image.Dispose();
                this.pictureBox2.Image = null;
            }
            var img = new Bitmap(fn);
            this.pictureBox2.Image = (Cut(img, cX, cY, cW, cH));
            if (this.pictureBox2.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => {
                    this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;
                }));
            }
            img.Dispose();
            if (this.button6.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => {
                    this.button6.Enabled = true;
                    this.button7.Enabled = true;
                    this.button8.Enabled = true;
                    this.button9.Enabled = true;
                }));
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = null;
            this.label1.Text = null;
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
        public Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
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
           // cX = Convert.ToInt32(this.textX.Text);
           // cY = Convert.ToInt32(this.textY.Text);
          //  cW = Convert.ToInt32(this.textW.Text);
           // cH = Convert.ToInt32(this.textH.Text);
          //  codestart = Convert.ToInt32(this.textCodeStart.Text);
            if (fn != null)
            {
                Bitmap bmp = new Bitmap(fn);
                this.pictureBox2.Image = GetBinaryzationImage1(Cut(bmp, cX, cY, cW, cH));
                this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;
            }

            Form test = Application.OpenForms["Setting"];  //查找是否打开过about窗体  
            if ((test == null) || (test.IsDisposed)) //如果没有打开过
            {
                Setting setting = new Setting();
                setting.Show();   //打开子窗体出来
            }
            else
            {
                test.Activate(); //如果已经打开过就让其获得焦点  
                test.WindowState = FormWindowState.Normal;//使Form恢复正常窗体大小
                

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
                this.textBox2.Text += path + "  " + "共" + this.label3.Text + "个jpg文件\r\n";
                if (filescount > 0)
                {
                    this.label4.Text = "0/" + filescount.ToString();
                    this.progressBar1.Maximum = filescount;//设置最大长度值
                    this.progressBar1.Value = 0;//设置当前值
                    this.progressBar1.Step = 1;//设置没次增长多少
                    currentnum = 0;
                    this.listView1.Items.Clear();
                    this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
                    for (int i = 0; i < filescount; i++)   //添加10行数据
                    {
                        ListViewItem lvi = new ListViewItem((i + 1).ToString());
                        //lvi.Text =(i+1).ToString();
                        lvi.SubItems.Add(files[i].Replace(path, ""));
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                        this.listView1.Items.Add(lvi);
                    }
                    this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
                    Thread thread = new Thread(() => showPic(files[currentnum]));
                    thread.Start();
                    Thread thread1 = new Thread(() => orconce(files[currentnum]));
                    thread1.Start();
                    this.label6.Text = "1";
                }
               
            }
        }
        public void orconce(string filename)
        {
            //识别图片文字
            var img = new Bitmap(filename);
            var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.Default);
            ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
            var page = ocr.Process(BlodBitmap(Cut(img, cX, cY, cW, cH)));
            string ocrtext = page.GetText().Replace(" ", "").Replace("\n", "");
            //using (var page = ocr.Process(Cut(img, 1869, 561, 1865, 661)))
            {
                if (this.textBox1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.textBox1.Text = ocrtext;
                    }));
                }
                else
                {
                    this.textBox1.Text = ocrtext;
                }
                page.Dispose();
                ocr.Dispose();
                img.Dispose();
            }
            int l = this.textBox1.Text.Length;
            if (l >= 10)
            {
                int start = this.textBox1.Text.IndexOf(codestart.ToString());
                if (start < 0)
                {
                    if (this.label1.InvokeRequired)//不同线程为true，所以这里是true
                    {
                        BeginInvoke(new Action(() =>
                        {
                            this.label1.Text = "无，50%，长度够，但没有找到关键字";
                        }));
                    }
                    else
                    {
                        this.label1.Text = "无，50%，长度够，但没有找到关键字";
                    }
                }
                else
                {
                    if (this.label1.InvokeRequired)//不同线程为true，所以这里是true
                    {
                        BeginInvoke(new Action(() => {
                            this.label1.Text = "订单编号：" + this.textBox1.Text.Substring(start, 10);
                        }));
                    }
                    else
                    {
                        this.label1.Text = "订单编号：" + this.textBox1.Text.Substring(start, 10);
                    }
                }
            }
            else
            {
                int start = this.textBox1.Text.IndexOf(codestart.ToString());
                if (start < 0)
                {
                    if (this.label1.InvokeRequired)//不同线程为true，所以这里是true
                    {
                        BeginInvoke(new Action(() => {
                            this.label1.Text = "无 0%，长度不够，也没有找到关键字";
                        }));
                    }
                    else
                    {
                        this.label1.Text = "无 0%，长度不够，也没有找到关键字";
                    }
                }
                else
                {
                    if (this.label1.InvokeRequired)//不同线程为true，所以这里是true
                    {
                        BeginInvoke(new Action(() => {
                            this.label1.Text = "无 70%，长度不够，但有关键字！！！！";
                        }));
                    }
                    else
                    {
                        this.label1.Text = "无 70%，长度不够，但有关键字！！！！";
                    }
                }
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
                --currentnum;
                if (currentnum < 0)
                {
                    currentnum = 0;
                    MessageBox.Show("到头了");
                    return;
                }
                this.label6.Text = (currentnum + 1).ToString();
                this.textBox1.Text = null;
                this.label1.Text = null;
                Thread thread = new Thread(() => showPic(files[currentnum]));
                thread.Start();
                Thread thread1 = new Thread(() => orconce(files[currentnum]));
                thread1.Start();
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                currentnum = 0;
                MessageBox.Show("到头了");
                this.label6.Text = (currentnum + 1).ToString();
                this.textBox1.Text = null;
                this.label1.Text = null;
                Thread thread = new Thread(() => showPic(files[currentnum]));
                thread.Start();
                Thread thread1 = new Thread(() => orconce(files[currentnum]));
                thread1.Start();
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                currentnum = files.Count() - 1;
                MessageBox.Show("到底了" + currentnum.ToString());
                this.label6.Text = (currentnum + 1).ToString();
                this.textBox1.Text = null;
                this.label1.Text = null;
                Thread thread = new Thread(() => showPic(files[currentnum]));
                thread.Start();
                Thread thread1 = new Thread(() => orconce(files[currentnum]));
                thread1.Start();
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.panel2.Left = (int)((this.pictureBox1.Width - panel2w) * 0.5);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            needstop = true;
        }
        //next
        private void button8_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                if (++currentnum > files.Count() - 1)
                {
                    currentnum = files.Count() - 1;
                    MessageBox.Show("到底了" + currentnum.ToString());
                    return;
                }
                this.label6.Text = (currentnum + 1).ToString();
                this.textBox1.Text = null;
                this.label1.Text = null;
                Thread thread = new Thread(() => showPic(files[currentnum]));
                thread.Start();
                Thread thread1 = new Thread(() => orconce(files[currentnum]));
                thread1.Start();
            }
            else
            {
                MessageBox.Show("文件未找到");
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListView.SelectedIndexCollection indexes = this.listView1.SelectedIndices;
                if (indexes.Count > 0)
                {

                    if (this.button8.Enabled)
                    {
                        int index = indexes[0];
                        currentnum = Convert.ToInt16(this.listView1.Items[index].SubItems[0].Text) - 1;//获取第一列的值
                        this.label6.Text = (currentnum + 1).ToString();
                        this.textBox1.Text = null;
                        this.label1.Text = null;
                        Thread thread = new Thread(() => showPic(files[currentnum]));
                        thread.Start();
                        Thread thread1 = new Thread(() => orconce(files[currentnum]));
                        thread1.Start();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        //激活更新
        private void Form1_Activated(object sender, EventArgs e)
        {
            string respath = @".\Resource1.resx";
            //读取资源文件的值
            using (ResXResourceSet rest = new ResXResourceSet(respath))
            {
                codestart = Convert.ToInt32(rest.GetString("Ocr_start_text"));
                cX = Convert.ToInt32(rest.GetString("x"));
                cY = Convert.ToInt32(rest.GetString("y"));
                cH = Convert.ToInt32(rest.GetString("h"));
                cW = Convert.ToInt32(rest.GetString("w"));
                this.label7.Text = "(" + codestart.ToString() + "," + cX.ToString() + "," + cY.ToString() + "," + cH.ToString() + "," + cW.ToString() + ")";
            }
            /*
            cX = Convert.ToInt32(this.textX.Text);
            cY = Convert.ToInt32(this.textY.Text);
            cW = Convert.ToInt32(this.textW.Text);
            cH = Convert.ToInt32(this.textH.Text);
            codestart = Convert.ToInt32(this.textCodeStart.Text);
            */
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
           // this.pictureBox1.Image = null;
            //this.pictureBox1.Image= ZoomImage(bigbitmap,this.pictureBox1.Height,this.pictureBox1.Width);
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
            int tempn = 0;//记录n
            for (int i = 0; i < n; i++)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                string ans = files[i].Replace(path, "");
                sw.Start();
                //识别图片文字
                var img = new Bitmap(files[i]);
                //显示
                if (this.pictureBox2.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        if (this.pictureBox2.Image!=null)
                        {
                            this.pictureBox2.Image.Dispose();
                        }
                        
                        this.pictureBox2.Height = (int)this.pictureBox2.Width * cH / cW;
                        this.pictureBox2.Image = (Cut(img, cX, cY, cW, cH));
                    }));
                }
                if (this.pictureBox1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        if (this.pictureBox1.Image != null)
                        {
                            this.pictureBox1.Image.Dispose();
                        }
                        this.pictureBox1.Load(files[i]);
                        this.label6.Text = (currentnum + 1).ToString();
                    }));
                }
                var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.TesseractOnly);
                ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
                var page = ocr.Process(BlodBitmap(Cut(img, cX, cY, cW, cH)));//100, 584, 3687, 713
                //var page = ocr.Process(Cut(img, 1875, 579, 1917, 693));
                string ocr_text = page.GetText().Replace(" ", "").Replace("\n", "");
                if (this.textBox1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.textBox1.Text = ocr_text;
                    }));
                }
                page.Dispose();
                img.Dispose();
                ocr.Dispose();
                int l = ocr_text.Length;
                int acc = 0;
                if (l >= 10)
                {
                    int start = ocr_text.IndexOf(codestart.ToString());
                    if (start < 0)//长度够，没有关键字
                    {
                        acc = 50;
                        ans += " >" + acc.ToString() + "%，编号：" + code1 + "_" + num.ToString();
                        //list.Add(filename + "-" + code);
                        code = code1 + "_" + num.ToString();
                        num++;
                    }
                    else
                    {
                        code = ocr_text.Substring(start, 10); //更新
                        code1 = code;
                        if (code.Length == 10)
                        {
                            acc = 100;//长度够，有关键字
                            ans += " >" + acc.ToString() + "% 编号：" + code;
                        }
                        else
                        {
                            acc = 90;//裁剪后长度不够，有关键字！！！
                            ans += " >" + acc.ToString() + "% 编号：" + code;
                        }
                        num = 0;
                        num++;
                    }
                }
                else
                {
                    int start = ocr_text.IndexOf(codestart.ToString());
                    if (start < 0)//长度不够，也没有找到关键字
                    {
                        acc = 0;
                        ans += " >" + acc.ToString() + "%，编号：" + code1 + "_" + num.ToString();
                        //list.Add(filename + "-" + code);
                        code = code1 + "_" + num.ToString();
                        num++;
                    }
                    else//长度不够，找到关键字
                    {
                        acc = 70;
                        ans += " >" + acc.ToString() + "%，编号：" + code1 + "_" + num.ToString();
                        //list.Add(filename + "-" + code);
                        code = code1 + "_" + num.ToString();
                        num++;
                    }
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
                    BeginInvoke(new Action(() => { this.label4.Text = (i + 1).ToString() + "/" + n.ToString(); }));
                }
                if (this.progressBar1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => { this.progressBar1.Value += this.progressBar1.Step; }));
                }
                dt.Rows.Add(files[i], code, dettime.ToString(), acc);
                Thread thread1 = new Thread(() => updatelistview(i, code, dettime.ToString(), acc.ToString()));
                thread1.Start();
                if (this.label1.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.label1.Text = code;
                    }));
                }
                /*
                if (this.label6.InvokeRequired)//不同线程为true，所以这里是true
                {
                    BeginInvoke(new Action(() => {
                        this.label6.Text = (currentnum + 1).ToString();
                    }));
                }
                */
                try
                {
                    if (dt.Rows[i]["acc"].ToString() == "100" || dt.Rows[i]["acc"].ToString() == "0")
                    {
                        System.IO.File.Copy(dt.Rows[i]["url"].ToString(), path + "/new/" + dt.Rows[i]["code"].ToString() + ".jpg", true);
                        File.AppendAllText(path + "/new/log.txt", "\r\n" + "复制" + dt.Rows[i]["url"].ToString().Replace(path, "") + " 到 " + dt.Rows[i]["code"].ToString() + ".jpg <" + dt.Rows[i]["acc"].ToString() + ">");
                    }
                    else
                    {
                        File.AppendAllText(path + "/new/log.txt", "\r\n" + "复制" + dt.Rows[i]["url"].ToString().Replace(path, "") + " 到 " + dt.Rows[i]["code"].ToString() + ".jpg失败 <" + dt.Rows[i]["acc"].ToString() + ">!!!");
                        needstop = true;
                    }
                }
                catch (Exception exp)
                {
                    File.AppendAllText(path + "/new/error.txt", "\r\n" + "复制" + dt.Rows[i]["url"].ToString().Replace(path, "") + " 到 " + dt.Rows[i]["code"].ToString() + ".jpg失败，" + exp.ToString());
                    needstop = true;
                }
                tempn = i + 1;
                //判断是否结束
                if (needstop)
                {
                    MessageBox.Show("识别出错，将结束剩下的操作！");
                    needstop = false;
                    break;
                }
                currentnum++;
            }
            if (this.button5.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button5.Enabled = true; }));
            }
            if (this.button7.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button7.Enabled = true; }));
            }
            if (this.button8.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button8.Enabled = true; }));
            }
            if (this.button6.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button6.Enabled = true; }));
            }
            if (this.button9.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.button9.Enabled = true; }));
            }
            //MessageBox.Show("识别完毕");
            sw1.Stop();
            TimeSpan ts3 = sw1.Elapsed;
            if (this.textBox2.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() => { this.textBox2.Text += "共耗时：" + (ts3.TotalMilliseconds / 1000.0).ToString("0.00") + "秒"; ; this.textBox2.SelectionStart = this.textBox2.TextLength; this.textBox2.ScrollToCaret(); }));
            }
            File.AppendAllText(path + "/new/log.txt", "\r\n" + "共耗时：" + (ts3.TotalMilliseconds / 1000.0).ToString("0.00") + "秒");
            MessageBox.Show("复制完毕");
            needstop = false;
            //回复listview
            
        }
        public void updatelistview(int i, string code, string time, string acc)
        {
            if (this.listView1.InvokeRequired)//不同线程为true，所以这里是true
            {
                BeginInvoke(new Action(() =>
                {
                    this.listView1.BeginUpdate();
                    this.listView1.Items[i].SubItems[2].Text = code;
                    this.listView1.Items[i].SubItems[3].Text = time;
                    this.listView1.Items[i].SubItems[4].Text = acc;
                    if (Convert.ToInt16(acc) < 100&& Convert.ToInt16(acc)!=0)
                    {
                        this.listView1.Items[i].BackColor = Color.Red;
                        this.listView1.Items[i].ForeColor = Color.White;
                    }
                    else
                    {
                        this.listView1.Items[i].BackColor = Color.Green;
                        this.listView1.Items[i].ForeColor = Color.White;
                    }
                    this.listView1.EndUpdate();
                    this.listView1.EnsureVisible(i);//滚动到指定的行位置
                }));
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            dt.Dispose();
            dt = new DataTable();
            dt.Columns.Add("url", Type.GetType("System.String"));
            dt.Columns.Add("code", Type.GetType("System.String"));
            dt.Columns.Add("time", Type.GetType("System.String"));
            dt.Columns.Add("acc", Type.GetType("System.String"));
            // var ocr = new TesseractEngine("./tessdata", "chi_sim");
            //list = new List<string>();
            // for (int i = 0; i < n; i++)
            {
                this.button5.Enabled = false;
                this.button6.Enabled = false;
                this.button7.Enabled = false;
                this.button8.Enabled = false;
                this.button9.Enabled = false;
                needstop = false;
                currentnum = 0;
                Task task = new Task(() => pic_orc());
                task.Start();
                // this.textBox2.Text += task.Result;
            }
        }
    }
}
