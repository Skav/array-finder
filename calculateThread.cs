using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace array_finder
{
    public class CalculateThread
    {
        private ConcurrentQueue<int> jobs;
        private ConcurrentQueue<string> results;
        private int threadNo;
        private string searchType;

        public CalculateThread(ConcurrentQueue<int> jobs, ConcurrentQueue<string> results, int threadNo, string searchType)
        {
            this.jobs = jobs;
            this.results = results;
            this.threadNo = threadNo;
            this.searchType = searchType;
        }

        public void Run()
        {
            if (searchType == "linear")
                StartLinearSearch();
            else if (searchType == "binary")
                StartBinarySearch();
            else
                Console.WriteLine("Invalid search type, work will not start");
        }

        private int[] GenerateDataSetFromParameters(int dataSetParams)
        {
            var dataSet = new int[dataSetParams];

            for (int i = 0; i < dataSetParams; i++)
                dataSet[i] = i;

            return dataSet;
        }

        private void StartLinearSearch()
        {
            Console.WriteLine($"==== Thread no. {threadNo} starting linear search ====");
            var _stopwatch = new Stopwatch();

            LinearSearch(new[] { 1, 2, 3 }, 1); // Init functions to avoid incorrect time values for 1st element in array
            LinearSearchInstr(new[] { 1, 2, 3 }, 1); 

            while (jobs.Count > 0)
            {
                Console.WriteLine($"==== Thread {threadNo}: Taking element from jobs ====");
                jobs.TryDequeue(out int dataSetParams); //getting job from queue
                if (dataSetParams == null) // checking if any jobs was avaliable to take
                    break;

                var dataSet = GenerateDataSetFromParameters(dataSetParams); //creating array with values on which will be measured algorithm efficiency

                Console.WriteLine($"==== Threaad {threadNo}: Starting measure tick for pessimistic case ====");

                _stopwatch.Reset();
                _stopwatch.Start();
                LinearSearch(dataSet, -1);
                _stopwatch.Stop();

                var maxTime = _stopwatch.ElapsedTicks;
                Console.WriteLine($"==== Thread {threadNo}: pessimistic case on {dataSet.Length} elements table takes {maxTime} ticks ====");
                results.Enqueue($"{dataSet.Length};{maxTime};pessimistic;linear;ticks"); // adding values to results queue

                var maxIndex = LinearSearchInstr(dataSet, -1);
                results.Enqueue($"{dataSet.Length};{maxIndex};pessimistic;linear;intr");  // adding values to results queue

                Console.WriteLine($"==== Thread {threadNo}: ended up measure pesimistic situation, starting up avg case ====");
                _stopwatch.Reset();
                _stopwatch.Start();
                LinearSearch(dataSet, 0);
                _stopwatch.Stop();
                results.Enqueue($"{dataSet.Length};{((double) maxTime + (double) _stopwatch.ElapsedTicks) / 2};avg;linear;ticks");  // adding values to results queue

                var minIndex = LinearSearchInstr(dataSet, 0);
                results.Enqueue($"{dataSet.Length};{((double) maxIndex + (double) minIndex) / 2};avg;linear;intr");  // adding values to results queue

                Console.WriteLine($"==== Thread {threadNo}: ended up measure linear search for {dataSet.Length} elements table ====");
            }

        }

        private void StartBinarySearch()
        {
            Console.WriteLine($"==== Thread {threadNo}: starting bianry search ====");
            var _stopwatch = new Stopwatch();

            BinarySearch(new[] { 1, 2, 3 }, 1); // Init a functions to avoid incorrect time values for 1st element in array
            BinarySearchInstr(new[] { 1, 2, 3 }, 1); 

            while (jobs.Count > 0)
            {
                Console.WriteLine($"==== Thread {threadNo}: Taking element from jobs ====");
                jobs.TryDequeue(out int dataSetParams); //getting job from queue
                if (dataSetParams == null) // checking if any jobs was avaliable to take
                    break;

                var dataSet = GenerateDataSetFromParameters(dataSetParams); //creating array with values on which will be measured algorithm efficiency

                Console.WriteLine($"==== Threaad {threadNo}: Starting measure tick for pessimistic case ====");
                _stopwatch.Reset();
                _stopwatch.Start();
                BinarySearch(dataSet, -1);
                _stopwatch.Stop();
                var maxTime = _stopwatch.ElapsedTicks;

                Console.WriteLine($"==== Thread {threadNo}: pessimistic case on {dataSet.Length} elements table took {maxTime} ticks ====");
                results.Enqueue($"{dataSet.Length};{maxTime};pessimistic;binary;ticks");  // adding values to results queue

                var maxIndex = BinarySearchInstr(dataSet, -1);
                results.Enqueue($"{dataSet.Length};{maxIndex};pessimistic;bianry;intr");  // adding values to results queue

                Console.WriteLine($"==== Thread {threadNo}: ended up calculing pesimistic situation, starting up avg situation ====");
                ulong sumOfTicks = 0;

                // measure execution time for every item in array and sum them up
                foreach (var item in dataSet)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    BinarySearch(dataSet, item);
                    _stopwatch.Stop();

                    sumOfTicks += (ulong) _stopwatch.ElapsedTicks;
                }
                results.Enqueue($"{dataSet.Length};{(double) sumOfTicks / (double) dataSet.Length};avg;binary;ticks");  // adding values to results queue
                Console.WriteLine($"==== Thread {threadNo}: ended up calculing avg situation for ticks, it took {sumOfTicks} ticks ====");

                ulong sumOfInstr = 0;

                // measure number of checks for every item in array and sum them up
                foreach (var item in dataSet)  
                {
                    sumOfInstr += (ulong) BinarySearchInstr(dataSet, 1);
                }

                results.Enqueue($"{dataSet.Length};{(double) sumOfInstr / (double) dataSet.Length};avg;binary;intr");  // adding values to results queue
                Console.WriteLine($"==== Thread {threadNo}: ended up calculing avg situation for intr, it took {sumOfInstr} instr ====");

                Console.WriteLine($"==== Thread {threadNo}: ended up calculing binary search for {dataSet.Length} elements table ====");
            }
        }

        private bool BinarySearch(int[] dataSet, int number)
        {
            int left = 0;
            int right = dataSet.Length - 1;
            int mid;

            while (left <= right)
            {
                mid = (left + right) / 2;
                if (dataSet[mid] == number) return true;
                else if (dataSet[mid] > number) right = mid - 1;
                else left = mid + 1;
            }

            return false;
        }

        private int BinarySearchInstr(int[] dataSet, int number)
        {
            int left = 0;
            int right = dataSet.Length - 1;
            int mid;
            int index = 0;

            while (left <= right)
            {
                mid = (left + right) / 2;
                index++;
                if (dataSet[mid] == number) return index;
                else if (dataSet[mid] > number)
                {
                    right = mid - 1;
                    index++;
                }
                else left = mid + 1;
            }

            return index;
        }

        private bool LinearSearch(int[] dataSet, int number)
        {
            for (int i = 0; i < dataSet.Length; i++)
                if (dataSet[i] == number)
                    return true;
            return false;
        }

        private int LinearSearchInstr(int[] dataSet, int number)
        {
            int index = 0;
            for (int i = 0; i < dataSet.Length; i++)
            {
                index++;
                if (dataSet[i] == number)
                    return index;
            }

            return index;
        }
    }
}