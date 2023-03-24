using Grpc.Net.Client;
using ClinicServiceNamespace;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            ClinicServiceClient client = new(channel);

            var createClientResponse = client.CreateClinet(new CreateClientRequest
            {
                Document = "doc 1",
                FirstName = "FName1",
                Patronymic = "PName1",
                Surname = "SName1"
            });

            Console.WriteLine($"{createClientResponse.ClientId}");
            Console.WriteLine("All Clients:");

            var getClientResponse = client.GetClients(new GetClientsRequest());

            foreach (var item in getClientResponse.Clients)
            {
                Console.WriteLine($"{item.ClientId}, {item.FirstName}, {item.Surname}, {item.Document}");
            }
            Console.ReadKey();
        }
    }
}