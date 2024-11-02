namespace UpdateDB
{
    internal class UpdateFile
    {
        public string FullName { get; set; }
        public string Extension { get; set; }

        public UpdateFile(string fullName, string extension)
        {
            this.FullName = fullName;
            this.Extension = extension;
        }

        public static List<UpdateFile> GetFiles(string directoryPath)
        {
            List<UpdateFile> files = new();

            if (Directory.Exists(directoryPath))
            {
                string[] directoryFiles = Directory.GetFiles(directoryPath);

                foreach (string file in directoryFiles)
                {
                    FileInfo fileInfo = new(file);
                    files.Add(new UpdateFile(fileInfo.Name, fileInfo.Extension));
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
            }
            return files;
        }
    }
}
