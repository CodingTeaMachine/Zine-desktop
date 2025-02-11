using System.Data;
using System.IO.Compression;
using SharpCompress.Archives;
using SkiaSharp;


namespace Zine.App.Helpers;

public static class Image
{
	
	public static readonly string[] DotExtensions =
	[
		".bmp",
		".gif",
		".jpg", ".jpeg",
		".png",
		".tif", ".tiff",
		".webp"
	];

	public static string[] Extensions => DotExtensions.Select(extension => extension.Remove(0, 1)).ToArray();
	
	/// <summary>
	///
	/// </summary>
	/// <param name="entry"></param>
	/// <param name="height"></param>
	/// <param name="width"></param>
	/// <param name="quality"></param>
	/// <param name="filterQuality"></param>
	/// <returns></returns>
	/// <exception cref="DataException"></exception>
	/// <exception cref="NotSupportedException"></exception>
	public static byte[] GetResizedImage(IArchiveEntry entry, int height, int width, int quality = 100, SKFilterQuality filterQuality = SKFilterQuality.High)
	{
		using var originalImage = GetImageBitmapFromArchiveEntry(entry);
		using var resizedImage = originalImage.Resize(new SKImageInfo(width, height), filterQuality);

		if (resizedImage == null)
		{
			throw new DataException("Could not resize image");
		}

		using var imageToReturn = SKImage.FromBitmap(resizedImage);
		var imageFormat = GetImageFormat(entry);

		return imageToReturn.Encode(imageFormat, quality).ToArray();
	}

	public static ImageDimensions GetImageDimensions(IArchiveEntry entry)
	{
		using var image = GetImageBitmapFromArchiveEntry(entry);
		return new ImageDimensions(image.Width, image.Height);
	}

	public static float GetAspectRatio(IArchiveEntry entry)
	{
		var imageDimensions = GetImageDimensions(entry);

		return (float)imageDimensions.Height / imageDimensions.Width;
	}

	public static bool IsSupported(string imageName)
	{
		return DotExtensions.Any(extension => extension == Path.GetExtension(imageName));
	}



	private static SKEncodedImageFormat GetImageFormat(IArchiveEntry entry)
	{
		using var entryStream = entry.OpenEntryStream();
		var codec = SKCodec.Create(entryStream);
		return
			codec?.EncodedFormat
			?? ImageFormatFactory.Get(entryStream);
	}

	/// <param name="entry"></param>
	/// <returns></returns>
	/// <exception cref="DataException"></exception>
	private static SKBitmap GetImageBitmapFromArchiveEntry(IArchiveEntry entry)
	{
		using var entryStream = entry.OpenEntryStream();

		using var memoryStream = new MemoryStream();
		entryStream.CopyTo(memoryStream);
		var imageBytes = memoryStream.ToArray();

		var originalImage = SKBitmap.Decode(imageBytes);

		if (originalImage == null)
		{
			throw new DataException("Could not decode image");
		}

		return originalImage;

	}
}
