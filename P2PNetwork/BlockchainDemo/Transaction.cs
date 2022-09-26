using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainDemo
{
    public class Transaction
    {
        //Payer
        public string FromAddress { get; set; }
        //Receiver
        public string ToAddress { get; set; }
        public int Amount { get; set; }

        public Transaction(string fromAddress, string toAddress, int amount)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;

        }
    }
}
