using Allinone.ControlSpace.IOSpace;
using Allinone.ControlSpace.MachineSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.DBSpace;
using JetEazy.FormSpace;
using JetEazy.PlugSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.RUNUISpace
{
    public partial class RunMainX6JcetSFUI : UserControl
    {
        public RunMainX6JcetSFUI()
        {
            InitializeComponent();
        }

        IShopfloor m_ishopfloor;

        RcpDBClass RCPDB
        {
            get { return Universal.RCPDB; }
        }
        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }
        //FatekPLCClass PLC
        //{
        //    get { return Universal.PLC; }
        //}

        JzMainX6MachineClass MACHINE;

        JzMainX6IOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        TextBox txtlotNo;
        Label lblLoadRecipeState;
        Button btnClearCurrentRecord;
        Label lblMESStrip;
        Label lblInspectCurrentStrip;

        int iCurrentStrip = 0;
        int iMesStrip = 0;
        int StripBUFF = 0;

        bool m_SureClear = false;

        public void AddInspectCurrentStrip(bool isadd)
        {
            //只有PASS时 才加1
            if (isadd)
            {
                iCurrentStrip++;
                if (iCurrentStrip >= (iMesStrip + INI.JCET_STRIP_BUFF))
                {
                    PLCIO.Ready = false;

                    //PLC.JCETSTOPHANDLE = true;
                    lblLoadRecipeState.Text = "StopHandle";
                    //PLC.JCETSTOPHANDLE = false;

                    MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_NORMAL,
                        "StripQty=" + iMesStrip.ToString() +
                        ",BUFF=" + INI.JCET_STRIP_BUFF.ToString() +
                        ",StopHandle.");


                    m_SureClear = true;


                    //MessageBox.Show("已到達Strip數量，請確認。");
                    //OnTrigger(StatusEnum.JCET_CLEAR);
                    //if (DialogResult.OK == MessageBox.Show("已到達Strip數量，請確認。", "Shopfloor Sure", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    //{

                    //    txtlotNo.Text = "";
                    //    txtlotNo.Enabled = true;

                    //    iCurrentStrip = 0;
                    //    iMesStrip = 0;

                    //    lblLoadRecipeState.Text = "";
                    //    lblLoadRecipeState.BackColor = Control.DefaultBackColor;
                    //}
                }
            }
        }

        public void Init(JzMainX6MachineClass machine)
        {
            MACHINE = machine;

            txtlotNo = textBox1x;
            lblInspectCurrentStrip = label5x;
            lblLoadRecipeState = label6x;
            lblMESStrip = label4x;

            btnClearCurrentRecord = button1x;

            m_ishopfloor = new EzShopfloorClass();

            lblLoadRecipeState.Text = "";

            //txtlotNo.KeyDown += new KeyEventHandler(txtlotNo_KeyDown);
            txtlotNo.KeyPress += new KeyPressEventHandler(txtlotNo_KeyPress);
            btnClearCurrentRecord.Click += new EventHandler(btnClearCurrentRecord_Click);
        }

        LoginForm LOGINFRM;
        void btnClearCurrentRecord_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("是否要清除記錄，重新載入參數？", "Shopfloor Sure", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                LOGINFRM = new LoginForm(ACCDB, Universal.UIPATH, INI.LANGUAGE);

                if (LOGINFRM.ShowDialog() == DialogResult.OK)
                {
                    txtlotNo.Text = "";
                    //lblInspectCurrentStrip.Text = "0";
                    //lblMESStrip.Text = "0";

                    txtlotNo.Enabled = true;

                    iCurrentStrip = 0;
                    iMesStrip = 0;

                    lblLoadRecipeState.Text = "";
                    lblLoadRecipeState.BackColor = Control.DefaultBackColor;

                    PLCIO.Ready = false;

                    MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_NORMAL, "Clear All Record.");
                    OnTrigger(RunStatusEnum.JCET_CLEAR);
                }

                LOGINFRM.Dispose();


            }
        }

        void txtlotNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string strLotNo = txtlotNo.Text.Trim();
                MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_NORMAL, "LotNo=" + strLotNo);
                bool bOK = m_ishopfloor.GetRecipeFromLot(strLotNo, INI.JCET_WEBSERVICE_URL);
                if (bOK)
                {
                    //MES Strip
                    iMesStrip = m_ishopfloor.GetMESStrip();
                    //lblMESStrip.Text = m_ishopfloor.GetMESStrip().ToString();
                    
                    bool IsDifferentVersionX = RCPDB.FindName(m_ishopfloor.GetVersion()) == -1;
                    //IsDifferentVersionX = RCPDB.Index == 0;//選擇出系統參數則重新掃入
                    Universal.IsJcetChangeRecipe = false;
                    lblLoadRecipeState.Text = (IsDifferentVersionX ? "LD FAIL" : "LD PASS");
                    lblLoadRecipeState.BackColor = (IsDifferentVersionX ? Color.Red : Color.Lime);

                    PLCIO.Ready = !IsDifferentVersionX;

                    if (IsDifferentVersionX)
                    {
                        Universal.StripVersionName = "NULL";
                        string errormsg = "LotNo調用參數失敗，請查看。\r\n" + "Shopfloor中調用資料:" + m_ishopfloor.GetVersion();

                        MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_Exception, "Shopfloor code -2:" + errormsg);

                        MessageBox.Show(errormsg, "Shopfloor code -2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Universal.StripVersionName = m_ishopfloor.GetVersion();
                        Universal.IsJcetChangeRecipe = true;
                        txtlotNo.Enabled = false;
                        Universal.ShowMessage = m_ishopfloor.GetShowMessage();

                        MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_NORMAL, "OnTrigger:StatusEnum.JCET_CHANGE_RECIPE");
                        OnTrigger(RunStatusEnum.JCET_CHANGE_RECIPE);
                    }
                }
                else
                {
                    Universal.StripVersionName = "NULL";
                    string errormsg = "超時或未找到資料，請查看。";

                    MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_Exception, "Shopfloor code -1:" + errormsg);

                    MessageBox.Show(errormsg, "Shopfloor code -1", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public delegate void TriggerHandler(RunStatusEnum Status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RunStatusEnum Status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(Status);
            }
        }

        public void Tick()
        {
            lblInspectCurrentStrip.Text = iCurrentStrip.ToString();
            lblMESStrip.Text = iMesStrip.ToString();


            if (INI.JCET_IS_USE_SHOPFLOOR)
            {
                if (m_SureClear)
                {
                    m_SureClear = false;
                    if (DialogResult.OK == MessageBox.Show("已到達Strip數量，請確認。", "Shopfloor Sure", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {

                        txtlotNo.Text = "";
                        txtlotNo.Enabled = true;

                        iCurrentStrip = 0;
                        iMesStrip = 0;

                        lblLoadRecipeState.Text = "";
                        lblLoadRecipeState.BackColor = Control.DefaultBackColor;

                        OnTrigger(RunStatusEnum.JCET_CLEAR);
                    }
                }

                if (Universal.IsTimeKeepCheck)
                {
                    if (Universal.TimeCheckStop.minDuriation >= 3)
                    {
                        PLCIO.Ready = false;

                        MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_NORMAL,
                            "CurrentStripQty=" + iCurrentStrip.ToString() +
                            "StripQty=" + iMesStrip.ToString() +
                            ",BUFF=" + INI.JCET_STRIP_BUFF.ToString() +
                            ",TimeOver.");

                        Universal.IsTimeKeepCheck = false;

                        txtlotNo.Text = "";
                        txtlotNo.Enabled = true;

                        iCurrentStrip = 0;
                        iMesStrip = 0;

                        lblLoadRecipeState.Text = "";
                        lblLoadRecipeState.BackColor = Control.DefaultBackColor;

                        OnTrigger(RunStatusEnum.JCET_CLEAR);

                        MessageBox.Show(INI.JCET_TIMESTOP_SET + " 分鐘内無料停機清空。", "Shopfloor Time", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
            }

        }
    }
}
