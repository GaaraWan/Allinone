using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JzScreenPoints
{
    public enum CCDTYPEEnum
    {
        EPIX,
        TIS,
        TISUSB,
        IDS,
        PTG,
        FILE,
        AISYS,
        SERVER,
        USBCAM,
    }
    public enum LanguageEnum
    {
        COUNT = 10,

        UNIVERSAL = 0,
        MAINFORM = 1,
        ESSUI = 2,
        RCPUI = 3,
        RUNUI = 4,
        INIUI = 5,
        MAINUI=6,

        LOGINFORM = 7,
        ACCOUNTFORM = 8,
        RCPSELECTFORM = 9,
    }
    public enum ESSStatusEnum
    {
        EXIT,
        LOGIN,
        LOGOUT,
        LOGINCOMPLETE,

        ACCOUNTMANAGE,
        ACCOUNTMANAGECOMPLETE,

        RECIPESELECT,
        RECIPESELECTED,

        DISABLE,
        OPERATE,

        RUN,
        RECIPE,
        SETUP,

        EDIT,

        SHOPFLOORON,
        SHOPFLOOROFF,

        FASTCAL,

        LOAD,
        UNLOAD,
    }
    public enum INIStatusEnum
    {
        CHANGELANGUAGE,

        MODIFY,
        OK,
        CANCEL,
    }

    public enum RCPStatusEnum
    {   
        MODIFY,
        OK,
        CANCEL,
    }

    public enum RUNStatusEnum
    {
        START,
        STOP,

        RECIPESELECTED,
        RECIPENOTSELECTED,
    }
    public enum DBStatusEnum
    {
        ADD,
        MODIFY,
        NONE,
    }
    public enum ProcessEnum : int
    {
        COUNT = 4,

        PUTPRODUCT = 0,
        TAKEPRODUCT = 1,
        PUTBOX = 2,
        TAKEBOX = 3,
    }
    public enum OPTypeEnum
    {
        ASN,
        BAS,
        EHS,

        SIDE,
        SETUP,
    }
    public enum DisplayOPTypeEnum
    {
        NONE,
        SIMPLE,
        ADJUST,
        GETKEYBOARDRANGE,

        SELECT,
        RESIZE,
        MOVE,

        FIRST,
        SECOND,
        THIRD,

        CHECKSELECT,
        CHECKRESIZE,
        CHECKMOVE,

        PTMOVE,
    }

    public enum SideOPTypeEnum : int
    {
        COUNT = 3,

        BASIS = 0,
        ENHANCE = 1,
        RUN = 2,
    }
}
