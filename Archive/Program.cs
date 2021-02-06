using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Archive
{
    internal class Program
    {
        
        private static string directoryPath = @"D:\archive\";
        private static string sourceFile = "";
        private static string compressedFile = "";
        private static string targetFile = "";
        private static FileInfo fileToCompress;
        private static FileInfo fileCompressed;
        private static FileInfo fileToDecompress;
        private static Thread openReadThread;
        private static Thread createFileThread;
        private static Thread compressionFileThread;
        private static void Main(string[] args)
        {
            var compressMethod = "compress";
            var compressMethod2 = "compress2";
            var decompressMethod = "decompress";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            if (args.Length == 3)
            {
                if (args[0] == compressMethod)
                    //(String.Compare(compressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1) чет не пошло
                {
                   

                    sourceFile = args[1];
                    compressedFile = args[2];
                    fileToCompress = new FileInfo(directoryPath + sourceFile);
                    fileCompressed = new FileInfo(directoryPath + compressedFile);

                    openReadThread = new Thread(Compress.OpenReadToCompress);
                    openReadThread.Start();

                    createFileThread = new Thread(Compress.CreateFileCompressed);
                    createFileThread.Start();

                    compressionFileThread = new Thread(Compress.Compression);
                    compressionFileThread.Start();




                }
                else if (args[0] == compressMethod2)
                {


                    sourceFile = args[1];
                    compressedFile = args[2];
                    fileToCompress = new FileInfo(directoryPath + sourceFile);
                    fileCompressed = new FileInfo(directoryPath + compressedFile);
                    FileStream sourceStream = fileToCompress.OpenRead();
                    FileStream targetStream = fileCompressed.Create();
                    GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

                    using (sourceStream)
                    {
                        if (fileToCompress.Extension != ".gz")
                        {
                            using (targetStream)
                            {
                                using (compressionStream)
                                {
                                    sourceStream.CopyTo(compressionStream);
                                    Console.WriteLine(0);
                                    Console.WriteLine("Для завершения нажмите любую клавишу");
                                    // Console.ReadLine();
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException(fileToCompress + " уже является сжатым файлом");
                        }
                    }



                }
                else if (args[0] == decompressMethod)
                    //(String.Compare(decompressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1)
                {
                    compressedFile = args[1];
                    targetFile = args[2];
                    fileCompressed = new FileInfo(directoryPath + compressedFile);
                    fileToDecompress = new FileInfo(directoryPath + targetFile);

                    Thread decompressThread = new Thread(Decompress);
                    decompressThread.Start();

                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }
            openReadThread.Join();
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Для завершения нажмите любую клавишуууууууу");
            Console.ReadLine();
        }

        public static class Compress
        {
            public static FileStream sourceStream = fileToCompress.OpenRead();
            public static FileStream targetStream = fileCompressed.Create();
            public static GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            public static void OpenReadToCompress()
            {
                using (sourceStream)
                {
                    if (fileToCompress.Extension != ".gz")
                    {
                        createFileThread.Join();
                    }
                    else
                    {
                        throw new ArgumentException(fileToCompress + " уже является сжатым файлом");
                    }
                }
            }
            public static void CreateFileCompressed()
            {
                //Thread.Sleep(500);
                using (targetStream)
                {
                    compressionFileThread.Join();
                }
            }
            public static void Compression()
            {
                using (compressionStream)
                {
                    sourceStream.CopyTo(compressionStream);
                    Console.WriteLine(0);
                    Console.WriteLine("Для завершения нажмите любую клавишу");
                   // Console.ReadLine();
                }
            }
        }


        public static void Decompress()
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