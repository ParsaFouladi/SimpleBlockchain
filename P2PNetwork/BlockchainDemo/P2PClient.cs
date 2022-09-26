using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace BlockchainDemo
{
    public class P2PClient
    {
        //A dictionary(like in python) for saving server information, such as link
        IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        //The function for connecting the server
        //the url is the link of the server that the client will be connected to
        public void Connect(string url)
        {
            //Check whether the client is already connected to a server or not
            if (!wsDict.ContainsKey(url))
            {
                //Create a new websocket object for connecting to the server
                WebSocket ws = new WebSocket(url);

                //By onMessage we can handle message events from the server
                ws.OnMessage += (sender, e) => 
                {
                    //Check if the server sent 'Hi Client' (for handshacking at the beginning)
                    if (e.Data.Substring(0,9) == "Hi Client")
                    {
                        //Write the message in the console
                        Console.WriteLine(e.Data);
                        //Set nameOtherSide
                        Program.nameOtherSide=e.Data.Substring(16);
                        
                    }

                    
                    
                    //This will be executed for interactions after handshacking
                    else
                    {
                        Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);
                        if (newChain.IsValid() && newChain.Chain.Count > Program.PhillyCoin.Chain.Count)
                        {
                            List<Transaction> newTransactions = new List<Transaction>();
                            newTransactions.AddRange(newChain.PendingTransactions);
                            newTransactions.AddRange(Program.PhillyCoin.PendingTransactions);

                            newChain.PendingTransactions = newTransactions;
                            Program.PhillyCoin = newChain;
                        }
                    }
                };
                ws.Connect();
                ws.Send($"Hi Server, I'am {Program.name}");
                ws.Send(JsonConvert.SerializeObject(Program.PhillyCoin));
                wsDict.Add(url, ws);
            }
        }

        //Function for sending data to the server
        public void Send(string url, string data)
        {
            //find the url of the server
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    //Send the data to server's url
                    item.Value.Send(data);
                }
            }
        }

        //broadcat data for everybody in the server
        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
        }

        public IList<string> GetServers()
        {
            IList<string> servers = new List<string>();
            foreach (var item in wsDict)
            {
                servers.Add(item.Key);
            }
            return servers;
        }

        //Close connections
        public void Close()
        {
            foreach (var item in wsDict)
            {
                item.Value.Close();
            }
        }
    }
}
