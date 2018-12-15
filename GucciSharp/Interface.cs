

namespace GucciSharp
{
    using System;
    using System.Collections.Generic;
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
        public float Balance
        {
            get
            {
                var info = this.Info().Result;
                return info["balance"]["availableBalance"].Value<float>();
            }
        }

        /// <summary>
        /// Contains the current unconfirmed balance (this cannot be spent until confirmed) in the wallet.
        /// </summary>
        public float UnconfirmedBalance
        {
            get
            {
                var info = this.Info().Result;
                return info["balance"]["lockedAmount"].Value<float>();
            }
        }

        /// <summary>
        /// Contains the current address of the wallet.
        /// </summary>
        public string Address
        {
            get
            {
                var info = this.Info().Result;
                return info["address"].Value<string>();
            }
        }

        /// <summary>
        /// Fetches the response of <c>/hello</c> from the API.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> Hello()
        {
            var response = await Url.Combine(this.apiBase, "/hello").WithHeader("Authorization", this.apiKey).GetStringAsync();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Fetches the response of <c>/info</c> from the API.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> Info()
        {
            var response = await Url.Combine(this.apiBase, "/info").WithHeader("Authorization", this.apiKey).GetStringAsync();
            HandleErrors(response);
            return JObject.Parse(response);
        }

        /// <summary>
        /// Fetches the response of <c>/transactions</c> from the API optionally passing in a query for a payment ID to filter by.
        /// </summary>
        /// <returns>A JSON Object containing the response from the API.</returns>
        public async Task<JObject> Transactions(string paymentId = "")
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
        public async Task<JObject> Send(string recipient, float amount, float fee, string paymentId = "", int mixin = 0)
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
        public bool Check()
        { 
            var response = this.Hello().Result;
            return response["hello"].Value<string>() == "world";
        }

        /// <summary>
        /// Get a list of transactions that have happened to/from the wallet in the last 1000 blocks.
        /// </summary>
        /// <param name="transactionId">Optional Transaction ID to filter for.</param>
        /// <returns>A list of transactions.</returns>
        public List<Transaction> GetTransactions(string paymentId = "")
        {
            var response = this.Transactions(paymentId).Result;
            var output = new List<Transaction>();

            foreach (JObject obj in response["transactions"])
            {
                var transaction = obj.ToObject<Transaction>();
                output.Add(transaction);
            }

            return output;
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
        public string SendTransaction(string recipient, float amount, float fee, string paymentId = "", int mixin = 1)
        {
            var response = this.Send(recipient, amount, fee, paymentId, mixin).Result;
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
    }
}
