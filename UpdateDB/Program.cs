namespace UpdateDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = @"C:\projects\images\diskopripajku\2024";

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);

                foreach (string file in files)
                {
                    FileInfo fileInfo = new (file);
                    Console.WriteLine($"File Name: {fileInfo.Name}");
                    Console.WriteLine($"File extension: {fileInfo.Extension}");
                    Console.WriteLine($"Creation Time: {fileInfo.CreationTime}");
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
            }
        }
    }
}
