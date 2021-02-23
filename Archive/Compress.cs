using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Archive
{
    public class Compress
    {
        public static FileStream sourceStream = Program.fileToCompress.OpenRead();
        public static FileStream targetStream = Program.fileCompressed.Create();
        public static GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
        public static ConcurrentQueue<int> QueueCount = new ConcurrentQueue<int>();
        public static ConcurrentQueue<byte[]> QueueBuffer = new ConcurrentQueue<byte[]>();
        public static AutoResetEvent WaitHandler = new AutoResetEvent(false);

        public static void Reading()
        {
            using (sourceStream)
            {
                if (Program.fileToCompress.Extension != ".gz")
                {
                    if (QueueCount.IsEmpty & QueueBuffer.IsEmpty)
                    {
                        var bufferSize = 81920;
                        var buffer = new byte[bufferSize];
                        WaitHandler.Set();
                        while (true)
                        {
                            var readBuffer = sourceStream.Read(buffer, 0, buffer.Length);
                            if (readBuffer == 0)
                            {
                                break;
                            }

                            var item = buffer.ToArray();
                            QueueCount.Enqueue(readBuffer);
                            QueueBuffer.Enqueue(item);

                            if (QueueBuffer.Count >= 5000)
                            {
                                WaitHandler.Reset();
                                WaitHandler.WaitOne();
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(Program.fileToCompress + " уже является сжатым файлом");
                }
            }

            Console.WriteLine(0);
            Program.readingThread.Abort();
        }

        //public static void Writing()
        //{
        //    Program.openReadThread.Join();
        //    //waitHandler.WaitOne();
        //    Thread.Sleep(1000);
        //    using (targetStream)
        //    {
        //        if (!queueCount.IsEmpty | !queueBuffer.IsEmpty)
        //        {
        //            while (!queueCount.IsEmpty | !queueBuffer.IsEmpty) // рабочая запись
        //            {
        //                queueCount.TryDequeue(out var writeCount);
        //                queueBuffer.TryDequeue(out var writeBuffer);
        //                targetStream.Write(writeBuffer, 0, writeCount);
        //            }
        //        }
        //    }
        //}

        public static void Compressed()
        {
            WaitHandler.WaitOne();
            Thread.Sleep(300);
            using (compressionStream)
            {
                if (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                {
                    while (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                    {
                        QueueCount.TryDequeue(out var result);
                        QueueBuffer.TryDequeue(out var resultByte);
                        compressionStream.Write(resultByte, 0, result);

                        if (Program.readingThread.IsAlive & (QueueBuffer.Count <= 2500))
                        {
                            WaitHandler.Set();
                            Thread.Sleep(300);
                        }
                    }
                }
            }

            WaitHandler.Close();
            Program.compressedThead.Abort();
        }

        //public static void OpenReadToCompress()
        //{
        //    using (sourceStream)
        //    {
        //        if (Program.fileToCompress.Extension != ".gz")
        //        {
        //            Program.createFileThread.Join();
        //        }
        //        else
        //        {
        //            throw new ArgumentException(Program.fileToCompress + " уже является сжатым файлом");
        //        }
        //    }
        //}
        //public static void CreateFileCompressed()
        //{
        //    //Thread.Sleep(500);
        //    using (targetStream)
        //    {
        //        Program.compressionFileThread.Join();
        //    }
        //}
        //public static void Compression()
        //{
        //    using (compressionStream)
        //    {
        //        sourceStream.CopyTo(compressionStream);
        //        Console.WriteLine(0);
        //        Console.WriteLine("Для завершения нажмите любую клавишу");
        //        // Console.ReadLine();
        //    }
        //}
        //}
    }
}