using System;
using System.IO;

namespace VhdChangedFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(args[0]))
            {
                throw new ArgumentException("Give a directory of VHDs");
            }
            var (oldVhds, newVhds) = VhdLib.Files.PickDiffVhd(args[0]);

            var newNtfs = VhdLib.Ntfs.GetNtfs(newVhds);
            var oldNtfs = VhdLib.Ntfs.GetNtfs(oldVhds);

            var newFiles = VhdLib.Ntfs.GetNewFiles(newNtfs, oldNtfs);
            foreach (var file in newFiles)
            {
                var meme = file.Replace("\\", "/");
                var saveDir = "./new_files/" + meme;
                Directory.CreateDirectory(Directory.GetParent(saveDir).FullName);
                var fs = File.Create(saveDir);
                var ntfsFs = newNtfs.OpenFile(file, FileMode.Open);

                var buffer = new byte[2048];
                int i;
                do
                {
                    i = ntfsFs.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, i);
                } while (i > 0);
            }
        }
    }
}