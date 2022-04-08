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
                Console.WriteLine("3. Calculate time for linear ");
                Console.WriteLine("5. Exit");

                if (!int.TryParse(Console.ReadLine(), out choosenOption))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input, try again");
                }
                else
                {
                    if(choosenOption >= 1 && choosenOption <= 5)
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
            var dataSet = new List<int[]>();

            switch (menu())
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                default:
                    Console.WriteLine("Unexcepted value, closing program");
                    Environment.Exit(0);
                    break;
            }


        }
    }
}