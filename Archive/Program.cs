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
        public static Thread compressionThread;
        public static Thread waitThread;
        //public static Thread decompressedThread;

        private static void Main(string[] args)
        {
            var compressMethod = "compress";
            var decompressMethod = "decompress";

            var stopWatch = new Stopwatch();
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


                    readingThread = new Thread(Compress.Reading);
                    //readingThread.Start();

                    compressionThread = new Thread(Compress.Compressed);
                    //compressionThread.Start();
                    //object value = null;
                    //readingThread = new Thread(Queue.QueueReading);
                    //readingThread = new Thread(() => { value = Compress.Reading(); });

                    readingThread.Start();

                    //compressionThread = new Thread(Queue.QueueWriting);
                    //compressionThread = new Thread(() => { value = Compress.Compressed(); });
                    compressionThread.Start();

                    readingThread.Join();
                    compressionThread.Join();

                }
                else if (args[0] == decompressMethod)
                    //(String.Compare(decompressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1)
                {
                    compressedFile = args[1];
                    targetFile = args[2];
                    fileCompressed = new FileInfo(directoryPath + compressedFile);
                    fileToDecompress = new FileInfo(directoryPath + targetFile);

                    readingThread = new Thread(Decompress.Reading);
                    readingThread.Start();

                    waitThread = new Thread(Decompress.Wait);
                    waitThread.Start();

                    compressionThread = new Thread(Decompress.Decompressed);
                    compressionThread.Start();
                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }

            //if (Compress.Compressed() == 1)
            //{
            //    Console.WriteLine("Поздравляю, все работает");
            //}

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