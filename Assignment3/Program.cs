using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace Assignment3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var categoryzs = new List<Categoryz>
            {
                new Categoryz { Id = 1, Name = "Beverages" },
                new Categoryz { Id = 2, Name = "Condiments" },
                new Categoryz { Id = 3, Name = "Confections" }
            };


            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            server.Start();
            Console.WriteLine("Server started ...");
            
            while (true)
            {
                Console.WriteLine("\nWaiting for new client ...");
                var client = server.AcceptTcpClient();
                var t = new Thread(start: new ParameterizedThreadStart(Handle));
                t.Start(client);  
            }
            void Handle(object obj)
            {
                var client = (TcpClient)obj;
                Console.WriteLine("\n----------------------------------------------NEW THREAD CREATED!");

                var request = client.ReadRequest();
                Console.WriteLine("Request: " + request.ToJson() + "\n");
                try
                    {
                        switch (request.Method)
                        {
                            case "create":
                                Console.WriteLine("The client is requesting the method: Create ");
                                if (!HasBody(request))
                                {
                                    client.SendResponse(7, null);
                                    break;
                                }
                                Create(request, client);
                                break;

                            case "read":
                                Console.WriteLine("The client is requesting the method: Read");
                                Read(request, client);
                                break;

                            case "update":
                                Console.WriteLine("The client is requesting the method: Update");
                                if (!HasBody(request))
                                {
                                    client.SendResponse(8, null);
                                    break;
                                }

                                Update(request, client);
                                break;

                            case "delete":
                                Console.WriteLine("The client is requesting the method: Delete");
                                Delete(request, client);
                                break;

                            case "echo":
                                Console.WriteLine("The client is requesting the method: Echo");
                                if (!HasBody(request))
                                {
                                    client.SendResponse(7, null);
                                    break;
                                }
                                Echo(request, client);
                                break;

                            default:
                                ErrorFunction(request, 1);
                                break;
                        }
                    }
                    catch
                    {
                        ErrorFunction(request, 2);
                }
                finally
                {
                    client.Close();
                    Console.WriteLine("Client Closed...");
                }      
            }

            void Create(Request request, TcpClient client)
            {
				Int32 newDate;
                try {
                    if (request.Path is string) {
                        try {
							if(Int32.TryParse(request.Date, out newDate)){
								categoryzs.Add(new Categoryz { Id = categoryzs.Count + 1, Name = request.Body });
							}
							else{
								ErrorFunction(request,0);
							}
						}
                        catch {
                            ErrorFunction(request,0);
                        }
                    }
                    else {
                        ErrorFunction(request,0);
                    }
                }
                catch {
                    ErrorFunction(request,0);
                }
            }

            void Read(Request request, TcpClient client)
            {

                if (request.Path == "/api/categories")
                {
                    Console.WriteLine("Sending all categories: ");
                    client.SendResponse(1, categoryzs.ToJson());
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

                                Console.WriteLine("Found request!");
                                client.SendResponse(1, categoryzs[0].ToJson());
                                break;
                            }
                            Console.WriteLine("Request: " + requestPath + ", didn't match element: " + element.Id);
                        }
                        if (doesElementExists != false) return;

                        client.SendResponse(5, null);
                    }
                    catch
                    {
                        Console.WriteLine("No path was found");
                        client.SendResponse(4, null);
                    }
                }

            }
            void Update(Request request, TcpClient client)
            {

                if (request.Path.Contains("/api/categories/"))
                {
                     var requestPathId = Convert.ToInt32(request.Path.Split('/')[3]);
               
                    //Hvis id som der requestes eksisterer i "categoryz"
                    if (requestPathId <= categoryzs.Count)
                    {
                        //id fra request og id fra category matcher og navnet skiftes med det fra request.
                        categoryzs[requestPathId].Name = request.Body.FromJson<Categoryz>().Name;
                        Console.WriteLine("request-id: " + requestPathId);

                        client.SendResponse(3, categoryzs[requestPathId].ToJson());
                    }

                    else
                    {
                        client.SendResponse(5, null);
                    }
                }
                else
                {
                    client.SendResponse(4,null);
                }
            }

            void Delete(Request request, TcpClient client)
            {
                Console.WriteLine("Method is under construction..");
                try
                {
                    var requestPath = request.Path.Split('/')[3];
                }
                catch
                {
                    client.SendResponse(4, null);
                }

            }

            void Echo(Request request, TcpClient client, string path = "")
            {
                client.SendResponse(1, request.Body);
                Console.WriteLine("Can not handle request yet...");
            }

            bool HasBody(Request request)
            {
                var tempHasBody = request.Body != null;
                return tempHasBody;
            }

            void ErrorFunction(Request request, int sender) {
				var errors = new List<string>{};
				if(sender == 1){ errors.Add("illegal method"); }
				if(sender == 2){ errors.Add("missing method"); }

				try{ if (request.Path is string){} }
				catch{ errors.Add("missing path"); }

				try{ if (request.Path is string){} }
				catch{ errors.Add("missing path"); }


            }
        }
    }
}
