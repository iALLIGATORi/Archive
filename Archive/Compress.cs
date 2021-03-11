using System;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace Archive
{
    internal class Compress
    {
        private static readonly ConcurrentQueue<(int, byte[])> QueueCompress = new ConcurrentQueue<(int, byte[])>();
        private static readonly AutoResetEvent FirstEvent = new AutoResetEvent(false);
        private static readonly AutoResetEvent SecondEvent = new AutoResetEvent(true);
        public static int ReturnedError;

        public static int Reading()
        {
            using (var sourceStream = Program.FileToCompress.OpenRead())
            {
                if (Program.FileToCompress.Extension != ".gz")
                {
                    try
                    {
                        var bufferSize = 81920;
                        var buffer = new byte[bufferSize];
                        while (true)
                        {
                            var readBuffer = sourceStream.Read(buffer, 0, buffer.Length);
                            if (readBuffer == 0)
                            {
                                break;
                            }

                            var item = buffer.ToArray();
                            QueueCompress.Enqueue((readBuffer, item));
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

            Program.ReadingThread.Abort();
            ReturnedError = 0;
            return ReturnedError;
        }

        public static int Compressed()
        {
            FirstEvent.WaitOne(-1);
            using (var targetStream = Program.FileCompressed.Create())
            {
                using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                {
                    try
                    {
                        while (!QueueCompress.IsEmpty)
                        {
                            FirstEvent.WaitOne(100);
                            QueueCompress.TryDequeue(out var writeBuffer);
                            compressionStream.Write(writeBuffer.Item2, 0, writeBuffer.Item1);
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
            }

            Program.CompressionThread.Abort();
            ReturnedError = 0;
            return ReturnedError;
        }
    }
}