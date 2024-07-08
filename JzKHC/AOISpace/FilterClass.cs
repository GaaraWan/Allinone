using System;
using System.Collections.Generic;
using System.Text;

namespace JzKHC.AOISpace
{
    class FilterClass
    {
    }

    class PropertyFilterClass
    {
        public int Gap = 0;
        int Range = 0;
        int MinProperty = 0;
        int Max = -10000;
        int Min = 100000;
        int Mean = 0;
        int ModeIndex = 0;
        public int Mode = 0;

        int TotalProperty = 0;
        int Total = 1;

        public int Count
        {
            get
            {
                return SortProperty.Length;
            }
        }
        public int GetPorpertyCount(int Rank)
        {
            return SortProperty[Count - Rank] / 1000;
        }
        public int GetPorperty(int Rank)
        {
            return ((SortProperty[Count - Rank] % 1000) * Gap) + MinProperty;
        }
        public int[] SortProperty = new int[10];

        public PropertyFilterClass()
        {

        }
        public void Initial(int PropertyMin,int PropertyMax,int PropertyGap)
        {
            Gap = PropertyGap;
            MinProperty = PropertyMin;
            Range = (int)Math.Ceiling((PropertyMax - PropertyMin) / (double)PropertyGap) + 1;

            SortProperty = new int[Range];

            Total = 1;
            Max = -10000;
            Min = 10000;
            TotalProperty = 0;
        }
        public void Add(int Property)
        {
            Max = Math.Max(Property, Max);
            Min = Math.Min(Property, Min);

            int i = (int)Math.Ceiling((Property - MinProperty) / (double)Gap);

            if (i < 0)
                i = 0;

            SortProperty[i] += 1000;

 
            TotalProperty += Property;
            Total++;
        }
        public void Complete()
        {
            Mean = MinProperty + (TotalProperty / Total);

            int i = 0;
            int MaxValue = -1;

            ModeIndex = -1;

            while (i < Range)
            {
                if (MaxValue < SortProperty[i])
                {
                    MaxValue = SortProperty[i];
                    ModeIndex = i;
                }

                SortProperty[i] += i;
                i++;
            }

            Mode = MinProperty + (ModeIndex * Gap);

            Array.Sort(SortProperty);

        }

    }
}
