using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.IO;
using System.Threading;

namespace NethereumDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var senderAddress = "0x9e9079d28e180c77a73dfe042cfa862918ea401b";
            var passphrase = "Shut the fuck up";

            var web3 = new Web3();
            var unlocked = web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, passphrase, 120).GetAwaiter().GetResult();

            var abiFile = @"C:\Program Files\Geth\bin\greeter";
            var abi = File.ReadAllText(abiFile + ".abi");
            var bytecode = File.ReadAllText(abiFile + ".bin");
            var transactionHash = web3.Eth.DeployContract.SendRequestAsync(abi, bytecode, senderAddress, new HexBigInteger(220000), "Robert").GetAwaiter().GetResult();

            // 0x9ac21e68595ffd8443d8e9a4e59f643871863679536943b90e94b190877d2de1
            TransactionReceipt receipt = null;
            while (receipt == null)
            {
                receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).GetAwaiter().GetResult();
                if (receipt == null)
                    Thread.Sleep(1000);
            }

            var contractAddress = receipt.ContractAddress;

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var func = contract.GetFunction("greet");

            // Local dry run
            var funcResult = func.CallAsync<string>().GetAwaiter().GetResult();

            var funcTranHash = func.SendTransactionAsync(senderAddress).GetAwaiter().GetResult();

            receipt = null;
            while (receipt == null)
            {
                receipt = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(funcTranHash).GetAwaiter().GetResult();
                if (receipt == null)
                    Thread.Sleep(1000);
            }

            //Console.WriteLine("FuncResult: " + receip);


        }
    }
}