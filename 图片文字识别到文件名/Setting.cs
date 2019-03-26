using System;
using System.Windows.Forms;
using System.Resources;
using System.Xml;

namespace 图片文字识别到文件名
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }
        string respath = @".\Resource1.resx";
        private void Setting_Load(object sender, EventArgs e)
        {
            //读取资源文件的值

            using (ResXResourceSet rest = new ResXResourceSet(respath))
            {
                this.textCodeStart.Text = rest.GetString("Ocr_start_text");
                this.textX.Text = rest.GetString("x");
                this.textY.Text = rest.GetString("y");
                this.textH.Text = rest.GetString("h");
                this.textW.Text = rest.GetString("w");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(respath);
            XmlNodeList xnlist = xmlDoc.GetElementsByTagName("data");//这个data是固定
            foreach (XmlNode node in xnlist)
            {
                if (node.Attributes != null)
                {
                    if (node.Attributes["xml:space"].Value == "preserve")//这个preserve也是固定的
                    {
                        if (node.Attributes["name"].Value == "w")//String1是你想要编辑的
                        {
                            node.InnerText = this.textW.Text;//给他赋值就OK了
                        }
                        if (node.Attributes["name"].Value == "h")//String1是你想要编辑的
                        {
                            node.InnerText = this.textH.Text;//给他赋值就OK了
                        }
                        if (node.Attributes["name"].Value == "x")//String1是你想要编辑的
                        {
                            node.InnerText = this.textX.Text;//给他赋值就OK了
                        }
                        if (node.Attributes["name"].Value == "y")//String1是你想要编辑的
                        {
                            node.InnerText = this.textY.Text;//给他赋值就OK了
                        }
                        if (node.Attributes["name"].Value == "Ocr_start_text")//String1是你想要编辑的
                        {
                            node.InnerText = this.textCodeStart.Text;//给他赋值就OK了
                        }
                    }
                }
            }
            xmlDoc.Save(respath);//别忘记保存
            MessageBox.Show("保存成功！");
        }
    }
}
