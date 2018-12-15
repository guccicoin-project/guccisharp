using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GucciSharp
{
    public class Transaction
    {
        /// <summary>
        /// The hash of the block that the transaction is contained in.
        /// </summary>
        public string blockHash { get; set; }
        
        /// <summary>
        /// The total number of guccicoins transferred in this transaction.
        /// </summary>
        public float transactionAmount { get; set; }

        /// <summary>
        /// The block number that the transaction is contained.
        /// </summary>
        public int blockIndex { get; set; }

        /// <summary>
        /// The extra data that is embedded in this transaction.
        /// </summary>
        public string extra { get; set; }

        /// <summary>
        /// The number of guccicoins used for the payment fee.
        /// </summary>
        public float fee { get; set; }

        /// <summary>
        /// Whether the transaction is a coinbase transaction.
        /// </summary>
        public bool isBase { get; set; }

        /// <summary>
        /// The payment ID of the transaction.
        /// </summary>
        public string paymentId { get; set; }

        /// <summary>
        /// The state of the transaction.
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// A UNIX timestamp of when the transaction was created.
        /// </summary>
        public int timestamp { get; set; }

        /// <summary>
        /// The hash of the transaction.
        /// </summary>
        public string transactionHash { get; set; }

        /// <summary>
        /// The address that the transaction is affecting.
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// The amount of guccicoins that are being sent to the <c>address</c>.
        /// </summary>
        public float amount { get; set; }

        /// <summary>
        /// The type of transaction.
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// Whether the transaction is adding money to the interface's wallet.
        /// </summary>
        public bool inbound { get; set; }

        /// <summary>
        /// How long it takes from the transaction being created to the funds able to be spent.
        /// </summary>
        public int unlockTime { get; set; }
    }
}
