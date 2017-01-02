using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LineInter
{
    public class Program
    {
        public const int quan = 2048 * 2;

        public static float[] vecx = new float[quan];
        public static float[] vecy = new float[quan];
        public static float[] vecz = new float[quan];

        public static float[] vecx2 = new float[quan];
        public static float[] vecy2 = new float[quan];
        public static float[] vecz2 = new float[quan];

        public static float[] vecx3 = new float[quan];
        public static float[] vecy3 = new float[quan];
        public static float[] vecz3 = new float[quan];

        public  static void Main(string[] args)
        {
           // int quan = 2048*2;

            Vector3[] V1 = new Vector3[quan];
            Vector3[] V2 = new Vector3[quan];
            Vector3[] V3 = new Vector3[quan];

            var vecsiz = Vector<float>.Count;



            //float[] vecx = new float[quan];
            //float[] vecy = new float[quan];
            //float[] vecz = new float[quan];

            //float[] vecx2 = new float[quan];
            //float[] vecy2 = new float[quan];
            //float[] vecz2 = new float[quan];

            //float[] vecx3 = new float[quan];
            //float[] vecy3 = new float[quan];
            //float[] vecz3 = new float[quan];



            Random ra = new Random(quan);

            for (int i = 0; i < quan; i++)
            {
                V1[i].X = (float)ra.NextDouble() * 10f;
                V1[i].Y = (float)ra.NextDouble() * 10f;
                V1[i].Z = (float)ra.NextDouble() * 10f;

                vecx[i] = V1[i].X;
                vecy[i] = V1[i].Y;
                vecz[i] = V1[i].Z;

                V2[i].X = (float)ra.NextDouble() * 10f;
                V2[i].Y = (float)ra.NextDouble() * 10f;
                V2[i].Z = (float)ra.NextDouble() * 10f;

                vecx2[i] = V2[i].X;
                vecy2[i] = V2[i].Y;
                vecz2[i] = V2[i].Z;

                V3[i].X = (float)ra.NextDouble() * 10f;
                V3[i].Y = (float)ra.NextDouble() * 10f;
                V3[i].Z = (float)ra.NextDouble() * 10f;

                vecx3[i] = V3[i].X;
                vecy3[i] = V3[i].Y;
                vecz3[i] = V3[i].Z;
            }



            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Vector3 tmp1;
                Vector3 tmp2;

                int counter = 0;
                for (int i = 0; i < 5000; i++)
                {
                    for (int ind = 0; ind < quan; ind++)
                    {
                        tmp1 = (V1[ind] - V3[ind]);
                        tmp2 = (V2[ind] - V3[ind]);
                        tmp1.Z = 0;
                        tmp2.Z = 0;

                        var area = (Vector3.Cross(tmp1, tmp2)).Z;

                        if (area > 2)
                            counter++;
                    }
                }

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds + " ms NaiveSIMD " + counter);


                sw = new Stopwatch();
                sw.Start();


                Vector<float> v1xs = new Vector<float>();
                Vector<float> v2xs = new Vector<float>();
                Vector<float> v3xs = new Vector<float>();

                Vector<float> v1ys = new Vector<float>();
                Vector<float> v2ys = new Vector<float>();
                Vector<float> v3ys = new Vector<float>();

                //Vector<float> v1z = new Vector<float>();
                //Vector<float> v2z = new Vector<float>();
                //Vector<float> v3z = new Vector<float>();

                Vector<float> kps = new Vector<float>();
                Vector<float> kp2s = new Vector<float>();


                counter = 0;
                for (int i = 0; i < 5000; i++)
                {
                    for (int ind = 0; ind < quan; ind += vecsiz)
                    {
                        v1xs = new Vector<float>(vecx, ind);
                        v2xs = new Vector<float>(vecx2, ind);
                        v3xs = new Vector<float>(vecx3, ind);

                        v1ys = new Vector<float>(vecy, ind);
                        v2ys = new Vector<float>(vecy2, ind);
                        v3ys = new Vector<float>(vecy3, ind);

                        //v1z = new Vector<float>(vecz, ind);
                        //v2z = new Vector<float>(vecz2, ind);
                        //v3z = new Vector<float>(vecz3, ind);

                        v1xs -= v3xs;
                        v2xs -= v3xs;

                        v1ys -= v3ys;
                        v2ys -= v3ys;

                        //v1z -= v3z;
                        //v2z -= v3z;

                        kps = v1xs * v2ys;
                        kp2s = v2xs * v1ys;

                        kps -= kp2s;

                        //i=0
                        //j=0


                        for (int ve = 0; ve < vecsiz; ve++)
                        {
                            if (kps[ve] > 2)
                                counter++;
                        }

                    }
                }

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds + " ms SuperSIMD " + counter);


                //i might go with the supersimd instead of trying to fixing this
                sw = new Stopwatch();
                sw.Start();
                counter = 0;
                for (int i = 0; i < 5000; i++)
                {
                    Parallel.For(0, quan/ vecsiz, ind =>
                    {
                        var inds = ind * vecsiz;
                        Vector<float> v1x = new Vector<float>(vecx, inds);
                        Vector<float> v2x = new Vector<float>(vecx2, inds);
                        Vector<float> v3x = new Vector<float>(vecx3, inds);

                        Vector<float> v1y = new Vector<float>(vecy, inds);
                        Vector<float> v2y = new Vector<float>(vecy2, inds);
                        Vector<float> v3y = new Vector<float>(vecy3, inds);

                        Vector<float> kp = new Vector<float>();
                        Vector<float> kp2 = new Vector<float>();



                        //v1z = new Vector<float>(vecz, ind);
                        //v2z = new Vector<float>(vecz2, ind);
                        //v3z = new Vector<float>(vecz3, ind);

                        v1x -= v3x;
                        v2x -= v3x;

                        v1y -= v3y;
                        v2y -= v3y;

                        //v1z -= v3z;
                        //v2z -= v3z;

                        kp = v1x * v2y;
                        kp2 = v2x * v1y;

                        kp -= kp2;

                        //i=0
                        //j=0


                        for (int ve = 0; ve < vecsiz; ve++)
                        {
                            if (kp[ve] > 2)
                                System.Threading.Interlocked.Add(ref counter, 1);
                        }

                    });
                }

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds + " ms CrapySIMD " + counter);
                Console.WriteLine();
            }
        }
    }
}
