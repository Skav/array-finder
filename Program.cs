using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;

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
                Console.WriteLine("Chooose what you want to do: ");
                Console.WriteLine("1. Create new data set");
                Console.WriteLine("2. Load existing data set");
                Console.WriteLine("3. Calculate execution time for linear search");
                Console.WriteLine("4. Calculate execution time for binary search");
                Console.WriteLine("5. Exit");

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

            Console.WriteLine(int.MaxValue);

            while (isRunning)
            {
                switch (menu())
                {
                    case 1:
                        try
                        {
                            CreateDataSetParameters();
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Task terminated, do you want to try again? (y/n)");

                            string option = Console.ReadLine();
                            if (option.ToLower().Equals("y"))
                                CreateDataSetParameters();
                            else
                                Console.WriteLine("Back to the menu...");
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
            Console.Write("Enter minimum array lenght: ");
            while (!int.TryParse(Console.ReadLine(), out minArrayValue) || minArrayValue < 0 ||
                   minArrayValue > int.MaxValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write("Enter maximum array lenght: ");
            while (!int.TryParse(Console.ReadLine(), out maxArrayValue) || maxArrayValue < 0 ||
                   maxArrayValue > int.MaxValue || maxArrayValue < minArrayValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write("Enter numbers of arrays: ");
            while (!int.TryParse(Console.ReadLine(), out arrayNumber) || arrayNumber <= 0 ||
                   arrayNumber > int.MaxValue)
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
                foreach (var (key, value) in dataSetsFiles)
                {
                    dataSetOptions[index] = key;
                    index++;
                    Console.WriteLine($"{index}. {key}");
                }
            } while (!int.TryParse(Console.ReadLine(), out choosenOption) || choosenOption < 1 ||
                     choosenOption > dataSetOptions.Length);

            var dataSetParameters = fileOperator.ReadDataSetFromFile(dataSetOptions[choosenOption - 1]);
            var dataSetFile = dataSetOptions[choosenOption - 1];

            return (dataSetParameters, dataSetFile);
        }

        static void StartCalculating(List<int> dataSet, string dataSetFile, string searchType)
        {
            if (!dataSet.Any())
                throw new ArgumentException("dataSet is empty, you need to load it first!");

            if (!searchType.Equals("linear") && !searchType.Equals("binary"))
                throw new ArgumentException("Wrong search type, choose between binary or linear");

            var jobs = new ConcurrentQueue<int>();
            var results = new ConcurrentQueue<string>();
            var threadsList = new List<Thread>();

            foreach (var item in dataSet)
            {
                jobs.Enqueue(item);
            }

            for (int i = 0; i < 1; i++)
            {
                var threadObject = new CalculateThread(jobs, results, i, searchType);
                var thread = new Thread(new ThreadStart(threadObject.Run));
                thread.Start();
                threadsList.Add(thread);
            }

            foreach (var thread in threadsList)
            {
                thread.Join();
            }

            if (jobs.Count == 0 || results.Count > 0)
            {
                var fileOperator = new FileOperator();
                fileOperator.CreateResultsFile(searchType, results.ToArray());
            }

            Console.WriteLine("Task ended sucesfully");
        }

        static int[] GenerateDataSetParameters(int minArrayLenght, int maxArrayLenght, int numberOfArrays)
        {
            if (numberOfArrays > maxArrayLenght - minArrayLenght || minArrayLenght > maxArrayLenght)
                throw new ArgumentException("Parameters values are incorrect");

            var dataSets = new int[numberOfArrays];
            int arrayLenghtJump = (maxArrayLenght - minArrayLenght) / numberOfArrays;
            Console.WriteLine(arrayLenghtJump);
            for (int i = 0; i < numberOfArrays; i++)
            {
                dataSets[i] = minArrayLenght + i*arrayLenghtJump;
            }

            return dataSets;
        }
    }
}