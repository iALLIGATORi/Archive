using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace Archive
{
    public class Queue
    {
        public static FileStream Fs = new FileStream("C:\\archive\\testvideo.mp4", FileMode.Open);
        public static FileStream Fsw = new FileStream("C:\\archive\\testvideonew.mp4", FileMode.Create);

        //public static FileStream fs = new FileStream("C:\\archive\\text20gb.txt", FileMode.Open);
        //public static FileStream fsw = new FileStream("C:\\archive\\text20gbnew.txt", FileMode.Create);

        public static ConcurrentQueue<(int, byte[])> QueueCompress = new ConcurrentQueue<(int, byte[])>();
        private static readonly AutoResetEvent are = new AutoResetEvent(false);
        private static readonly AutoResetEvent are2 = new AutoResetEvent(true);


        public static void QueueReading()
        {
            using (Fs)
            {
                try
                {
                    var bufferSize = 81920;
                    var buffer = new byte[bufferSize];
                    while (true)
                    {
                        var readBuffer = Fs.Read(buffer, 0, buffer.Length);
                        if (readBuffer == 0)
                        {
                            break;
                        }

                        var item = buffer.ToArray();
                        QueueCompress.Enqueue((readBuffer, item));
                        are.Set();
                        are2.WaitOne(1000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    //return 1;
                }

                Program.ReadingThread.Abort();
            }
        }

        public static void QueueWriting()
        {
            are.WaitOne(-1);
            using (Fsw)
            {
                try
                {
                    while (!QueueCompress.IsEmpty)
                    {
                        are.WaitOne(1000);
                        QueueCompress.TryDequeue(out var resultCom);
                        //QueueCompress.TryDequeue(out var resultCom);
                        Fsw.Write(resultCom.Item2, 0, resultCom.Item1);
                        are2.Set();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    //return 1;
                }
            }

            Program.CompressionThread.Abort();
        }
    }
}