using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Archive
{
    public class Program
    {
        private static readonly string DirectoryPath = @"C:\archive\";
        private static string sourceFile = "";
        private static string compressedFile = "";
        private static string targetFile = "";
        public static FileInfo FileToCompress;
        public static FileInfo FileCompressed;
        public static FileInfo FileToDecompress;
        public static Thread ReadingThread;
        public static Thread CompressionThread;
        public static Thread CloserThread;
        static CancellationTokenSource cts = new CancellationTokenSource();


        private static void Main(string[] args)
        {
            var compressMethod = "compress";
            var decompressMethod = "decompress";

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (args.Length == 3)
            {
                if (string.Compare(compressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sourceFile = args[1];
                    compressedFile = args[2];
                    FileToCompress = new FileInfo(DirectoryPath + sourceFile);
                    FileCompressed = new FileInfo(DirectoryPath + compressedFile);

                    ReadingThread = new Thread(() => { Compress.Reading(); });
                    ReadingThread.Start();

                    CompressionThread = new Thread(() => { Compress.Compressed(); });
                    CompressionThread.Start();

                    ReadingThread.Join();
                    CompressionThread.Join();
                    Console.WriteLine(Compress.ReturnedError);
                }
                else if (string.Compare(decompressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    compressedFile = args[1];
                    targetFile = args[2];
                    FileCompressed = new FileInfo(DirectoryPath + compressedFile);
                    FileToDecompress = new FileInfo(DirectoryPath + targetFile);

                    ReadingThread = new Thread(() => { Decompress.Reading(); });
                    ReadingThread.Start();

                    CompressionThread = new Thread(() =>
                    {

                        if (Console.ReadKey(true).KeyChar.ToString().ToUpperInvariant() == "C")
                            cts.Cancel();
                        Decompress.Decompressed();
                    });
                    CompressionThread.Start();

                    CloserThread = new Thread(() => { Close.Closer(cts.Token); });

                    ReadingThread.Join();
                    CompressionThread.Join();
                    cts.Dispose();
                    Console.WriteLine(Compress.ReturnedError);
                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }


            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Для завершения нажмите любую клавишуууууууу");
            Console.ReadLine();
        }
    }

}