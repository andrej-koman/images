using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        int[] years = [2024, 2023];
        for (int i = 0; i < years.Count(); i++)
        {
            int year = years[i];
            string directoryPath = @"C:\projects\images\diskopripajku\" + year;
            int fileCount = 0;

            // Create CSV from the file information
            var csv = new StringBuilder();
            var headerLine = "name,format,year";
            csv.AppendLine(headerLine);

            // Prepare directories for different file sizes
            PrepareDirectory(Path.Combine(directoryPath, "180x120"));
            PrepareDirectory(Path.Combine(directoryPath, "720x480"));
            PrepareDirectory(Path.Combine(directoryPath, "1280x853"));

            if (Directory.Exists(directoryPath))
            {
                string[] filesToPrepare = Directory.GetFiles(directoryPath);

                // First pass to rename any files starting with "_"
                foreach (string file in filesToPrepare)
                {
                    FileInfo fileInfo = new(file);
                    if (fileInfo.Name.StartsWith("_"))
                    {
                        string newFileName = fileInfo.Name.Substring(1);
                        File.Move(file, Path.Combine(directoryPath, newFileName));
                        Console.WriteLine($"Renamed {fileInfo.Name} to {newFileName}.");
                    }
                }

                filesToPrepare = Directory.GetFiles(directoryPath);
                foreach (string file in filesToPrepare)
                {
                    FileInfo fileInfo = new(file);
                    var newLine = $"{fileInfo.Name},{fileInfo.Extension},{year}";
                    csv.AppendLine(newLine);

                    ResizeImage(file, Path.Combine(directoryPath, "180x120", fileInfo.Name), 180, 120);
                    ResizeImage(file, Path.Combine(directoryPath, "720x480", fileInfo.Name), 720, 480);
                    ResizeImage(file, Path.Combine(directoryPath, "1280x853", fileInfo.Name), 1280, 853);

                    Console.WriteLine($"Processed Image {fileCount + 1}");
                    fileCount++;
                }
            }

            Console.WriteLine($"Found {fileCount} files for year " + year + ".");

            var csvPath = @"C:\projects\images\diskopripajku\" + year + ".csv";
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }

            File.WriteAllText(csvPath, csv.ToString());
            Console.WriteLine($"CSV file created at {csvPath}.");
        }
    }

    static void PrepareDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        Console.WriteLine($"Created {Path.GetFileName(path)} folder.");
    }

    static void ResizeImage(string inputPath, string outputPath, int width, int height)
    {
        try
        {
            using (var image = Image.FromFile(inputPath))
            {
                // If the image is vertical, rotate it
                if (image.Width < image.Height)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    Console.WriteLine($"Rotated {Path.GetFileName(inputPath)} to the left.");
                }
                using (var newImage = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        graphics.DrawImage(image, 0, 0, width, height);
                    }

                    FileInfo fileInfo = new(inputPath);
                    if (fileInfo.Extension.ToLower() == ".jpg")
                    {
                        newImage.Save(outputPath, ImageFormat.Jpeg);
                        Console.WriteLine($"Saved {Path.GetFileName(inputPath)} with resolution {width}/{height} to {outputPath} with format JPEG");
                    }
                    else
                    {
                        Console.WriteLine($"Cannot save {Path.GetFileName(inputPath)} with resolution {width}/{height} to {outputPath} with format {fileInfo.Extension}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resizing {Path.GetFileName(inputPath)}: {ex.Message}");
        }
    }
}
