using System;
using System.IO;

namespace SubstitutionBreaker
{
    class LangData
    {
        public Tuple<string, int>[] charFreq;
        public int[,] bigramData;
        public Tuple<string, int>[] bigramMostCommonData;
        public Tuple<string, int>[] trigramData;

        public LangData()
        {
            ReadCharData();
            ReadBigramData();
            ReadBigramDataMostCommon();
            ReadTrigramData();

        }

        private void ReadCharData()
        {
            var charData = new FileStream(".\\CharFreq.csv", FileMode.Open, FileAccess.Read, FileShare.Read);
            charFreq = new Tuple<string, int>[26];

            string chr;
            int count;
            string data;

            using (StreamReader reader = new StreamReader(charData))
            {
                for (int i = 0; i < charFreq.Length; i++)
                {
                    data = reader.ReadLine();
                    data = data.Substring(0, data.Length - 1);

                    chr = ((char)( i + 'A')).ToString();
                    count = (Int32)(Double.Parse(data) * 1000);
                    charFreq[i] =new Tuple<string, int>(chr, count);
                }
            }

            RadixSort(charFreq);

            charData.Dispose();
        }
        private void ReadBigramData()
        {
            var bigramDataFile = new FileStream(".\\BigramData.csv", FileMode.Open, FileAccess.Read, FileShare.Read);
            bigramData = new int[26, 26];

            string data;
            using (StreamReader reader = new StreamReader(bigramDataFile))
            {
                for (int i = 0; i < charFreq.Length; i++)
                {
                    data = reader.ReadLine();

                    int cur = 0, next = data.IndexOf(',');
                    
                    for (int j = 0; next != -1; j++)
                    {        
                        bigramData[i, j] = Int32.Parse(data.Substring(cur, next - cur));

                        cur = next + 1;
                        next = data.IndexOf(',', cur);
                    }
                }
            }

            bigramDataFile.Dispose();
        }

        private void ReadBigramDataMostCommon()
        {
            int value;
            string key;
            var bigramDataFile = new FileStream(".\\BigramMostCommonData.csv", FileMode.Open, FileAccess.Read, FileShare.Read);
            bigramMostCommonData = new Tuple<string, int>[50];

            string data;
            using (StreamReader reader = new StreamReader(bigramDataFile))
            {
                for (int i = 0; i < bigramMostCommonData.Length; i++)
                {
                    data = reader.ReadLine().ToUpper();

                    int cur = 0, next = data.IndexOf(',');

                    key = data.Substring(cur, next - cur);

                    cur = next + 1;

                    value = Int32.Parse(data.Substring(cur, data.Length - cur));

                    bigramMostCommonData[i] = new Tuple<string, int>(key, value);
                }
            }

            RadixSort(bigramMostCommonData);

            bigramDataFile.Dispose();
        }

        private void ReadTrigramData()
        {
            int value;
            string key;
            var trigramDataFile = new FileStream(".\\TrigramMostCommonData.csv", FileMode.Open, FileAccess.Read, FileShare.Read);
            trigramData = new Tuple<string, int>[50];

            string data;
            using (StreamReader reader = new StreamReader(trigramDataFile))
            {
                for (int i = 0; i < trigramData.Length; i++)
                {
                    data = reader.ReadLine().ToUpper();

                    int cur = 0, next = data.IndexOf(',');

                    key = data.Substring(cur, next - cur);

                    cur = next + 1;
                  
                    value = Int32.Parse(data.Substring(cur, data.Length - cur));

                    trigramData[i] = new Tuple<string, int>(key, value);
                }
            }

            RadixSort(trigramData);

            trigramDataFile.Dispose();
        }

        private static void RadixSort(Tuple<string, int>[] nGramCount)
        {
            int m = GetMax(nGramCount);
            for (int exp = 1; m / exp > 0; exp *= 10)
                CountSort(nGramCount, exp);
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
                count[(nGramCount[i].Item2 / exp) % 10]++;

            // Change count[i] so that count[i] now contains
            // actual position of this digit in output[]
            for (i = 1; i < 10; i++)
                count[i] += count[i - 1];

            // Build the output array
            for (i = nGramCount.Length - 1; i >= 0; i--)
            {
                output[count[(nGramCount[i].Item2 / exp) % 10] - 1] = nGramCount[i];
                count[(nGramCount[i].Item2 / exp) % 10]--;
            }

            // Copy the output array to nGramCount[], so that nGramCount[] now
            // contains sorted numbers according to current digit
            for (i = 0; i < nGramCount.Length; i++)
                nGramCount[i] = output[i];
        }
    }
}
