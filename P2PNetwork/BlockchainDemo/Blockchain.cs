using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainDemo
{
    public class Blockchain
    {

        public IList<Transaction> PendingTransactions = new List<Transaction>();
        //List of blocks (our cahin)
        public IList<Block> Chain { set;  get; }
        //Number of leading zeros
        public int Difficulty { set; get; } = 2;
        //The reward for the miner
        public int Reward = 1; 

        public Blockchain()
        {
            
        }


        public void InitializeChain()
        {
            //Create and initialize our chain
            Chain = new List<Block>();
            //Create the very first block
            AddGenesisBlock();
        }

        public Block CreateGenesisBlock()
        {
            //give the block its necessary inputs (for time we give the current time, for previous hash we give null because it is the first block)
            //(and for transactions list we give PendingTransactions which is empty at the begining)
            Block block = new Block(DateTime.Now, null, PendingTransactions);
            //Start mining for our current transaction
            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        public void AddGenesisBlock()
        {
            //Add the very first block to our chain
            Chain.Add(CreateGenesisBlock());
        }
        
        public Block GetLatestBlock()
        {
            //Get the last block
            return Chain[Chain.Count - 1];
        }

        public void CreateTransaction(Transaction transaction)
        {
            //Add the transaction to the list of transactions of the block
            PendingTransactions.Add(transaction);
        }
        public void ProcessPendingTransactions(string minerAddress)
        {
            //Update the information for the new block (add the transactions related to block to it)
            Block block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);
            AddBlock(block);

            //Renew the transactions list
            PendingTransactions = new List<Transaction>();
            //Create a new transaction for rewarding the miner (this transaction will be added to the next block)
            CreateTransaction(new Transaction(null, minerAddress, Reward));
        }

        public void AddBlock(Block block)
        {
            //Get the current last block
            Block latestBlock = GetLatestBlock();
            //Set the index of the new block 
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            //block.Hash = block.CalculateHash();
            //Mine for the new block
            //PROOF OF WORK
            block.Mine(this.Difficulty);
            //Add the block to the chain
            Chain.Add(block);
        }

        //Check if anything has been modified by a malware or anything
        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                //check if the current block's hash is altered by recalculating hash of the block
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    //If they are not equal, the block and the chain have a problem and is not valid
                    return false;
                }

                //Check whether the previous block's hash is changed in anyway
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }

            //If everything is ok, the chain is valid
            return true;
        }

        //Get the current balance of an user
        public int GetBalance(string address)
        {
            //Initial balance for everyone
            int balance = 100;

            for (int i = 0; i < Chain.Count; i++)
            {
                for (int j = 0; j < Chain[i].Transactions.Count; j++)
                {
                    var transaction = Chain[i].Transactions[j];

                    //if he is paying, the amount will be removed from his account
                    if (transaction.FromAddress == address)
                    {
                        balance -= transaction.Amount;
                    }


                    //if he is receiving money, his balance will be increased 
                    if (transaction.ToAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }
    }
}
