using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class OtherSetExposureForm : Form
    {
        public OtherSetExposureForm()
        {
            InitializeComponent();
            this.Load += OtherSetExposureForm_Load;
        }

        private void OtherSetExposureForm_Load(object sender, EventArgs e)
        {
            init();
        }

        Button btnKeycap;
        Button btnFrame;

        enum CAM
        {
            COUNT=5,
            CAM0=0,
            CAM1,
            CAM2,
            CAM3,
            CAM4,
        }
        NumericUpDown[] numkeycap = new NumericUpDown[(int)CAM.COUNT];
        NumericUpDown[] numframe = new NumericUpDown[(int)CAM.COUNT];

        void init()
        {
            btnFrame = button2;
            btnKeycap = button1;


            numkeycap[(int)CAM.CAM0] = numericUpDown1;
            numkeycap[(int)CAM.CAM1] = numericUpDown2;
            numkeycap[(int)CAM.CAM2] = numericUpDown3;
            numkeycap[(int)CAM.CAM3] = numericUpDown4;
            numkeycap[(int)CAM.CAM4] = numericUpDown5;

            numframe[(int)CAM.CAM0] = numericUpDown10;
            numframe[(int)CAM.CAM1] = numericUpDown9;
            numframe[(int)CAM.CAM2] = numericUpDown8;
            numframe[(int)CAM.CAM3] = numericUpDown7;
            numframe[(int)CAM.CAM4] = numericUpDown6;

            btnKeycap.Click += BtnKeycap_Click;
            btnFrame.Click += BtnFrame_Click;

            string[] strs = INI.KEYCAPEXPOSURE.Split(';');
            int i = 0;
            foreach(string s in strs)
            {
                numkeycap[i].Value = decimal.Parse(s.Split('#')[1]);
                i++;
            }
            strs = INI.FRAMEEXPOSURE.Split(';');
            i = 0;
            foreach (string s in strs)
            {
                numframe[i].Value = decimal.Parse(s.Split('#')[1]);
                i++;
            }
        }

        private void BtnFrame_Click(object sender, EventArgs e)
        {
            int i = 0;
            string str = "";
            while (i < (int)CAM.COUNT)
            {
                str += i.ToString() + "#" + numframe[i].Value.ToString() + ";";
                i++;
            }
            str = RemoveLastChar(str, 1);
            INI.FRAMEEXPOSURE = str;
            INI.Save();
            MessageBox.Show("Complete");
        }

        private void BtnKeycap_Click(object sender, EventArgs e)
        {
            int i = 0;
            string str = "";
            while (i < (int)CAM.COUNT)
            {
                str += i.ToString() + "#" + numkeycap[i].Value.ToString() + ";";
                i++;
            }
            str = RemoveLastChar(str, 1);
            INI.KEYCAPEXPOSURE = str;
            INI.Save();
            MessageBox.Show("Complete");
        }

        private string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }
    }
}
