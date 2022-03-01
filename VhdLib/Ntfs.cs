using System.Security.Cryptography;
using DiscUtils.Ntfs;
using DiscUtils.Streams;
using DiscUtils.Vhd;

namespace VhdLib;

public class Ntfs
{
    public static NtfsFileSystem GetNtfs(List<DiskImageFile> disks)
    {
        var disk = new Disk(disks, Ownership.None);

        // Boot Parameter Block
        // Contains the magic number "NTFS    "
        var sequence = new[] {0x4E, 0x54, 0x46, 0x53, 0x20, 0x20, 0x20, 0x20};
        var bpmIndex = FindPosition(disk.Content, Array.ConvertAll(sequence, x=> (byte) x));
        if (bpmIndex == -1)
        {
            throw new IndexOutOfRangeException("Can't find boot parameter block.");
        }
            
        var realParentStream = new OffsetStream(disk.Content, bpmIndex - 3);
        return new NtfsFileSystem(realParentStream);
    }
    
    public static long FindPosition(Stream stream, byte[] sequence)
    {
        if (sequence.Length > stream.Length)
        {
            return -1;
        }
            
        var buffer = new byte[sequence.Length];
        var bufStream = new BufferedStream(stream, sequence.Length);
        while (bufStream.Read(buffer, 0, sequence.Length) == sequence.Length)
        {
            if (sequence.SequenceEqual(buffer))
            {
                return bufStream.Position - sequence.Length;
            }

            bufStream.Position -= sequence.Length - 1;
        }

        return -1;
    }
    
    public static List<string> GetNewFiles(NtfsFileSystem newNtfs, NtfsFileSystem oldNtfs)
    {
        if (newNtfs.Root.FullName != oldNtfs.Root.FullName)
        {
            throw new InvalidDataException("Different root directory");
        }

        var result = new List<string>();
        var (commonDirectories, newDirectories) = GetDirectories(newNtfs, oldNtfs, newNtfs.Root.FullName);
        var newFiles = 0;
        var changedFiles = 0;
        foreach (var file in commonDirectories.SelectMany(newNtfs.GetFiles))
        {
            if (!oldNtfs.FileExists(file))
            {
                result.Add(file);
                newFiles++;
                continue;
            }
                
            using var sha1 = SHA1.Create();
            var newFileHash = sha1.ComputeHash(newNtfs.OpenFile(file, FileMode.Open));
            var oldFileHash = sha1.ComputeHash(oldNtfs.OpenFile(file, FileMode.Open));

            if (!newFileHash.SequenceEqual(oldFileHash))
            {
                result.Add(file);
                changedFiles++;
            }
        }

        foreach (var file in newDirectories.SelectMany(newNtfs.GetFiles))
        {
            result.Add(file);
            newFiles++;
        }
        
        Console.WriteLine($"Total new files: {newFiles}");
        Console.WriteLine($"Total changed files: {changedFiles}");
        Console.WriteLine($"Total new directories: {newDirectories.Count}");
        
        return result;
    }

    public static Tuple<List<string>, List<string>> GetDirectories(NtfsFileSystem newNtfs, NtfsFileSystem oldNtfs, string parentDirNew)
    {
        var commonDirectories = new List<string>();
        var newDirectories = new List<string>();
        foreach (var dir in newNtfs.GetDirectories(parentDirNew))
        {
            if (!oldNtfs.DirectoryExists(dir))
            {
                newDirectories.Add(dir);
            }
            else
            {
                commonDirectories.Add(dir);
            
                var (childCommonDirectories, childNewDirectories) = GetDirectories(newNtfs, oldNtfs, dir);
                commonDirectories.AddRange(childCommonDirectories);
                newDirectories.AddRange(childNewDirectories);
            }
            
        }

        return Tuple.Create(commonDirectories, newDirectories);
    }
}