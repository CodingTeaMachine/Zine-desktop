using System.Data;
using System.IO.Compression;
using SkiaSharp;


namespace Zine.App.Helpers;

public static class Image
{
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
	public static byte[] GetResizedImage(ZipArchiveEntry entry, int height, int width, int quality = 100, SKFilterQuality filterQuality = SKFilterQuality.High)
	{
		using var entryStream = entry.Open();

		using var memoryStream = new MemoryStream();
		entryStream.CopyTo(memoryStream);
		var imageBytes = memoryStream.ToArray();

		//entryStream.Seek(0, SeekOrigin.Begin);

		using var originalImage = SKBitmap.Decode(imageBytes);

		if (originalImage == null)
		{
			throw new DataException("Could not decode image");
		}

		using var resizedImage = originalImage.Resize(new SKImageInfo(width, height), filterQuality);

		if (resizedImage == null)
		{
			throw new DataException("Could not resize image");
		}

		using var imageToReturn = SKImage.FromBitmap(resizedImage);
		var imageFormat = GetImageFormat(entry);

		return imageToReturn.Encode(imageFormat, quality).ToArray();
	}



	private static SKEncodedImageFormat GetImageFormat(ZipArchiveEntry entry)
	{
		using var entryStream = entry.Open();
		var codec = SKCodec.Create(entryStream);
		if (codec != null)
		{
			return codec.EncodedFormat;
		}

		//If we couldn't create the codec from the entry
		var filename = entry.Name.Replace("#", "_");
		var fileExtension = Path.GetExtension(filename).Remove(0, 1).ToLower();

		return fileExtension switch
		{
			"jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
			"png" => SKEncodedImageFormat.Png,
			"bmp" => SKEncodedImageFormat.Bmp,
			"gif" => SKEncodedImageFormat.Gif,
			"webp" => SKEncodedImageFormat.Webp,
			"ico" => SKEncodedImageFormat.Ico,
			"wbmp" => SKEncodedImageFormat.Wbmp,
			_ => throw new NotSupportedException($"Unsupported file extension: {fileExtension}")
		};
	}


}
