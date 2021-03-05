using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace Archive
{
    public class Queue
    {
        public static FileStream fs = new FileStream("C:\\archive\\text20gb.txt", FileMode.Open);
        public static FileStream fsw = new FileStream("C:\\archive\\text20gbnew.txt", FileMode.Create);

        public static ConcurrentQueue<(int, byte[])> QueueCompress = new ConcurrentQueue<(int, byte[])>();
        //public static AutoResetEvent WaitHandler = new AutoResetEvent(false);
        static object locker = new object();

        public static (int, byte[]) QueueCounter(int count, byte[] bytes)
        {
            (int, byte[]) item = (count, bytes);
            QueueCompress.Enqueue(item);

            return item;

        }

        public static void QueueReading()
        {
            using (fs)
            {
                if (QueueCompress.IsEmpty)
                {
                    var bufferSize = 81920;
                    var buffer = new byte[bufferSize];
                    //WaitHandler.Set();
                    while (true)
                    {
                        //Thread.Sleep(1);
                        var readBuffer = fs.Read(buffer, 0, buffer.Length);
                        if (readBuffer == 0)
                        {
                            break;
                        }

                        var item = buffer.ToArray();
                        QueueCompress.Enqueue((readBuffer, item));

                        //if (QueueBuffer.Count >= 5000)
                        //{
                        //    WaitHandler.Reset();
                        //    WaitHandler.WaitOne();
                        //}
                    }
                }
                Program.readingThread.Abort();
            }
        }

        public static void QueueWriting()
        {
            //WaitHandler.WaitOne();
            Thread.Sleep(500);
            using (fsw)
            {
                if (!QueueCompress.IsEmpty)
                {
                    while (!QueueCompress.IsEmpty)
                    {
                        //foreach (var bytes in QueueBuffer)
                        //{
                        //    foreach (var count in QueueCount)
                        //    {
                        //        fsw.Write(bytes, 0, count);
                        //    }
                        //}
                        QueueCompress.TryDequeue(out var resultCom);
                        fsw.Write(resultCom.Item2, 0, resultCom.Item1);

                        //if (Program.readingThread.IsAlive & (QueueBuffer.Count <= 2500))
                        //{
                        //    WaitHandler.Set();
                        //    Thread.Sleep(500);
                        //}
                    }
                }
            }
            Program.compressionThread.Abort();
        }
    }
}