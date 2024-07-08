using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

using JzKHC.ControlSpace;
using JzKHC.DBSpace;

using System.Drawing;
using System.Drawing.Imaging;

namespace JzKHC
{
    class Universal
    {
        public const string KHCCollectionPath = @"D:\JETEAZY\KHC";
        const string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + KHCCollectionPath + @"\DB\DATA.mdb;Jet OLEDB:Database Password=12892414;";
        const string LogcnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + KHCCollectionPath + @"\LOGDB\Template.mdb";

        public static OleDbConnection Datacn;

        public static OleDbConnection Logcn;

        public static string GlobalImageTypeString = ".png";
        public static ImageFormat GlobalImageFormat = ImageFormat.Png;

        public static int FindRegion = 520; //Find the Y Height
        public static int FindMinYPosition = -100;
        public const int AnalyzeDistance = 70;
        public static int GlobalPassInteger = 0;
        public static bool IsFindingBackward = true;

        public static string GlobalPassString = "";
        public static string GlobalPassLocation = "";

        public static string GlobalDBName = "";
        public static string GlobalProcessName = "";
        public static string GlobalSerialString = "";

        public const string PICPATH = KHCCollectionPath + @"\PICFX\";

        public static RecipeDBClass RECIPEDB;
        public static KeyboardClass KEYBOARD;
        public static CCDOfflineClass CCD;

        public static bool Initial()
        {
            bool ret = true;

            

            Logcn = new OleDbConnection(LogcnString);
            Datacn = new OleDbConnection(DATACNNSTRING);
            RECIPEDB = new RecipeDBClass("db_recipe", "rcp00", "rcp01", "RECIPE", Datacn);
            KEYBOARD = new KeyboardClass();
            CCD = new CCDOfflineClass();

            RECIPEDB.Initial();

            return ret;
        }
    }
}
