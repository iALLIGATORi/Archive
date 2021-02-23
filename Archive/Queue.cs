using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archive
{
    public class Queue
    {
        public static FileStream fs = new FileStream("D:\\archive\\queue.txt", FileMode.Open);
        public static FileStream fsw = new FileStream("D:\\archive\\queueResult.txt", FileMode.Create);
        public static ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
        public static ConcurrentQueue<int> test = new ConcurrentQueue<int>();
        public static ConcurrentQueue<byte[]> queueBuffer = new ConcurrentQueue<byte[]>();
        public static byte[] buffer = new byte[8];
        public static int read = 1;
        public static void QueueReading()
        {
            using (fs)
            {

                //for (int i = 0; i < 10; i++)
                //{
                //    test.Enqueue(i);
                //}

                //test.Enqueue(2);
                //test.Enqueue(3);
                //test.Enqueue(4);
                //test.Enqueue(5);
                //test.Enqueue(6);
                //test.Enqueue(7);
                //test.Enqueue(8);
                //test.Enqueue(9);
                //test.Enqueue(10);

                if (queue.IsEmpty | queueBuffer.IsEmpty)
                {
                    while (read != 0)
                    {
                        read = fs.Read(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            break;
                        }
                        var received = buffer.Take(read).ToArray();
                        //queue.Enqueue(read);
                        queueBuffer.Enqueue(received);
                    }
                }

            }
        }

        public static void QueueWriting()
        {
            Thread.Sleep(100);
            using (fsw)
            {
                //while (!test.IsEmpty)
                //{
                //    test.TryDequeue(out int tested);
                //    Console.WriteLine(tested);
                //}


                while (!queue.IsEmpty | !queueBuffer.IsEmpty)
                {
                    queue.TryDequeue(out int result);
                    queueBuffer.TryDequeue(out byte[] resultByte);
                    fsw.Write(resultByte, 0, result);
                }
            }



        }

    }
}
