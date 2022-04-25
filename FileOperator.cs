using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace array_finder;

public class FileOperator
{
    private string directoryPath;
    public Dictionary<string, string> dataSetFiles = new Dictionary<string, string>();

    public FileOperator()
    {
        this.directoryPath = Directory.GetCurrentDirectory();
    }

    public FileOperator(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public Dictionary<string, string> GetAllDataSetFiles()
    {
        if (!dataSetFiles.Any())
        {
            foreach (string file in Directory.GetFiles(this.directoryPath, "dataset-*.csv"))
            {
                dataSetFiles.Add(Path.GetFileName(file), $"{this.directoryPath}/{file}");
            }
        }

        return this.dataSetFiles;
    }

    public string CreateDataSetFile(int[] arrayValues)
    {
        string data = "";
        string fileName = $"{this.directoryPath}/dataset-{DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss")}.csv";
        using (FileStream fs = File.Create(fileName))
        {
            foreach (var item in arrayValues)
            {
                data += $"{item}{Environment.NewLine}";
            }

            data = data.Substring(0, data.Length - 1);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            fs.Write(bytes, 0, bytes.Length);
        }

        return fileName;
    }

    public string CreateResultsFile(string name, string[] values)
    {
        string fileName = $"{this.directoryPath}/results-{name}-{DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss")}.csv";
        using (FileStream fs = File.Create(fileName))
        {
            var data = "array_lenght,value,type,searchType,compare_type\n";
            foreach (var item in values)
            {
                data += $"{item},";
                data += "\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            fs.Write(bytes, 0, bytes.Length);
        }

        return fileName;
    }

    public List<int> ReadDataSetFromFile(string fileName)
    {
        string fileContent = File.ReadAllText($"{this.directoryPath}/{fileName}");
        var strings = fileContent.Split(Environment.NewLine);
        var dataSet = new List<int>(Array.ConvertAll(strings, int.Parse));
        
        return dataSet;
    }
}