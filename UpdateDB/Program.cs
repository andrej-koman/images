using System.Text;

namespace UpdateDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = @"C:\projects\images\diskopripajku\2024";

            List<UpdateFile> files = UpdateFile.GetFiles(directoryPath);

            Console.WriteLine($"Found {files.Count} files.");

            // Create CSV from the file information
            int year = 2024;
            var csv = new StringBuilder();

            var headerLine = "name,format,year,blurDataUrl";
            csv.AppendLine(headerLine);
            foreach (UpdateFile file in files)
            {
                var newLine = $"{file.FullName},{file.Extension},{year},";
                csv.AppendLine(newLine);
            }

            var csvPath = @"C:\projects\images\diskopripajku\2024.csv";

            // If file exists, delete it
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }

            File.WriteAllText(csvPath, csv.ToString());
            Console.WriteLine($"CSV file created at {csvPath}.");
        }

    }
}
