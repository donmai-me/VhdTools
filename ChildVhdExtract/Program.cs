using System;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Vhd;

namespace ChildVhdExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            var vhdPath = args[0];

            var diskImage = new DiskImageFile(vhdPath, FileAccess.Read);
            var info = diskImage.Information;
            
            Console.WriteLine($"Disc geometry is {info.Geometry}");
            Console.WriteLine($"Disc type {info.DiskType}");
            Console.WriteLine($"Number of extents {diskImage.Extents.Count}");


            var stream = diskImage.OpenContent(null, Ownership.Dispose);
            
            foreach (var streamExtent in stream.Extents)
            {
                if (streamExtent.Length == 0) continue;
            
                Console.WriteLine($"At {streamExtent.Start} there is extent of size {streamExtent.Length}");
            
                using var fs = new FileStream($"./output/{streamExtent.Start}", FileMode.Create);
                using var ms = new MemoryStream((int)streamExtent.Length);
                stream.Seek(streamExtent.Start, SeekOrigin.Begin);
                stream.Read(ms.GetBuffer(), 0, (int)streamExtent.Length);
                fs.Write(ms.GetBuffer());
            }
        }
    }
}