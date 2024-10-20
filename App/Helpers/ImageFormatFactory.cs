using SkiaSharp;

namespace Zine.App.Helpers;

public static class ImageFormatFactory
{
    /// <summary>
    /// The inner KeyValuePair's structure => byte offset -> byte's to match
    /// </summary>
    private static readonly Dictionary<SKEncodedImageFormat, KeyValuePair<int, byte[]>[]> ImageFormatMagicBytesMap = new()
    {
        { SKEncodedImageFormat.Jpeg,
            [
                new KeyValuePair<int, byte[]>(0, [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01]), // .jpg / .jpeg
                new KeyValuePair<int, byte[]>(0, [0xFF, 0xD8, 0xFF, 0xEE]), // .jpg / .jpeg
                new KeyValuePair<int, byte[]>(0, [0xFF, 0xD8, 0xFF, 0xE1, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00]) // .jpg / .jpeg
            ]
        },
        {
            SKEncodedImageFormat.Png,
            [
                new KeyValuePair<int, byte[]>(0, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]), // .png
            ]
        },
        {
            SKEncodedImageFormat.Bmp,
            [
                new KeyValuePair<int, byte[]>(0, [0x42, 0x4D ]), // .bmp
            ]
        },
        {
            SKEncodedImageFormat.Gif,
            [
                new KeyValuePair<int, byte[]>(0, [0x47, 0x49, 0x46, 0x38, 0x37, 0x61]), // .gif
                new KeyValuePair<int, byte[]>(0, [0x47, 0x49, 0x46, 0x38, 0x39, 0x61]), // .gif
            ]
        }
    };

    public static SKEncodedImageFormat Get(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return GetFormatFromBytes(memoryStream.ToArray());
    }

    public static SKEncodedImageFormat Get(string filePath)
    {
        var fileBuffer = File.ReadAllBytes(filePath);
        return Get(fileBuffer);
    }

    public static SKEncodedImageFormat Get(byte[] bytes)
    {
        return GetFormatFromBytes(bytes);
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static SKEncodedImageFormat GetFormatFromBytes(byte[] buffer)
    {
        try
        {
            return ImageFormatMagicBytesMap.First(pair => IsFromFormat(buffer, pair.Key)).Key;
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException("Unsupported image type", e);
        }
    }

    private static bool IsFromFormat(byte[] buffer, SKEncodedImageFormat format)
    {
        var formatMagicBytes = ImageFormatMagicBytesMap[format];

        return formatMagicBytes.Any(formatByteInfo =>
        {
            var bufferChunk = new ArraySegment<byte>(buffer, formatByteInfo.Key, formatByteInfo.Value.Length);
            return formatByteInfo.Value.SequenceEqual(bufferChunk);
        });
    }

}
