using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class MergeSort
    {
        public static List<Genome> Sort(ref List<Genome> pGenomes)
        {
            List<Genome> left;
            List<Genome> right;
            List<Genome> result = new List<Genome>(pGenomes.Count);
            if (pGenomes.Count <= 1)
                return pGenomes;
            int mp = pGenomes.Count / 2;
            left = new List<Genome>(mp);
            if (pGenomes.Count % 2 == 0)
                right = new List<Genome>(mp);
            else
                right = new List<Genome>(mp+1);
            for (int i = 0; i < mp; i++)
                left[i] = pGenomes[i];
            int x = 0;
            for (int i = mp; i < pGenomes.Count; i++)
            {
                right[x] = pGenomes[i];
                x++;
            }
            left = Sort(ref left);
            right = Sort(ref right);
            result = Merge(ref left, ref right);
            return result;
        }

        public static List<Genome> Merge(ref List<Genome> left, ref List<Genome> right)
        {
            int resultLength = right.Count + left.Count;
            List<Genome> result = new List<Genome>(resultLength);
            int indexLeft = 0, indexRight = 0, indexResult = 0;
            while (indexLeft < left.Count || indexRight < right.Count)
            {
                if (indexLeft < left.Count && indexRight < right.Count)
                {
                    if (left[indexLeft].Fitness <= right[indexRight].Fitness)
                    {
                        result[indexResult] = left[indexLeft];
                        indexLeft++;
                        indexResult++;
                    }
                    else
                    {
                        result[indexResult] = right[indexRight];
                        indexRight++;
                        indexResult++;
                    }
                }
                else if (indexLeft < left.Count)
                {
                    result[indexResult] = left[indexLeft];
                    indexLeft++;
                    indexResult++;
                }
                else if (indexRight < right.Count)
                {
                    result[indexResult] = right[indexRight];
                    indexRight++;
                    indexResult++;
                }
            }
            return result;
        }
    }
}
