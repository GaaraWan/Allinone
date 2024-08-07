using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JetEazy.PlugSpace
{
    public class CamActClass
    {
        private static CamActClass _instance = null;
        private int _stepCount = 1;
        private int _stepCurrent = 0;
        private Bitmap[] _bmps = new Bitmap[1];
        private CamActClass()
        {

        }
        public static CamActClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CamActClass();
                return _instance;
            }
        }
        public int StepCount
        {
            get { return _stepCount; }
            //set { _stepCount = value; }
        }
        public int StepCurrent
        {
            get
            {
                int ret = _stepCurrent;
                return ret;
                //return _stepCurrent;
            }
            set { _stepCurrent = value; }
        }
        public int GetCurrentStep()
        {
            int ret = _stepCurrent;
            return ret;
        }
        public void ResetStepCurrent()
        {
            _stepCurrent = 0;
        }
        public void SetStepCount(int stepcount)
        {
            ResetStepCurrent();
            _stepCount = stepcount;
            _bmps = new Bitmap[_stepCount];

            int i = 0;
            while (i < _stepCount)
            {
                _bmps[i] = new Bitmap(1, 1);
                i++;
            }
        }
        public void AddStep()
        {
            _stepCount++;
            SetStepCount(_stepCount);
        }
        public void CutStep()
        {
            if (_stepCount <= 0)
                return;

            _stepCount--;
            SetStepCount(_stepCount);
        }
        public void SetImage(Bitmap bmpinput, int index)
        {
            if (bmpinput == null)
                return;

            if (_bmps.Length == 0)
                return;

            if (index >= _bmps.Length)
                return;

            if (_bmps[index] == null)
                _bmps[index] = new Bitmap(1, 1);

            _bmps[index].Dispose();
            _bmps[index] = new Bitmap(bmpinput);
        }
        public Bitmap GetImage(int index)
        {
            if (index >= _bmps.Length)
                return new Bitmap(1, 1);

            return _bmps[index];
        }

    }
}
