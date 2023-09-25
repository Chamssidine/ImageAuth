using ImageAuthApi.Data;

namespace ImageAuthApi.Utils
{
  
    public class ImageDowlnoader
    {

        public async Task<UserDataAndImageList?> DownLoadImageFrom(Models.Data source)
        {
            if(source != null)
            {
                HttpParser parser = new HttpParser();
                try
                {
                    await parser.DownloadImageAsync(source.Porte, 0);
                    await parser.DownloadImageAsync(source.Fenetre, 1);
                    await parser.DownloadImageAsync(source.Toiture, 2);
                    await parser.DownloadImageAsync(source.Maison, 3);
                    //parser.DownloadImageAsync(source.Autrephoto,4);
                    UserDataAndImageList imageDataList = new();
                    imageDataList.Image = ImageObject.GetInstance().Image;
                    return imageDataList;
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                Console.WriteLine("error");
                return null;
            }
        }
    }
}
