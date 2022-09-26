using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainDemo
{
    public class Block
    {
        //Which block it is
        public int Index { get; set; }
        //Time of the transaction
        public DateTime TimeStamp { get; set; }
        //Hash of the previous block
        public string PreviousHash { get; set; }
        //Hash of the current block
        public string Hash { get; set; }

        //Transaction data
        //Rememeber that in a block we can have more than one transaction
        public IList<Transaction> Transactions { get; set; }

        //verifying the source data that matches with the generated hash is trivial.
        //Because a specific piece of data can only get a specific hash, the source data must be changed to generate a different hash.
        //This is solved by introducing “nonce” in the data structure.
        //The nonce is an integer. By increasing the nonce, the hash algorithm can generate a different hash.
        //This process will be ended until the generated hash meets the requirement, we call it difficulty.
        public int Nonce { get; set; } = 0;

        //Creat a block
        public Block(DateTime timeStamp, string previousHash, IList<Transaction> transactions)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
        }

        //Calculate hash of the currunt block 
        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            //Combine all the needed for creating hash of the current block
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{JsonConvert.SerializeObject(Transactions)}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            //Convert into a string
            return Convert.ToBase64String(outputBytes);
        }

        public void Mine(int difficulty)
        {
            //Number of leading zeros (called 'difficulty' technically)
            var leadingZeros = new string('0', difficulty);

            //Brute-Forcing for finding the desired hash
            while (this.Hash == null || this.Hash.Substring(0, difficulty) != leadingZeros)
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
            }
        }
    }
}
