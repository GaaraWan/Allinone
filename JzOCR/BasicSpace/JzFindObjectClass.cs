using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JzOCR.BasicSpace
{
    class JzFindObjectClass:JetEazy.BasicSpace.JzFindObjectClass
    {
       
        public void SortByArea()
        {
            int i = 0;

            SortingList.Clear();

            foreach (FoundClass found in FoundList)
            {
                SortingList.Add(found.Area.ToString("00000") + "," + i.ToString());
                i++;
            }

            SortingList.Sort();
            SortingList.Reverse();


        }
        public void SortByX()
        {
            //int i = 0;

            //SortingList.Clear();

            //foreach (FoundClass found in FoundList)
            //{
            //    SortingList.Add(found.Location.X.ToString("00000") + "," + i.ToString());
            //    i++;
            //}

            //SortingList.Sort();
            //SortingList.Reverse();

            for (int i = 0; i < FoundList.Count; i++)
            {
                FoundClass foundi = FoundList[i];
                for (int j = i + 1; j < FoundList.Count; j++)
                {
                    FoundClass foundj = FoundList[j];
                    if (foundi.Location.X > foundj.Location.X)
                    {
                        FoundClass foundtemp = foundj.Clone();
                        FoundList[j] = foundi.Clone();
                        FoundList[i] = foundtemp.Clone();

                        foundi = FoundList[i];
                        foundj = FoundList[j];
                    }
                }
            }
        }
        public void SortByY(bool IsBiggerFirst)
        {
            int i = 0;

            SortingList.Clear();

            foreach (FoundClass found in FoundList)
            {
                SortingList.Add(found.rect.Y.ToString("00000") + "," + i.ToString());
                i++;
            }

            SortingList.Sort();

            if (IsBiggerFirst)
                SortingList.Reverse();
        }
    }
}
