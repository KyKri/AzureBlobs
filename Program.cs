using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace blob_app
{
    class Program
    {
        static void Main(string[] args)
        {
            BlobOperations().GetAwaiter().GetResult();
        }

        public static async Task BlobOperations()
        {
            string connectionString = Environment.GetEnvironmentVariable("storageconnectionstring");
            
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                // Create blob container if not already created
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference("my-container");
                await blobContainer.CreateIfNotExistsAsync();

                // Set access on the blob container
                BlobContainerPermissions blobPermissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await blobContainer.SetPermissionsAsync(blobPermissions);

                // Create a file for upload
                string currentDir = Directory.GetCurrentDirectory();
                string fileName = "hello.txt";
                string file = Path.Combine(currentDir, fileName);
                await File.WriteAllTextAsync(file, "Hello, World!");
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
