using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public static int BufferSize = 81920;
        public static byte[] buffer = new byte[BufferSize];
        public static int readBuffer = 1;
        private static readonly ConcurrentQueue<byte[]> queueBuffer = new ConcurrentQueue<byte[]>();
        private static readonly ConcurrentQueue<int> queueCount = new ConcurrentQueue<int>();


        public static void Reading()
        {
            using (sourceStream)
            {
                if (Program.fileToCompress.Extension != ".gz")
                {
                    while (readBuffer != 0)
                    {
                        readBuffer = sourceStream.Read(buffer, 0, buffer.Length);
                        if (readBuffer == 0)
                        {
                            break;
                        }
                        //var received = buffer.Take(readBuffer).ToArray();
                        var received = new byte[readBuffer];
                        Array.Copy(buffer, 0, received, 0, readBuffer);
                        queueCount.Enqueue(readBuffer);
                        queueBuffer.Enqueue(received);
                    }
                }
                else
                {
                    throw new ArgumentException(Program.fileToCompress + " уже является сжатым файлом");
                }

                Program.createFileThread.Join();
            }

            Console.WriteLine(0);
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
            //Program.openReadThread.Join();
            //waitHandler.WaitOne();
            Thread.Sleep(1000);
            using (targetStream)
            {
                //using (compressionStream)
                //{
                    if (!queueCount.IsEmpty | !queueBuffer.IsEmpty)
                    {
                        while (!queueCount.IsEmpty | !queueBuffer.IsEmpty) // рабочее сжатие
                        {
                            queueCount.TryDequeue(out var writeCount);
                            queueBuffer.TryDequeue(out var writeBuffer);
                            targetStream.Write(writeBuffer, 0, writeCount);
                        }
                    }

                //}
            }
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