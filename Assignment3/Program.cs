using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Categoryz> categoryzs = new List<Categoryz>();
            categoryzs.Add(new Categoryz { Id = 1 , Name = "Beverages" });
            categoryzs.Add(new Categoryz { Id = 2 , Name = "Condiments" });
            categoryzs.Add(new Categoryz { Id = 3 , Name = "Confections" });

            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            server.Start();
            Console.WriteLine("Server started ...");


            while (true)
            {   
                var client = server.AcceptTcpClient();
                var request = client.ReadRequest();
                switch (request.Method)
                {
                    case "create":
                        Console.WriteLine("The client is requesting the method: Create");
                        if (!hasBody(request))
                        {
                            client.sendResponse(7, null);
                            break;
                        }
                        create(request, client);
                        break;

                    case "read":
                        Console.WriteLine("The client is requesting the method: Read");
                        
                        read(request, client);
                        break;

                    case "update":
                        Console.WriteLine("The client is requesting the method: Update");
                        if (!hasBody(request))
                        {
                            client.sendResponse(7, null);
                            break;
                        }
                        update();
                        break;

                    case "delete":
                        Console.WriteLine("The client is requesting the method: Delete");
                        delete();
                        
                        break;

                    case "echo":                        
                        Console.WriteLine("The client is requesting the method: Echo");
                        if (!hasBody(request))
                        {
                            client.sendResponse(7, null);
                            break;
                        }
                        echo(request, client);
                        break;
                }

                client.Close();
            }

            void create(Request request, TcpClient client)
            {
                categoryzs.Add(new Categoryz{ Id = categoryzs.Count + 1, Name = request.Body});
            }

            void read(Request request, TcpClient client)
            {

                if (request.Path == "/api/categories")
                {

                    Console.WriteLine("Jeg kører!");
                    var responseObject = new Response { Status = "1 Ok", Body = categoryzs.ToJson() };
                    var responseSerialize = JsonConvert.SerializeObject(responseObject);
                    Console.WriteLine(responseSerialize.ToString());
                    client.SendAnswer(responseSerialize);
                }
                else
                {


                    try
                    {
                        Console.WriteLine("Trying to find readpath ...");
                        var requestPath = request.Path.Split('/')[3];
                        Console.WriteLine("Path found! \n" + requestPath);

                        var intRequestPath = Convert.ToInt32(requestPath);
                        Console.WriteLine("Converted string to integer: " + intRequestPath);

                        Console.WriteLine("Checking if the path exists ...");

                        var doesElementExists = false;
                        foreach (var element in categoryzs)
                        {

                            if (element.Id == intRequestPath)
                            {
                                doesElementExists = true;

                                client.sendResponse(1, categoryzs[0].ToJson());

                                Console.WriteLine("Found request!");
                                //Console.WriteLine(responseSerialize.ToString());
                                Console.WriteLine("Response sent!");
                                break;
                            }
                            else Console.WriteLine("Request: " + requestPath + ", didn't match element: " + element.Id);

                        }
                        if (doesElementExists == false)
                        {
                            // Dette er blot en midlertidig måde at sende et response på.
                            client.sendResponse(5, null);
                            Console.WriteLine("Response status sent: 5 Not found\n");
                        }


                    }
                    catch
                    {
                        client.sendResponse(4, null);

                        Console.WriteLine("No path was found");
                        Console.WriteLine("Response status sent: 4 Bad Request\n");
                    }
                }

            }
            void update()
            {
                Console.WriteLine("Can not handle request yet...");

            }
            void delete()
            {
                Console.WriteLine("Can not handle request yet...");
            }

            void echo(Request request, TcpClient client, string path = "")
            {
                client.sendResponse(1, request.Body);
                Console.WriteLine("Can not handle request yet...");
            }

            bool hasBody(Request request)
            {
                
                var tempHasBody = true;
                if (request.Body == null)
                tempHasBody = false;

                return tempHasBody;
            }


        }
    }
}
