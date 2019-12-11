using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SubstitutionBreaker
{
    public partial class SCS : Form
    {
        private Data text = null;
        private NGramData cipherTextData = null;
        private LangData engData = null;
        int shift;
        int runs = 0;

        double[] wieghts = new double[] { .7, .8, .9, 1, .9, .8, .7 };

        public SCS()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            engData = new LangData();
        }

        //Open File
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        text = new Data(reader.ReadToEnd());
                    }
                }
            }

            shift = 0;
            runs = 0;
            cipherTextData = new NGramData(text.cipherText);
            CheckData();

            SetTBs();
            C1.Text = MakePrintable(text.cipherText);
            C2.Text = MakePrintable(text.cipherText);
            C3.Text = MakePrintable(text.cipherText);
            C4.Text = MakePrintable(text.cipherText);
            P1.Text = MakePrintable(text.Plaintext);
            P2.Text = MakePrintable(text.Plaintext);
            P3.Text = MakePrintable(text.Plaintext);
            P4.Text = MakePrintable(text.Plaintext);

            AffCipherText1.Items.Clear();
            AffCipherText2.Items.Clear();
            for (int i = 0; i < cipherTextData.uGramArray.Length; i++)
            {
                AffCipherText1.Items.Add(cipherTextData.uGramArray[cipherTextData.uGramArray.Length - i - 1]);
                AffCipherText2.Items.Add(cipherTextData.uGramArray[cipherTextData.uGramArray.Length - i - 1]);
            }

            AffPlainText1.Items.Clear();
            AffPlainText2.Items.Clear();
            for (int i = 0; i < engData.charFreq.Length; i++)
            {
                AffPlainText1.Items.Add(engData.charFreq[engData.charFreq.Length - i - 1]);
                AffPlainText2.Items.Add(engData.charFreq[engData.charFreq.Length - i - 1]);
            }

            HillCipherText1.Items.Clear();
            HillCipherText2.Items.Clear();
            for (int i = 0; i < cipherTextData.bGramArrayNonRepeat.Length; i++)
            {
                HillCipherText1.Items.Add(cipherTextData.bGramArrayNonRepeat[cipherTextData.bGramArrayNonRepeat.Length - i - 1]);
                HillCipherText2.Items.Add(cipherTextData.bGramArrayNonRepeat[cipherTextData.bGramArrayNonRepeat.Length - i - 1]);
            }

            HillPlainText1.Items.Clear();
            HillPlainText2.Items.Clear();
            for (int i = 0; i < engData.bigramMostCommonData.Length; i++)
            {
                HillPlainText1.Items.Add(engData.bigramMostCommonData[engData.bigramMostCommonData.Length - i - 1]);
                HillPlainText2.Items.Add(engData.bigramMostCommonData[engData.bigramMostCommonData.Length - i - 1]);
            }

            K2.Text = text.getAssign();
            K1.Text = "Shift:  ...";
        }

        private void CheckData()
        {
            if (cipherTextData.uGramArray.Length < 26)
            {
                int i = 0, diff = 26 - cipherTextData.uGramArray.Length;

                for (int j = 0; j < 26 && i < diff; j++)
                {
                    if (!Exists(j))
                    {
                        text.assign[j] = "0";
                        i++;
                    }
                }
            }
        }

        private bool Exists(int j)
        {
            bool found = false;
            foreach (var item in cipherTextData.uGramArray)
            {
                found = item.Item1 == ((char) (j + 'A')).ToString();
                if (found)
                {
                    return found;
                }
            }
            return found;
        }

        private void SetTBs()
        {
            int i = 0;
            A.Text = text.assign[i++];
            B.Text = text.assign[i++];
            C.Text = text.assign[i++];
            D.Text = text.assign[i++];
            E.Text = text.assign[i++];
            F.Text = text.assign[i++];
            G.Text = text.assign[i++];
            H.Text = text.assign[i++];
            I.Text = text.assign[i++];
            J.Text = text.assign[i++];
            K.Text = text.assign[i++];
            L.Text = text.assign[i++];
            M.Text = text.assign[i++];
            N.Text = text.assign[i++];
            O.Text = text.assign[i++];
            P.Text = text.assign[i++];
            Q.Text = text.assign[i++];
            R.Text = text.assign[i++];
            S.Text = text.assign[i++];
            T.Text = text.assign[i++];
            U.Text = text.assign[i++];
            V.Text = text.assign[i++];
            W.Text = text.assign[i++];
            X.Text = text.assign[i++];
            Y.Text = text.assign[i++];
            Z.Text = text.assign[i++];
        }

        private string MakePrintable(string Text)
        {
            StringBuilder res = new StringBuilder(Text.Length * 2);
            int i = 0;

            for (; i + 5 < Text.Length;)
            {
                res.Append(Text.Substring(i, 5) + " ");
                if ((i = i + 5) % 8 == 0)
                {
                    res.Append("\n");
                }
            }
            res.Append(Text.Substring(i) + "\n");

            return res.ToString();
        }
        //End Opening File

        //Reset
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                text = new Data(text.cipherText);
                P1.Text = text.Plaintext;
                shift = 0;
                runs = 0;
                K1.Text = shift.ToString();

                SetTBs();
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }
        //End Reset

        //Save a report
        private void printReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = "Report.txt";
                save.Filter = "Text File | *.txt";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter writer = new StreamWriter(save.OpenFile());

                    string report = text.ToString();
                    int cur = 0, next = report.IndexOf('$');

                    while (next != -1)
                    {
                        writer.WriteLine(report.Substring(cur, next - cur));
                        cur = next + 1;
                        next = report.IndexOf('$', cur);
                    }

                    writer.WriteLine(report.Substring(cur));

                    writer.Dispose();
                }
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }
        //End Report

        //Shift
        private void ShiftGuess_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                K1.Text = "Shift: " + shift;
                text.setShift(shift++);
                P1.Text = MakePrintable(text.Plaintext);
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }
        //End Shift

        //Sub
        //Get the English Language weights for the ciphertext
        private void SubGuess_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                MakeGuess(runs++);
                P2.Text = MakePrintable(text.Plaintext);
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }

        private void MakeGuess(int runs)
        {
            double[,] guessWeight = new double[26, 26];

            UnigramAnalisis(guessWeight);
            if (runs > 0)
            {
                BigramAnalisis(guessWeight);
                TrigramAnalisis(guessWeight);
            }

            printArray(guessWeight);
        }

        private void printArray(double[,] guessWeight)
        {
            StringBuilder res = new StringBuilder(26 * 61);
            List<Tuple<int, int, double>> topPick;

            res.Append("\n");

            for (int x = 0; x < 26; x++)
            {
                topPick = GetTopValuesInRow(guessWeight, x);

                res.AppendFormat("{0,4:s}  | ", ((char) (x + 'A')).ToString());

                for (int k = 0; k < 3 && k < topPick.Count; k++)
                {
                    res.AppendFormat("{0,4:s} ", ((char) (topPick[k].Item2 + 'A')).ToString());
                    res.AppendFormat("{0,8:f6} ", topPick[k].Item3);
                    res.Append(" | ");
                }


                res.Append("\n");
            }

            res.Append("\n");

            K2.Text = text.getAssign();
            K2.Text += res.ToString();
        }

        private List<Tuple<int, int, double>> GetTopValuesInRow(double[,] guessWeight, int x)
        {
            List<Tuple<int, int, double>> topPick = new List<Tuple<int, int, double>>();

            int y;
            double w;
            bool found;

            if (text.assign[x] == "-")
            {
                for (y = 0; y < 26; y++)
                {
                    if (NotInArray(y))
                    {
                        if (!topPick.Any())
                        {
                            topPick.Add(new Tuple<int, int, double>(x, y, guessWeight[x, y]));
                        }
                        else
                        {
                            found = false;
                            w = guessWeight[x, y];

                            for (int k = 0; k < topPick.Count; k++)
                            {
                                if (!topPick.Contains(new Tuple<int, int, double>(x, y, w)))
                                {
                                    if (w > topPick[k].Item3)
                                    {
                                        topPick.Insert(k, new Tuple<int, int, double>(x, y, w));
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                            {
                                topPick.Add(new Tuple<int, int, double>(x, y, w));
                            }
                        }
                    }
                }
            }
            return topPick;
        }

        private bool NotInArray(int j)
        {
            foreach (var item in text.assign)
            {
                if (item == ((char) (j + 'A')).ToString())
                {
                    return false;
                }
            }
            return true;
        }

        private void UnigramAnalisis(double[,] guessWeight)
        {
            int x, y, curr, diff = engData.charFreq.Length - cipherTextData.uGramArray.Length;

            //Char freq weight
            for (int i = 0; i < cipherTextData.uGramArray.Length; i++)
            {
                for (int j = 0; j < wieghts.Length; j++)
                {
                    curr = i - 3 + j;
                    if (curr >= cipherTextData.uGramArray.Length)
                    {
                        curr = i;
                    }
                    if (curr > 0)
                    {
                        x = cipherTextData.uGramArray[curr].Item1[0] - 'A';
                        y = engData.charFreq[i + diff].Item1[0] - 'A';
                        guessWeight[x, y] += wieghts[j] * (50.0 / (101 - i));
                    }
                }
            }
        }

        private void BigramAnalisis(double[,] guessWeight)
        {
            int letter1, letter2;
            double countMod = 0, bigramMod;
            ;
            string bCipher;
            bool inArray1, inArray2;

            for (int i = 0; i < cipherTextData.bGramArray.Length; i++)
            {
                bCipher = cipherTextData.bGramArray[i].Item1;
                bigramMod = ((double) cipherTextData.bGramArray[i].Item2 / cipherTextData.bGramArray[cipherTextData.bGramArray.Length - 1].Item2) + .5;

                countMod = 0;

                letter1 = text.assign[cipherTextData.bGramArray[i].Item1[0] - 'A'][0];
                letter2 = text.assign[cipherTextData.bGramArray[i].Item1[1] - 'A'][0];

                inArray1 = letter1 == '-';
                inArray2 = letter2 == '-';

                if (inArray1 && inArray2 || !inArray1 && !inArray2)
                { }
                else if (inArray1)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        countMod += engData.bigramData[j, letter2 - 'A'];
                    }

                    countMod = countMod / 26;

                    for (int j = 0; j < 26; j++)
                    {
                        guessWeight[cipherTextData.bGramArray[i].Item1[0] - 'A', j] += ((engData.bigramData[j, letter2 - 'A'] / countMod * (bigramMod) - 1));
                    }
                }
                else
                {
                    for (int j = 0; j < 26; j++)
                    {
                        countMod += engData.bigramData[letter1 - 'A', j];
                    }

                    for (int j = 0; j < 26; j++)
                    {
                        guessWeight[cipherTextData.bGramArray[i].Item1[0] - 'A', j] += ((engData.bigramData[letter1 - 'A', j] / countMod * (bigramMod) - 1));
                    }
                }
            }
        }

        private void TrigramAnalisis(double[,] guessWeight)
        {
            int x, y;
            double mod;
            string tCipher, tEng;

            for (int i = 0; i < cipherTextData.tGramArray.Length; i++)
            {
                tCipher = cipherTextData.tGramArray[i].Item1;

                for (int j = 0; j < engData.trigramData.Length; j++)
                {
                    mod = GetTMod(i, j);

                    if (mod > 0)
                    {
                        tEng = engData.trigramData[j].Item1;

                        for (int k = 0; k < 3; k++)
                        {
                            x = tCipher[k] - 'A';
                            y = tEng[k] - 'A';

                            guessWeight[x, y] += mod * (50.0 / (101 - j));
                        }
                    }
                }
            }
        }

        private double GetTMod(int pos1, int pos2)
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                if (text.assign[cipherTextData.tGramArray[pos1].Item1[i] - 'A'] == engData.trigramData[pos2].Item1[i].ToString())
                {
                    count++;
                }
            }
            if (count == 0)
            {
                return 0;
            }
            else if (count == 1)
            {
                return .1;
            }
            else
            {
                return .5;
            }
        }

        //Sub Assignment
        private void CharMap_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                text.setAssign(0, A.Text.ToUpper());
                text.setAssign(1, B.Text.ToUpper());
                text.setAssign(2, C.Text.ToUpper());
                text.setAssign(3, D.Text.ToUpper());
                text.setAssign(4, E.Text.ToUpper());
                text.setAssign(5, F.Text.ToUpper());
                text.setAssign(6, G.Text.ToUpper());
                text.setAssign(7, H.Text.ToUpper());
                text.setAssign(8, I.Text.ToUpper());
                text.setAssign(9, J.Text.ToUpper());
                text.setAssign(10, K.Text.ToUpper());
                text.setAssign(11, L.Text.ToUpper());
                text.setAssign(12, M.Text.ToUpper());
                text.setAssign(13, N.Text.ToUpper());
                text.setAssign(14, O.Text.ToUpper());
                text.setAssign(15, P.Text.ToUpper());
                text.setAssign(16, Q.Text.ToUpper());
                text.setAssign(17, R.Text.ToUpper());
                text.setAssign(18, S.Text.ToUpper());
                text.setAssign(19, T.Text.ToUpper());
                text.setAssign(20, U.Text.ToUpper());
                text.setAssign(21, V.Text.ToUpper());
                text.setAssign(22, W.Text.ToUpper());
                text.setAssign(23, X.Text.ToUpper());
                text.setAssign(24, Y.Text.ToUpper());
                text.setAssign(25, Z.Text.ToUpper());

                P2.Text = MakePrintable(text.Plaintext);
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }
        //End Sub

        //Affine
        private void AffCipherText1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AffCipherText2.Text != "" && AffPlainText1.Text != "" && AffPlainText2.Text != "")
            {
                MakeAffGuess();
            }
        }

        private void AffCipherText2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AffCipherText1.Text != "" && AffPlainText1.Text != "" && AffPlainText2.Text != "")
            {
                MakeAffGuess();
            }
        }
        private void AffGuess_Click(object sender, EventArgs e)
        {
            MakeAffGuess();
        }

        private void MakeAffGuess()
        {
            if (text != null)
            {
                Tuple<int, int> guess1;
                Tuple<int, int> guess2;

                guess1 = new Tuple<int, int>(AffCipherText1.Text.Substring(1, 1)[0] - 'A', AffPlainText1.Text.Substring(1, 1)[0] - 'A');
                guess2 = new Tuple<int, int>(AffCipherText2.Text.Substring(1, 1)[0] - 'A', AffPlainText2.Text.Substring(1, 1)[0] - 'A');

                int inverse = -1;

                inverse = GetZ26Inverse(guess1.Item2 - guess2.Item2);

                if (inverse > 0)
                {
                    text.a = MakePos(guess1.Item1 - guess2.Item1) * inverse % 26;
                    text.b = MakePos(guess1.Item1 - text.a * guess1.Item2) % 26;

                    int aInverse = GetZ26Inverse(text.a);
                    if (aInverse > 0)
                    {
                        for (int i = 0; i < 26; i++)
                        {
                            text.setAssign(i, ((char) (MakePos((i - text.b) * aInverse) % 26 + 'A')).ToString());
                        }
                    }

                    K3.Text = "A: " + text.a + " |B: " + text.b + "\n";
                    K3.Text += text.getAssign();
                    P3.Text = MakePrintable(text.Plaintext);
                }
                else
                {
                    K3.Text = "Invalid Assignment";
                }
            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
        }

        private int GetZ26Inverse(int v)
        {
            int vPos = MakePos(v);

            switch (vPos % 26)
            {
                case 1:
                    return 1;
                case 3:
                    return 9;
                case 5:
                    return 21;
                case 7:
                    return 15;
                case 9:
                    return 3;
                case 11:
                    return 19;
                case 15:
                    return 7;
                case 17:
                    return 23;
                case 19:
                    return 11;
                case 21:
                    return 5;
                case 23:
                    return 17;
                case 25:
                    return 25;
                default:
                    return -1;
            }
        }

        private int MakePos(int v)
        {
            int res = v;

            while (res < 0)
            {
                res += 26;
            }

            return res;
        }
        //End Affine

        //Hill Cipher
        private void HillGuess_Click(object sender, EventArgs e)
        {
            MakeHillGuess();
        }

        private void MakeHillGuess()
        {
            if (text != null)
            {
                int n = int.Parse(Hill_N.Text);
                int i;

                Matrix plain = null;
                Matrix cipher = null;
                Matrix cipherInverse = null;

                Tuple<string, string> guess1, guess2;

                guess1 = new Tuple<string, string>(HillCipherText1.Text.Substring(1, 2), HillPlainText1.Text.Substring(1, 2));
                guess2 = new Tuple<string, string>(HillCipherText2.Text.Substring(1, 2), HillPlainText2.Text.Substring(1, 2));


                int[] plainMatrix = new int[n * n];
                int[] cipherMatrix = new int[n * n];
                string p, c;

                c = guess1.Item1;
                p = guess1.Item2;
                for (i = 0; i < 2; i++)
                {
                    plainMatrix[i * n] = p[i] - 'A';
                    cipherMatrix[i * n] = c[i] - 'A';
                }

                c = guess2.Item1;
                p = guess2.Item2;
                for (i = 0; i < 2; i++)
                {
                    plainMatrix[i * n + 1] = p[i] - 'A';
                    cipherMatrix[i * n + 1] = c[i] - 'A';
                }

                plain = new Matrix(plainMatrix, n);
                cipher = new Matrix(cipherMatrix, n);

                cipherInverse = cipher.GetInverse();


                if (cipherInverse != null)
                {
                    text.keyInverse = Matrix.MultMatrix2(plain, cipherInverse);
                    text.key = text.keyInverse.GetInverse();

                    if (text.key != null)
                    {
                        text.KeyMatrixDecrypt();

                        K4.Text = "Encryption Key:\n" + text.key.ToString() + "\n\n";
                        K4.Text += "Decryption Key:\n" + text.keyInverse.ToString();
                        P4.Text = MakePrintable(text.Plaintext);
                    }
                    else
                    {
                        K4.Text = "Decryption Key Guess:\n" + text.keyInverse.ToString() + "\n\n";
                        K4.Text = "Invalid Key";
                    }
                }
                else
                {
                    K4.Text = "Invalid Guess";
                }

            }
            else
            {
                MessageBox.Show("No file selected", "Select a File", MessageBoxButtons.OK);
            }
            return;
        }

        private void HillCipherText1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HillCipherText2.Text != "" && HillPlainText1.Text != "" && HillPlainText2.Text != "")
            {
                MakeHillGuess();
            }
        }

        private void HillCipherText2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HillCipherText1.Text != "" && HillPlainText1.Text != "" && HillPlainText2.Text != "")
            {
                MakeHillGuess();
            }
        }
        //End Hill



        private void ShiftHelp_Click(object sender, EventArgs e)
        {
            //ToDo add help page
        }

        private void SubHelp_Click(object sender, EventArgs e)
        {
            //ToDo add help page
        }

        private void AffineHelp_Click(object sender, EventArgs e)
        {
            //ToDo add help page
        }

        private void HillHelp_Click(object sender, EventArgs e)
        {
            //ToDo add help page
        }
    }
}
