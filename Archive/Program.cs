using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Archive
{
    public class Program
    {
        public static string directoryPath = @"C:\archive\";
        public static string sourceFile = "";
        public static string compressedFile = "";
        public static string targetFile = "";
        public static FileInfo fileToCompress;
        public static FileInfo fileCompressed;
        public static FileInfo fileToDecompress;
        public static Thread readingThread;
        public static Thread compressedThead;
        public static Thread decompressedThread;

        private static void Main(string[] args)
        {
            var compressMethod = "compress";
            var compressMethod2 = "compress2";
            var decompressMethod = "decompress";

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            //var Text = File.Create("C:\\archive\\text20gb.txt");
            //using (StreamWriter sw = new StreamWriter("C:\\archive\\text20gb.txt", false, System.Text.Encoding.Default))
            //{
            //    for (int i = 0; i < 2000000000; i++)
            //    {
            //        sw.WriteLine(i);
            //    }
            //}

            if (args.Length == 3)
            {
                if (args[0] == compressMethod)
                    //(String.Compare(compressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1) чет не пошло
                {
                    sourceFile = args[1];
                    compressedFile = args[2];
                    fileToCompress = new FileInfo(directoryPath + sourceFile);
                    fileCompressed = new FileInfo(directoryPath + compressedFile);

                    readingThread = new Thread(Compress.Reading);
                    readingThread.Start();

                    compressedThead = new Thread(Compress.Compressed);
                    compressedThead.Start();


                    //compressionFileThread = new Thread(Compress.CopyTo);
                    //compressionFileThread.Start();

                    //Compress.CopyTo();
                    //Compress.Reading();
                    //Compress.Writing();
                }
                else if (args[0] == compressMethod2)
                {
                    sourceFile = args[1];
                    compressedFile = args[2];
                    fileToCompress = new FileInfo(directoryPath + sourceFile);
                    fileCompressed = new FileInfo(directoryPath + compressedFile);
                    var sourceStream = fileToCompress.OpenRead();
                    var targetStream = fileCompressed.Create();
                    var compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

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

                    decompressedThread = new Thread(Decompress.Decompressed);
                    decompressedThread.Start();
                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }

            compressedThead.Join();
            readingThread.Join();
            //decompressThread.Join();
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            var ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            //if (fileToCompress.Length != fileCompressed.Length)
            //{
            //    throw new ArgumentException(Program.fileToCompress + " был полностью записан");
            //}
            Console.WriteLine("Для завершения нажмите любую клавишуууууууу");
            Console.ReadLine();
        }
    }
}