using System;
using System.IO;
using System.IO.Compression;


namespace Archive
{
    class Program
    {
        private static string _directoryPath = @"D:\archive\";
        private static string _sourceFile = "";
        private static string _compressedFile = "";
        private static string _targetFile = "";
        static void Main(string[] args)
        {
            string compressMethod = "compress";
            string decompressMethod = "decompress";

            if (args.Length == 3)
            {
                if (args[0] == compressMethod)
                    //(String.Compare(compressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1) чет не пошло
                {
                    _sourceFile = args[1];
                    _compressedFile = args[2];
                    FileInfo fileToCompress = new FileInfo(_directoryPath + _sourceFile);
                    FileInfo fileCompressed = new FileInfo(_directoryPath + _compressedFile);
                    // создание сжатого файла
                    Compress(fileToCompress, fileCompressed);
                }
                else if (args[0] == decompressMethod)
                    //(String.Compare(decompressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1)
                {
                    _compressedFile = args[1];
                    _targetFile = args[2];
                    FileInfo fileCompressed = new FileInfo(_directoryPath + _compressedFile);
                    FileInfo fileToDecompress = new FileInfo(_directoryPath + _targetFile);
                    // чтение из сжатого файла
                    Decompress(fileCompressed, fileToDecompress);
                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }

            Console.WriteLine("Для завершения нажмите любую клавишу");
            Console.ReadLine();
        }

        public static void Compress(FileInfo fileToCompress, FileInfo fileCompressed)
        {
            try
            {
                // поток для чтения исходного файла
                using (FileStream sourceStream = fileToCompress.OpenRead())
                {
                    // поток для записи сжатого файла
                    using (FileStream targetStream = fileCompressed.Create())
                    {
                        // поток архивации
                        using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                        {
                            sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                            Console.WriteLine(0);

                            //Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            //    fileToCompress, sourceStream.Length.ToString(), targetStream.Length.ToString());
                        }
                    }
                }
            }
            catch (Exception e) //when(!fileToCompress.Exists)
            {
                Console.WriteLine($"Ошибка: { e.Message}");
                Console.WriteLine(1);
            }
        }

        public static void Decompress(FileInfo fileCompressed, FileInfo fileToDecompress)
        {
            try
            {
                // поток для чтения из сжатого файла
                using (FileStream sourceStream = fileCompressed.OpenRead())
                {
                    // поток для записи восстановленного файла
                    using (FileStream targetStream = fileToDecompress.Create())
                    {
                        // поток разархивации
                        using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                            Console.WriteLine(0);

                            //Console.WriteLine("Восстановлен файл: {0}", targetFile);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: { e.Message}");
                Console.WriteLine(1);
            }
        }
    }
}