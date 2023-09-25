using ImageAuthApi.Data;
using ImageAuthApi.Models;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Net.Http;

namespace ImageAuthApi.Utils;

public class HttpParser
{
    public static readonly HttpClient HttpClient = new HttpClient();
 
    public async Task<string> GetImageUrlAsync(string apiUrl)
    {
        try
        {
           

            string response = await HttpClient.GetStringAsync(apiUrl);
            string imageUrl = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(response)[0];
            return imageUrl;
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
    public async Task<OperationResult> DownloadImageAsync( 
        string imageUrl,
        int imageId
    ) 
    {
        string folderPath = "D:\\ProjetBlockChain\\ImageAuthApiV2\\ImageAuthApi\\ImageAuthApi\\Data";
        string filePath = Path.Combine(folderPath, $"{imageId.ToString()}" + ".jpg");
        OperationResult result = new();
        ImageObject imageList = ImageObject.GetInstance();
        try
        {
          
            HttpResponseMessage response = await HttpClient.GetAsync(imageUrl);
 
            if (response.IsSuccessStatusCode)
            {
              
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                if(imageBytes.Length > 0)
                {
                    imageList.Image.Add(imageId, imageBytes);
                    File.WriteAllBytes(filePath, imageBytes);
                    result.Message = "image downloaded!";
                    result.IsSuccess = true;
                    Console.WriteLine("imageList size: " + imageList.Image.Count);
                }
                else
                {
                    result.Message = "could not  download the image!";
                    result.IsSuccess = false;
                }
               
            }
            else
            {
                result.Message = $"HTTP request failed with status code: {response.StatusCode}";
            }
            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"An error occurred: {ex.Message}";
            return result;
        }
    }
}
