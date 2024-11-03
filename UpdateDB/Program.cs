using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace UpdateDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = @"C:\projects\images\diskopripajku\2024";
            int fileCount = 0;
            int year = 2024;

            // Create CSV from the file information
            var csv = new StringBuilder();

            var headerLine = "name,format,year";
            csv.AppendLine(headerLine);

            // Prepare directories for different file sizes

            // 180x120px
            // If the directory exists, empty it
            if (Directory.Exists(Path.Combine(directoryPath, "180x120")))
            {
                Directory.Delete(Path.Combine(directoryPath, "180x120"), true);
            }
            Directory.CreateDirectory(Path.Combine(directoryPath, "180x120"));

            // 720x480px
            // If the directory exists, empty it
            if (Directory.Exists(Path.Combine(directoryPath, "720x480")))
            {
                Directory.Delete(Path.Combine(directoryPath, "720x480"), true);
            }
            Directory.CreateDirectory(Path.Combine(directoryPath, "720x480"));

            // 1280x853px 
            // If the directory exists, empty it
            if (Directory.Exists(Path.Combine(directoryPath, "1280x853")))
            {
                Directory.Delete(Path.Combine(directoryPath, "1280x853"), true);
            }
            Directory.CreateDirectory(Path.Combine(directoryPath, "1280x853"));

            if (Directory.Exists(directoryPath))
            {
                string[] directoryFiles = Directory.GetFiles(directoryPath);

                foreach (string file in directoryFiles)
                {
                    FileInfo fileInfo = new(file);

                    // Add the info to CSV file
                    var newLine = $"{fileInfo.Name},{fileInfo.Extension},{year}";
                    csv.AppendLine(newLine);

                    // Resize the image to all sizes needed and save them
                    // 180x120px
                    ResizeImage(file, Path.Combine(directoryPath, "180x120", fileInfo.Name), 180, 120);

                    // 720x480px
                    ResizeImage(file, Path.Combine(directoryPath, "720x480", fileInfo.Name), 720, 480);

                    // 1280x853px
                    ResizeImage(file, Path.Combine(directoryPath, "1280x853", fileInfo.Name), 1280, 853);

                    Console.WriteLine($"Processed Image {fileCount + 1}");

                    fileCount++;
                }
            }

            Console.WriteLine($"Found {fileCount} files.");

            // Save the CSV
            var csvPath = @"C:\projects\images\diskopripajku\2024.csv";
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }
            File.WriteAllText(csvPath, csv.ToString());
            Console.WriteLine($"CSV file created at {csvPath}.");
        }

        static void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            if (OperatingSystem.IsWindows())
            {
                using (var originalImage = new Bitmap(inputPath))
                {
                    var resizedImage = new Bitmap(width, height);
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        graphics.DrawImage(originalImage, 0, 0, width, height);
                    }

                    resizedImage.Save(outputPath, ImageFormat.Jpeg); // Saves in JPEG format; adjust format as needed
                }
            }
        }
    }
}
