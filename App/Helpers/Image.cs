using System.Data;
using SharpCompress.Archives;
using SkiaSharp;


namespace Zine.App.Helpers;

public static class Image
{
	
	private static readonly string[] DotExtensions =
	[
		".bmp",
		".gif",
		".jpg", ".jpeg",
		".png",
		".tif", ".tiff",
		".webp"
	];

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

	public static float GetAspectRatio(IArchiveEntry entry)
	{
		var imageDimensions = GetDimensions(entry);

		return (float)imageDimensions.Height / imageDimensions.Width;
	}

	public static bool IsSupported(string imageName)
	{
		//TODO: Refacor like the compression format ones, to check bytes
		return DotExtensions.Any(extension => extension == Path.GetExtension(imageName).ToLower());
	}

	public static ImageDimensions GetDimensions(IArchiveEntry entry)
	{
		using var image = GetImageBitmapFromArchiveEntry(entry);
		return new ImageDimensions(image.Width, image.Height);
	}

	private static SKEncodedImageFormat GetImageFormat(IArchiveEntry entry)
	{
		var entryStream = entry.OpenEntryStream();
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
		var entryStream = entry.OpenEntryStream();
		var originalImage = SKBitmap.Decode(entryStream);


		if (originalImage == null)
		{
			throw new DataException($"Could not decode image: {entry.Key}");
		}

		return originalImage;
	}
}
