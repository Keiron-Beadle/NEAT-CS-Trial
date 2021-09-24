using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT_Attempt
{
    class MergeSort
    {
        public static Genome[] Sort(Genome[] pGenomes)
        {
            Genome[] left;
            Genome[] right;
            Genome[] result = new Genome[pGenomes.Length];
            if (pGenomes.Length <= 1)
                return pGenomes;
            int mp = pGenomes.Length / 2;
            left = new Genome[mp];
            if (pGenomes.Length % 2 == 0)
                right = new Genome[mp];
            else
                right = new Genome[mp+1];
            for (int i = 0; i < mp; i++)
                left[i] = pGenomes[i];
            int x = 0;
            for (int i = mp; i < pGenomes.Length; i++)
            {
                right[x] = pGenomes[i];
                x++;
            }
            left = Sort(left);
            right = Sort(right);
            result = Merge(left, right);
            return result;
        }

        public static Genome[] Merge(Genome[] left, Genome[] right)
        {
            int resultLength = right.Length + left.Length;
            Genome[] result = new Genome[resultLength];
            int indexLeft = 0, indexRight = 0, indexResult = 0;
            while (indexLeft < left.Length || indexRight < right.Length)
            {
                if (indexLeft < left.Length && indexRight < right.Length)
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
                else if (indexLeft < left.Length)
                {
                    result[indexResult] = left[indexLeft];
                    indexLeft++;
                    indexResult++;
                }
                else if (indexRight < right.Length)
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
