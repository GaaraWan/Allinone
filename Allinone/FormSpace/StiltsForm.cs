using Allinone.UISpace;
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
    public partial class StiltsForm : Form
    {

     public   StiltsUI stiltsUI;
        public StiltsForm()
        {
            InitializeComponent();

            stiltsUI = stiltsUI1;

            this.CenterToParent();
        }
    }
}
