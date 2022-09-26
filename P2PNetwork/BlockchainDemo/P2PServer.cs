using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BlockchainDemo
{
    //WebSocketBehaviour is a class for servicing clients in the server
    public class P2PServer: WebSocketBehavior
    {
        //For checking if everything is synced
        bool chainSynched = false;
        WebSocketServer wss = null;

        public void Start()
        {
            //Set the server
            wss = new WebSocketServer($"ws://127.0.0.1:{Program.Port}");
            //Set the service
            wss.AddWebSocketService<P2PServer>("/Blockchain");
            wss.Start();
            Console.WriteLine($"Started server at ws://127.0.0.1:{Program.Port}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data.Substring(0,9) == "Hi Server")
            {

                Console.WriteLine(e.Data);
                //Set nameOtherSide
                Program.nameOtherSide = e.Data.Substring(16);
                Send($"Hi Client, I'am {Program.name}");
                
                
            }
            else
            {
                //Get the data from client and decode it from json
                Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);

                //Check if the chain is valid and choose the longest chain (we must choose longest chain for validity always)
                if (newChain.IsValid() && newChain.Chain.Count > Program.PhillyCoin.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    newTransactions.AddRange(Program.PhillyCoin.PendingTransactions);

                    newChain.PendingTransactions = newTransactions;
                    Program.PhillyCoin = newChain;
                }

                if (!chainSynched)
                {
                    Send(JsonConvert.SerializeObject(Program.PhillyCoin));
                    chainSynched = true;
                }
            }
        }
    }
}
