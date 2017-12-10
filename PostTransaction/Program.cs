using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosdbOperations
{
    class Program
    {
        private static readonly string endpointUrl = "XXXXX";
        private static readonly string authorizationKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        private static readonly string databaseId = "BankAccount";
        private static readonly string collectionId = "DetailInfo";
  
       public static void Main(string[] args)
        {
            try
            {

                CreateDocumentClient().Wait();
                Console.ReadLine();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
        }

        private static async Task CreateDocumentClient()
        {
            // Create a new instance of the DocumentClient

            using (var client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
                Database database = client.CreateDatabaseQuery("SELECT * FROM c WHERE c.id ='BankAccount'").AsEnumerable().First(); 
                DocumentCollection collection = client.CreateDocumentCollectionQuery(database.CollectionsLink, "SELECT * FROM c WHERE c.id = 'DetailInfo'").AsEnumerable().First();
                await CreateDocuments(client);
            }
        }
        private async static Task CreateDocuments(DocumentClient client)
        {
            Guid obj = Guid.NewGuid();
            ArrayList accountnumber = new ArrayList
            {
                "SR12345","BC67890","BR98765","AR87456","GS33456","MS21315"
            };
            Random r = new Random();
            int accountnumberindex = r.Next(0, accountnumber.Count - 1);
       
            ArrayList amount = new ArrayList
            {
                100,200,300,400,500,600
            };
            Random amountr = new Random();
            int amountindex = amountr.Next(0, amount.Count - 1);

            ArrayList transactiontype = new ArrayList
            {
                "DR","CR","DR","CR","DR","CR"
            };
            Random transactiontyper = new Random();
            int transactiontypeindex = transactiontyper.Next(0, transactiontype.Count - 1);
            
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId,collectionId);
            Console.WriteLine();
            Console.WriteLine("**** Posting Transaction ****");
            Console.WriteLine();
            dynamic document1Definition = new
            {
                id = obj.ToString(),
                AccountNumber = accountnumber[accountnumberindex],
                Amount = amount[amountindex],
                TransType = transactiontype[transactiontypeindex],
                Remark = (string)transactiontype[transactiontypeindex]=="DR"?"Expense":"Deposit"
        };
            var result = await client.CreateDocumentAsync(collectionLink, document1Definition);
            var document = result.Resource;
            Console.WriteLine("New Transaction Posted : {0}\r\n{1}", document.Id, document);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Re run the program if you want to post new transaction.");
        }
    }
}