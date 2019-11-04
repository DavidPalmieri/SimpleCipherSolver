using System;
using System.Text;

namespace SubstitutionBreaker
{
    //This class manages a matrix Z26
    internal class Matrix
    {
        public int[] matrix;
        public int size;

        public override string ToString()
        {
            StringBuilder res = new StringBuilder(2 * size * size * 11);

            for (int i = 0; i < matrix.Length; i++)
            {
                if (i % size == 0 && i != 0)
                {
                    res.Append("\n");
                }

                res.AppendFormat("{0,4} ",matrix[i]);
            }

            return res.ToString();
        }

        public Matrix(int size)
        {
            System.Random r = new System.Random(1);

            this.size = size;
            matrix = new int[size * size];
        }

        public Matrix(int[] matrix, int size)
        {
            this.matrix = matrix;
            this.size = size;
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

        private static int MakePos(int v)
        {
            int res = v;

            while (res < 0)
            {
                res += 26;
            }

            return res;
        }

        public static Matrix SubMatrix(int i, int j, Matrix m)
        {
            Matrix res = new Matrix(m.size - 1);
            int posM, posRes;
            for (posM = 0, posRes = 0; posM < m.matrix.Length; posM++)
            {
                if (posM % m.size == i || posM >= j * m.size && posM < (j * m.size + m.size))
                { }
                else
                {
                    res.matrix[posRes++] = m.matrix[posM];
                }
            }

            return res;
        }

        public static int Determinent(Matrix m)
        {
            if (m.size == 2)
            {
                return MakePos( m.matrix[0] * m.matrix[3]-  m.matrix[1] * m.matrix[2])% 26;
            }
            else
            {
                int res = 0;
                int[] partialRes = new int[m.size];

                for (int i = 0; i < partialRes.Length; i++)
                {
                    partialRes[i] = m.matrix[i] * Determinent(SubMatrix(i, 0, m)) * (int) Math.Pow(-1, i);
                }

                foreach (var d in partialRes)
                {
                    res += d;
                }

                return MakePos(res) % 26;
            }
        }

        public Matrix GetInverse()
        {
            int det = Determinent(this);
            int detIn = GetZ26Inverse(det);
            if (det == 0||detIn<=0)
            {
                return null;
            }
            else if (size == 2 )
            {
                return GetInverse2(detIn);
            }
            else
            {
                return GetInverseGeneral();
            }
        }

        private Matrix GetInverse2(int detInverse)
        {
            Matrix res = new Matrix(2);

            res.matrix[0] = MakePos(matrix[3] * detInverse) % 26;
            res.matrix[1] = MakePos(-matrix[1] * detInverse) % 26;
            res.matrix[2] = MakePos(-matrix[2] * detInverse) % 26;
            res.matrix[3] = MakePos(matrix[0] * detInverse) % 26;

            return res;
        }

        //May not work
        private Matrix GetInverseGeneral()
        {
            Matrix res = new Matrix(size);
            res.matrix = GetID(size);
            Matrix workingMat = new Matrix(size);
            matrix.CopyTo(workingMat.matrix, 0);

            for (int i = 0; i < size; i++)
            {
                if (workingMat.matrix[GetPos(i, i)] * (workingMat.matrix[GetPos(i, i)]) == 1)
                { }
                //Swap
                if (workingMat.matrix[GetPos(i, i)] == 0)
                {
                    SwapRows(workingMat, res, i);
                }
                //Scale
                ScaleRow(workingMat, res, i);
                //
                AddRow(workingMat, res, i);
            }

            return res;
        }

        private void CleanUp(Matrix m)
        {
            for (int i = 0; i < m.matrix.Length; i++)
            {
                if (m.matrix[i] < 0)
                {
                    m.matrix[i] = MakePos(m.matrix[i]);
                }
                else if (m.matrix[i] >= 26)
                {
                    m.matrix[i] = m.matrix[i] % 26;
                }
            }
        }

        private int FindGCD(int item1, int item2)
        {
            return FindGCDHelper(Math.Max(item1, item2), Math.Min(item1, item2));
        }

        private int FindGCDHelper(int item1, int item2)
        {
            int x = item1, y = item2;

            while (y != 0)
            {
                int tmp = x % y;
                x = y;
                y = tmp;
            }

            return x;
        }

        private int[] GetID(int size)
        {
            int[] res = new int[size * size];
            int i, j;
            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    if (j == i)
                    {
                        res[GetPos(i, j)] = 1;
                    }
                    else
                    {
                        res[GetPos(i, j)] = 0;
                    }
                }
            }
            return res;
        }

        private void AddRow(Matrix workingMat, Matrix res, int i)
        {
            int[] wrkRow = new int[size];
            int[] resRow = new int[size];
            int scalarNum;
            int rWrk, rRes;

            for (int j = 0; j < size; j++)
            {
                wrkRow[j] = workingMat.matrix[GetPos(j, i)];
                resRow[j] = res.matrix[GetPos(j, i)];
            }

            for (int j = 0; j < size; j++)
            {
                if (i == j)
                { }
                else
                {
                    scalarNum = -workingMat.matrix[GetPos(i, j)];
                    for (int k = 0; k < size; k++)
                    {
                        rWrk = workingMat.matrix[GetPos(k, j)] * wrkRow[k] + wrkRow[k] * workingMat.matrix[GetPos(k, j)] * scalarNum;
                        rRes = res.matrix[GetPos(k, j)] * resRow[k] + resRow[k] * res.matrix[GetPos(k, j)] * scalarNum;

                        workingMat.matrix[GetPos(k, j)] = rWrk;
                        res.matrix[GetPos(k, j)] = rRes;
                    }
                }
            }
            CleanUp(workingMat);
            CleanUp(res);
        }

        private void ScaleRow(Matrix workingMat, Matrix res, int i)
        {
            int startWrk = i * size;
            int scalarNum = workingMat.matrix[GetPos(i, i)];
            int rWrk, rRes;

            for (int j = startWrk; j < startWrk + size; j++)
            {
                rWrk = workingMat.matrix[j] * scalarNum;

                rRes = res.matrix[j] * scalarNum;

                workingMat.matrix[j] = rWrk;
                res.matrix[j] = rRes;
            }
            CleanUp(workingMat);
            CleanUp(res);
        }

        private void SwapRows(Matrix workingMat, Matrix res, int i)
        {
            int tmp;
            int startCurr, startWrk = i * size;

            for (int j = GetPos(i, i) + size; j < matrix.Length; j += size)
            {
                if (workingMat.matrix[j] != 0)
                {
                    startCurr = j / size * size;
                    for (int k = 0; k < size; k++)
                    {
                        tmp = workingMat.matrix[k + startCurr];
                        workingMat.matrix[k + startCurr] = workingMat.matrix[k + startWrk];
                        workingMat.matrix[k + startWrk] = tmp;

                        tmp = res.matrix[k + startCurr];
                        res.matrix[k + startCurr] = res.matrix[k + startWrk];
                        res.matrix[k + startWrk] = tmp;
                    }
                    CleanUp(workingMat);
                    CleanUp(res);
                    return;
                }
            }
            CleanUp(workingMat);
            CleanUp(res);
        }

        private int GetPos(int x, int y)
        {
            return x + size * y;
        }

        public static Matrix MultMatrix2(Matrix m1, Matrix m2)
        {
            Matrix res = new Matrix(m2.size);

            if (m1.size == m2.size)
            {
                res.matrix[0] = MakePos(m1.matrix[0] * m2.matrix[0] + m1.matrix[1] * m2.matrix[2])%26;
                res.matrix[1] = MakePos(m1.matrix[0] * m2.matrix[1] + m1.matrix[1] * m2.matrix[3])%26;
                res.matrix[2] = MakePos(m1.matrix[2] * m2.matrix[0] + m1.matrix[3] * m2.matrix[2])%26;
                res.matrix[3] = MakePos(m1.matrix[2] * m2.matrix[1] + m1.matrix[3] * m2.matrix[3])%26;
            }

            return res;
        }

        public static string MultMatrix2(string text, Matrix m)
        {
            StringBuilder res = new StringBuilder(3);

            if (text.Length == m.size)
            {
                res.Append( MakePos(text[0] * m.matrix[0] + text[1] * m.matrix[1]) % 26);
                res.Append( MakePos(text[0] * m.matrix[2] + text[1] * m.matrix[3]) % 26);
            }

            return res.ToString();
        }
    }
}