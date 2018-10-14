using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EchoServer
{
    public static class Util
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public static T FromJson<T>(this string element)
        {
            return JsonConvert.DeserializeObject<T>(element);
        }

        public static void SendAnswer(this TcpClient client, string answer)

        {
            var msg = Encoding.UTF8.GetBytes(answer);
            client.GetStream().Write(msg, 0, msg.Length);
        }

        public static Request ReadRequest(this TcpClient client)
        {
            var strm = client.GetStream();
            //strm.ReadTimeout = 250;
            byte[] req = new byte[2048];
            using (var memStream = new MemoryStream())
            {
                int bytesread = 0;
                do
                {
                    bytesread = strm.Read(req, 0, req.Length);
                    memStream.Write(req, 0, bytesread);

                } while (bytesread == 2048);

                var requestData = Encoding.UTF8.GetString(memStream.ToArray());
                return JsonConvert.DeserializeObject<Request>(requestData);
            }
        }

        public static void sendResponse(this TcpClient client, int statuscode, string body)
        {
            var thisBody = body;
            var status = "";
            if (statuscode == 1)
            {
                status = "1 Ok";
            }

            if (statuscode == 2)
            {
                status = "2 Created";
            }

            if (statuscode == 3)
            {
                status = "3 Updated";
            }

            if (statuscode == 4)
            {
                status = "4 Bad Request";
            }

            if (statuscode == 5)
            {
                status = "5 Not found";
            }

            if (statuscode == 6)
            {
                status = "6 Error";
            }

            if (statuscode == 7)
            {
                status = "missing body";
            }

           
            var ResponseObject = new Response { Status = status, Body = thisBody };
            var ResponseSerialize = JsonConvert.SerializeObject(ResponseObject);
            client.SendAnswer(ResponseSerialize);
            Console.WriteLine(ResponseSerialize);
        }

        public static void sendListResponse(this TcpClient client, int statuscode, List<Categoryz> categories)
        {
            
            var status = "";
            if (statuscode == 1)
            {
                status = "1 Ok";
            }

            if (statuscode == 2)
            {
                status = "2 Created";
            }

            if (statuscode == 3)
            {
                status = "3 Updated";
            }

            if (statuscode == 4)
            {
                status = "4 Bad Request";
            }

            if (statuscode == 5)
            {
                status = "5 Not found";
            }

            if (statuscode == 6)
            {
                status = "6 Error";
            }

            
            if (statuscode == 7)
            {
                status = "missing body";
            }


            var ResponseObject = new { Status = status, Body = categories };
            var ResponseSerialize = JsonConvert.SerializeObject(ResponseObject);
            client.SendAnswer(ResponseSerialize);
            Console.WriteLine(ResponseSerialize);

        }
    }
}
