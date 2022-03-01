if (File.Exists(args[0]))
{
    throw new ArgumentException("Give a directory of VHDs");
}

var ntfs = VhdLib.Ntfs.GetNtfs(VhdLib.Files.PickVhds(args[0]));

// yea I know I know
var (directories, _) = VhdLib.Ntfs.GetDirectories(ntfs, ntfs, ntfs.Root.FullName);

var files = directories.SelectMany(ntfs.GetFiles).ToList();
foreach (var f in files)
{
    var file = f.Replace("\\", "/");
    var saveDir = "./output/" + file;
    Directory.CreateDirectory(Directory.GetParent(saveDir).FullName);
    
    var fs = File.Create(saveDir);
    var ntfsFs = ntfs.OpenFile(f, FileMode.Open);

    var buffer = new byte[2048];
    int i;
    do
    {
        i = ntfsFs.Read(buffer, 0, buffer.Length);
        fs.Write(buffer, 0, i);
    } while (i > 0);
}