using ImageAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ImageAuthApi.Utils
{
    public class JsonFileUploader
    {
        private readonly List<string> _hashList = new();
        public readonly List<OperationResult> Error = new();


        public async Task<OperationResult> UploadJsonFile(IFormFile jsonFile)
        {
            try
            {
                OperationResult result = new();
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var json = await reader.ReadToEndAsync();
                    var rootObjectsList = JsonConvert.DeserializeObject<Root>(json);
                    if (rootObjectsList != null && rootObjectsList.data.Count > 0)
                    {
                        JsonHasher jsonHasher = new JsonHasher();
                        for (int i = 0; i < rootObjectsList.data.Count; i++)
                        {
                            Console.WriteLine(rootObjectsList.data.Count);
                            result = jsonHasher.HashThisJson(new JsonResult(rootObjectsList.data[i]));
                            if (result.IsSuccess)
                            {
                                _hashList.Add(jsonHasher.JsonHash);
                                Console.WriteLine(jsonHasher.JsonHash);
                            }
                            else
                            {
                                Error.Add(result);
                            }
                        }                          
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                return new OperationResult($"Error:{ex.Message}", false);
            }
        }

        public List<string> GetHashList()
        {
            return _hashList;
        }
    }
}
