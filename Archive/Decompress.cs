using System;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Archive
{
    internal class Decompress
    {
        public static ConcurrentQueue<int> QueueCount = new ConcurrentQueue<int>();
        public static ConcurrentQueue<byte[]> QueueBuffer = new ConcurrentQueue<byte[]>();
        public static AutoResetEvent WaitHandler = new AutoResetEvent(false);

        public static void Reading()
        {
            using (var sourceStream = Program.fileCompressed.OpenRead())
            {
                using (var compressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                {
                    if (Program.fileCompressed.Extension == ".gz")
                    {
                        try
                        {
                            if (QueueCount.IsEmpty & QueueBuffer.IsEmpty)
                            {
                                var bufferSize = 81920;
                                var buffer = new byte[bufferSize];
                                WaitHandler.Set();
                                while (true)
                                {
                                    var readBuffer = compressionStream.Read(buffer, 0, buffer.Length);
                                    if (readBuffer == 0)
                                    {
                                        break;
                                    }

                                    var item = buffer.ToArray();
                                    QueueCount.Enqueue(readBuffer);
                                    QueueBuffer.Enqueue(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка чтения: {ex.Message}");
                            Console.WriteLine(1);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(Program.fileCompressed + " не является сжатым файлом");
                    }
                }
            }

            Console.WriteLine(0);
            Program.readingThread.Abort();
        }

        public static void Decompressed()
        {
            WaitHandler.WaitOne();
            Thread.Sleep(300);
            using (var targetStream = Program.fileToDecompress.Create())
            {
                try
                {
                    if (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                    {
                        while (!QueueCount.IsEmpty & !QueueBuffer.IsEmpty)
                        {
                            QueueCount.TryDequeue(out var result);
                            QueueBuffer.TryDequeue(out var resultByte);
                            targetStream.Write(resultByte, 0, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка декомпрессии: {ex.Message}");
                    Console.WriteLine(1);
                }
            }

            Program.compressionThread.Abort();
        }

        public static void Wait()
        {
            WaitHandler.WaitOne();
            //Thread.Sleep(300);
            while (true)
            {
                if (QueueBuffer.Count >= 10)
                {
                    WaitHandler.Reset();
                }
                else if (Program.readingThread.IsAlive & (QueueBuffer.Count <= 5))
                {
                    WaitHandler.Set();
                }
                else
                {
                    WaitHandler.Close();
                    break;
                }
            }

            Program.waitThread.Abort();
        }
    }
}