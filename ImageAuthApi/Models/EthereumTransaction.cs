using Nethereum.RPC.Eth.DTOs;

namespace ImageAuthApi.Models
{
    public class EthereumTransaction
    {
        public EthereumTransaction( string transactionHash, string from, string to, string value, string gasPrice, string gas, string status, string logs )
        {
            TransactionHash = transactionHash;
            From = from;
            To = to;
            Value = value;
            GasPrice = gasPrice;
            Gas = gas;
            Status = status;
            Logs = logs;
        }
        public EthereumTransaction()
        {

        }
        public string TransactionHash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string GasPrice { get; set; }
        public string Gas { get; set; }
        public string Status { get; set; }
        public string Logs { get; set; }
        public string key { get; set; }

         
    }
}
