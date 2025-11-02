using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        int[] years = [2025, 2024, 2023];
        // Get the parent directory of the current directory (go one folder up)
        string baseDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "diskopripajku");
        Console.WriteLine($"Base directory set to: {baseDirectory}");
        for (int i = 0; i < years.Count(); i++)
        {
            int year = years[i];
            string directoryPath = Path.Combine(baseDirectory, year.ToString());
            int fileCount = 0;

            var csv = new StringBuilder();
            var headerLine = "name,format,year";
            csv.AppendLine(headerLine);

            PrepareDirectory(Path.Combine(directoryPath, "720"));
            PrepareDirectory(Path.Combine(directoryPath, "1080"));

            if (Directory.Exists(directoryPath))
            {
                string[] filesToPrepare = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => {
                        string ext = Path.GetExtension(f).ToLowerInvariant();
                        return ext == ".jpg" || ext == ".jpeg";
                    })
                    .ToArray();

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

                filesToPrepare = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => {
                        string ext = Path.GetExtension(f).ToLowerInvariant();
                        return ext == ".jpg" || ext == ".jpeg";
                    })
                    .ToArray();
                    
                foreach (string file in filesToPrepare)
                {
                    FileInfo fileInfo = new(file);
                    var newLine = $"{fileInfo.Name},{fileInfo.Extension},{year}";
                    csv.AppendLine(newLine);

                    ResizeImageByHeight(file, Path.Combine(directoryPath, "720", fileInfo.Name), 720, 1280);
                    ResizeImageByHeight(file, Path.Combine(directoryPath, "1080", fileInfo.Name), 1080, 1920);

                    Console.WriteLine($"Processed Image {fileCount + 1}");
                    fileCount++;
                }
            }

            Console.WriteLine($"Found {fileCount} files for year " + year + ".");

            var csvPath = Path.Combine(baseDirectory, year + ".csv");
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

    static void ResizeImageByHeight(string inputPath, string outputPath, int targetHeight, int maxWidth)
    {
        try
        {
            using (var image = Image.FromFile(inputPath))
            {
                // Handle EXIF orientation
                const int orientationId = 0x0112;
                if (Array.IndexOf(image.PropertyIdList, orientationId) > -1)
                {
                    var orientation = (int)image.GetPropertyItem(orientationId).Value[0];
                    switch (orientation)
                    {
                        case 3:
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 6:
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 8:
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                    image.RemovePropertyItem(orientationId);
                }

                double sourceAspect = (double)image.Width / image.Height;
                
                // Calculate width based on height and aspect ratio
                int targetWidth = (int)(targetHeight * sourceAspect);
                
                Console.WriteLine($"Processing image {Path.GetFileName(inputPath)}: {image.Width}x{image.Height}");
                
                // Check if width exceeds maximum
                if (targetWidth > maxWidth)
                {
                    // Need to crop - center crop the source image
                    targetWidth = maxWidth;
                    double targetAspect = (double)maxWidth / targetHeight;
                    
                    int sourceWidth = (int)(image.Height * targetAspect);
                    int sourceHeight = image.Height;
                    int sourceX = (image.Width - sourceWidth) / 2;
                    int sourceY = 0;
                    
                    Console.WriteLine($"  -> Width {(int)(targetHeight * sourceAspect)} exceeds max {maxWidth}, cropping to {targetWidth}x{targetHeight}");
                    
                    using (var newImage = new Bitmap(targetWidth, targetHeight))
                    {
                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                            // Draw cropped image
                            graphics.DrawImage(image, 
                                new Rectangle(0, 0, targetWidth, targetHeight),
                                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                                GraphicsUnit.Pixel);
                        }

                        SaveImage(newImage, inputPath, outputPath, targetWidth, targetHeight);
                    }
                }
                else
                {
                    // No cropping needed - just resize
                    Console.WriteLine($"  -> Resizing to {targetWidth}x{targetHeight}");
                    
                    using (var newImage = new Bitmap(targetWidth, targetHeight))
                    {
                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                            // Draw full image
                            graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                        }

                        SaveImage(newImage, inputPath, outputPath, targetWidth, targetHeight);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resizing {Path.GetFileName(inputPath)}: {ex.Message}");
        }
    }

    static void SaveImage(Bitmap image, string inputPath, string outputPath, int width, int height)
    {
        FileInfo fileInfo = new(inputPath);
        if (fileInfo.Extension.ToLowerInvariant() == ".jpg" || fileInfo.Extension.ToLowerInvariant() == ".jpeg")
        {
            image.Save(outputPath, ImageFormat.Jpeg);
            Console.WriteLine($"  -> Saved as {width}x{height}");
        }
        else
        {
            Console.WriteLine($"  -> Skipping unsupported format: {fileInfo.Extension}");
        }
    }

}
