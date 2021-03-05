using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Archive
{
    internal class Compress
    {
        public static ConcurrentQueue<int> QueueCount = new ConcurrentQueue<int>();
        public static ConcurrentQueue<byte[]> QueueBuffer = new ConcurrentQueue<byte[]>();

        public static void Reading()
        {
            using (FileStream sourceStream = Program.fileToCompress.OpenRead())
            {
                if (Program.fileToCompress.Extension != ".gz")
                {
                    try
                    {
                        if (QueueCount.IsEmpty & QueueBuffer.IsEmpty)
                        {
                            var bufferSize = 81920;
                            var buffer = new byte[bufferSize];
                            //WaitHandler.Set();
                            while (true)
                            {
                                //Thread.Sleep(10);
                                var readBuffer = sourceStream.Read(buffer, 0, buffer.Length);
                                if (readBuffer == 0)
                                {
                                    break;
                                }

                                var item = buffer.ToArray();
                                QueueCount.Enqueue(readBuffer);
                                QueueBuffer.Enqueue(item);

                                //if (QueueBuffer.Count >= 5000)
                                //{
                                //    Thread.Sleep(1000);
                                //}
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        Console.WriteLine(1);
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

        public static void Compressed()
        {
            Thread.Sleep(300);
            using (FileStream targetStream = Program.fileCompressed.Create())
            {
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                {
                    try
                    {
                        if (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                        {
                            while (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                            {
                                QueueCount.TryDequeue(out var result);
                                QueueBuffer.TryDequeue(out var resultByte);
                                compressionStream.Write(resultByte, 0, result);
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
            Program.compressionThread.Abort();
        }
        //public static void Wait()
        //{
        //    WaitHandler.WaitOne();

        //    Program.compressionThread.Interrupt();
        //    Program.waitThread.Start();

        //    while (true)
        //    {
        //        if (QueueBuffer.Count >= 5000)
        //        {
        //            WaitHandler.Reset();
        //            //WaitHandler.WaitOne();
        //            Program.readingThread.
        //        }
        //        else if (Program.readingThread.IsAlive & (QueueBuffer.Count <= 2500))
        //        {
        //            WaitHandler.Set();
        //            //Thread.Sleep(300);
        //        }
        //        else
        //        {
        //            WaitHandler.Close();
        //            break;
        //        }
        //    }
        //    Program.waitThread.Abort();
        //}
    }


}