

namespace GucciSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using Flurl;
    using Flurl.Http;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Interface
    {

        /// <summary>
        /// Contains the key for accessing the API.
        /// </summary>
        private string apiKey;

        /// <summary>
        /// Contains the base URL for accessing the API.
        /// </summary>
        private string apiBase;

        /// <summary>
        /// Create a new API interface.
        /// </summary>
        /// <param name="key">The API key of the account you are accessing.</param>
        /// <param name="baseUrl">The base URL of the API which should be used.</param>
        public Interface(string key, string baseUrl = "https://webwallet.guccicoin.cf/api/")
        {
            this.apiKey = key;
            this.apiBase = baseUrl;
        }

        /// <summary>
        /// Contains the current confirmed balance in the wallet.
        /// </summary>
        public float BalanceSync
        {
            get
            {
                var info = this.RawInfo().Result;
                return info["balance"]["availableBalance"].Value<float>();
            }
        }

        /// <summary>
        /// Contains the current unconfirmed balance (this cannot be spent until confirmed) in the wallet.
        /// </summary>
        public float UnconfirmedBalanceSync
        {
            get
            {
                var info = this.RawInfo().Result;
                return info["balance"]["lockedAmount"].Value<float>();
            }
        }

        /// <summary>
        /// Contains the current address of the wallet.
        /// </summary>
        public string AddressSync
        {
            get
            {
                var info = this.RawInfo().Result;
                return info["address"].Value<string>();
            }
        }

        /// <summary>
        /// Generate a random 64-bit hex string to be used as a payment ID.
        /// </summary>
        /// <returns>A random 64-bit hex string to be used as a payment ID.</returns>
        public static string CreatePaymentId()
        {
            var r = new Random();
            var buffer = new byte[32];
            r.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            return result;
        }

        /// <summary>
        /// Fetches the response of <c>/hello</c> from the API.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> RawHello()
        {
            var response = await Url.Combine(this.apiBase, "/hello").WithHeader("Authorization", this.apiKey).GetStringAsync();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Fetches the response of <c>/info</c> from the API.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> RawInfo()
        {
            var response = await Url.Combine(this.apiBase, "/info").WithHeader("Authorization", this.apiKey).GetStringAsync();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Fetches the response of <c>/transactions</c> from the API optionally passing in a query for a payment ID to filter by.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> RawTransactions(string paymentId = "")
        {
            var query = string.IsNullOrEmpty(paymentId) ? string.Empty : $"?id={paymentId}";
            var response = await Url.Combine(this.apiBase, "/transactions", query).WithHeader("Authorization", this.apiKey).GetStringAsync();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Sends a request to <c>/send</c> and gets the response from the API.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> RawSend(string recipient, float amount, float fee, string paymentId = "", int mixin = 0)
        {
            var response = await Url.Combine(this.apiBase, "/send").WithHeader("Authorization", this.apiKey)
                               .PostUrlEncodedAsync(new
                                                        {
                                                            recipient = recipient,
                                                            amount = amount,
                                                            fee = fee,
                                                            paymentid = paymentId,
                                                            mixin = mixin
                                                        }).ReceiveString();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Check whether we can access the API and that authentication is working properly.
        /// </summary>
        /// <returns>A boolean to show whether there was a successful connection (<c>true</c> means that there was a successful connection).</returns>
        public bool CheckSync()
        { 
            var response = this.RawHello().Result;
            return response["hello"].Value<string>() == "world";
        }

        /// <summary>
        /// Get a list of transactions that have happened to/from the wallet in the last 1000 blocks.
        /// </summary>
        /// <param name="transactionId">Optional Transaction ID to filter for.</param>
        /// <returns>A list of transactions.</returns>
        public List<Transaction> GetTransactionsSync(string paymentId = "")
        {
            var response = this.RawTransactions(paymentId).Result;
            return (from JObject obj in response["transactions"] select obj.ToObject<Transaction>()).ToList();
        }

        /// <summary>
        /// Create a new transaction to broadcast to the blockchain.
        /// </summary>
        /// <param name="recipient">The address of the person to send funds to.</param>
        /// <param name="amount">The amount of Guccicoin to send the person.</param>
        /// <param name="fee">The amount of Guccicoin to use as a transaction fee (0.1 minimum).</param>
        /// <param name="paymentId">The payment ID to use when sending the transaction.</param>
        /// <param name="mixin">The mixin number to use (change this if you keep getting errors).</param>
        /// <returns></returns>
        public string SendTransactionSync(string recipient, float amount, float fee, string paymentId = "", int mixin = 1)
        {
            var response = this.RawSend(recipient, amount, fee, paymentId, mixin).Result;
            return response["hash"].Value<string>();
        }

        /// <summary>
        /// Check whether we can access the API and that authentication is working properly.
        /// </summary>
        /// <returns>A boolean to show whether there was a successful connection (<c>true</c> means that there was a successful connection).</returns>
        public async Task<bool> CheckAsync()
        {
            var response = await this.RawHello();
            return response["hello"].Value<string>() == "world";
        }

        /// <summary>
        /// Get a list of transactions that have happened to/from the wallet in the last 1000 blocks.
        /// </summary>
        /// <param name="transactionId">Optional Transaction ID to filter for.</param>
        /// <returns>A list of transactions.</returns>
        public async Task<List<Transaction>> GetTransactionsAsync(string paymentId = "")
        {
            var response = await this.RawTransactions(paymentId);
            return (from JObject obj in response["transactions"] select obj.ToObject<Transaction>()).ToList();
        }

        /// <summary>
        /// Create a new transaction to broadcast to the blockchain.
        /// </summary>
        /// <param name="recipient">The address of the person to send funds to.</param>
        /// <param name="amount">The amount of Guccicoin to send the person.</param>
        /// <param name="fee">The amount of Guccicoin to use as a transaction fee (0.1 minimum).</param>
        /// <param name="paymentId">The payment ID to use when sending the transaction.</param>
        /// <param name="mixin">The mixin number to use (change this if you keep getting errors).</param>
        /// <returns></returns>
        public async Task<string> SendTransactionAsync(string recipient, float amount, float fee, string paymentId = "", int mixin = 1)
        {
            var response = await this.RawSend(recipient, amount, fee, paymentId, mixin);
            return response["hash"].Value<string>();
        }

        /// <summary>
        /// Check a response for errors and throw the errors if there are any.
        /// </summary>
        /// <param name="response">The raw JSON string from the API's response.</param>
        private static void HandleErrors(string response)
        {
            var jsonResponse = JObject.Parse(response);
            if (!jsonResponse["ok"].Value<bool>())
            {
                throw new Exception(jsonResponse["error"].Value<string>());
            }
        }

        /// <summary>
        /// Contains the current confirmed balance in the wallet.
        /// </summary>
        public async Task<float> BalanceAsync()
        {
            var info = await this.RawInfo();
            return info["balance"]["availableBalance"].Value<float>();
        }

        /// <summary>
        /// Contains the current unconfirmed balance (this cannot be spent until confirmed) in the wallet.
        /// </summary>
        public async Task<float> UnconfirmedBalanceAsync()
        {
            var info = await this.RawInfo();
            return info["balance"]["lockedAmount"].Value<float>();
        }

        /// <summary>
        /// Contains the current address of the wallet.
        /// </summary>
        public async Task<string> AddressAsync()
        {
            var info = await this.RawInfo();
            return info["address"].Value<string>();
        }
    }
}
