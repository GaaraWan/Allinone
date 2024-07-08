using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JzKHC
{
    enum CornerPosition
    {
        NONE = -1,
        LEFTTOP = 0,
        RIGHTTOP = 1,
        LEFTBOTTOM = 2,
        RIGTHBOTTOM = 3,
    }
    public enum TeachingTypeEnum : int
    {
        COUNT = 6,

        LINE = 0,
        BASE = 1,
        KEYCAP = 2,
        ORG = 3,
        ANALYZE = 4,
        BACKGROUD = 5,
    }

    public enum SideEnum : int
    {
        COUNT = 7,

        SIDE0 = 0,
        SIDE1 = 1,
        SIDE2 = 2,
        SIDE3 = 3,

        SIDE4 = 4,
        SIDE5 = 5,
        SIDE6 = 6,
        SIDE7 = 7,
        SIDE8 = 8,
        //SIDE9=9,
        //SIDE10=10,

    }
    public enum BesideEnum : int
    {
        COUNT = 4,

        LEFT = 0,
        RIGHT = 1,
        TOP = 2,
        BOTTOM = 3,

        NONE = -1,
    }
    public enum EditStatusEnum : int
    {
        NORMAL = 0,
        EDIT = 1,
        ADD = 2,
        COPY = 3,
        OPERATING = 4,
    }
    public enum DirectionEnum : int
    {
        VERTICAL = 0,
        HORIZONTAL = 1,
    }
    public enum OPTypeEnum : int
    {
        NONE = 0,
        SIMPLE = 1,
        MOVETOASSEMBLE = 2,
        GETKEYBOARDRANGE = 3,

        SELECT = 4,
        RESIZE = 5,
        MOVE = 6,

        CHECKSELECT = 7,
        CHECKRESIZE = 8,
        CHECKMOVE = 9,

        PTMOVE = 10,
    }
    public enum LoginStatusEnum : int
    {
        LOGOUT = 0,
        LOGIN = 1,
    }
    public enum OperationStatusEnum : int
    {
        PRODUCT = 0,
        RECIPE = 1,
        SETUP = 2,

        RECIPEEDIT = 3,
        SETUPEDIT = 4,
        RESULT = 5,
    }
    public enum ControlsEnum : int
    {
        COUNT = 13,

        LIGHT00 = 0,
        LIGHT01 = 1,
        LIGHT02 = 2,
        LIGHT03 = 3,
        LIGHT04 = 4,
        LIGHT05 = 5,
        LIGHT06 = 6,
        LIGHT07 = 7,
        LIGHT08 = 8,
        LIGHT09 = 9,
        LIGHT10 = 10,
        LIGHT11 = 11,
        LIGHT12 = 12,
    }
    public enum NoCheckEnum : int
    {
        COUNT = 4,

        NOCHECK_ARROUND = 0,
        NOCHECK_DUST = 1,
        NOCHECK_BIAS = 2,
        NOCHECK_SPEC = 3,
    }

    public enum GetMapModeEnum : int
    {
        UNDO = 0,
        TEACHNING = 1,
        ONLOAD = 2,
        CALCULATION = 3,
    }
    public enum EdgeDetecModeEnum : int
    {
        COUNT = 2,

        NONE = 0,
        BASIC = 1,
        SLOP = 2,
        HIGHTLIGHT = 3,
    }
    public enum TeachingControlEnum : int
    {
        ALIASNAME = 0,
        CONTRAST = 1,
        RESOLUTION = 2,
        EXAMDIFF = 3,
        STANDARDHEIGHT = 4,

        BASEHEIGHT0 = 5,
        BASEHEIGHT1 = 6,
        BASEHEIGHT2 = 7,
        BASEHEIGHT3 = 8,

        BESIDEDIFF = 9,
        EDGEINSIDE = 10,
        ISHORIZONTAL = 11,

        UPPERBOUND = 12,
        LOWERBOUND = 13,

        REPORTINDEX = 14,
    }
    public enum DegreeModeEnum : int
    {
        ORIGION = 0,
        LIVE = 1,
    }
    public enum ResonEnum : int
    {
        THISTIMEOK = -1,

        NOTFOUND = 0,
        STANDARDERROR = 1,
        SELFERROR = 2,
        NOFLAT = 3,
        CENTEROVER = 4,
        XOVER = 5,
        YOVER = 6,
        PLUSFLAT = 7,
        NEGFLAT = 8,
    }

    public enum AISYSActionEnum : int
    {
        NOTHING = -1,
        NORMAL_VIEW = 0,
        SINGLE_CAPTURE = 1,
        ALL_CAPTURE = 2,
    }
}
