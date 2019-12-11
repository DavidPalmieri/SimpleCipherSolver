using System.Text;
using System.Text.RegularExpressions;

namespace SubstitutionBreaker
{
    class Data
    {
        public string[] assign = new string[26];
        public string cipherText, Plaintext;
        public int shift;
        public int a, b;
        public Matrix key, keyInverse;

        public Data(string cipherText)
        {
            Regex rgx = new Regex("[^A-Z]");
            cipherText = rgx.Replace(cipherText, "");

            this.cipherText = cipherText;

            for (int i = 0; i < assign.Length; i++)
            {
                assign[i] = "-";
            }
            mapCharSub();
            shift = 0;
            a = -1;
            b = -1;
            key = new Matrix(2);
            keyInverse = new Matrix(2);
        }

        public void setAssign(int i, string s)
        {
            if (s == "")
            {
                assign[i] = "-";
            }
            else
            {
                assign[i] = s;
            }

            mapCharSub();
        }


        public void setShift(int i)
        {
            shift = i;
            mapCharShift();
        }

        private void mapCharSub()
        {
            StringBuilder res = new StringBuilder(cipherText.Length);
            for (int i = 0; i < cipherText.Length; i++)
            {
                if (cipherText[i] == '\n' || cipherText[i] == '\r')
                {
                    res.Append("\n");
                }
                else
                {
                    res.Append(assign[cipherText[i] - 'A']);
                }
            }

            Plaintext = res.ToString();
        }

        private void mapCharShift()
        {
            StringBuilder res = new StringBuilder();
            int t1, t2, t3;

            foreach (char c in cipherText.ToCharArray())
            {
                t1 = c - 'A' - shift;
                while (t1 < 0)
                {
                    t1 += 26;
                }
                t2 = (t1) % 26;
                t3 = t2 + 'A';

                res.Append((char) t3);
            }

            Plaintext = res.ToString();
        }

        internal string getAssign()
        {
            StringBuilder res = new StringBuilder(assign.Length * 3);
            for (int i = 0; i < assign.Length; i++)
            {
                res.Append((char) (i + 'A'));
            }

            res.Append("\n");

            for (int i = 0; i < assign.Length; i++)
            {
                res.Append(assign[i]);
            }

            return res.ToString();
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder(cipherText.Length * 2 + 200 + key.size * key.size * 4);
            int i;

            res.Append("Shift: " + shift + "$$");

            res.Append("Key Matrix: $");
            for (i = 0; i < key.size * key.size; i++)
            {
                if (i % key.size == 0)
                {
                    res.Append("$");
                }
                res.Append((char) (key.matrix[i] + 'A') + " ");
            }

            res.Append("$$A: " + a + " |B: " + b + "$$");

            res.Append("Char Substitutions:$");
            for (int c = 'A'; c <= 'Z'; c++)
            {
                res.Append((char) c);
            }

            res.Append("$");

            foreach (string s in assign)
            {
                res.Append(s);
            }

            res.Append("$$");

            res.Append("Texts:$");
            for (i = 0; i + 40 < cipherText.Length; i += 40)
            {
                res.Append(cipherText.Substring(i, 40) + "$");
                res.Append(Plaintext.Substring(i, 40) + "$");
                res.Append("$");
            }
            res.Append(cipherText.Substring(i) + "$");
            res.Append(Plaintext.Substring(i) + "$");

            return res.ToString();
        }

        public void KeyMatrixDecrypt()
        {
            StringBuilder res = new StringBuilder(cipherText.Length);
            string encryptedChunk, decryptedChunk;
            for (int i = 0; i < cipherText.Length; i += key.size)
            {
                encryptedChunk = cipherText.Substring(i, key.size);

                decryptedChunk = Decrypt(encryptedChunk);

                res.Append(decryptedChunk);
            }
            Plaintext = res.ToString();
        }

        private string Decrypt(string chunk)
        {
            StringBuilder decrypt = new StringBuilder(3);
            Matrix tmp = new Matrix(chunk.Length);
            tmp.matrix[0] = chunk[0] - 'A';
            tmp.matrix[2] = chunk[1] - 'A';
            Matrix res = Matrix.MultMatrix2(keyInverse, tmp);

            decrypt.Append((char) ((res.matrix[0]) + 'A')).ToString();
            decrypt.Append((char) ((res.matrix[2]) + 'A')).ToString();

            return decrypt.ToString();
        }
    }
}
