using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CentralNegocio.Shared.Helpers
{
    public static class Util
    {
        public static async Task<T> ConvertResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            string data = string.Empty;
            string nodeError = string.Empty;
            string pages = string.Empty;
            if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode!= HttpStatusCode.InternalServerError)
            {
                string strResult = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(strResult))
                    throw new HttpRequestException($"{(int)httpResponseMessage.StatusCode}|{httpResponseMessage}-{strResult}");
            }
            JsonObject jsonObject = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync())!.AsObject();
            return await Task.Run(() => JsonSerializer.Deserialize<T>(jsonObject)!);
            #region test
                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    data = jsonObject["data"]!.GetValue<string>()!;
                //    nodeError = jsonObject["error"]!.GetValue<string>()!;
                //    pages = jsonObject["pages"]!.GetValue<string>()!;
                //}
                //else
                //    nodeError = jsonObject["error"]!.GetValue<string>()!;                            

                //return await ConvertObject<T>(JsonSerializer.Serialize((object)new
                //{
                //    error = nodeError,
                //    data,
                //    pages
                //}, (JsonSerializerOptions?)null));

                //return await ConvertObject<T>(jsonObject.GetValue<string>());
            #endregion
        }

        public static async Task<T> ConvertObject<T>(string dataJson)
        {
            string dataJson2 = dataJson;
            return await Task.Run(() => JsonSerializer.Deserialize<T>(dataJson2)!);
        }
    }
}
