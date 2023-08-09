using System.IO.Compression;

var basePath = @"C:\Program Files (x86)\Steam\steamapps\common\Baldurs Gate 3\Data\";
var pakMod = true;
var toggleTooltips = true;
var paks = Directory.GetFiles(basePath);
foreach (var pakFile in paks)
{
    if (pakFile.Contains("Texture")) continue;
    var pak = new LSLib.LS.PackageReader(pakFile).Read();
    foreach (var rootTemplate in pak.Files.Where(f => f.Name.Contains("RootTemplate")))
    {
        try
        {
            var lsfStream = rootTemplate.MakeStream();
            using (var lsfReader = new LSLib.LS.LSFReader(lsfStream))
            {
                var lsf = lsfReader.Read();
                var needUpdate = false;
                var gameObjects = lsf.Regions.First(r => r.Key == "Templates").Value.Children.Where(c => c.Key == "GameObjects");
                foreach (var nodes in gameObjects)
                {
                    foreach (var node in nodes.Value)
                    {
                        var itemType = node.Attributes.First(f => f.Key == "Type").Value.ToString();
                        var validTypes = new List<string> { "item" };
                        if (!validTypes.Contains(itemType)) continue;
                        if (node.Attributes.ContainsKey("Tooltip") && (byte)node.Attributes["Tooltip"].Value != 2) node.Attributes.Remove("Tooltip");
                        if (!node.Attributes.ContainsKey("Tooltip"))
                        {
                            var tooltip = new LSLib.LS.NodeAttribute(LSLib.LS.NodeAttribute.DataType.DT_Byte);
                            tooltip.Value = (byte)2;
                            node.Attributes.Add("Tooltip", tooltip);
                            using (var ms = new MemoryStream())
                            {
                                var lsfWriter = new LSLib.LS.LSFWriter(ms);
                                lsfWriter.Write(lsf);
                                ms.Position = 0;
                                var fileName = basePath + rootTemplate.Name;
                                if (pakMod) fileName = "mod\\" + rootTemplate.Name;
                                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write)) ms.CopyTo(file);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            rootTemplate.ReleaseStream();
        }
    }
}

if (pakMod)
{
    var pkg = new LSLib.LS.Package();
    var files = Directory.GetFiles("mod", "*", SearchOption.AllDirectories);
    foreach (var file in files)
    {
        using (var fsFile = LSLib.LS.FilesystemFileInfo.CreateFromEntry(file, file.Replace("mod\\", "")))
            pkg.Files.Add(fsFile);
    }
    using (var pakFile = new LSLib.LS.PackageWriter(pkg, "HighlightEverything.pak"))
    {
        //pakFile.Compression = LSLib.LS.Enums.CompressionMethod.Zlib; // sucks compared to regular zip on entire uncompressed .pak
        //pakFile.CompressionLevel = LSLib.LS.Enums.CompressionLevel.MaxCompression;
        pakFile.Write();
    }
    if (File.Exists("HighlightEverything.zip")) File.Delete("HighlightEverything.zip");
    using (var zip = ZipFile.Open("HighlightEverything.zip", ZipArchiveMode.Create))
        zip.CreateEntryFromFile("HighlightEverything.pak", "HighlightEverything.pak");
    File.Delete("HighlightEverything.pak");
}


if (toggleTooltips)
{
    var pkg = new LSLib.LS.Package();
    using (var fsFile = LSLib.LS.FilesystemFileInfo.CreateFromEntry("WorldTooltips.xaml", @"Public\Game\GUI\Widgets\" + "WorldTooltips.xaml"))
        pkg.Files.Add(fsFile);
    using (var pakFile = new LSLib.LS.PackageWriter(pkg, "ToggleTooltips.pak"))
    {
        pakFile.Write();
    }
    if (File.Exists("ToggleTooltips.zip")) File.Delete("ToggleTooltips.zip");
    using (var zip = ZipFile.Open("ToggleTooltips.zip", ZipArchiveMode.Create))
        zip.CreateEntryFromFile("WorldTooltips.xaml", @"Public\Game\GUI\Widgets\" + "WorldTooltips.xaml");
    return;
    if (File.Exists("ToggleTooltips.zip")) File.Delete("ToggleTooltips.zip");
    using (var zip = ZipFile.Open("ToggleTooltips.zip", ZipArchiveMode.Create))
        zip.CreateEntryFromFile("ToggleTooltips.pak", "ToggleTooltips.pak");
    File.Delete("ToggleTooltips.pak");
}
