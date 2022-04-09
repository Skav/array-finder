using System.IO;
using System.Text;

namespace array_finder;

public class DataSetOperator
{
    private string directoryPath;
    public Dictionary<string, string> dataSetFiles;
    
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
            foreach (string fileName in Directory.GetFiles(this.directoryPath, "*.json"))
            {
                dataSetFiles.Add(fileName, $"{this.directoryPath}/{fileName}");
            }
        }

        return this.dataSetFiles;
    }

    public void CreateDataSetFile(string fileName, List<int[]> arrayValues)
    {
        string data = "";
        using (FileStream fs = File.Create($"{this.directoryPath}/{fileName}-{DateTime.Today}.txt"))
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
    }

    public List<int[]> ReadDataSetFromFile(string fileName)
    {
        var dataSet = new List<int[]>();
        string fileContent = File.ReadAllText($"{this.directoryPath}/{fileName}.txt");
        string[] lines = fileContent.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            var lineValues = line.Split(',');
            var numberArray = new int[lineValues.Length];
            for (int i = 0; i < lineValues.Length - 1; i++)
            {
                numberArray[i] = Convert.ToInt32(lineValues[i]);
            }
            dataSet.Add(numberArray);
        }

        return dataSet;
    }
}