using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace Archive
{
    public class Queue
    {
        public static FileStream fs = new FileStream("C:\\archive\\testvideo.mp4", FileMode.Open);
        public static FileStream fsw = new FileStream("C:\\archive\\testvideoResult.mp4", FileMode.Create);

        public static ConcurrentQueue<int> QueueCount = new ConcurrentQueue<int>();
        public static ConcurrentQueue<byte[]> QueueBuffer = new ConcurrentQueue<byte[]>();
        public static AutoResetEvent WaitHandler = new AutoResetEvent(false);

        public static void QueueReading()
        {
            using (fs)
            {
                if (QueueCount.IsEmpty & QueueBuffer.IsEmpty)
                {
                    var bufferSize = 81920;
                    var buffer = new byte[bufferSize];
                    WaitHandler.Set();
                    while (true)
                    {
                        var readBuffer = fs.Read(buffer, 0, buffer.Length);
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

                Program.readingThread.Abort();
            }
        }

        public static void QueueWriting()
        {
            WaitHandler.WaitOne();
            Thread.Sleep(500);
            using (fsw)
            {
                if (!QueueCount.IsEmpty | !QueueBuffer.IsEmpty)
                {
                    while (!QueueCount.IsEmpty | !QueueBuffer.IsEmpty)
                    {
                        QueueCount.TryDequeue(out var result);
                        QueueBuffer.TryDequeue(out var resultByte);
                        fsw.Write(resultByte, 0, result);

                        if (Program.readingThread.IsAlive & (QueueBuffer.Count <= 2500))
                        {
                            WaitHandler.Set();
                            Thread.Sleep(500);
                        }
                    }
                }
            }

            WaitHandler.Close();
            Program.compressedThead.Abort();
        }
    }
}