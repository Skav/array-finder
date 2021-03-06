using System;
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
            bool isRunning = true;
            var dataSet = new List<int[]>();
            string dataSetFile = "";

            while (isRunning)
            {
                switch (menu())
                {
                    case 1:
                        try
                        {
                            createDataSet();
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Task terminated, do you want to try again? (y/n)");
                            
                            string option = Console.ReadLine();
                            if(option.ToLower().Equals("y"))
                                createDataSet();
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
                            (dataSet, dataSetFile) = loadDataSet();
                            Console.WriteLine($"===  Load data set from file: {dataSetFile}  ===");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 3:
                        startCalculing(dataSet, dataSetFile, "linear");
                        break;
                    case 4:
                        startCalculing(dataSet, dataSetFile, "binary");
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

        static void createDataSet()
        {
            var fileOperator = new DataSetOperator();
            int minArrayValue, maxArrayValue, arrayNumber;
            Console.Write("Enter minimum array lenght: ");
            while (!int.TryParse(Console.ReadLine(), out minArrayValue) || minArrayValue < 0 ||
                   minArrayValue >= Int32.MaxValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write("Enter maximum array lenght: ");
            while (!int.TryParse(Console.ReadLine(), out maxArrayValue) || maxArrayValue < 0 ||
                   maxArrayValue >= Int32.MaxValue || maxArrayValue < minArrayValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.Write("Enter numbers of arrays: ");
            while (!int.TryParse(Console.ReadLine(), out arrayNumber) || arrayNumber <= 0 ||
                   arrayNumber >= Int32.MaxValue)
            {
                Console.WriteLine("Inccorect value, try again");
            }

            Console.WriteLine("Data set save to: " +
                              fileOperator.CreateDataSetFile(generateDataSetValues(minArrayValue, maxArrayValue,
                                  arrayNumber)));
        }

        static (List<int[]>, string) loadDataSet()
        {
            var fileOperator = new DataSetOperator();
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

            var dataSet = fileOperator.ReadDataSetFromFile(dataSetOptions[choosenOption - 1]);
            string dataSetFile = dataSetOptions[choosenOption - 1];

            return (dataSet, dataSetFile);
        }

        static void startCalculing(List<int[]> dataSet, string dataSetFile, string searchType)
        {
            if (!dataSet.Any())
                throw new ArgumentException("dataSet is empty, you need to load it first!");

            if (!searchType.Equals("linear") || searchType.Equals("binary"))
                throw new ArgumentException("Wrong search type, choose between binary or linear");

            Console.WriteLine($"Starting calculate execution time for linear search on data set from {dataSetFile}");

            /*
             *Some fancy multit threaded functions and objects
             */
        }

        static List<int[]> generateDataSetValues(int minArrayLenght, int maxArrayLenght, int numberOfArrays)
        {
            if (numberOfArrays > maxArrayLenght - minArrayLenght || minArrayLenght > maxArrayLenght)
                throw new ArgumentException("Parameters values are incorrect");

            var dataSets = new List<int[]>();
            var random = new Random();
            var usedArrayLenghts = new int[numberOfArrays];
            for (int i = numberOfArrays - 1; i >= 0; i--)
            {
                int lenght = 0;
                do
                {
                    lenght = random.Next(minArrayLenght, maxArrayLenght);
                } while (usedArrayLenghts.Contains(lenght));

                usedArrayLenghts[i] = lenght;
                var data = new int[lenght];
                for (int j = 0; j <= lenght - 1; j++)
                {
                    data[j] = j;
                }

                dataSets.Add(data);
            }

            return dataSets;
        }
    }
}