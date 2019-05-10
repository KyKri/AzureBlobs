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
                Console.WriteLine("Creating blob container");
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference("my-container");
                await blobContainer.CreateIfNotExistsAsync(); //To-do handle exception Microsoft.WindowsAzure.Storage.StorageException

                // Set access on the blob container
                BlobContainerPermissions blobPermissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await blobContainer.SetPermissionsAsync(blobPermissions);

                // Create a file for upload
                Console.WriteLine("Creating temp file");
                string currentDir = Directory.GetCurrentDirectory();
                string fileName = "hello.txt";
                string file = Path.Combine(currentDir, fileName);
                await File.WriteAllTextAsync(file, "Hello, World!");

                // Upload the file to the blob container
                Console.WriteLine("Uploading file");
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromFileAsync(file);

                // List blobs in the container
                Console.WriteLine("Listing blobs in container:");
                BlobContinuationToken token = null;
                do 
                {
                    var results = await blobContainer.ListBlobsSegmentedAsync(null, token);
                    token = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine("- " + item.Uri);
                    }
                } while (token != null);

                // Download the file from blob container
                Console.WriteLine("Downloading file");
                string downloadFile = file.Replace(".txt", "_downloaded.txt");
                await blockBlob.DownloadToFileAsync(downloadFile, FileMode.Create);

                // Delete blob container and files
                Console.WriteLine("Blob container created, file uploaded, and copy downloaded.\n" +
                    "Hit any key to delete resources.");
                Console.ReadLine();
                if (blobContainer != null)
                {
                    await blobContainer.DeleteIfExistsAsync();
                }
                File.Delete(file);
                File.Delete(downloadFile);
                Console.WriteLine("Blob container and files have been deleted.");
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
