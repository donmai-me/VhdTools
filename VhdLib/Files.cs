using System.Text.RegularExpressions;
using DiscUtils.Vhd;

namespace VhdLib;

public static class Files
{
    static string[] GetVhds(string folderPath)
    {
        var vhds = Directory.GetFiles(folderPath, "*.vhd");
        if (vhds.Length == 0)
        {
            throw new ArgumentException("No VHD found", nameof(folderPath));
        }
        Array.Sort(vhds);

        return vhds;
    }
    public static Tuple<List<DiskImageFile>, List<DiskImageFile>> PickDiffVhd(string vhdsFolderPath)
        {
            if (!Directory.Exists(vhdsFolderPath))
            {
                throw new DirectoryNotFoundException();
            }

            var vhds = GetVhds(vhdsFolderPath);
            if (vhds.Length < 2)
            {
                throw new ArgumentException("Need at least two VHDs", nameof(vhdsFolderPath));
            }
            
            Console.WriteLine($"Found {vhds.Length} files");

            foreach (var vhd in vhds.Select((s, i) => new {Value = s, Index = i}))
            {
                if (vhd.Index == 0)
                {
                    continue;
                }
                
                Console.WriteLine($"{vhd.Index - 1}: {vhd.Value}");
            }

            var index = 1;

            if (vhds.Length > 2)
            {
                Console.Write($"Pick a VHD to check for new/modified files [0-{vhds.Length-2}]: ");
                var answer = Console.ReadLine();
                if (answer is null)
                {
                    throw new NullReferenceException();
                }

                index = int.Parse(answer) + 1;
                if (index <= 0 || index >= vhds.Length)
                {
                    throw new Exception("Invalid index");
                }
            }
            
            
            var parent = new List<DiskImageFile>();
            var child = new List<DiskImageFile>();
            for (var i = index; i >= 0; i--)
            {
                var diskImage = new DiskImageFile(vhds[i], FileAccess.Read);
                child.Add(diskImage);
                if (i != index)
                {
                    parent.Add(diskImage);
                }
            }

            return Tuple.Create(parent, child);
        }

    public static List<DiskImageFile> PickVhds(string vhdsFolderPath)
    {
        if (!Directory.Exists(vhdsFolderPath))
        {
            throw new DirectoryNotFoundException();
        }

        var vhds = GetVhds(vhdsFolderPath);
        Console.WriteLine($"Found {vhds.Length} files");

        foreach (var vhd in vhds.Select((s, i) => new {Value = s, Index = i}))
        {
            Console.WriteLine($"{vhd.Index}: {vhd.Value}");
        }

        Console.WriteLine("Give indices of parent and child VHDs to extract (In reverse chronological order, parent VHD is last)");
        Console.WriteLine("Example: 3, 2, 1, 0");
        Console.Write("Pick VHDs: ");
        var answer = Console.ReadLine();
        if (answer is null)
        {
            throw new NullReferenceException();
        }
        
        answer = Regex.Replace(answer, @"\s+", "");
        var indices = answer.Split(",").Select(int.Parse).ToArray();
        
        
        if (indices == null || indices.Length == 0)
        {
            throw new NullReferenceException();
        }

        return indices.Select(i => new DiskImageFile(vhds[i], FileAccess.Read)).ToList();
    }
}