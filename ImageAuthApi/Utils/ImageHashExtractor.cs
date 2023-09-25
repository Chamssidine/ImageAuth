using ImageAuthApi.Data;
using ImageAuthApi.Models;

namespace ImageAuthApi.Utils;

public class ImageHashExtractor
{

    public List<string> ExtractHashFrom( List<UserDataAndImageList> imageList )
    {

        OperationResult OpResult = new();
        List<OperationResult> ErrorResult = new();
        ImgHasher hasher = new();
        List<string> hashList = new();
        List<string> imageNameList = new();
        for (int i = 0 ; i < imageList.Count ; i++)
        {
            try
            {
                for(int j = 0 ; j < imageList[i].Image.Count; j++)
                {
                    OpResult = hasher.HashThis(imageList[i].Image[j]);
                    if (OpResult.IsSuccess)
                    {
                        hashList.Add(hasher.ImageHash);
                        Console.WriteLine($"Image Hash: {hasher.ImageHash}");
                    }
                    else
                    {
                        Console.WriteLine(OpResult.Message);
                        ErrorResult.Add(OpResult);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }
        }
        return hashList;
    }
}
