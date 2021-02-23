using System;
using System.IO.Compression;

namespace Archive
{
    internal class Decompress
    {
        public static void Decompressed()
        {
            try
            {
                using (var sourceStream = Program.fileCompressed.OpenRead())
                {
                    using (var targetStream = Program.fileToDecompress.Create())
                    {
                        using (var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                            Console.WriteLine(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine(1);
            }
        }
    }
}