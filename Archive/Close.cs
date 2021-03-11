using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archive
{
    class Close
    {
        public static void Closer(object obj)
        {
            CancellationToken ct = (CancellationToken)obj;

            while (ct.IsCancellationRequested)
            {
                Program.CompressionThread.Abort();
                Program.ReadingThread.Abort();
            }

            Console.WriteLine("Остановлено");
            Console.ReadKey(true);
        }
    }
}
