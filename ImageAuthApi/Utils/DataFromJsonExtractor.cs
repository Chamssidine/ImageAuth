using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Newtonsoft.Json;
 
public class DataFromJsonExtractor
{
    public static readonly  DataFromJsonExtractor dataFromJsonExtractor = new();
     
 
    public async Task<List<Data>> GetObjectFromJsonFile( IFormFile jsonFile )
    {
        if (jsonFile == null || jsonFile.Length == 0)
            throw new ArgumentException("Invalid or empty JSON file.");

        var userDataList = new List<Data>();

        try
        {
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                var jsonString = await reader.ReadToEndAsync();
                var rootObjectsList = JsonConvert.DeserializeObject<Root>(jsonString);

                if (rootObjectsList != null && rootObjectsList.data.Count > 0)
                {
                    var httpParser = new HttpParser();

                    //foreach (var data in rootObjectsList.data)
                    //{
                    //    data.Fenetre = await httpParser.GetImageUrlAsync(data.Fenetre);
                    //    data.Porte = await httpParser.GetImageUrlAsync(data.Porte);
                    //    data.Toiture = await httpParser.GetImageUrlAsync(data.Toiture);
                    //    data.Maison = await httpParser.GetImageUrlAsync(data.Maison);
                    //    data.Autrephoto = await httpParser.GetImageUrlAsync(data.Autrephoto);
                    //    userDataList.Add(data);
                    //}
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while processing the JSON file.", ex);
        }

        return userDataList;
    }
}
