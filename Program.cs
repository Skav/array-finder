using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace array_finder
{
    class Program
    {
        static int menu()
        {
            int choosenOption = -1;
            bool invalidInput = true;
            while (invalidInput)
            {
                Console.WriteLine("1. Create new data set");
                Console.WriteLine("2. Load existing data set");
                Console.WriteLine("3. Calculate execution time for linear search");
                Console.WriteLine("4. Calculate execution time for binary search");
                Console.WriteLine("5. Exit");
                Console.Write("Chooose what you want to do: ");

                if (!int.TryParse(Console.ReadLine(), out choosenOption))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try again");
                }
                else
                {
                    if (choosenOption >= 1 && choosenOption <= 5)
                        invalidInput = false;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Number out of range, try again");
                    }
                }
            }

            return choosenOption;
        }

        static void Main(string[] args)
        {
            var isRunning = true;
            var dataSet = new List<int>();
            var dataSetFile = "";

            while (isRunning)
            {
                switch (menu())
                {
                    case 1:
                        try
                        {
                            CreateDataSetParameters();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 2:
                        try
                        {
                            (dataSet, dataSetFile) = LoadDataSet();
                            Console.WriteLine($"===  Load data set from file: {dataSetFile}  ===");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    case 3:
                        try
                        {
                            StartCalculating(dataSet, dataSetFile, "linear");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 4:
                        try
                        {
                            StartCalculating(dataSet, dataSetFile, "binary");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        break;
                    case 5:
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Unexcepted value, closing program");
                        Environment.Exit(0);
                        break;
                }
            }
        }

        static void CreateDataSetParameters()
        {
            var fileOperator = new FileOperator();
            int minArrayValue, maxArrayValue, arrayNumber;
            Console.Write($"Enter minimum array lenght (0-{int.MaxValue}): ");
            while (!int.TryParse(Console.ReadLine(), out minArrayValue) || minArrayValue < 0 ||
                   minArrayValue > int.MaxValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write($"Enter maximum array lenght ({minArrayValue}-{int.MaxValue}): ");
            while (!int.TryParse(Console.ReadLine(), out maxArrayValue) || maxArrayValue < 0 ||
                   maxArrayValue > int.MaxValue || maxArrayValue < minArrayValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write($"Enter numbers of arrays (1-{maxArrayValue - minArrayValue}): ");
            while (!int.TryParse(Console.ReadLine(), out arrayNumber) || arrayNumber <= 0 ||
                   arrayNumber > int.MaxValue || arrayNumber > maxArrayValue-minArrayValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.WriteLine("Data set save to: " +
                              fileOperator.CreateDataSetFile(GenerateDataSetParameters(minArrayValue, maxArrayValue, 
                                  arrayNumber)));
        }

        static (List<int>, string) LoadDataSet()
        {
            var fileOperator = new FileOperator();
            var dataSetsFiles = fileOperator.GetAllDataSetFiles();
            var dataSetOptions = new string[dataSetsFiles.Count];

            Console.WriteLine("Choose which dataset would you like to load: ");

            int choosenOption = -1;
            do
            {
                if (choosenOption != -1)
                    Console.WriteLine("Invalid input");

                int index = 0;

                //filling up table with available files and printing it
                foreach (var key in dataSetsFiles.Keys)
                {
                    dataSetOptions[index] = key;
                    index++;
                    Console.WriteLine($"{index}. {key}");
                }
            } while (!int.TryParse(Console.ReadLine(), out choosenOption) || choosenOption < 1 ||
                     choosenOption > dataSetOptions.Length);

            var dataSetParameters = fileOperator.ReadDataSetFromFile(dataSetOptions[choosenOption - 1]);
            var dataSetFile = dataSetOptions[choosenOption - 1]; //saving name of choosen data set file

            return (dataSetParameters, dataSetFile);
        }

        static void StartCalculating(List<int> dataSet, string dataSetFile, string searchType)
        {
            if (!dataSet.Any())
                throw new ArgumentException("dataSet is empty, you need to load it first!");

            if (!searchType.Equals("linear") && !searchType.Equals("binary"))
                throw new ArgumentException("Wrong search type, choose between binary or linear");

            var jobs = new ConcurrentQueue<int>(); //creating queue with jobs to calculate by program
            var results = new ConcurrentQueue<string>(); //creating queue to store results after calculating data by program
            var threadsList = new List<Thread>();

            // filling jobs
            foreach (var item in dataSet)
            {
                jobs.Enqueue(item);
            }

            //Init a threads classes and threads, you can change number of threads by editing a value of max loop iteration - default 1 
            for (int i = 0; i < 1; i++)
            {
                var threadObject = new CalculateThread(jobs, results, i, searchType);
                var thread = new Thread(new ThreadStart(threadObject.Run));
                thread.Start();
                threadsList.Add(thread);
            }

            //waiting for threads to finish jobs
            foreach (var thread in threadsList)
            {
                thread.Join();
            }

            // After all threads end job, creating .csv file with data
            if (jobs.Count == 0 || results.Count > 0)
            {
                var fileOperator = new FileOperator();
                var filePath = fileOperator.CreateResultsFile(searchType, results.ToArray());
                Console.WriteLine("Task ended sucesfully");
                Console.WriteLine($"File was saved to: {filePath}");
            }
        }

        /*
         * This functione create a array which contains a lenght of arrays which will be use later to create actual arrays with values to measured algorithm efficiency
         * 
         * This function only create array lenght, not actual arrays with values in it mostly because files with every value for every table was too big
        */
        static int[] GenerateDataSetParameters(int minArrayLenght, int maxArrayLenght, int numberOfArrays)
        {
            if (numberOfArrays > maxArrayLenght - minArrayLenght || minArrayLenght > maxArrayLenght)
                throw new ArgumentException("Parameters values are incorrect");

            var dataSets = new int[numberOfArrays];
            int arrayLenghtJump = (maxArrayLenght - minArrayLenght) / numberOfArrays;
            for (int i = 0; i < numberOfArrays; i++)
            {
                dataSets[i] = minArrayLenght + i*arrayLenghtJump;
            }

            return dataSets;
        }
    }
}