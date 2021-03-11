using System;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Archive
{
    internal class Decompress
    {
        private static readonly ConcurrentQueue<(int, byte[])> QueueDecompress = new ConcurrentQueue<(int, byte[])>();
        private static readonly AutoResetEvent FirstEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent SecondEvent = new AutoResetEvent(true);
        public static int ReturnedError;

        public static int Reading()
        {
            using (var sourceStream = Program.FileCompressed.OpenRead())
            {
                using (var compressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                {
                    if (Program.FileCompressed.Extension == ".gz")
                    {
                        try
                        {
                            var bufferSize = 81920;
                            var buffer = new byte[bufferSize];
                            while (true)
                            {
                                var readBuffer = compressionStream.Read(buffer, 0, buffer.Length);
                                if (readBuffer == 0)
                                {
                                    break;
                                }

                                var item = buffer.ToArray();
                                QueueDecompress.Enqueue((readBuffer, item));
                                FirstEvent.Set();
                                SecondEvent.WaitOne(100);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                            ReturnedError = 1;
                            return ReturnedError;
                        }
                    }
                    else
                    {
                        Console.WriteLine(Program.FileToCompress + " уже является сжатым файлом");
                        ReturnedError = 1;
                        return ReturnedError;
                    }
                }
            }

            Program.ReadingThread.Abort();
            ReturnedError = 0;
            return ReturnedError;
        }

        public static int Decompressed()
        {
            FirstEvent.WaitOne(-1);
            using (var targetStream = Program.FileToDecompress.Create())
            {
                try
                {
                    while (!QueueDecompress.IsEmpty)
                    {
                        FirstEvent.WaitOne(100);
                        QueueDecompress.TryDequeue(out var writeBuffer);
                        targetStream.Write(writeBuffer.Item2, 0, writeBuffer.Item1);
                        SecondEvent.Set();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    ReturnedError = 1;
                    return ReturnedError;
                }
                finally
                {
                    FirstEvent.Dispose();
                    SecondEvent.Dispose();
                }
            }

            Program.CompressionThread.Abort();
            ReturnedError = 0;
            return ReturnedError;
        }
    }
}