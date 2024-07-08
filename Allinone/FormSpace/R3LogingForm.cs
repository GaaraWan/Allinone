using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class R3LoginForm : Form
    {
        TextBox tbLoadin;
        Button btnCancel;
        
        public R3LoginForm( )
        {
            InitializeComponent();
            this.Load += R3LogingForm_Load;
        }
        public R3LoginForm(string Msg)
        {
            InitializeComponent();
            this.Load += R3LogingForm_Load;
            this.Text = Msg;
            isPEOK = false;
        }

        private void R3LogingForm_Load(object sender, EventArgs e)
        {
            btnCancel = button1;
            tbLoadin = textBox1;

            btnCancel.Click += BtnCancel_Click;
            tbLoadin.Focus();
            tbLoadin.KeyUp += TbLoadin_KeyUp;

            this.FormClosed += R3LogingForm_FormClosed;
            this.CenterToParent();
        }

        private void R3LogingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
          //  DialogResult = DialogResult.Cancel;
        }

        bool isPEOK = false;
        private void TbLoadin_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode== Keys.Enter)
            {
                try
                {
                    if (Universal.IsNoUseCCD)
                    {
                        BypassDebug();
                        return;
                    }
                    string strmess = tbLoadin.Text.Replace(Environment.NewLine, "");
                    strmess = strmess.Trim();
                    strmess = DecodeBase64(Encoding.UTF8, strmess);
                    string[] strResult = strmess.Split('@');

                    if (strResult.Length > 1)
                    {
                        if (strResult[0] == "R3BYPASS")
                        {
                            JetEazy.BasicSpace.JzToolsClass tools = new JetEazy.BasicSpace.JzToolsClass();
                            string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 登入复判功能,登入者:" + strResult[1] + Environment.NewLine;
                            string strpath = "D:\\Log\\Retrial.log";

                            if (!System.IO.Directory.Exists("D:\\Log\\"))
                                System.IO.Directory.CreateDirectory("D:\\Log\\");


                            tools.SaveDataEX(strsavedata, strpath);

                            Universal.isR3ByPass = true;
                            Universal.watchR3RyPass.Reset();
                            Universal.watchR3RyPass.Start();
                            DialogResult = DialogResult.OK;
                            Universal.OnR3TickStop("1");
                            this.Close();
                            return;
                        }
                        if (strResult[0] == "PEBYPASS")
                        {
                            JetEazy.BasicSpace.JzToolsClass tools = new JetEazy.BasicSpace.JzToolsClass();
                            string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 登入复判功能,登入者:" + strResult[1] + Environment.NewLine;
                            string strpath = "D:\\Log\\Retrial.log";

                            if (!System.IO.Directory.Exists("D:\\Log\\"))
                                System.IO.Directory.CreateDirectory("D:\\Log\\");


                            tools.SaveDataEX(strsavedata, strpath);


                            this.Text = "请JET人员输入授权码!";
                            isPEOK = true;
                        }
                        if (isPEOK && strResult[0] == "JETBYPASS")
                        {
                            JetEazy.BasicSpace.JzToolsClass tools = new JetEazy.BasicSpace.JzToolsClass();
                            string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 登入复判功能,登入者:" + strResult[1] + Environment.NewLine;
                            string strpath = "D:\\Log\\Retrial.log";

                            if (!System.IO.Directory.Exists("D:\\Log\\"))
                                System.IO.Directory.CreateDirectory("D:\\Log\\");


                            tools.SaveDataEX(strsavedata, strpath);

                            Universal.isR3ByPass = true;
                            Universal.watchR3RyPass.Reset();
                            Universal.watchR3RyPass.Start();
                         
                            Universal.OnR3TickStop("1");
                            this.Close();
                            DialogResult = DialogResult.OK;
                            return;
                        }
                    }

                    tbLoadin.Text = "";
                    tbLoadin.Focus();

                }
                catch
                {
                    tbLoadin.Text = "";
                    tbLoadin.Focus();
                }
            }
        }

        private void BypassDebug()
        {
            JetEazy.BasicSpace.JzToolsClass tools = new JetEazy.BasicSpace.JzToolsClass();
            string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 登入复判功能,登入者:" +"Debug"+ Environment.NewLine;
            string strpath = "D:\\Log\\Retrial.log";

            if (!System.IO.Directory.Exists("D:\\Log\\"))
                System.IO.Directory.CreateDirectory("D:\\Log\\");


            tools.SaveDataEX(strsavedata, strpath);

            Universal.isR3ByPass = true;
            Universal.watchR3RyPass.Reset();
            Universal.watchR3RyPass.Start();
            DialogResult = DialogResult.OK;
            Universal.OnR3TickStop("1");
            this.Close();
            return;
        }


        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private  string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }

            return decode;
        }
    }
}
