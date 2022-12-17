/*
    DELAPORTATION - Teleport your files anywhere!

    Coded by: George Delaportas (G0D/ViR4X)
    Copyright (C) 2017
*/

using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using static DELAPORTATION.Models;

namespace DELAPORTATION
{
    public static class WebServices
    {
        // Web service invocator (HTTP/S) [POST]
        public static object WebService(string API, object DataModel)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var NewHTTPClientHandler = new HttpClientHandler())
            {
                NewHTTPClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (var NewHTTPClient = new HttpClient(NewHTTPClientHandler))
                {
                    var Address = new Uri(API);
                    var JSONDataModel = JsonConvert.SerializeObject(DataModel);
                    var Content = new StringContent("gate=service&payload=" + JSONDataModel, Encoding.UTF8);
                    var ServiceResponse = new WebResponseModel() { Auth = false, Status = ResponseStatusCodes.ERROR, Data = null };

                    NewHTTPClient.BaseAddress = Address;
                    NewHTTPClient.DefaultRequestHeaders.Accept.Clear();
                    //NewHTTPClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";
                    Content.Headers.ContentEncoding.Add("gzip");

                    try
                    {
                        var Response = NewHTTPClient.PostAsync(Address, Content).Result;

                        if (Response.IsSuccessStatusCode)
                        {
                            var DataObject = Response.Content.ReadAsStringAsync().Result;

                            return JsonConvert.DeserializeObject<WebResponseModel>(DataObject);
                        }

                        ServiceResponse.Data = Response.StatusCode.ToString();

                        return ServiceResponse;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                        return ServiceResponse;
                    }
                }
            }
        }
    }
}
