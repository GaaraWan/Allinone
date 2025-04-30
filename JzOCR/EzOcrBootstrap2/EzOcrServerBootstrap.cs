using EzOcr.Model;
using System.Diagnostics;

namespace EzUtils
{
    public class EzOcrServerBootstrap
    {
        #region PRIVATE_DATA
        static EzOcrServerBootstrap m_singleton = null;
        PythonExecutor m_executor = new PythonExecutor();
        IxOcrClientModel m_model = null;
        #endregion

        protected EzOcrServerBootstrap()
        {
#if DEBUG
            this.OptShowCmdWindow = true;
#endif
        }
        public static EzOcrServerBootstrap Singleton()
        {
            if (m_singleton == null)
                m_singleton = new EzOcrServerBootstrap();
            return m_singleton;
        }

        public IxOcrClientModel Model
        {
            get { return m_model; }
        }
        public bool OptShowCmdWindow
        {
            get
            {
                return PythonExecutor.OptShowWindow;
            }
            set
            {
                PythonExecutor.OptShowWindow = value;
            }
        }
        
        public void StartServer()
        {
            m_executor.Run(null);
            
            m_model = EzOcrClientModel.Singleton();
            for (int i = 0; i < 20; i++)
            {
                if (m_model.IsConnected())
                    break;
                System.Threading.Thread.Sleep(1000);
                m_model.Connect();
            }

            for (int i = 0; i < 20; i++)
            {
                if (m_model.IsReady())
                    break;
                System.Threading.Thread.Sleep(1000);
            }

            for (int i = 0; i < 20; i++)
            {
                if (!string.IsNullOrEmpty(m_model.GetServerVersion()))
                    break;
                System.Threading.Thread.Sleep(1000);
            }
        }
        public void ResetServer()
        {
            Terminate();
            StartServer();
        }
        public void Terminate()
        {
            try
            {
                EzOcrClientModel.Disconnect();
            }
            catch
            {

            }
            m_model = null;
            m_executor.Terminate();
        }
    }
}
