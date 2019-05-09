using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace blob_app
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                
            }
            else
            {
                Console.WriteLine("The environment variable storageconnectionstring is not set." 
                    + "\nPlease set it and rerun this program.");
                Console.WriteLine("Hit any key to exit this program");
                Console.ReadLine();
            }
        }
    }
}
