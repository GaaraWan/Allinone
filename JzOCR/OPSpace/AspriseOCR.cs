﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Windows.Forms; // for dll browsing dialog

// Asprise C# VB.NET OCR SDK Library API. <a href='http://asprise.com/ocr/docs/html/?src=csharp_src' target='_blank'>Read the developer's guide here.</a>
namespace asprise_ocr_api
{
    /// <summary>
    /// Represents an Asprise OCR engine.
    /// <a href='http://asprise.com/ocr/docs/html/?src=csharp_src' target='_blank'>Read the developer's guide here.</a>
    /// </summary>
    public class AspriseOCR
    {
        private const string OCR_DLL_NAME_32 = "aocr.dll";
        private const string OCR_DLL_NAME_64 = "aocr_x64.dll";

        /// <summary> Highest speed, accuracy may suffer - default option </summary>
        public const String SPEED_FASTEST = "fastest";
        /// <summary> less speed, better accuracy </summary>
        public const String SPEED_FAST = "fast";
        /// <summary>lowest speed, best accuracy </summary>
        public const String SPEED_SLOW = "slow";

        /// <summary>Recognize  text </summary>
        public const String RECOGNIZE_TYPE_TEXT = "text";
        /// <summary>Recognize barcode </summary>
        public const String RECOGNIZE_TYPE_BARCODE = "barcode";
        /// <summary>Recognize both text and barcode </summary>
        public const String RECOGNIZE_TYPE_ALL = "all";

        /// <summary>Output recognition result as plain text </summary>
        public const String OUTPUT_FORMAT_PLAINTEXT = "text";
        /// <summary>Output recognition result in XML format with additional information if coordination, confidence, runtime, etc. </summary>
        public const String OUTPUT_FORMAT_XML = "xml";
        /// <summary>Output recognition result as searchable PDF </summary>
        public const String OUTPUT_FORMAT_PDF = "pdf";
        /// <summary>Output to editable format RTF (can be edited in MS Word) </summary>
        public const String OUTPUT_FORMAT_RTF = "rtf";

        // Common used languages
        /// <summary>eng (English) </summary>
        public const String LANGUAGE_ENG = "eng";
        /// <summary>spa (Spanish) </summary>
        public const String LANGUAGE_SPA = "spa";
        /// <summary>por (Portuguese)</summary>
        public const String LANGUAGE_POR = "por";
        /// <summary>deu (German) </summary>
        public const String LANGUAGE_DEU = "deu";
        /// <summary>fra (French) </summary>
        public const String LANGUAGE_FRA = "fra";
        // around 30 languages are supported - use their ISO 639 3-letter as the id

        // ------------------------ dictionary properties ------------------------

        /// <summary>set to 'true' to skip using the default built in dict. Default value: 'false' - can only be used for StartEngine</summary>
        public const String START_PROP_DICT_SKIP_BUILT_IN_DEFAULT = "START_PROP_DICT_SKIP_BUILT_IN_DEFAULT";

        /// <summary>set to 'true' to skip using all built-in dicts. Default value: 'false' - can only be used for StartEngine </summary>
        public const String START_PROP_DICT_SKIP_BUILT_IN_ALL = "START_PROP_DICT_SKIP_BUILT_IN_ALL";
        /// <summary>Path to your custom dictionary (words are separated using line breaks). Default value: null. - can only be used for StartEngine </summary>
        public const String START_PROP_DICT_CUSTOM_DICT_FILE = "START_PROP_DICT_CUSTOM_DICT_FILE";
        /// <summary>Path to your custom templates (templates are separated using line breaks). Default value: null. - can only be used for StartEngine </summary>
        public const String START_PROP_DICT_CUSTOM_TEMPLATES_FILE = "START_PROP_DICT_CUSTOM_TEMPLATES_FILE";

        /// <summary>Percentage measuring the importance of the dictionary (0: not at all; 100: extremely important; default: 10) </summary>
        public const String PROP_DICT_DICT_IMPORTANCE = "PROP_DICT_DICT_IMPORTANCE";

        // ------------------------ general options ------------------------
        /// <summary>Page type </summary>
        public const String PROP_PAGE_TYPE = "PROP_PAGE_TYPE";

        /// <summary>Page type value: auto </summary>
        public const String PROP_PAGE_TYPE_AUTO_DETECT = "auto";
        /// <summary>Page type value: a single block of text </summary>
        public const String PROP_PAGE_TYPE_SINGLE_BLOCK = "single_block";
        /// <summary>Page type value: a single column of text </summary>
        public const String PROP_PAGE_TYPE_SINGLE_COLUMN = "single_column";
        /// <summary>Page type value: a single line of text </summary>
        public const String PROP_PAGE_TYPE_SINGLE_LINE = "single_line";
        /// <summary>Page type value: a single word </summary>
        public const String PROP_PAGE_TYPE_SINGLE_WORD = "single_word";
        /// <summary>Page type value: a single char </summary>
        public const String PROP_PAGE_TYPE_SINGLE_CHARACTOR = "single_char";
        /// <summary>Page type value: scattered text </summary>
        public const String PROP_PAGE_TYPE_SCATTERED = "scattered";

        /// <summary>Limit charset to a set of predefined chars </summary>
        public const String PROP_LIMIT_TO_CHARSET = "PROP_LIMIT_TO_CHARSET";

        /// <summary>Set to 'true' to set the output level as word instead of the default, line. </summary>
        public const String PROP_OUTPUT_SEPARATE_WORDS = "PROP_OUTPUT_SEPARATE_WORDS";

        /// <summary>The DPI to be used to render the PDF file; default is 300 if not specified </summary>
        public const String PROP_INPUT_PDF_DPI = "PROP_INPUT_PDF_DPI";

        // ------------------------ Image pre-processing ------------------------
        /// <summary>Image pre-processing type </summary>
        public const String PROP_IMG_PREPROCESS_TYPE = "PROP_IMG_PREPROCESS_TYPE";
        /// <summary>Use system default </summary>
        public const String PROP_IMG_PREPROCESS_TYPE_DEFAULT = "default";
        /// <summary>Default + page orientation detection </summary>
        public const String PROP_IMG_PREPROCESS_TYPE_DEFAULT_WITH_ORIENTATION_DETECTION = "default_with_orientation_detection";
        /// <summary>Custom, need to set PROP_IMG_PREPROCESS_CUSTOM_CMDS </summary>
        public const String PROP_IMG_PREPROCESS_TYPE_CUSTOM = "custom";

        /// <summary>Custom mage pre-processing command </summary>
        public const String PROP_IMG_PREPROCESS_CUSTOM_CMDS = "PROP_IMG_PREPROCESS_CUSTOM_CMDS";

        // ------------------------ Table detection ------------------------
        /// <summary>table will be detected by default; set this property to true to skip detection. </summary>
        public const String PROP_TABLE_SKIP_DETECTION = "PROP_TABLE_SKIP_DETECTION";

        /// <summary>default is 31 if not specified </summary>
        public const String PROP_TABLE_MIN_SIDE_LENGTH = "PROP_TABLE_MIN_SIDE_LENGTH";

        /// <summary>Save intermediate images generated for debug purpose - don't specify or empty string to skip saving </summary>
        public const String PROP_SAVE_INTERMEDIATE_IMAGES_TO_DIR = "PROP_SAVE_INTERMEDIATE_IMAGES_TO_DIR";

        // ------------------------ PDF output specific ------------------------
        /// <summary>PDF output file - required for PDF output. Valid prop value: absolute path to the target output file. </summary>
        public const String PROP_PDF_OUTPUT_FILE = "PROP_PDF_OUTPUT_FILE";
        /// <summary>The DPI of the images or '0' to auto-detect. Optional. Valid prop value: 0(default: auto-detect), 300, 200, etc. </summary>
        public const String PROP_PDF_OUTPUT_IMAGE_DPI = "PROP_PDF_OUTPUT_IMAGE_DPI";

        /// <summary>Font to be used for PDF output. Optional. Valid values: "serif" (default), "sans". </summary>
        public const String PROP_PDF_OUTPUT_FONT = "PROP_PDF_OUTPUT_FONT";

        /// <summary>Make text visible - for debugging and analysis purpose. Optional. Valid prop values false(default), true. </summary>
        public const String PROP_PDF_OUTPUT_TEXT_VISIBLE = "PROP_PDF_OUTPUT_TEXT_VISIBLE";

        /// <summary>Convert images into black/white to reduce PDF output file size. Optional. Valid prop values: false(default), true. </summary>
        public const String PROP_PDF_OUTPUT_IMAGE_FORCE_BW = "PROP_PDF_OUTPUT_IMAGE_FORCE_BW";

        /// <summary>Set to 'text' or 'xml' to return information when the output format is PDF </summary>
        public const String PROP_PDF_OUTPUT_RETURN_TEXT = "PROP_PDF_OUTPUT_RETURN_TEXT";
        /// <summary>Return text </summary>
        public const String PROP_PDF_OUTPUT_RETURN_TEXT_FORMAT_PLAINTEXT = "text";
        /// <summary>Return xml </summary>
        public const String PROP_PDF_OUTPUT_RETURN_TEXT_FORMAT_XML = "xml";

        /// <summary>Set to true to output PDF/A instead of normal PDF. </summary>
        public const String PROP_PDF_OUTPUT_PDFA = "PROP_PDF_OUTPUT_PDFA";

        /// <summary>Optionally specifies path to the custom font to be embedded in PDF/A </summary>
        public const String PROP_PDF_OUTPUT_PDFA_FONT_FILE = "PROP_PDF_OUTPUT_PDFA_FONT_FILE";

        // ------------------------ RTF specific ------------------------
        /// <summary>RTF output file - required for RTF output. Valid prop value: absolute path to the target output file. </summary>
        public const String PROP_RTF_OUTPUT_FILE = "PROP_RTF_OUTPUT_FILE";
        /// <summary>default is LETTER, may set to A4. </summary>
        public const String PROP_RTF_PAPER_SIZE = "PROP_RTF_PAPER_SIZE";

        /// <summary>Return text in 'text' or 'xml' format when the output format is set to RTF. </summary>
        public const String PROP_RTF_OUTPUT_RETURN_TEXT = "PROP_RTF_OUTPUT_RETURN_TEXT";
        /// <summary>Return text </summary>
        public const String PROP_RTF_OUTPUT_RETURN_TEXT_FORMAT_PLAINTEXT = "text";
        /// <summary>Return xml </summary>
        public const String PROP_RTF_OUTPUT_RETURN_TEXT_FORMAT_XML = "xml";

        /// <summary>Do not change unless you are told so. </summary>
        public static String CONFIG_PROP_SEPARATOR = "|";
        /// <summary>Do not change unless you are told so. </summary>
        public static String CONFIG_PROP_KEY_VALUE_SEPARATOR = "=";

        /// <summary>Recognize all pages. </summary>
        public const int PAGES_ALL = -1;

        /// <summary>
        /// Unmanaged code access (32bit).
        /// </summary>
        private static class OcrDll32
        {
            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_version();

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern int com_asprise_ocr_setup(int queryOnly);

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_list_supported_langs();

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_start(string lang, string speed, string propSpec, string propSeparator, string propKeyValueSpeparator);

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_stop(Int64 handle);

            [DllImport(OCR_DLL_NAME_32, EntryPoint = "com_asprise_ocr_recognize")]
            public static extern IntPtr com_asprise_ocr_recognize(Int64 handle, string imgFiles, int pageIndex, int startX, int startY, int width, int height, string recognizeType, string outputFormat, string propSpec, string propSeparator, string propKeyValueSpeparator);

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_input_license(string licenseeName, string licenseCode);

            [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_util_delete(Int64 handle, bool isArray);
        }

        /// <summary>
        /// Unmanaged code access (64bit).
        /// </summary>
        private static class OcrDll64
        {
            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_version();

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern int com_asprise_ocr_setup(int queryOnly);

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_list_supported_langs();

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_start(string lang, string speed, string propSpec, string propSeparator, string propKeyValueSpeparator);

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_stop(Int64 handle);

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern IntPtr com_asprise_ocr_recognize(Int64 handle, string imgFiles, int pageIndex, int startX, int startY, int width, int height, string recognizeType, string outputFormat, string propSpec, string propSeparator, string propKeyValueSpeparator);

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_input_license(string licenseeName, string licenseCode);

            [DllImport(OCR_DLL_NAME_64, CharSet = CharSet.Ansi)]
            public static extern void com_asprise_ocr_util_delete(Int64 handle, bool isArray);
        }

        private IntPtr _handle = new IntPtr(0);

        /// <summary>
        /// Whether the OCR engine is currently running.
        /// </summary>
        public bool IsEngineRunning
        {
            get { return _handle.ToInt64() > 0; }
        }

        /// <summary>
        /// Starts the OCR engine; does nothing if the engine has already been started.
        /// </summary>
        /// <param name="lang">e.g., "eng"</param>
        /// <param name="speed">e.g., "fastest"</param>
        /// <param name="startProperties">property specifications, can be a single Dictionary object or inline specification in pairs. Valid property names are defined in this class, e.g., START_PROP_DICT_CUSTOM_DICT_FILE, etc.</param>
        public void StartEngine(string lang, string speed, params object[] startProperties)
        {
            Dictionary<string, string> dict = readProperties(startProperties);

            if (IsEngineRunning)
            {
                return;
            }
            if (lang == null || speed == null || lang.Trim().Length == 0 || speed.Trim().Length == 0)
            {
                throw new Exception("Invalid arguments.");
            }

            _handle = Is64BitProcess ?
                OcrDll64.com_asprise_ocr_start(lang, speed, dictToString(dict), CONFIG_PROP_SEPARATOR, CONFIG_PROP_KEY_VALUE_SEPARATOR) :
                OcrDll32.com_asprise_ocr_start(lang, speed, dictToString(dict), CONFIG_PROP_SEPARATOR, CONFIG_PROP_KEY_VALUE_SEPARATOR);

            if (_handle.ToInt64() < 0)
            {
                _handle = new IntPtr(0);
                throw new Exception("Failed to start engine. Error code: " + _handle.ToInt64());
            }
        }

        /// <summary>
        /// Stops the OCR engine; does nothing if it has already been stopped.
        /// </summary>
        public void StopEngine()
        {
            if (!IsEngineRunning)
            {
                return;
            }
            if (Is64BitProcess)
            {
                OcrDll64.com_asprise_ocr_stop(_handle.ToInt64());
            }
            else
            {
                OcrDll32.com_asprise_ocr_stop(_handle.ToInt64());
            }
        }

        private Thread threadDoingOCR;

        /// <summary>
        /// Performs OCR on the given input files.
        /// </summary>
        /// <param name="files">comma ',' separated image file path (JPEG, BMP, PNG, TIFF)</param>
        /// <param name="pageIndex">-1 for all pages or the specified page (first page is 1) for multi-page image format like TIFF</param>
        /// <param name="startX">-1 for whole page or the starting x coordinate of the specified region</param>
        /// <param name="startY">-1 for whole page or the starting y coordinate of the specified region</param>
        /// <param name="width">-1 for whole page or the width of the specified region</param>
        /// <param name="height">-1 for whole page or the height of the specified region</param>
        /// <param name="recognizeType">valid values: RECOGNIZE_TYPE_TEXT, RECOGNIZE_TYPE_BARCODE or RECOGNIZE_TYPE_ALL.</param>
        /// <param name="outputFormat">valid values: OUTPUT_FORMAT_PLAINTEXT, OUTPUT_FORMAT_XML, OUTPUT_FORMAT_PDF or OUTPUT_FORMAT_RTF.</param>
        /// <param name="additionalProperties">additional properties, can be a single Dictionary object or inline specification in pairs. Valid property names are defined in this class, e.g., PROP_INCLUDE_EMPTY_BLOCK, etc.</param>
        /// <returns>text (plain text, xml) recognized for OUTPUT_FORMAT_PLAINTEXT, OUTPUT_FORMAT_XML</returns>
        public string Recognize(string files, int pageIndex, int startX, int startY, int width, int height, string recognizeType, string outputFormat, params object[] additionalProperties)
        {
            if (threadDoingOCR != null)
            {
                throw new Exception("Currently " + threadDoingOCR + " is using this OCR engine. Please create multiple OCR engine instances for multi-threading. ");
            }

            Dictionary<string, string> dict = readProperties(additionalProperties);
            if (outputFormat.Equals(OUTPUT_FORMAT_PDF))
            {
                string pdfOutputFile = dict.ContainsKey(PROP_PDF_OUTPUT_FILE) ? dict[PROP_PDF_OUTPUT_FILE] : null;
                if (pdfOutputFile == null)
                {
                    throw new Exception("You must specify PDF output through property named: " + PROP_PDF_OUTPUT_FILE);
                }

                if (!dict.ContainsKey(PROP_OUTPUT_SEPARATE_WORDS))
                {
                    dict[PROP_OUTPUT_SEPARATE_WORDS] = "true"; // default as separate
                }
            }
            if (outputFormat.Equals(OUTPUT_FORMAT_RTF))
            {
                string rtfOutputFile = dict.ContainsKey(PROP_RTF_OUTPUT_FILE) ? dict[PROP_RTF_OUTPUT_FILE] : null;
                if (rtfOutputFile == null)
                {
                    throw new Exception("You must specify RTF output through property named: " + PROP_RTF_OUTPUT_FILE);
                }
            }

            try
            {
                threadDoingOCR = Thread.CurrentThread;

                IntPtr ptr = (Is64BitProcess ?
                    OcrDll64.com_asprise_ocr_recognize(_handle.ToInt64(), files, pageIndex, startX, startY, width, height, recognizeType, outputFormat, dictToString(dict), CONFIG_PROP_SEPARATOR, CONFIG_PROP_KEY_VALUE_SEPARATOR) :
                    OcrDll32.com_asprise_ocr_recognize(_handle.ToInt64(), files, pageIndex, startX, startY, width, height, recognizeType, outputFormat, dictToString(dict), CONFIG_PROP_SEPARATOR, CONFIG_PROP_KEY_VALUE_SEPARATOR)
                );

                string s = Marshal.PtrToStringAnsi(ptr);
                string sInUnicode = null;
                if (s != null && s.Length > 0 && ptr.ToInt64() > 0)
                {
                    sInUnicode = utf8ToUnicode(s);
                    // clean up
                    deleteC(ptr, true);
                }
                return sInUnicode;
            }
            finally
            {
                threadDoingOCR = null;
            }

        }
        internal static Dictionary<string, string> readProperties(Object[] propSpec)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (propSpec == null || propSpec.Length == 0 || (propSpec.Length == 1 && propSpec[0] == null))
            {
                // nothing to do.
            }
            else if (propSpec.Length == 1 && (propSpec[0] as String != null))
            {
                // parse properties
                dict = stringToDict((String)propSpec[0]);
            }
            else if (propSpec != null && propSpec.Length > 0 &&
              (propSpec[0] as Dictionary<string, string> != null))
            {
                foreach (KeyValuePair<string, string> pair in (Dictionary<string, string>)propSpec[0])
                {
                    if (pair.Key != null)
                    {
                        dict[pair.Key.ToString()] = pair.Value == null ? null : pair.Value.ToString();
                    }
                }
            }
            else if (propSpec != null && propSpec.Length > 0)
            {
                if (propSpec.Length % 2 == 1)
                {
                    throw new Exception("You must specify additional properties in key/value pair. Current length: " + propSpec.Length);
                }
                for (var p = 0; p < propSpec.Length; p += 2)
                {
                    string key = (string)propSpec[p];
                    object val = propSpec[p + 1];
                    if (key != null)
                    {
                        dict[key] = val == null ? "" : val.ToString();
                    }
                }
            }

            // validation 
            foreach (KeyValuePair<string, string> pair in dict)
            {
                if (pair.Key.Contains(CONFIG_PROP_KEY_VALUE_SEPARATOR))
                {
                    throw new Exception("Please change CONFIG_PROP_KEY_VALUE_SEPARATOR to a different value as \"" +
                        pair.Key + "\" contains \"" + CONFIG_PROP_KEY_VALUE_SEPARATOR + "\"");
                }
                if (pair.Value.Contains(CONFIG_PROP_SEPARATOR))
                {
                    throw new Exception("Please change CONFIG_PROP_SEPARATOR to a different value as \"" +
                            pair.Value + "\" contains \"" + CONFIG_PROP_SEPARATOR + "\"");
                }
            }
            return dict;
        }

        internal static Dictionary<string, string> stringToDict(String s)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (s == null || s.Trim().Length == 0)
            {
                return dict;
            }
            string[] props = s.Split(new string[] { CONFIG_PROP_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string prop in props)
            {
                string[] parts = prop.Split(new string[] { CONFIG_PROP_KEY_VALUE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    dict[parts[0]] = parts[1];
                }
            }
            return dict;
        }

        /// <summary>
        /// The library version.
        /// </summary>
        /// <returns>The library version.</returns>
        public static string GetLibraryVersion()
        {
            return Marshal.PtrToStringAnsi(Is64BitProcess ?
                OcrDll64.com_asprise_ocr_version() :
                OcrDll32.com_asprise_ocr_version());
        }

        /// <summary>
        /// Performs one-time setup; does nothing if setup has already been done.
        /// </summary>
        public static void SetUp()
        {
            string dllPath = loadDll();
            if (dllPath == null)
            {
                throw new SystemException("OCR dll not found. Please download the latest evaluation kit from asprise.com");
            }
            if (Is64BitProcess)
            {
                OcrDll64.com_asprise_ocr_setup(0);
            }
            else
            {
                OcrDll32.com_asprise_ocr_setup(0);
            }
        }

        /// <summary>
        /// Call this after setup is done; returns list of langs separated by ','
        /// </summary>
        /// <returns>The list of langs separated by ','</returns>
        public static string ListSupportedLangs()
        {
            return Marshal.PtrToStringAnsi(Is64BitProcess ?
                OcrDll64.com_asprise_ocr_list_supported_langs() :
                OcrDll32.com_asprise_ocr_list_supported_langs());
        }

        /// <summary>Input the license code </summary>
        /// <param name="licenseeName">Licensee name</param>
        /// <param name="licenseCode">License code</param>
        public static void InputLicense(string licenseeName, string licenseCode)
        {
            if (Is64BitProcess)
            {
                OcrDll64.com_asprise_ocr_input_license(licenseeName, licenseCode);
            }
            else
            {
                OcrDll32.com_asprise_ocr_input_license(licenseeName, licenseCode);
            }
        }

        /// <summary>Finds the OCR dll in system path or from bundle and return the path to the dll. </summary>
        public static string loadDll()
        {
            // 1. Search path.
            string dllFilePath = AspriseOCR.getOcrDllPath();
            if (dllFilePath == null)
            {
                // 2. Then parent folders
                string parentFolder = AspriseOCR.detectOcrDllInParentFolders();
                if (parentFolder != null)
                {
                    AspriseOCR.addToSystemPath(parentFolder);
                    // log("Folder containing ocr dll detected: " + parentFolder);
                }
            }
            dllFilePath = getOcrDllPath();
            if (dllFilePath != null)
            {
                return dllFilePath;
            }

            // 3. from DLL bundle
            string fromBundle = AspriseOCR.extractDllFromBundleTo(Directory.GetCurrentDirectory(), true);
            if (fromBundle != null)
            {
                // log("Using OCR dll from bundle: " + fromBundle);
            }

            return getOcrDllPath();
        }

        /// <summary>
        /// Search PATH and return the location of the ocr dll.
        /// </summary>
        /// <returns></returns>
        protected static string getOcrDllPath()
        {
            return searchFileInPath(getOcrDllName());
        }

        /// <summary>
        /// The simple name of the ocr dll file.
        /// </summary>
        /// <returns></returns>
        public static string getOcrDllName()
        {
            return Is64BitProcess ? OCR_DLL_NAME_64 : OCR_DLL_NAME_32;
        }

        /// <summary>
        /// Search the ancester directories and return the directory that contains ocr dll or null if not found.
        /// </summary>
        /// <returns></returns>
        private static string detectOcrDllInParentFolders()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            while (true)
            {
                if (File.Exists(Path.Combine(folder, getOcrDllName())))
                {
                    return folder;
                }
                else
                {
                    folder = Path.GetDirectoryName(folder);
                    if (folder == null)
                    {
                        break;
                    }
                }
            }
            return null;
        }

        private static string extractDllFromBundleTo(string dir, bool overwrite)
        {



            string bundleName = Is64BitProcess ? "asprise-ocr-dll-bundle-64" : "asprise-ocr-dll-bundle-32";
            Assembly bundleAssemly = null;
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = asm.GetName().Name;
                if (asm.GetName().Name == bundleName)
                {
                    bundleAssemly = asm;
                    break;
                }
            }

            if (bundleAssemly == null)
            {
                Console.Error.WriteLine("Assembly not loaded: " + bundleName);
                return null;
            }

            string resourceClassName = bundleName.Replace('-', '_') + ".Properties.Resources";
            Type typeResources = bundleAssemly.GetType(resourceClassName, false);
            if (typeResources == null)
            {
                Console.Error.WriteLine("OCR dll bundle not referenced: " + bundleName);
                return null;
            }
            MethodInfo[] methods = typeResources.GetMethods();
            MethodInfo methodAocrRes = null;
            string dllMd5 = null;
            for (int i = 0; methods != null && i < methods.Length; i++)
            {
                MethodInfo m = methods[i];
                if (m.Name.StartsWith("get_aocr"))
                {
                    methodAocrRes = m;
                    dllMd5 = m.Name.Substring(m.Name.LastIndexOf('_') + 1);
                    break;
                }
            }

            if (methodAocrRes == null)
            {
                Console.Error.WriteLine("OCR dll bundle not referenced, but unable to find resource in " + typeResources);
                return null;
            }

            string path = dir +
                ((dir.EndsWith("/") || dir.EndsWith("\\")) ? "" : "\\") +
              getOcrDllName();
            if (!overwrite && File.Exists(path))
            {
                return null;
            }

            File.WriteAllBytes(path, (byte[])methodAocrRes.Invoke(null, new object[0]));
            return bundleName;
        }

        /// <summary>
        /// Returns the absolute path of the first occurrence
        /// </summary>
        /// <param name="fileSimpleName"></param>
        /// <returns></returns>
        protected static string searchFileInPath(string fileSimpleName)
        {

            string path = getSystemPath();
            string[] folders = path.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            // insert current dir to folders
            string[] extended = new string[folders.Length + 1];
            extended[0] = Directory.GetCurrentDirectory();
            Array.Copy(folders, 0, extended, 1, folders.Length);
            folders = extended;
            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                folder = folder.Replace('/', '\\');
                if (!folder.EndsWith("\\"))
                {
                    folder += "\\";
                }
                string file = folder + fileSimpleName;
                if (File.Exists(file))
                {
                    return file;
                }
            }
            return null;
        }

        /// <summary>
        /// Running in 64bit mode?
        /// </summary>
        protected static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

        /// <summary>
        /// Performs native C/C++ delete
        /// </summary>
        /// <param name="ptr">pointer</param>
        /// <param name="isArray">whether delete []</param>
        protected static void deleteC(IntPtr ptr, bool isArray)
        {
            if (Is64BitProcess)
            {
                OcrDll64.com_asprise_ocr_util_delete(ptr.ToInt64(), isArray);
            }
            else
            {
                OcrDll32.com_asprise_ocr_util_delete(ptr.ToInt64(), isArray);
            }
        }

        /// <summary>
        /// Returns the system path
        /// </summary>
        /// <returns>System path</returns>
        protected static string getSystemPath()
        {
            return Environment.GetEnvironmentVariable("PATH");
        }

        /// <summary>
        /// Adds the given directory to the PATH variable.
        /// </summary>
        /// <param name="dir">The folder to be added to PATH</param>
        public static void addToSystemPath(string dir)
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dir);
        }

        public static string dictToString(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dict)
            {
                if (sb.Length > 0)
                {
                    sb.Append(CONFIG_PROP_SEPARATOR);
                }
                sb.Append(pair.Key);
                sb.Append(CONFIG_PROP_KEY_VALUE_SEPARATOR);
                sb.Append(pair.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the ToString() for non-null object or ""/"null" for null.
        /// </summary>
        /// <param name="obj">target object</param>
        /// <param name="nullAsEmpty">true to return "" for null; false "null"</param>
        /// <returns></returns>
        protected static string objectToString(object obj, bool nullAsEmpty = true)
        {
            if (obj == null)
            {
                return nullAsEmpty ? "" : "null";
            }
            return obj.ToString();
        }

        /// <summary>
        /// Returns the first non-null object or null if all arguments are null.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        protected static object firstNonNull(object o, params object[] others)
        {
            if (o != null)
            {
                return o;
            }

            for (var i = 0; others != null && i < others.Length; i++)
            {
                if (others[i] != null)
                {
                    return others[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Converts utf8 encoded string to unicode
        /// </summary>
        /// <param name="utf8String"></param>
        /// <returns></returns>
        protected static string utf8ToUnicode(string utf8String)
        {
            Encoding ansiEncoding = Encoding.GetEncoding(1252);

            byte[] utf8Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                utf8Bytes[i] = ansiEncoding.GetBytes(utf8String.Substring(i, 1))[0];
            }

            return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
        }

        /// <summary>return true if the given string is null or of lenght 0. </summary>
        public static bool isEmpty(string s)
        {
            return s == null || s.Trim().Length == 0;
        }

        /// <summary>save the aocr.xsl to the specified directory </summary>
        public static bool saveAocrXslTo(string dir, bool overwrite)
        {
            string path = dir +
                ((dir.EndsWith("/") || dir.EndsWith("\\")) ? "" : "\\") +
                "aocr.xsl";
            if (!overwrite && File.Exists(path))
            {
                return false;
            }

            string s = asprise_ocr_api.Properties.Resources.aocr_xsl;
            File.WriteAllText(path, s);

            return true;
        }
    }
}

namespace asprise_ocr_api.Properties
{
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("asprise_ocr_api.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;xsl:stylesheet version=&quot;1.0&quot; xmlns:xsl=&quot;http://www.w3.org/1999/XSL/Transform&quot;&gt;
        ///	&lt;xsl:output method=&apos;html&apos; version=&apos;1.0&apos; encoding=&apos;utf-8&apos; indent=&apos;yes&apos;/&gt;
        ///	&lt;xsl:template match=&quot;/asprise-ocr&quot;&gt;
        ///		&lt;HTML&gt;
        ///			&lt;HEAD&gt;
        ///				&lt;TITLE&gt;Asprise OCR Result of &lt;xsl:value-of select=&quot;@input&quot;/&gt;
        ///				&lt;/TITLE&gt;
        ///				&lt;style&gt;
        ///  body   {background-color:lightgray; font-family: arial;}
        ///  div.page   {background-color: white; border: outset 3px #666;}
        ///  div.pageNo {position: absolute; right [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string aocr_xsl
        {
            get
            {
                return ResourceManager.GetString("aocr_xsl", resourceCulture);
            }
        }
    }
}
