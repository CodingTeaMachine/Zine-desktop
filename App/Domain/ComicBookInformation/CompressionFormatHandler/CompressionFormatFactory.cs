namespace Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;

public static class CompressionFormatFactory
{
    public static readonly List<string> ComicFileExtensions = [".cb7", ".cba", ".cbr" ,".cbt", ".cbz"];

    /// <summary>
    /// The inner KeyValuePair's structure => byte offset -> byte's to match
    /// </summary>
    private static readonly Dictionary<CompressionFormat, KeyValuePair<int, byte[]>[]> CompressionFormatMagicBytesMap = new()
    {
        { CompressionFormat._7Z,
            [
                new KeyValuePair<int, byte[]>(0, [0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]) // .7z - 7 zip
            ]
        },
        {
            CompressionFormat.Ace,
            [
                new KeyValuePair<int, byte[]>(7, [0x2A, 0x2A, 0x41, 0x43, 0x45, 0x2A, 0x2A]) // .ace - ACE algorithm
            ]
        },
        {
            CompressionFormat.Rar,
            [
                new KeyValuePair<int, byte[]>(0, [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 ]), // .rar / Roshal ARchive v5 and onward
                new KeyValuePair<int, byte[]>(0, [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]), // .rar - Roshal ARchive v1.5 and onward
            ]
        },
        {
            CompressionFormat.Tar,
            [
                new KeyValuePair<int, byte[]>(0, [0x1F, 0x9D]), // z OR tar.z - Lempel-Ziv-Welch algorithm
                new KeyValuePair<int, byte[]>(0, [0x1F, 0XA0]), // z OR tar.z - LZH algorithm
                new KeyValuePair<int, byte[]>(257, [0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30, 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00]), // .tar - tar archive
                new KeyValuePair<int, byte[]>(0, [0x1F, 0x8B]), // .gz OR .tar.gz - GZIP algorithm
                new KeyValuePair<int, byte[]>(0, [0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]), // .xz OR .tar.xz - XZ compression utility using LZMA2 algorithm
            ]
        },
        {
            CompressionFormat.Zip,
            [
                new KeyValuePair<int, byte[]>(0, [0x50, 0x4B, 0x03, 0x04]), // .zip - zip algorithm
                new KeyValuePair<int, byte[]>(0, [0x50, 0x4B, 0x05, 0x06]), // .zip - zip algorithm (empty file)
                new KeyValuePair<int, byte[]>(0, [0x50, 0x4B, 0x07, 0x08]), // .zip - zip algorithm (spanned archive - currently not supported in our app)
            ]
        }
    };

    public static CompressionFormat GetFromFile(string filePath)
    {
        var fileBuffer = File.ReadAllBytes(filePath);

        if(IsZip(fileBuffer))
            return CompressionFormat.Zip;
        if(IsRar(fileBuffer))
            return CompressionFormat.Rar;
        if(Is7Z(fileBuffer))
            return CompressionFormat._7Z;
        if(IsTar(fileBuffer))
            return CompressionFormat.Tar;
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if(IsAce(fileBuffer))
            return CompressionFormat.Ace;

        return CompressionFormat.Unknown;
    }

    private static bool Is7Z(byte[] buffer) => IsFromFormat(buffer, CompressionFormat._7Z);
    private static bool IsAce(byte[] buffer) => IsFromFormat(buffer, CompressionFormat.Ace);
    private static bool IsRar(byte[] buffer) => IsFromFormat(buffer, CompressionFormat.Rar);
    private static bool IsTar(byte[] buffer) => IsFromFormat(buffer, CompressionFormat.Tar);
    private static bool IsZip(byte[] buffer) => IsFromFormat(buffer, CompressionFormat.Zip);

    private static bool IsFromFormat(byte[] buffer, CompressionFormat format)
    {
        var formatMagicBytes = CompressionFormatMagicBytesMap[format];

        return formatMagicBytes.Any(formatByteInfo =>
        {
            var bufferChunk = new ArraySegment<byte>(buffer, formatByteInfo.Key, formatByteInfo.Value.Length);
            return formatByteInfo.Value.SequenceEqual(bufferChunk);
        });
    }
}