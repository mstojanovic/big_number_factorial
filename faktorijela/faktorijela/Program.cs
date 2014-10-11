using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;
namespace faktorijela
{
        
    class Program
    {
        static readonly int procno = Environment.ProcessorCount;

        public static BigInteger Factorial(long x)
        {

            var parallelTasks =
                Enumerable.Range(1, procno)
                            .Select(i => Task.Factory.StartNew(() => Multiply(x, i),
                                         TaskCreationOptions.LongRunning))
                            .ToArray();


            Task.WaitAll(parallelTasks);


            BigInteger finalResult = 1;

            foreach (var partialResult in parallelTasks.Select(t => t.Result))
            {
                finalResult *= partialResult;
            }

            return finalResult;
        }


        public static BigInteger Multiply(long upperBound, int startFrom)
        {
            BigInteger result = 1;

            for (var i = startFrom; i <= upperBound; i += procno)
                result *= i;

            return result;
        }

        public static BigInteger Factorial_user(long x, int procnum)
        {
            Stopwatch utime = new Stopwatch();
            utime.Start();
            utime.Stop();
            BigInteger finalResult = 1;

			for(int j=1; j<=procnum; j++)
			{
                utime.Restart();
                var parallelTasks = Enumerable.Range(1, j).Select(i => Task.Factory.StartNew(() => Multiply_user(x, i, j),TaskCreationOptions.LongRunning)).ToArray();

				Task.WaitAll(parallelTasks);

				finalResult = 1;

				foreach (var partialResult in parallelTasks.Select(t => t.Result))
				{
					finalResult *= partialResult;
				}
                				
                utime.Stop();
                Console.WriteLine("Paralelno vrijeme na {0} dretve: {1}:", j, utime.Elapsed);
			}
            return finalResult;
        }


        public static BigInteger Multiply_user(long upperBound, int startFrom, int j)
        {
            BigInteger result = 1;

            for (var i = startFrom; i <= upperBound; i += j)
                result *= i;

            return result;
        }

        static void Main(string[] args)
        {
            Stopwatch ftime = new Stopwatch();
            Stopwatch stime = new Stopwatch();
            BigInteger fres = 0;
            
            Console.WriteLine("Broj:");
            int num = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Trenutno broj dretvi na kojima se racuna je {0}.", procno);
            Console.WriteLine("Unijeti broj dretvi na kojima se racuna, 0 za racun samo sa trenutnim broju dretvi:", procno);
            int thno = Convert.ToInt32(Console.ReadLine());
                        
            Console.WriteLine("Da li da se izvrsi i sekvencijalni racun?(y/n):");
            char sec = Convert.ToChar(Console.ReadLine());

            if (thno == 0)
            {
                ftime.Start();

                fres = Factorial(num);

                ftime.Stop();
                Console.WriteLine("Paralelno vrijeme za n={0}: {1}:", num, ftime.Elapsed);
                //Console.WriteLine(fres);
            }
            else
            {
                fres = Factorial_user(num, thno);
            }

            if (sec == 'y' || sec == 'Y' || sec == 'd' || sec == 'D')
            {
                BigInteger sres = 1;
                stime.Start();
                while (num > 1)
                {
                    sres = sres * num;
                    num--;
                }
                stime.Stop();
                Console.WriteLine("Sekvencijalno vrijeme za n={0}: {1}:", num, stime.Elapsed);
                //Console.WriteLine(sres);
            }

            Console.WriteLine("Ispis rezultata?(y/n)");
            char pres = Convert.ToChar(Console.ReadLine());
            if (pres == 'y' || pres == 'Y' || pres == 'd' || pres == 'D')
                Console.WriteLine(fres);
            
        }

    }
}

