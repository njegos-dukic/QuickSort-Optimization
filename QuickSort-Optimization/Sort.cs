using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace QuickSortOptimization
{
    public static class Sort
    {
        private static int maxDepth = Environment.ProcessorCount;
        public static int insertionThreshold = 47; // Array length to use InsertionSort instead of SequentialQuickSort

        private static void Swap(int[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        private static int Partition(int[] array, int from, int to, int pivot)
        {
            var arrayPivot = array[pivot];
            Swap(array, pivot, to - 1);
            var newPivot = from;
            for (int i = from; i < to - 1; i++)
                if (array[i].CompareTo(arrayPivot) != 1)
                {
                    Swap(array, newPivot, i);
                    newPivot++;
                }

            Swap(array, newPivot, to - 1);
            return newPivot;
        }

        public static void RecursiveQuickSort(int[] array)
        {
            RecursiveQuickSort(array, 0, array.Length);
        }

        private static void RecursiveQuickSort(int[] array, int from, int to)
        {
            if (to == from)
                return;

            else
            {
                int pivot = from + (to - from) / 2;
                pivot = Partition(array, from, to, pivot);

                RecursiveQuickSort(array, from, pivot);
                RecursiveQuickSort(array, pivot + 1, to);
            }
        }

        public static void ParallelQuickSort(int[] array)
        {
            ParallelQuickSort(array, 0, array.Length, maxDepth);
        }

        private static void ParallelQuickSort(int[] array, int from, int to, int depthRemaining)
        {
            if (to == from)
                return;

            else
            {
                int pivot = from + (to - from) / 2;
                pivot = Partition(array, from, to, pivot);

                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelQuickSort(array, from, pivot, depthRemaining - 1),
                        () => ParallelQuickSort(array, pivot + 1, to, depthRemaining - 1));
                }


                else
                {
                    ParallelQuickSort(array, from, pivot, 0);
                    ParallelQuickSort(array, pivot + 1, to, 0);
                }

            }
        }

        public static void TaskQuickSort(int[] array)
        {
            TaskQuickSort(array, 0, array.Length, maxDepth);
        }

        private static void TaskQuickSort(int[] array, int from, int to, int depthRemaining)
        {
            if (to == from)
                return;

            else
            {
                int pivot = from + (to - from) / 2;
                pivot = Partition(array, from, to, pivot);

                if (depthRemaining > 0)
                {
                    List<Task> running = new List<Task>
                    {
                        Task.Factory.StartNew(() => TaskQuickSort(array, from, pivot, depthRemaining - 1)),
                        Task.Factory.StartNew(() => TaskQuickSort(array, pivot + 1, to, depthRemaining - 1))
                    };

                    Task.WaitAll(running.ToArray());
                }

                else
                {
                    TaskQuickSort(array, from, pivot, 0);
                    TaskQuickSort(array, pivot + 1, to, 0);
                }
            }
        }

        public static void ParallelQuickSortPlusInsertionSort(int[] array)
        {
            ParallelQuickSortPlusInsertionSort(array, 0, array.Length, maxDepth);
        }

        private static void ParallelQuickSortPlusInsertionSort(int[] array, int from, int to, int depthRemaining)
        {
            if (to - from <= insertionThreshold)
            {
                InsertionSort(array, from, to);
            }

            else
            {
                int pivot = from + (to - from) / 2;
                pivot = Partition(array, from, to, pivot);

                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelQuickSortPlusInsertionSort(array, from, pivot, depthRemaining - 1),
                        () => ParallelQuickSortPlusInsertionSort(array, pivot + 1, to, depthRemaining - 1));
                }
                else
                {
                    ParallelQuickSortPlusInsertionSort(array, from, pivot, 0);
                    ParallelQuickSortPlusInsertionSort(array, pivot + 1, to, 0);
                }
            }
        }

        private static int[] DualPivotPartition(int[] arr, int low, int high)
        {
            if (arr[low] > arr[high])
                Swap(arr, low, high);

            int j = low + 1;
            int g = high - 1;
            int k = low + 1;
            int p = arr[low];
            int q = arr[high];

            while (k <= g)
            {

                if (arr[k] < p)
                {
                    Swap(arr, k, j);
                    j++;
                }

                else if (arr[k] >= q)
                {
                    while (arr[g] > q && k < g)
                        g--;

                    Swap(arr, k, g);
                    g--;

                    if (arr[k] < p)
                    {
                        Swap(arr, k, j);
                        j++;
                    }
                }
                k++;
            }
            j--;
            g++;

            Swap(arr, low, j);
            Swap(arr, high, g);

            return new int[] { j, g };
        }

        public static void DualPivotQuickSort(int[] array)
        {
            DualPivotQuickSort(array, 0, array.Length - 1);
        }

        private static void DualPivotQuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {

                int[] piv;
                piv = DualPivotPartition(arr, low, high);

                DualPivotQuickSort(arr, low, piv[0] - 1);
                DualPivotQuickSort(arr, piv[0] + 1, piv[1] - 1);
                DualPivotQuickSort(arr, piv[1] + 1, high);
            }
        }

        public static void DualPivotQuickSortWithInsertion(int[] array)
        {
            DualPivotQuickSortWithInsertion(array, 0, array.Length - 1);
        }

        private static void DualPivotQuickSortWithInsertion(int[] arr, int low, int high)
        {
            if (high - low <= insertionThreshold)
            {
                InsertionSort(arr, low, high + 1);
            }

            else
            {
                int[] piv;
                piv = DualPivotPartition(arr, low, high);

                DualPivotQuickSortWithInsertion(arr, low, piv[0] - 1);
                DualPivotQuickSortWithInsertion(arr, piv[0] + 1, piv[1] - 1);
                DualPivotQuickSortWithInsertion(arr, piv[1] + 1, high);
            }
        }

        public static void ParallelDualPivotQuickSort(int[] array)
        {
            ParallelDualPivotQuickSort(array, 0, array.Length - 1, maxDepth);
        }

        private static void ParallelDualPivotQuickSort(int[] arr, int low, int high, int depthRemaining)
        {
            if (low < high)
            {
                int[] piv;
                piv = DualPivotPartition(arr, low, high);

                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelDualPivotQuickSort(arr, low, piv[0] - 1, depthRemaining - 1),
                        () => ParallelDualPivotQuickSort(arr, piv[0] + 1, piv[1] - 1, depthRemaining - 1),
                        () => ParallelDualPivotQuickSort(arr, piv[1] + 1, high, depthRemaining - 1));
                }

                else
                {
                    ParallelDualPivotQuickSort(arr, low, piv[0] - 1, 0);
                    ParallelDualPivotQuickSort(arr, piv[0] + 1, piv[1] - 1, 0);
                    ParallelDualPivotQuickSort(arr, piv[1] + 1, high, 0);
                }
            }
        }

        public static void TaskDualPivotQuickSort(int[] array)
        {
            TaskDualPivotQuickSort(array, 0, array.Length - 1, maxDepth);
        }

        private static void TaskDualPivotQuickSort(int[] arr, int low, int high, int depthRemaining)
        {
            if (low < high)
            {
                int[] piv;
                piv = DualPivotPartition(arr, low, high);

                if (depthRemaining > 0)
                {
                    List<Task> running = new List<Task>
                    {
                        Task.Factory.StartNew(() => TaskDualPivotQuickSort(arr, low, piv[0] - 1, depthRemaining - 1)),
                        Task.Factory.StartNew(() => TaskDualPivotQuickSort(arr, piv[0] + 1, piv[1] - 1, depthRemaining - 1)),
                        Task.Factory.StartNew(() => TaskDualPivotQuickSort(arr, piv[1] + 1, high, depthRemaining - 1))
                    };

                    Task.WaitAll(running.ToArray());
                }

                else
                {
                    TaskDualPivotQuickSort(arr, low, piv[0] - 1, 0);
                    TaskDualPivotQuickSort(arr, piv[0] + 1, piv[1] - 1, 0);
                    TaskDualPivotQuickSort(arr, piv[1] + 1, high, 0);
                }
            }
        }

        public static void ParallelDualPivotQuickSortWithInsertion(int[] array)
        {
            ParallelDualPivotQuickSortWithInsertion(array, 0, array.Length - 1, maxDepth);
        }

        private static void ParallelDualPivotQuickSortWithInsertion(int[] arr, int low, int high, int depthRemaining)
        {
            if (high - low <= insertionThreshold)
            {
                InsertionSort(arr, low, high + 1);
            }

            else
            {
                int[] piv;
                piv = DualPivotPartition(arr, low, high);

                if (depthRemaining > 0)
                {
                    Parallel.Invoke(
                        () => ParallelDualPivotQuickSortWithInsertion(arr, low, piv[0] - 1, depthRemaining - 1),
                        () => ParallelDualPivotQuickSortWithInsertion(arr, piv[0] + 1, piv[1] - 1, depthRemaining - 1),
                        () => ParallelDualPivotQuickSortWithInsertion(arr, piv[1] + 1, high, depthRemaining - 1));
                }

                else
                {
                    ParallelDualPivotQuickSortWithInsertion(arr, low, piv[0] - 1, 0);
                    ParallelDualPivotQuickSortWithInsertion(arr, piv[0] + 1, piv[1] - 1, 0);
                    ParallelDualPivotQuickSortWithInsertion(arr, piv[1] + 1, high, 0);
                }
            }
        }

        private static void InsertionSort(int[] array, int from, int to)
        {
            for (int i = from + 1; i < to; i++)
            {
                var a = array[i];
                int j = i - 1;

                while (j >= from && array[j].CompareTo(a) == 1)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = a;
            }
        }
    }
}