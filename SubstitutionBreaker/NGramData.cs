using System;
using System.Collections.Generic;

namespace SubstitutionBreaker
{
    class NGramData
    {
        public Tuple<string, int>[] uGramArray;
        public Tuple<string, int>[] bGramArray;
        public Tuple<string, int>[] tGramArray;
        public Tuple<string, int>[] bGramArrayNonRepeat;

        public NGramData(string data)
        {

            Dictionary<string, int> uGram;
            Dictionary<string, int> bGram;
            Dictionary<string, int> tGram;
            Dictionary<string, int> bGramNonRepeat;

            uGram = MakeNGram(1, data);
            bGram = MakeNGram(2, data);
            tGram = MakeNGram(3, data);
            bGramNonRepeat = MakeNGramNonRepeat(2, data);

            uGramArray = ConvertToArray(uGram);
            bGramArray = ConvertToArray(bGram);
            tGramArray = ConvertToArray(tGram);
            bGramArrayNonRepeat = ConvertToArray(bGramNonRepeat);
        }

        public static Tuple<string, int>[] ConvertToArray(Dictionary<string, int> dict)
        {
            Tuple<string, int>[] res = new Tuple<string, int>[dict.Count];
            Tuple<string, int> cur;
            int pos = 0;
            foreach (var item in dict)
            {
                cur = new Tuple<string, int>(item.Key, item.Value);
                res[pos++] = cur;
            }

            RadixSort(res);
            return res;
        }

        private Dictionary<string, int> MakeNGram(int v, string data)
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            string nGram;

            //generate nGram counts
            for (int i = v - 1; i < data.Length - v; i++)
            {
                nGram = data.Substring(i, v);

                if (res.ContainsKey(nGram))
                {
                    res[nGram] = res[nGram] + 1;
                }
                else
                {
                    res[nGram] = 1;
                }
            }
            return res;
        }
        private static Dictionary<string, int> MakeNGramNonRepeat(int v, string data)
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            string nGram;

            //generate nGram counts
            for (int i = 0; i < data.Length - v; i += v)
            {
                nGram = data.Substring(i, v);

                if (res.ContainsKey(nGram))
                {
                    res[nGram] = res[nGram] + 1;
                }
                else
                {
                    res[nGram] = 1;
                }
            }
            return res;
        }

        private static void RadixSort(Tuple<string, int>[] nGramCount)
        {
            int m = GetMax(nGramCount);
            for (int exp = 1; m / exp > 0; exp *= 10)
            {
                CountSort(nGramCount, exp);
            }
        }

        private static int GetMax(Tuple<string, int>[] nGramCount)
        {
            int res = nGramCount[0].Item2;
            for (int i = 1; i < nGramCount.Length; i++)
            {
                if (res < nGramCount[i].Item2)
                {
                    res = nGramCount[i].Item2;
                }
            }
            return res;
        }

        private static void CountSort(Tuple<string, int>[] nGramCount, int exp)
        {
            Tuple<string, int>[] output = new Tuple<string, int>[nGramCount.Length]; // output array
            int i;
            int[] count = new int[10];


            // Store count of occurrences in count[]
            for (i = 0; i < nGramCount.Length; i++)
            {
                count[(nGramCount[i].Item2 / exp) % 10]++;
            }

            // Change count[i] so that count[i] now contains
            // actual position of this digit in output[]
            for (i = 1; i < 10; i++)
            {
                count[i] += count[i - 1];
            }

            // Build the output array
            for (i = nGramCount.Length - 1; i >= 0; i--)
            {
                output[count[(nGramCount[i].Item2 / exp) % 10] - 1] = nGramCount[i];
                count[(nGramCount[i].Item2 / exp) % 10]--;
            }

            // Copy the output array to nGramCount[], so that nGramCount[] now
            // contains sorted numbers according to current digit
            for (i = 0; i < nGramCount.Length; i++)
            {
                nGramCount[i] = output[i];
            }
        }
    }
}
