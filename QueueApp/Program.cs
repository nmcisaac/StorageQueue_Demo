using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace QueueApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "<<replace with storage connection string>>";

            QueueClient queue = new QueueClient(connectionString, "mystoragequeue");

            Console.WriteLine("Sending a message.....");
            await InsertMessageAsync(queue, $"Hello from Neil at {System.DateTime.Now.ToShortTimeString()}");

            Console.WriteLine("Press any key to continue.....");
            Console.ReadLine();

            Console.WriteLine("Receiving a message.....");
            Console.WriteLine($"Recieved: {await RetrieveNextMessageAsync(queue)}");

            Console.WriteLine("Press any key to continue.....");
            Console.ReadLine();
        }

        static async Task InsertMessageAsync(QueueClient theQueue, string newMessage)
        {
            if (null != await theQueue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue was created.");
            }
            await theQueue.SendMessageAsync(newMessage);
            Console.WriteLine($"Sent: {newMessage}");
        }

        static async Task<string> RetrieveNextMessageAsync(QueueClient theQueue)
        {
            if (await theQueue.ExistsAsync())
            {
                QueueProperties properties = await theQueue.GetPropertiesAsync();
                if (properties.ApproximateMessagesCount > 0)
                {
                    QueueMessage[] retrievedMessage = await theQueue.ReceiveMessagesAsync(1);
                    string theMessage = retrievedMessage[0].MessageText;
                    await theQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                    return theMessage;
                }
                return null;
            }
            return null;
        }
    }
}
