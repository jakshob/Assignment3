using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Assignment3
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

        public static void SendResponse(this TcpClient client, int statuscode, string body)
        {
            var thisBody = body;
            var status = "";

            switch (statuscode)
            {
                case 1:
                    status = "1 Ok";
                    break;
                case 2:
                    status = "2 Created";
                    break;
                case 3:
                    status = "3 Updated";
                    break;
                case 4:
                    status = "4 Bad Request";
                    break;
                case 5:
                    status = "5 Not found";
                    break;
                case 6:
                    status = "6 Error";
                    break;
                case 7:
                    status = "missing body";
                    break;
            }

            if (statuscode == 7)
            {
                status = "illegal body";
            }

            var responseObject = new Response { Status = status, Body = thisBody };
            var responseSerialize = JsonConvert.SerializeObject(responseObject);
            client.SendAnswer(responseSerialize);
            Console.WriteLine("Response sent: " + responseSerialize);
        }
    }
}
