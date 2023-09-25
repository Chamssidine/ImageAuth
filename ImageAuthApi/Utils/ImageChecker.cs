namespace ImageAuthApi.Utils;
using System.Drawing;
using System.IO;
public sealed class ImageChecker
{
    

    public bool IsImage(IFormFile file)
    {
        var dataIsImage = false;
        using (var memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);
            memoryStream.Position = 0; // Reset the position of the stream to the start
            try
            {
                using (var possibleImage = Image.FromStream(memoryStream))
                {
                    // If no exception is thrown in Image.FromStream, then the file is an image.
                    dataIsImage = true;
                }
            }
            catch
            {
                // If an exception is thrown in Image.FromStream, then the file is not an image.
                dataIsImage = false;
            }
        }
        return dataIsImage;
    }

}
