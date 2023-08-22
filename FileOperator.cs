using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace array_finder
{
    public class FileOperator
    {
        private string directoryPath;
        public Dictionary<string, string> dataSetFiles = new Dictionary<string, string>();

        public FileOperator()
        {
            this.directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // getting location of exectuable file
        }

        public FileOperator(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        /*
         * This function gets all excisting data set files in given location
         * FUnction return a dictionary with file name as key, and file location as value
         */
        public Dictionary<string, string> GetAllDataSetFiles()
        {
            if (!dataSetFiles.Any())
            {
                foreach (string file in Directory.GetFiles(this.directoryPath, "dataset-*.txt"))
                {
                    dataSetFiles.Add(Path.GetFileName(file), $"{Path.Combine(this.directoryPath, file)}");
                }
            }

            return this.dataSetFiles;
        }

        public string CreateDataSetFile(int[] arrayValues)
        {
            string data = "";
            string fileName = $"dataset-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.txt";
            using (FileStream fs = File.Create(Path.Combine(this.directoryPath, fileName)))
            {
                foreach (var item in arrayValues)
                {
                    data += $"{item}\n";
                }

                data = data.Substring(0, data.Length - 1);
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                fs.Write(bytes, 0, bytes.Length);
            }

            return Path.Combine(this.directoryPath, fileName);
        }

        public string CreateResultsFile(string algorithmType, string[] results)
        {
            string fileName = $"results-{algorithmType}-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv";
            using (FileStream fs = File.Create(Path.Combine(this.directoryPath, fileName)))
            {
                var data = "array_lenght;value;type;searchType;compare_type\n"; // setting columns names as 1st line in .csv file
                foreach (var item in results) //adding actual data to file
                    data += $"{item}\n";

                byte[] bytes = Encoding.UTF8.GetBytes(data);
                fs.Write(bytes, 0, bytes.Length);
            }

            return Path.Combine(this.directoryPath, fileName);
        }

        public List<int> ReadDataSetFromFile(string fileName)
        {
            string fileContent = File.ReadAllText(Path.Combine(this.directoryPath, fileName)); //getting all values from file
            var strings = fileContent.Split('\n'); // creating array by spliting every line
            var dataSet = new List<int>(Array.ConvertAll(strings, int.Parse)); //converting string array to int array

            return dataSet;
        }
    }
}