using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlvMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();

                FlvUtil.FlvMerge(new string[] { "1.flv", "1.flv", "1.flv", "1.flv" }, "o.flv");

                stopwatch.Stop();

                FlvFile.Tag metadataTag;
                using (FlvFile flvFile = new FlvFile("o.flv"))
                {
                    metadataTag = flvFile.ReadTag();
                }
                Console.WriteLine("Metadata: \r\n");
                Console.WriteLine(metadataTag);
                Console.WriteLine("\r\n");
                Console.WriteLine("Timer: {0} ms", stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                Console.ReadLine();
            }
        }
    }
}
