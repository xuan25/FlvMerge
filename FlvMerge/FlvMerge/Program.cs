using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlvMerge
{
    class Program
    {
        static readonly bool metadataOnly = false;

        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            while (true)
            {
                stopwatch.Start();

                byte[] debug = null;
                using (FlvFile flv = new FlvFile("1.flv"))
                {
                    using (FileStream stream = new FileStream("2.flv", FileMode.Create))
                    {
                        stream.Write(flv.Header.HeaderBytes, 0, flv.Header.HeaderBytes.Length);
                        do
                        {
                            try
                            {
                                FlvFile.Tag tag = flv.ReadTag();
                                if (tag.Type == FlvFile.Tag.TagType.Script)
                                {
                                    stream.Write(tag.TagBytes, 0, tag.TagBytes.Length);
                                    if (((FlvFile.Tag.ScriptTag.String)((FlvFile.Tag.ScriptTag)tag).Name).Value == "onMetaData")
                                        debug = tag.TagBytes;
                                }
                                else
                                {
                                    stream.Write(tag.TagBytes, 0, tag.TagBytes.Length);
                                }
                            }
                            catch (FlvFile.EofException)
                            {
                                break;
                            }
                        }
                        while (!metadataOnly);
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Metadata Script Tag: ");
                Console.WriteLine();
                Console.WriteLine(BitConverter.ToString(debug).Replace('-', ' '));
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(string.Format("Timer: {0} ms", stopwatch.ElapsedMilliseconds));

                Console.ReadLine();
                stopwatch.Reset();
            }
            
        }
    }
}
