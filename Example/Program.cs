using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    using GucciSharp;

    class Program
    {
        static void Main(string[] args)
        {
            // SETTING UP
            // Create a new interface object, passing in your API key.
            var guccicoin = new Interface(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoiYXV0aDB8NWMwZTFlYjkyYWM1NWMxY2U4NWE5NjM4Iiwia2V5IjoxNywiaWF0IjoxNTQ0ODc5MjY4fQ.ax8-_F0Ef39kB0TCGTrPpw0E7iWO1GwXGnlu1SeCfLE");

            // CONNECTION CHECK
            // Print out whether we are connected or not.
            // interface.Check() will return a boolean.
            Console.WriteLine(
                guccicoin.CheckSync() ? "Connected to the Guccicoin API!" : "There was a problem connecting to the API :(");
            
            Console.WriteLine("The connected wallet:");
            
            // BALANCES
            // TIP: add both Balance and UnconfirmedBalance to get the overall balance of the wallet.
            // interface.Balance contains a float of the current balance of the wallet.
            Console.WriteLine($" - has an confirmed balance of {guccicoin.BalanceSync:n2} GCX");
            
            // interface.UnconfirmedBalance contains a float of the unconfirmed balance of the wallet.
            Console.WriteLine($" - has an unconfirmed balance of {guccicoin.UnconfirmedBalanceSync:n2} GCX");

            // ADDRESS
            // interface.Address contains a string of the current wallet address.
            Console.WriteLine($" - has an address of {guccicoin.AddressSync}");

            // TRANSACTIONS
            // interface.GetTransactions() gets all the transactions in the last 1000 blocks. Pass a payment
            // id to filter transactions with that payment id. Useful for automated pay in systems.
            Console.WriteLine(" - has these transactions with payment id of 5d60ff...3852");
            foreach (var t in guccicoin.GetTransactionsSync(
                "5d60ffd9543bc08e760026fb1e63c4baf665522c38b6e47f326d8994cfea3852"))
            {
                Console.WriteLine($"    > Hash: {t.transactionHash}");
            }

            // SEND
            // interface.SendTransaction() sends a transaction. Check argument help for how to use.
            var hash = guccicoin.SendTransactionSync(
                "gucci3VA3eMd62N4DXM77M4K9FhPuLjW5VEMNmY8zdoSG8BUStCLsH6ZUK6LKTXrWzHbgLwxkF6oANLkd7NiTawtaBDG3n1P59W1p",
                50.0f,
                0.1f,
                "",
                1);
            Console.WriteLine($" - has sent 50.00 GCX (w/ 0.10 GCX fee) to gucci3VA3...9W1p with a hash of {hash}");

            Console.ReadLine();
        }
    }
}
