using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Archive
{
    public class Program
    {
        
        public static string directoryPath = @"D:\archive\";
        public static string sourceFile = "";
        public static string compressedFile = "";
        public static string targetFile = "";
        public static FileInfo fileToCompress;
        public static FileInfo fileCompressed;
        public static FileInfo fileToDecompress;
        public static Thread openReadThread;
        public static Thread createFileThread;
        public static Thread decompressThread;
        private static void Main(string[] args)
        {
            var compressMethod = "compress";
            var compressMethod2 = "compress2";
            var decompressMethod = "decompress";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Queue.QueueReading();
            //Queue.QueueWriting();
            if (args.Length == 3)
            {
                if (args[0] == compressMethod)
                    //(String.Compare(compressMethod, args[0], StringComparison.OrdinalIgnoreCase) == 1) чет не пошло
                {
                   

                    sourceFile = args[1];
                    compressedFile = args[2];
                    fileToCompress = new FileInfo(directoryPath + sourceFile);
                    fileCompressed = new FileInfo(directoryPath + compressedFile);

                    openReadThread = new Thread(Compress.Reading);
                    openReadThread.Start();

                    createFileThread = new Thread(Compress.Compressed);
                    createFileThread.Start();


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

                    decompressThread = new Thread(Decompress.Decompressed);
                    decompressThread.Start();

                }
                else
                {
                    throw new ArgumentException("Ввод недопустимых аргументов");
                }
            }

            createFileThread.Join();
            openReadThread.Join();
            //decompressThread.Join();
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
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