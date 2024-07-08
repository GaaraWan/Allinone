using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace
{
    public partial class R5MotorControlUI : UserControl
    {
        List<R5ControlParameter> mParlist = new List<R5ControlParameter>();
        List<R5Control> mControllist = new List<R5Control>();
        public R5MotorControlUI()
        {
            InitializeComponent();
        }
        public void Initial(List<R5ControlParameter> mPar)
        {
            mParlist = mPar;

            for (int i = 0; i < mParlist.Count; i++)
            {
                R5Control MyControl = new R5Control();

                int iTemp = i / 2;
                if (i % 2 == 1)
                    MyControl.Location = new Point(3+337, iTemp * 78 + 3);
                else
                    MyControl.Location = new Point(3, iTemp * 78 + 3);
                
                MyControl.Name = "r5Control" + i;
                MyControl.Size = new System.Drawing.Size(337, 69);

                MyControl.Initial(mParlist[i]);

                this.Controls.Add(MyControl);

                mControllist.Add(MyControl);
            }
        }

        public void Tick()
        {
            foreach (R5Control cont in mControllist)
                cont.Tick();
        }
    }
}
