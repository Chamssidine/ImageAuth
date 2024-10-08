﻿using ImageAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace ImageAuthApi.Utils
{
    public class JsonFileProcessor
    {
        private List<string> _hashList = new List<string>();
        private string _jsonHash;
        public readonly List<OperationResult> Error = new List<OperationResult>();

        public async Task<OperationResult> DeserializeAndHash(IFormFile jsonFile, JsonType jsonType)
        {
            try
            {
                OperationResult result = new();
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var json = await reader.ReadToEndAsync();
                    if(jsonType == JsonType.JsonArray)
                    {
                        var rootObjectsList = JsonConvert.DeserializeObject<Root>(json);

                        if (rootObjectsList?.data != null && rootObjectsList.data.Any())
                        {
                            JsonHasher jsonHasher = new JsonHasher();
                            Console.WriteLine(rootObjectsList.data[0]);
                            foreach (var item in rootObjectsList.data)
                            {
                                result = jsonHasher.HashThisJson(new JsonResult(item));
                                if (result.IsSuccess)
                                {
                                    _hashList.Add(jsonHasher.JsonHash);
                                    Console.WriteLine("hashData:"+jsonHasher.JsonHash);
                                }
                                  
                                else
                                    Error.Add(result);
                            }

                            // Return the result of the last iteration
                        }
                    }
                    else if(jsonType == JsonType.JsonObject) { }
                    {
                        var jsonObject  = JsonConvert.DeserializeObject<Object>(json);
                        if (jsonObject != null)
                        {
                            JsonHasher jsonHasher = new JsonHasher();
                            result = jsonHasher.HashThisJson(new JsonResult(jsonObject));
                            Console.WriteLine("json:"+jsonHasher.JsonHash);
                            if(result.IsSuccess)
                            {
                                _jsonHash =  jsonHasher.JsonHash;
                            }
                        }

                    }
                    return result;

                }
            }
            catch (Exception ex)
            {
                return new OperationResult($"Error: {ex.Message}", false);
            }
        }
        public async Task<OperationResult> HashJsonArray(IFormFile jsonFile)
        {
  
            try
            {
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var jsonString = await reader.ReadToEndAsync();
                    JsonHasher hasher = new JsonHasher();
                    var result = hasher.HashThisJson(jsonString);
                    if(result.IsSuccess)
                    {
                        _jsonHash = hasher.JsonHash;
                    }
                    return new OperationResult("jsonFile hashed successFuly", true);
                }
            }
            catch (Exception ex)
            {
                return new OperationResult(ex.Message, false);
            }
        }
        public List<string> GetHashList()
        {
            return _hashList;
        }
        public string GetJsonHash()
        {
            return _jsonHash;
        }

        /* update */

        public async Task<List<FilteredData>> Deserialize( IFormFile jsonFile )
        {
            try
            {
                var reader = new StreamReader(jsonFile.OpenReadStream());

                var json = await reader.ReadToEndAsync();

                FixedAuditData fixedData = JsonConvert.DeserializeObject<FixedAuditData>(json);
                var filteredJson = fixedData.ToJson();
                var jObject = JObject.Parse(filteredJson);
                List<FilteredData> filteredDataList = new List<FilteredData>();
                string id = fixedData.TransactId;
                foreach (var property in jObject.Properties())
                {
                    filteredDataList.Add(new FilteredData()
                    {
                        TransactId = id,
                        PropertyName = property.Name,
                        PropertyValue = (string)property.Value
                    });
                    ;
                }

                return filteredDataList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DataID>> DeserializeDataIdAsync( JsonResult jsonResult )
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(jsonResult.Value);
                List<DataID> liste = JsonConvert.DeserializeObject<List<DataID>>(jsonContent);
                return liste;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Object Deserialize<T>( JsonResult json )
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(json.ToJson());
                Console.WriteLine(result);
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult($"error {ex.Message}", false);
            }
        }
    }

}
