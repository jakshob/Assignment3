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
                try{
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
                                client.sendResponse(8, null);
                                break;
                            }

                            update(request, client);
                                break;
                        
                       case "delete":
                            Console.WriteLine("The client is requesting the method: Delete");
                            delete(request,client);
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
                }
                catch{
                    errorFunction();
                }                      

                client.Close();
            }
            
            void create(Request request, TcpClient client)
            {
				Int32 newDate;
                try {
                    if (request.Path is string) {
                        try {
							if(Int32.TryParse(request.Date, out newDate)){
								categoryzs.Add(new Categoryz { Id = categoryzs.Count + 1, Name = request.Body });
							}
							else{
								errorFunction();
							}
						}
                        catch {
                            errorFunction();
                        }
                    }
                    else {
                        errorFunction();
                    }
                }
                catch {
                    errorFunction();
                }
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
            void update(Request request, TcpClient client)
            {

                if (request.Path.Contains("/api/categories/"))
                {
                    //Der er en gentagelse af splitmetode her også
                     var requestPathId = Convert.ToInt32(request.Path.Split('/')[3]);
               
                    //Hvis id som der requestes eksisterer i "categoryz"
                    if (requestPathId <= categoryzs.Count)
                    {

                        //id fra request og id fra category matcher og navnet skiftes med det fra request.
                        categoryzs[requestPathId].Name = request.Body.FromJson<Categoryz>().Name;
                        Console.WriteLine("request-id: " + requestPathId);

                        //Gentagelse af send response. (alle gentagelser skal lige fixes i én metode el. lign.)
                        var responseObject = new Response { Status = "3 updated", Body = categoryzs[requestPathId].ToJson() };
                        var responseSerialize = JsonConvert.SerializeObject(responseObject);
                        Console.WriteLine(responseSerialize.ToString());
                        client.SendAnswer(responseSerialize);
                    }

                    else
                    {
                        //Send fejlkode + Gentagelse af send response
                        var responseObject = new Response { Status = "5 not found" };
                        var responseSerialize = JsonConvert.SerializeObject(responseObject);
                        Console.WriteLine(responseSerialize.ToString());
                        client.SendAnswer(responseSerialize);
                    }
                }
                else { Console.WriteLine("Missing Path");
                    //Send fejlkode + Gentagelse af send response
                    var responseObject = new Response { Status = "4 bad request" };
                    var responseSerialize = JsonConvert.SerializeObject(responseObject);
                    Console.WriteLine(responseSerialize.ToString());
                    client.SendAnswer(responseSerialize);
                }
                
                //Console.WriteLine("Can not handle request yet...");

            }
            void delete(Request request, TcpClient client)
            {
                Console.WriteLine("Methode is under construction..");
                try
                {
                    var requestPath = request.Path.Split('/')[3];
                }
                catch
                {
                    Console.WriteLine("No path wasfound");
                    client.sendResponse(4, null);
                }

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

            void errorFunction() {

            }
        }
    }
}
