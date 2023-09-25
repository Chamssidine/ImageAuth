using ImageAuthApi.Models;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace ImageAuthApi.Utils
{
    public class EthereumTransactionReader
    {

        private readonly Web3 _web3;

        public EthereumTransactionReader(Web3 web3)
        {
            _web3 = web3;
        }

        public async Task<Transaction> GetTransactionAsync(string transactionId )
        {
             var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync( transactionId );
             
            return transaction; 
        }
    }
}
