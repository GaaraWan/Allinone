using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotoRs485.BaseSpace
{
    public class LS_ControlMotorClass : LS_MODBUS_MOTOR
    {
        public MOTORCONTROLINI mINI;

        /// <summary>
        /// 初始化马达
        /// </summary>
        /// <param name="COM_INIAddress">Com口INI地址</param>
        /// <param name="Motor_INIAddress">马达参数INI地址</param>
        /// <param name="isDebug">是否Debug</param>
        public LS_ControlMotorClass( string COM_INIAddress, string Motor_INIAddress, bool isDebug)
        {
            base.mSERIALPORT = new SERIALPORT(COM_INIAddress, isDebug);
            LOADING(Motor_INIAddress);
        }
        /// <summary>
        /// 初始化马达
        /// </summary>
        /// <param name="iPro">站号</param>
        /// <param name="motor">Com口 类</param>
        public LS_ControlMotorClass( SERIALPORT motor,string Motor_INIAddress)
        {
            base.mSERIALPORT = motor;
            LOADING(Motor_INIAddress);
        }
        void LOADING(string INIAddress)
        {
            mINI = new MOTORCONTROLINI(INIAddress);
            base.PRO = (byte)mINI.iPRO;
            base.Initial();
            base.MotorAcceleration = mINI.iMotorAcceleration;
            base.MotorDeceleration = mINI.iMotorDeceleration;
            base.MotorHomeAddSubtract = mINI.iMotorHomeAddSubtract;
            base.MotorHomeMechanicsSpeed = mINI.iMotorHomeMechanicsSpeed;
            base.MotorHomeSpeed = mINI.iMotorHomeSpeed;
            base.MotorQuickStopDeceleration = mINI.iMotorQuickStopDeceleration;
            base.MotorZeroOffset = mINI.iMotorZeroOffset;
            base.MotorObjectSpeed = mINI.iSpeedHight;
        }
        /// <summary>
        /// 马达模式切换(重写)
        /// </summary>
        public override MotorOperationModeEnum MotorOperationMODE
        {
            get
            {
                return base.MotorOperationMODE;
            }
            set
            {
                if (base.MotorOperationMODE == value)
                    return;

                return;
                switch (value)
                {
                    case MotorOperationModeEnum.PositionMODE:
                        int iPosition = base.MotorObjectrPosition;
                        int iSpeed = base.MotorObjectSpeed;
                     //   base.SETMOTORINITIAL();
                        base.MotorObjectrPosition = iPosition;
                        base.MotorObjectSpeed = iSpeed;
                        base.MotorOperationMODE = MotorOperationModeEnum.PositionMODE;
                        break;
                    case MotorOperationModeEnum.SpeedMODE:
                        Stop();
                        base.MotorOperationMODE = MotorOperationModeEnum.SpeedMODE;
                        break;
                    case MotorOperationModeEnum.HOMEMODE:
                        base.MotorOperationMODE = MotorOperationModeEnum.HOMEMODE;
                        base.MotorHomeMode = MotorHomeModeCodeEnum.Mode0;
                        base.MotorHomeSpeed = 100;
                        base.MotorHomeAddSubtract = 100;
                        base.MotorHomeMechanicsSpeed = 10;
                        base.MotorZeroOffset = 0;
                        break;
                }
            }
        }
        /// <summary>
        /// 回HOME
        /// </summary>
        public void Home()
        {
            MotorOperationMODE = MotorOperationModeEnum.HOMEMODE;
            base.RunHome();
        }
        /// <summary>
        /// 到定位
        /// </summary>
        /// <param name="iPosition"></param>
        public void Position(int iPosition)
        {
            MotorOperationMODE = MotorOperationModeEnum.PositionMODE;
            base.MotorObjectrPosition = iPosition;
           // base.Run();


        }
       
      
        /// <summary>
        /// 到定位点
        /// </summary>
        public void GoPosition()
        {
            //MotorOperationMODE = MotorOperationModeEnum.PositionMODE;
            //base.MotorObjectrPosition = mINI.iGoPosition;
            base.GoPosition();

           
        }
        /// <summary>
        /// 到待命位置
        /// </summary>
        public void ComeBack()
        {
            MotorOperationMODE = MotorOperationModeEnum.PositionMODE;
            base.MotorObjectrPosition = mINI.iComeBack;
            base.Run();
        }

    }
}
