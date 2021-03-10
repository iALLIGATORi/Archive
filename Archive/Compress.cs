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
        public static ConcurrentQueue<(int, byte[])> QueueCompress = new ConcurrentQueue<(int, byte[])>();
        //private static Mutex mut = new Mutex();
        private static AutoResetEvent are = new AutoResetEvent(false);
        private static AutoResetEvent are2 = new AutoResetEvent(true);
        public static void Reading()
        {
            using (FileStream sourceStream = Program.fileToCompress.OpenRead())
            {
                if (Program.fileToCompress.Extension != ".gz")
                {
                    try
                    {
                        var bufferSize = 81920;
                        var buffer = new byte[bufferSize];
                        while (true)
                        {
                            //mut.WaitOne();

                            var readBuffer = sourceStream.Read(buffer, 0, buffer.Length);
                            if (readBuffer == 0)
                            {
                                break;
                            }

                            var item = buffer.ToArray();
                            QueueCompress.Enqueue((readBuffer, item));
                            //mut.ReleaseMutex();
                            are.Set();
                            are2.WaitOne(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        //return 1;
                    }
                    finally
                    {
                        Program.readingThread.Abort();
                    }
                }
                else
                {
                    Console.WriteLine(Program.fileToCompress + " уже является сжатым файлом");
                    //return 1;
                }
            }
            //return 0;
        }

        public static void Compressed()
        {
            are.WaitOne(-1);
            using (FileStream targetStream = Program.fileCompressed.Create())
            {
                using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                {
                    try
                    {
                        while (!QueueCompress.IsEmpty)
                        {
                            are.WaitOne(100);
                            //mut.WaitOne();
                            QueueCompress.TryDequeue(out var writeBuffer);
                            compressionStream.Write(writeBuffer.Item2, 0, writeBuffer.Item1);
                            //mut.ReleaseMutex();
                            are2.Set();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        //return 1;
                    }
                    finally
                    {
                        Program.compressionThread.Abort();
                        are.Dispose();
                        are2.Dispose();
                    }
                }
            }

            //return 0;
        }
    }


}