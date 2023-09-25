using ImageAuthApi.Models;

namespace ImageAuthApi.Utils
{
    public class Payloader
    {

        public async Task<List<OperationResult>> SendePayload(List<string> hashList,ContractManager _contractManager)
        {
            List<OperationResult> result = new List<OperationResult>();
            if (hashList.Count > 0)
            {

                for (int i = 0; i < hashList.Count; i++)
                {
                    DateTimeOffset currentDateTime = DateTimeOffset.UtcNow;
                    string formattedDateTime = currentDateTime.ToString("yyy-MM-dd HH:mm:ss");
                    var response = await _contractManager.SendData(hashList[i]);
                    Console.WriteLine("responseMessage:" + response.Message);
                    var dataFromContract = await _contractManager.GetHashDataByIndex(i);
                    var messageFromResponse = response;

                    //if (dataFromContract != null)
                    //{
                    //    HashData data = new(
                    //        0,
                    //        (int)dataFromContract.ReturnValue1.TimeOfSave,
                    //        dataFromContract.ReturnValue1.ImageHash,
                    //        messageFromResponse.Message,
                    //        imageNameList[i],
                    //        formattedDateTime
                    //    );

                    //    _dbManager.Save(_configuration, data);
                    //}
                    response.Message = $"data sent successfully. Transaction hash:{response.Message}";
                    result.Add(response);
                }
                return result;
            }
            else
            {
                return new List<OperationResult>(null);
            }
        }
    }
}
