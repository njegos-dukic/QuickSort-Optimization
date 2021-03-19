using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using VxSort;

namespace QuickSortOptimization
{
    class Demo
    {
        private static int[] vxArray;
        private static int[] embedArrray;
        private static int[] recArray;
        private static int[] parArray;
        private static int[] parInsArray;
        private static int[] taskArray;
        private static int[] dualArray;
        private static int[] dualInsArray;
        private static int[] parDualArray;
        private static int[] parInsDualArray;

        static void Main(string[] args)
        {
            Console.WriteLine("Optimization: \t | ON");
            Console.WriteLine("Number of cores: | " + Environment.ProcessorCount);
            Console.WriteLine("-------------------------\n");

            Exibit(1_000_000);
            Exibit(3_000_000);
            Exibit(5_000_000);
            Exibit(10_000_000);
            Exibit(30_000_000);
            Exibit(50_000_000);
            Exibit(100_000_000);
            Exibit(300_000_000);
        }

        private static void Exibit(long n)
        {
            Random prg = new Random();
            List<int> list = new List<int>();
            for (int i = 0; i < n; i++)
                list.Add(prg.Next());

            Console.WriteLine("Data: \t\t\t | " + ToLocale(n) + "\n");

            vxArray = list.ToArray();
            embedArrray = list.ToArray();
            recArray = list.ToArray();
            parArray = list.ToArray();
            parInsArray = list.ToArray();
            taskArray = list.ToArray();
            dualArray = list.ToArray();
            dualInsArray = list.ToArray();
            parDualArray = list.ToArray();
            parInsDualArray = list.ToArray();

            list.Clear();

            Stopwatch stopwatch;

            // VxSort
            {
                stopwatch = Stopwatch.StartNew();
                VxSort.VectorizedSort.UnstableSort<int>(vxArray);
                stopwatch.Stop();
                Console.WriteLine("NuGet Package VxSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("VxSort.txt", n + ": NuGet Package VxSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Ebmedded Sort
            {
                stopwatch = Stopwatch.StartNew();
                Array.Sort(embedArrray);
                stopwatch.Stop();
                Console.WriteLine("Embedded Array Sort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("ArraySort.txt", n + ": Embedded Array Sort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Recursive QS
            {
                stopwatch = Stopwatch.StartNew();
                Sort.RecursiveQuickSort(recArray);
                stopwatch.Stop();
                Console.WriteLine("Recursive QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("RecursiveQS.txt", n + ": Recursive QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Parallel QS
            {
                stopwatch = Stopwatch.StartNew();
                Sort.ParallelQuickSort(parArray);
                stopwatch.Stop();
                Console.WriteLine("Parallel QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("ParallelQS.txt", n + ": Parallel QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Parallel w\ Insertion
            {
                stopwatch = Stopwatch.StartNew();
                Sort.ParallelQuickSortPlusInsertionSort(parInsArray);
                stopwatch.Stop();
                Console.WriteLine("Paralell w\\ Insertion:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("ParIns.txt", n + ": Paralell w\\ Insertion:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Tasks Parallel QS
            {
                stopwatch = Stopwatch.StartNew();
                Sort.TaskQuickSort(taskArray);
                stopwatch.Stop();
                Console.WriteLine("Tasks QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("TasksQS.txt", n + ": Tasks QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Dual Pivot QS
            {
                stopwatch = Stopwatch.StartNew();
                Sort.DualPivotQuickSort(dualArray);
                stopwatch.Stop();
                Console.WriteLine("Dual Pivot QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("DualPivotQS.txt", n + ": Dual Pivot QuickSort:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Dual Pivot w\ Insertion
            {
                stopwatch = Stopwatch.StartNew();
                Sort.DualPivotQuickSortWithInsertion(dualInsArray);
                stopwatch.Stop();
                Console.WriteLine("Dual Pivot w\\ Insertion: | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("DualPivotIns.txt", n + ": Dual Pivot w\\ Insertion: | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Parallel Dual Pivot
            {
                stopwatch = Stopwatch.StartNew();
                Sort.ParallelDualPivotQuickSort(parDualArray);
                stopwatch.Stop();
                Console.WriteLine("Parallel Dual Pivot:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("ParallelDP.txt", n + ": Parallel Dual Pivot:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            // Parallel Dual Pivot w\ Insertion
            {
                stopwatch = Stopwatch.StartNew();
                Sort.ParallelDualPivotQuickSortWithInsertion(parInsDualArray);
                stopwatch.Stop();
                Console.WriteLine("Par. Dual Pivot w\\ Ins:\t | " + stopwatch.ElapsedMilliseconds + " ms");
                WriteResults("ParDPIns.txt", n + ": Par. Dual Pivot w\\ Ins:\t | " + stopwatch.ElapsedMilliseconds + " ms");
            }

            Console.WriteLine("-------------------------\n");

            TestSorting();
        }
        
        private static void WriteResults(string file, string content)
        {
            TextWriter tsw = new StreamWriter(file, true);
            tsw.WriteLine(content);
            tsw.Close();
        }

        private static bool CompareArrays(int[] arr1, int[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
                if (arr1[i] != arr2[i])
                    return false;

            return true;
        }

        private static String ToLocale(long n)
        {
            String returnValue = n.ToString();

            for (int i = returnValue.Length - 3; i > 0; i -= 3)
                returnValue = returnValue.Insert(i, ".");

            return returnValue;
        }

        private static void TestSorting()
        {
            if (!CompareArrays(embedArrray, vxArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, recArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, parArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, parInsArray))
                Console.WriteLine("1111  [Nizovi nisu jednaki]\n");

            if (!CompareArrays(vxArray, parInsArray))
                Console.WriteLine("1111  [Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, taskArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, dualArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, dualInsArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, parDualArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");

            if (!CompareArrays(embedArrray, parInsDualArray))
                Console.WriteLine("[Nizovi nisu jednaki]\n");
        }
    }
}