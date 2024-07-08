using System;
using System.Drawing;
using OpenCvSharp;
using System.Collections.Generic;

namespace JetEazy.BasicSpace
{
    public class CAoiCalibration : IDisposable
    {
        const float WorldScale = 1000f;
        const float ViewScale = 5000f;

        //const float WorldScale = 1f;
        //const float ViewScale = 1f;
        JzToolsClass JzTools = new JzToolsClass();

        #region PRIVATE_DATA
            private int m_iRows = 3;
            private int m_iCols = 3;
            private int iLoadCurrent = 0;
            private PointF[,] m_pointsInWorld;
            private PointF[,] m_pointsInView;
            private CvMat[,] m_matriceW2V;      // World to View
            private CvMat[,] m_matriceV2W;      // View to World
            private string SaveAlldata = "";
            private string LoadAlldata = "";
            private List<string> LoadList = new List<string>();
        #endregion

        public CAoiCalibration()
        {
            _initArrays();
        }
        public CAoiCalibration(CAoiCalibration src)
        {
            if (src == null)
            {
                _initArrays();
                return;
            }

            m_iRows = src.m_iRows;
            m_iCols = src.m_iCols;

            _initArrays();

            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    m_pointsInWorld[i, j] = src.m_pointsInWorld[i, j];
                    m_pointsInView[i, j] = src.m_pointsInView[i, j];
                }
            }

            for (int i = 0; i < m_iRows - 1; i++)
            {
                for (int j = 0; j < m_iCols - 1; j++)
                {
                    if (src.m_matriceW2V[i, j] != null)
                    {
                        JetEazy.QUtilities.QUtility.SafeDisposeObject(m_matriceW2V[i, j]);
                        m_matriceW2V[i, j] = src.m_matriceW2V[i, j].Clone();
                    }
                    if (src.m_matriceV2W[i, j] != null)
                    {
                        JetEazy.QUtilities.QUtility.SafeDisposeObject(m_matriceV2W[i, j]);
                        m_matriceV2W[i, j] = src.m_matriceV2W[i, j].Clone();
                    }
                }
            }
        }
        public CAoiCalibration Clone()
        {
            CAoiCalibration obj = new CAoiCalibration(this);
            return obj;
        }

        public void Dispose()
        {
            _disposeMatrice();
        }
        public void SetCalibrationPoints(PointF[,] pointsInView, PointF[,] pointsInWorld)
        {
            m_iRows = Math.Min(pointsInView.GetUpperBound(0), pointsInWorld.GetUpperBound(0)) + 1;
            m_iCols = Math.Min(pointsInView.GetUpperBound(1), pointsInWorld.GetUpperBound(1)) + 1;

            _initArrays();

            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    m_pointsInWorld[i, j] = new PointF(pointsInWorld[i, j].X / WorldScale, pointsInWorld[i, j].Y / WorldScale);
                    m_pointsInView[i, j] = new PointF(pointsInView[i, j].X / ViewScale, pointsInView[i, j].Y / ViewScale);
                }
            }
        }
        public void SetCalibrationPoints(int iRow, int iCol, PointF pointInView, PointF pointInWorld)
        {
            if (iRow < 0 || iRow >= m_iRows || iCol < 0 || iCol >= m_iCols)
                return;
            m_pointsInView[iRow, iCol] = new PointF(pointInView.X / ViewScale, pointInView.Y / ViewScale);
            m_pointsInWorld[iRow, iCol] = new PointF(pointInWorld.X / WorldScale, pointInWorld.Y / WorldScale);
        }
        public bool CalculateTransformMatrix()
        {
            //	vx0 = |m11 m12 m13 m14|  [1 wx0 wy0 wxy0]t
            //	vx1 = |m11 m12 m13 m14|  [1 wx1 wy1 wxy1]t
            //	vx2 = |m11 m12 m13 m14|  [1 wx2 wy2 wxy2]t
            //	vx3 = |m11 m12 m13 m14|  [1 wx3 wy3 wxy3]t

            //	vy0 = |m21 m22 m23 m24|  [1 x0 y0 xy0]t
            //	vy1 = |m21 m22 m23 m24|  [1 x1 y1 xy1]t
            //	vy2 = |m21 m22 m23 m24|  [1 x2 y2 xy2]t
            //	vy3 = |m21 m22 m23 m24|  [1 x3 y3 xy3]t
 
            //	| 1 wx0 wy0 wxy0 | m11 |   | vx0 |
            //	| 1 wx1 wy1 wxy1 | m12 |   | vx1 |
            //	| 1 wx2 wy2 wxy2 | m13 | = | vx2 | 
            //	| 1 wx3 wy3 wxy3 | m14 |   | vx3 |

            //	| 1 wx0 wy0 wxy0 | m21 |   | vy0 |
            //	| 1 wx1 wy1 wxy1 | m22 |   | vy1 |
            //	| 1 wx2 wy2 wxy2 | m23 | = | vy2 | 
            //	| 1 wx3 wy3 wxy3 | m24 |   | vy3 |

            try
            {
                for (int i = 0; i < m_iRows - 1; i++)
                {
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        PointF[] ptsV = new PointF[] { 
                            m_pointsInView[i, j],
                            m_pointsInView[i, j+1],
                            m_pointsInView[i+1, j+1],
                            m_pointsInView[i+1, j]
                        };

                        PointF[] ptsW = new PointF[] { 
                            m_pointsInWorld[i, j],
                            m_pointsInWorld[i, j+1],
                            m_pointsInWorld[i+1, j+1],
                            m_pointsInWorld[i+1, j]
                        };

                        JetEazy.QUtilities.QUtility.SafeDisposeObject(m_matriceW2V[i, j]);
                        JetEazy.QUtilities.QUtility.SafeDisposeObject(m_matriceV2W[i, j]);

                        _buildMatrix(out m_matriceW2V[i, j], ptsV, ptsW);   // Wolrd To View
                        _buildMatrix(out m_matriceV2W[i, j], ptsW, ptsV);   // View To World
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public void TransformWorldToView(PointF pointWorld, out PointF pointView)
        {
            int iRow;
            int iCol;

            _getWorldZone(out iRow, out iCol, new PointF(pointWorld.X / WorldScale, pointWorld.Y / WorldScale));

            CvMat mat = m_matriceW2V[iRow, iCol];

            double wx = pointWorld.X / WorldScale;
            double wy = pointWorld.Y / WorldScale;
            double wxy = wx * wy;
            double x = mat[0, 0] * 1 + mat[0, 1] * wx + mat[0, 2] * wy + mat[0, 3] * wxy;
            double y = mat[1, 0] * 1 + mat[1, 1] * wx + mat[1, 2] * wy + mat[1, 3] * wxy;

            pointView = new PointF((float)x * ViewScale, (float)y * ViewScale);
        }
        public void TransformViewToWorld(PointF pointView, out PointF pointWorld)
        {
            int iRow;
            int iCol;

            _getViewZone(out iRow, out iCol, new PointF(pointView.X / ViewScale, pointView.Y / ViewScale));

            CvMat mat = m_matriceV2W[iRow, iCol];
            double vx = pointView.X / ViewScale;
            double vy = pointView.Y / ViewScale;
            double vxy = vx * vy;
            double x = mat[0, 0] * 1 + mat[0, 1] * vx + mat[0, 2] * vy + mat[0, 3] * vxy;
            double y = mat[1, 0] * 1 + mat[1, 1] * vx + mat[1, 2] * vy + mat[1, 3] * vxy;

            pointWorld = new PointF((float)x * WorldScale, (float)y * WorldScale);
        }

        public void Load(string strIniFileName)
        {
            //JetEazy.Win32.Win32Ini.Load(ref m_iRows, strIniFileName, "Dimensions", "Rows");
            //JetEazy.Win32.Win32Ini.Load(ref m_iCols, strIniFileName, "Dimensions", "Cols");

            iLoadCurrent = 0;

            LoadAlldata = null;
            JzTools.ReadData(ref LoadAlldata, strIniFileName);

            string[] strs = LoadAlldata.Replace(Environment.NewLine, "@").Split('@');

            int i=0;
            while (i < strs.Length)
            {
                if (strs[i].IndexOf('=') > -1)
                    LoadList.Add(strs[i]);

                i++;
            }

            m_iRows =Convert.ToInt16(LoadList[0].Split('=')[1]);
            m_iCols = Convert.ToInt16(LoadList[1].Split('=')[1]);

            iLoadCurrent = 2;

            if (m_iRows < 2) m_iRows = 3;
            if (m_iCols < 2) m_iCols = 3;

            _initArrays();

            _load(m_matriceW2V, strIniFileName, "MATRIX");
            _load(m_matriceV2W, strIniFileName, "MATRIX_V2W");

            _load(m_pointsInView, strIniFileName, "ViewPoints");
            _load(m_pointsInWorld, strIniFileName, "WorldPoints");
        }
        public void Save(string strIniFileName)
        {
            //JzTools.SaveData("StartTime: "+JzTimes.DateTimeSerialString+Environment.NewLine, @"D:\LOGTIME.TXT",true);

            SaveAlldata = null;
            SaveAlldata = "[Dimensions]" + Environment.NewLine
                + "Rows=" + m_iRows.ToString() + Environment.NewLine
                + "Cols=" + m_iCols.ToString() + Environment.NewLine;
            //JetEazy.Win32.Win32Ini.Save(m_iRows, strIniFileName, "Dimensions", "Rows");
            //JetEazy.Win32.Win32Ini.Save(m_iCols, strIniFileName, "Dimensions", "Cols");

            _save(m_matriceW2V, strIniFileName, "MATRIX");
            _save(m_matriceV2W, strIniFileName, "MATRIX_V2W");
            _save(m_pointsInView, strIniFileName, "ViewPoints");
            _save(m_pointsInWorld, strIniFileName, "WorldPoints");

            JzTools.SaveData(SaveAlldata, strIniFileName);
            //JzTools.SaveData("EndTime: " + JzTimes.DateTimeSerialString + Environment.NewLine, @"D:\LOGTIME.TXT", true);
        }

        //public void CopyTo(CAoiCalibration calibration)
        //{
        //    calibration.m_iRows = m_iRows;
        //    calibration.m_iCols = m_iCols;

        //    calibration._initArrays();
            
            


        //}
        
        #region PRIVATE_FUNCTIONS

            private void _initArrays()
            {
                if (m_iRows < 2 || m_iCols < 2)
                    throw new Exception("Rows or Cols is too small.");
                
                m_pointsInView = new PointF[m_iRows, m_iCols];
                m_pointsInWorld = new PointF[m_iRows, m_iCols];

                _disposeMatrice();
                _initMatrice();
            }
            private void _initMatrice()
            {
                m_matriceW2V = new CvMat[m_iRows - 1, m_iCols - 1];
                m_matriceV2W = new CvMat[m_iRows - 1, m_iCols - 1];

                for (int i = 0; i < m_iRows - 1; i++)
                {
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        if (m_matriceW2V[i, j] == null)
                            m_matriceW2V[i, j] = new CvMat(2, 4, MatrixType.F32C1);

                        if (m_matriceV2W[i, j] == null)
                            m_matriceV2W[i, j] = new CvMat(2, 4, MatrixType.F32C1);
                    }
                }
            }
            private void _disposeMatrice()
            {
                _disposeMatrice(m_matriceW2V);
                m_matriceW2V = null;
                _disposeMatrice(m_matriceV2W);
                m_matriceV2W = null;
            }
            private void _disposeMatrice(CvMat[,] matrice)
            {
                if (matrice != null)
                {
                    int iRows = matrice.GetUpperBound(0) + 1;
                    int iCols = matrice.GetUpperBound(1) + 1;
                    for (int i = 0; i < iRows; i++)
                    {
                        for (int j = 0; j < iCols; j++)
                        {
                            JetEazy.QUtilities.QUtility.SafeDisposeObject(matrice[i, j]);
                            matrice[i, j] = null;
                        }
                    }
                }
            }
            private void _buildMatrix(out CvMat matT, PointF[] ptsV, PointF[] ptsW)
            {
                //	vx0 = |m11 m12 m13 m14|  w[1 x0 y0 xy0]t
                //	vx1 = |m11 m12 m13 m14|  w[1 x1 y1 xy1]t
                //	vx2 = |m11 m12 m13 m14|  w[1 x2 y2 xy2]t
                //	vx3 = |m11 m12 m13 m14|  w[1 x3 y3 xy3]t

                //	vy0 = |m21 m22 m23 m24|  w[1 x0 y0 xy0]t
                //	vy1 = |m21 m22 m23 m24|  w[1 x1 y1 xy1]t
                //	vy2 = |m21 m22 m23 m24|  w[1 x2 y2 xy2]t
                //	vy3 = |m21 m22 m23 m24|  w[1 x3 y3 xy3]t

                //	w| 1 x0 y0 xy0 | m11 |   | vx0 |
                //	w| 1 x1 y1 xy1 | m12 |   | vx1 |
                //	w| 1 x2 y2 xy2 | m13 | = | vx2 | 
                //	w| 1 x3 y3 xy3 | m14 |   | vx3 |

                //	w| 1 x0 y0 xy0 | m21 |   | vy0 |
                //	w| 1 x1 y1 xy1 | m22 |   | vy1 |
                //	w| 1 x2 y2 xy2 | m23 | = | vy2 | 
                //	w| 1 x3 y3 xy3 | m24 |   | vy3 |
                CvMat mat1XY = new CvMat(4, 4, MatrixType.F32C1);
                CvMat matVX = new CvMat(4, 1, MatrixType.F32C1);
                CvMat matVY = new CvMat(4, 1, MatrixType.F32C1);

                for (int i = 0; i < 4; i++)
                {
                    double vx = ptsV[i].X;
                    double vy = ptsV[i].Y;
                    double wx = ptsW[i].X;
                    double wy = ptsW[i].Y;

                    mat1XY[i, 0] = 1;
                    mat1XY[i, 1] = wx;
                    mat1XY[i, 2] = wy;
                    mat1XY[i, 3] = wx * wy;

                    matVX[i] = vx;
                    matVY[i] = vy;
                }

                CvMat matInv = new CvMat(4, 4, MatrixType.F32C1);


                mat1XY.Invert(matInv, InvertMethod.Normal);

                CvMat mat1R = matInv * matVX;
                CvMat mat2R = matInv * matVY;

                matT = new CvMat(2, 4, MatrixType.F32C1);
                for (int j = 0; j < 4; j++)
                {
                    matT[0, j] = mat1R[j];
                    matT[1, j] = mat2R[j];
                }

                JetEazy.QUtilities.QUtility.SafeDisposeObject(mat1XY);
                JetEazy.QUtilities.QUtility.SafeDisposeObject(matVX);
                JetEazy.QUtilities.QUtility.SafeDisposeObject(matVY);
                JetEazy.QUtilities.QUtility.SafeDisposeObject(matInv);
                JetEazy.QUtilities.QUtility.SafeDisposeObject(mat1R);
                JetEazy.QUtilities.QUtility.SafeDisposeObject(mat2R);
            }
            private void _getWorldZone(out int iRow, out int iCol, PointF pointWorld)
            {
                iRow = 0;
                iCol = 0;

                #region SEARCH_ROW
                    for (int i = 0; i < m_iRows - 1; i++)
                    {
                        iRow = i;
                        for (int j = 0; j < m_iCols - 1; j++)
                        {
                            if (pointWorld.Y <= m_pointsInWorld[i + 1, j + 1].Y)
                            {
                                i = m_iRows - 1;
                                break;
                            }
                        }
                    }
                #endregion

                #region SEARCH_COL
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        iCol = j;
                        if (pointWorld.X <= m_pointsInWorld[iRow, j + 1].X)
                        {
                            break;
                        }
                    }
                #endregion

#if(OPT_LEGACY)
                PointF ptCenter = m_pointsInWorld[1, 1];
                if (pointWorld.X <= ptCenter.X)
                {
                    iCol = 0;
                    iRow = (pointWorld.Y <= ptCenter.Y) ? 0 : 1;
                }
                else
                {
                    iCol = 1;
                    iRow = (pointWorld.Y <= ptCenter.Y) ? 0 : 1;
                }
#endif
            }
            private void _getViewZone(out int iRow, out int iCol, PointF pointView)
            {
                iRow = 0;
                iCol = 0;

                #region SEARCH_ROW
                for (int i = 0; i < m_iRows - 1; i++)
                {
                    iRow = i;
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        if (pointView.Y <= m_pointsInView[i + 1, j + 1].Y)
                        {
                            i = m_iRows - 1;
                            break;
                        }
                    }
                }
                #endregion

                #region SEARCH_COL
                for (int j = 0; j < m_iCols - 1; j++)
                {
                    iCol = j;
                    if (pointView.X <= m_pointsInView[iRow, j + 1].X)
                    {
                        break;
                    }
                }
                #endregion
            }

            private void _load(PointF[,] pts, string strIniFileName, string strAppName)
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        double x = 0;
                        double y = 0;
                        _loadItem(ref x, strIniFileName, strAppName, "X", i, j);
                        _loadItem(ref y, strIniFileName, strAppName, "Y", i, j);
                        pts[i, j] = new PointF((float)x, (float)y);
                    }
                }
            }
            private void _save(PointF[,] pts, string strIniFileName, string strAppName)
            {
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        double x = pts[i, j].X;
                        double y = pts[i, j].Y;

                        if (i == 0 && j == 0)
                        {
                            _saveItem(x, strIniFileName, strAppName, "X", i, j);
                            _saveItem(y, strIniFileName, "", "Y", i, j);
                        }
                        else
                        {
                            _saveItem(x, strIniFileName, "", "X", i, j);
                            _saveItem(y, strIniFileName, "", "Y", i, j);
                        }
                    }
                }
            }

            private void _load(CvMat[,] mx, string strIniFileName, string strAppName)
            {
                for (int i = 0; i < m_iRows - 1; i++)
                {
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        _load(mx[i, j], strIniFileName, strAppName + "_" + i + "_" + j);
                    }
                }
            }
            private void _save(CvMat[,] mx, string strIniFileName, string strAppName)
            {
                for (int i = 0; i < m_iRows - 1; i++)
                {
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        _save(mx[i, j], strIniFileName, strAppName + "_" + i + "_" + j);
                    }
                }
            }
            private void _load(CvMat mx, string strIniFileName, string strAppName)
            {
                int iRows = mx.Rows;
                int iCols = mx.Cols;
                for (int i = 0; i < iRows; i++)
                {
                    for (int j = 0; j < iCols; j++)
                    {
                        double value = 0;
                        _loadItem(ref value, strIniFileName, strAppName, "M", i, j);
                        mx[i, j] = value;
                    }
                }
            }
            private void _save(CvMat mx, string strIniFileName, string strAppName)
            {
                int iRows = mx.Rows;
                int iCols = mx.Cols;
                for (int i = 0; i < iRows; i++)
                {
                    for (int j = 0; j < iCols; j++)
                    {
                        double value = mx[i, j];
                        if (i == 0 && j == 0)
                            _saveItem(value, strIniFileName, strAppName, "M", i, j);
                        else
                            _saveItem(value, strIniFileName, "", "M", i, j);
                    }
                }
            }

            private void _loadItem(ref double value, string strIniFileName, string strAppName, string strKey, int iRow, int iCol)
            {
                string strKeyA = strKey + "_" + iRow + "_" + iCol;
                string strValue = "";
                //JetEazy.Win32.Win32Ini.Load(ref strValue, strIniFileName, strAppName, strKeyA);

                strValue = LoadList[iLoadCurrent].Split('=')[1];
                double.TryParse(strValue, out value);

                iLoadCurrent++;
            }
            private void _saveItem<T>(T value, string strIniFileName, string strAppName, string strKey, int iRow, int iCol)
            {
                string strKeyA = strKey + "_" + iRow + "_" + iCol;
                string strValue = value.ToString();
                //JetEazy.Win32.Win32Ini.Save(strValue, strIniFileName, strAppName, strKeyA);
                if (strAppName != "")
                    SaveAlldata += "[" + strAppName + "]" + Environment.NewLine + strKeyA + "=" + strValue + Environment.NewLine;
                else
                    SaveAlldata += strKeyA + "=" + strValue + Environment.NewLine;
            }

        #endregion
    }
}
