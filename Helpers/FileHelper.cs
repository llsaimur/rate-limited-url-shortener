namespace UrlShortener.Helpers
{
    public static class FileHelper
    {
        public static string LoadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("path is null, empty or contains whitespace.");

                return string.Empty;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"{path} does not exist");

                return string.Empty;
            }

            return File.ReadAllText(path);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            if(string.IsNullOrWhiteSpace(relativePath))
            {
                Console.WriteLine("relative path is null, empty or contains whitespace.");

                return string.Empty;
            }

            return Path.Combine(AppContext.BaseDirectory, relativePath);
        }
    }
}
