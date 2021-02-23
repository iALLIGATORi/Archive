using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archive
{
    class Decompress : Program
    {
        public static void Decompressed()
        {
            try
            {
                using (var sourceStream = fileCompressed.OpenRead())
                {
                    using (var targetStream = fileToDecompress.Create())
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
