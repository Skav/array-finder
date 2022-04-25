using System.Collections.Concurrent;
using System.Diagnostics;

namespace array_finder;

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
        if(searchType == "linear")
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

        LinearSearch(new[] {1, 2, 3}, 1); // Init function to avoid incorrect time values for 1st element in array

        while (jobs.Count > 0)
        {
            Console.WriteLine($"==== Thread {threadNo}: Taking element from jobs ====");
            jobs.TryDequeue(out int dataSetParams);
            if (dataSetParams == null)
                break;
            
            var dataSet = GenerateDataSetFromParameters(dataSetParams);

            Console.WriteLine($"=== Threaad {threadNo}: Starting measure tick for pessimistic case ===");
            
            _stopwatch.Reset();
            _stopwatch.Start();
            LinearSearch(dataSet, -1);
            _stopwatch.Stop();
            var maxTime = _stopwatch.ElapsedTicks;

            Console.WriteLine($"=== Thread {threadNo}: pessimistic case on {dataSet.Length} elements table takes {maxTime} ticks ===");

            results.Enqueue($"{dataSet.Length},{maxTime},pessimistic,linear,ticks");
            
            var (resultMax, maxIndex) = LinearSearchInstr(dataSet, -1);
            results.Enqueue($"{dataSet.Length},{maxIndex},pessimistic,linear,intr");
            
            Console.WriteLine($"==== Thread {threadNo}: ended up measure pesimistic situation, starting up avg case ====");

            _stopwatch.Reset();
            _stopwatch.Start();
            LinearSearch(dataSet, 1);
            _stopwatch.Stop();
            results.Enqueue($"{dataSet.Length},{(maxTime + _stopwatch.ElapsedTicks) / 2},avg,linear,ticks");
            
            var (resultMin, minIndex) = LinearSearchInstr(dataSet, 1);
            results.Enqueue($"{dataSet.Length},{(maxIndex+minIndex) / 2},avg,linear,intr");

            Console.WriteLine($"==== Thread {threadNo}: ended up measure linear search for {dataSet.Length} elements table ====");
        }
        
    }

    private void StartBinarySearch()
    {
        Console.WriteLine($"==== Thread {threadNo}: starting bianry search ====");
        var _stopwatch = new Stopwatch();

        BinarySearch(new[] {1, 2, 3}, 1); // Init function to avoid incorrect time values for 1st element in array

        while (jobs.Count > 0)
        {
            Console.WriteLine($"==== Thread {threadNo}: Taking element from jobs ====");
            jobs.TryDequeue(out int dataSetParams);
            if (dataSetParams == null)
                break;

            var dataSet = GenerateDataSetFromParameters(dataSetParams);
            
            Console.WriteLine($"=== Threaad {threadNo}: Starting measure tick for pessimistic case ===");
            
            _stopwatch.Reset();
            _stopwatch.Start();
            BinarySearch(dataSet, -1);
            _stopwatch.Stop();
            var maxTime = _stopwatch.ElapsedTicks;

            Console.WriteLine($"=== Thread {threadNo}: pessimistic case on {dataSet.Length} elements table takes {maxTime} ticks ===");

            results.Enqueue($"{dataSet.Length},{maxTime},pessimistic,binary,ticks");
            
            var (resultMax, maxIndex) = BinarySearchInstr(dataSet, -1);
            results.Enqueue($"{dataSet.Length},{maxIndex},pessimistic,bianry,intr");
            
            Console.WriteLine($"==== Thread {threadNo}: ended up calculing pesimistic situation, starting up avg situation ====");

            _stopwatch.Reset();
            _stopwatch.Start();
            BinarySearch(dataSet, 1);
            _stopwatch.Stop();
            results.Enqueue($"{dataSet.Length},{(maxTime + _stopwatch.ElapsedTicks) / 2},avg,binary,ticks");
            
            var (resultMin, minIndex) = BinarySearchInstr(dataSet, 1);
            results.Enqueue($"{dataSet.Length},{(maxIndex+minIndex) / 2},avg,binary,intr");

            Console.WriteLine($"==== Thread {threadNo}: ended up calculing linear search for {dataSet.Length} elements table ====");
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
    
    private (bool, int) BinarySearchInstr(int[] dataSet, int number)
    {
        int left = 0;
        int right = dataSet.Length - 1;
        int mid;
        int index = 0;

        while (left <= right)
        {
            mid = (left + right) / 2;
            index++;
            if (dataSet[mid] == number) return (true, index);
            else if (dataSet[mid] > number)
            {
                right = mid - 1;
                index++;
            }
            else left = mid + 1;
        }

        return (false, index);
    }

    private bool LinearSearch(int[] dataSet, int number)
    {
        for (int i = 0; i < dataSet.Length; i++)
            if (dataSet[i] == number)
                return true;
        return false;
    }
    
    private (bool, int) LinearSearchInstr(int[] dataSet, int number)
    {
        int index = 0;
        for (int i = 0; i < dataSet.Length; i++)
        {
            index++;
            if (dataSet[i] == number)
                return (true, index);
        }

        return (false, index);
    }
}