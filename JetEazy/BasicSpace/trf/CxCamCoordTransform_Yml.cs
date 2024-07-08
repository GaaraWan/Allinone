using System;
using OpenCvSharp;
using JetEazy.QMath;
using CvMat = OpenCvSharp.Mat;
using MatrixType = OpenCvSharp.MatType;
using CvScalar = OpenCvSharp.Scalar;
using CvPoint2D32f = OpenCvSharp.Point2f;
using CvFileStorage = OpenCvSharp.FileStorage;


namespace JetEazy.Aoi.TransformD
{
    partial class CxCamCoordTransform
    {
        /// <summary>
        /// NOT READY YET
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadYml(string fileName)
        {
            m_name = System.IO.Path.GetFileName(fileName);

            using (CvFileStorage fs = new CvFileStorage(fileName, null, FileStorageMode.Read))
            {
                // string str = null;
                // JetEazy.Win32.Win32Ini.Load(ref str, fileName, "Scale", "World");
                // if (!string.IsNullOrEmpty(str)) double.TryParse(str, out SCALE_OF_WORLD);
                // JetEazy.Win32.Win32Ini.Load(ref str, fileName, "Scale", "View");
                // if (!string.IsNullOrEmpty(str)) double.TryParse(str, out SCALE_OF_VIEW);
                CvFileNode node = fs.GetFileNodeByName(null, "Scales_World_and_View");
                using (var mx = fs.Read<CvMat>(node))
                {
                    SCALE_OF_WORLD = mx[0, 0];
                    SCALE_OF_VIEW = mx[0, 1];
                }

                //var str = fs.ReadStringByName(node, "World");
                //if (!string.IsNullOrEmpty(str)) double.TryParse(str, out SCALE_OF_WORLD);
                //str = fs.ReadStringByName(node, "View");
                //if (!string.IsNullOrEmpty(str)) double.TryParse(str, out SCALE_OF_VIEW);

                // JetEazy.Win32.Win32Ini.Load(ref m_iRows, fileName, "Dimensions", "Rows");
                // JetEazy.Win32.Win32Ini.Load(ref m_iCols, fileName, "Dimensions", "Cols");
                node = fs.GetFileNodeByName(null, "Dimensions");
                m_iRows = fs.ReadIntByName(node, "Rows");
                m_iCols = fs.ReadIntByName(node, "Cols");

                bool bEmptyMatrix = false;
                if (m_iRows < 2) { m_iRows = 2; bEmptyMatrix = true; }
                if (m_iCols < 2) { m_iCols = 2; bEmptyMatrix = true; }

                _initArrays();

                if (!bEmptyMatrix)
                {
                    _load(m_matriceW2V, fs, "MATRIX");
                    _load(m_matriceV2W, fs, "MATRIX_V2W");
                }

                _load(m_pointsInView, fs, "ViewPoints");
                _load(m_pointsInWorld, fs, "WorldPoints");
            }
        }
        /// <summary>
        /// NOT READY YET
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveYml(string fileName)
        {
            using (CvFileStorage fs = new CvFileStorage(fileName, null, FileStorageMode.Write))
            {
                //JetEazy.Win32.Win32Ini.Save(SCALE_OF_WORLD, fileName, "Scale", "World");
                //JetEazy.Win32.Win32Ini.Save(SCALE_OF_VIEW, fileName, "Scale", "View");
                //JetEazy.Win32.Win32Ini.Save(m_iRows, fileName, "Dimensions", "Rows");
                //JetEazy.Win32.Win32Ini.Save(m_iCols, fileName, "Dimensions", "Cols");
                using (var mx = new CvMat(1, 2, MatrixType.F64C1, new double[] { SCALE_OF_WORLD, SCALE_OF_VIEW }))
                {
                    fs.Write("Scales_World_and_View", mx);
                }

                fs.StartWriteStruct("Dimensions", NodeType.Map | NodeType.Flow);
                fs.WriteInt("Rows", m_iRows);
                fs.WriteInt("Cols", m_iCols);
                fs.EndWriteStruct();

                _save(m_matriceW2V, fs, "MATRIX");
                _save(m_matriceV2W, fs, "MATRIX_V2W");
                _save(m_pointsInView, fs, "ViewPoints");
                _save(m_pointsInWorld, fs, "WorldPoints");
            }
        }

        private void _load(QVector[,] pts, CvFileStorage fs, string appName)
        {
            try
            {
                CvFileNode node = fs.GetFileNodeByName(null, appName);
                if (node != null)
                {
                    for (int i = 0; i < m_iRows; i++)
                    {
                        for (int j = 0; j < m_iCols; j++)
                        {
                            //> _load(mx[i, j], strIniFileName, strAppName + "_" + i + "_" + j);
                            QVector v = new QVector(2);
                            v.x = 0;
                            v.y = 0;
                            try
                            {
                                using (CvMat mx = fs.Read<CvMat>(node))
                                {
                                    v.x = mx[0, 0];
                                    v.y = mx[0, 1];
                                }
                            }
                            catch
                            {
                            }
                            pts[i, j] = v;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // NOT READY YET!
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        private void _save(QVector[,] pts, CvFileStorage fs, string appName)
        {
            try
            {
                fs.StartWriteStruct(appName, NodeType.Seq);
                for (int i = 0; i < m_iRows - 1; i++)
                {
                    for (int j = 0; j < m_iCols - 1; j++)
                    {
                        var v = pts[i, j];
                        var mx = new CvMat(1, 2, MatrixType.F64C1, v.V);
                        fs.Write(null, mx);
                        mx.Dispose();
                    }
                }
                fs.EndWriteStruct();
            }
            catch
            {
            }
        }
        private void _load(CvMat[,] mxx, CvFileStorage fs, string appName)
        {
            try
            {
                CvFileNode node = fs.GetFileNodeByName(null, appName);
                if (node != null)
                {
                    for (int i = 0; i < m_iRows - 1; i++)
                    {
                        for (int j = 0; j < m_iCols - 1; j++)
                        {
                            //_load(mx[i, j], strIniFileName, strAppName + "_" + i + "_" + j);
                            using (CvMat mxNew = fs.Read<CvMat>(node))
                            {
                                mxNew.Copy(mxx[i, j]);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                // NOT READY YET!
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
        private void _save(CvMat[,] mxx, CvFileStorage fs, string appName)
        {
            try
            {
                fs.StartWriteStruct(appName, NodeType.Seq);
                for (int i = 0; i < m_iRows; i++)
                {
                    for (int j = 0; j < m_iCols; j++)
                    {
                        fs.Write(null, mxx[i, j]);
                    }
                }
                fs.EndWriteStruct();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
    }
}
