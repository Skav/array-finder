using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace array_finder;

public class DataSetOperator
{
    private string directoryPath;
    public Dictionary<string, string> dataSetFiles = new Dictionary<string, string>();
    
    public DataSetOperator()
    {
        this.directoryPath = Directory.GetCurrentDirectory();
    }

    public DataSetOperator(string directoryPath)
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

    public string CreateDataSetFile(List<int[]> arrayValues)
    {
        string data = "";
        string fileName = $"{this.directoryPath}/dataset-{DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss")}.csv";
        using (FileStream fs = File.Create(fileName))
        {
            foreach (var items in arrayValues)
            {
                foreach (var number in items)
                {
                    data += $"{number},";
                }

                data = data.Remove(data.Length - 1);
                data += "\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            fs.Write(bytes, 0, bytes.Length);
        }

        return fileName;
    }

    public List<int[]> ReadDataSetFromFile(string fileName)
    {
        var dataSet = new List<int[]>();
        string fileContent = File.ReadAllText($"{this.directoryPath}/{fileName}");
        string[] lines = fileContent.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            var lineValues = line.Split(',');
            if (lineValues[0].Equals(""))
                break;
            var numberArray = new int[lineValues.Length];
            for (int i = 0; i < lineValues.Length; i++)
            {
                numberArray[i] = Convert.ToInt32(lineValues[i]);
            }
            dataSet.Add(numberArray);
        }

        return dataSet;
    }
}