using AllDailyDuties_ActivityService.Models.Shared;
using AllDailyDuties_ActivityService.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllDailyDuties_ActivityService.Services
{
    public class MessageService : IMessageService
    {
        private CosmosClient _client;
        public MessageService(CosmosClient client)
        {
            _client = client;
               
        }

        public async Task CreateObject<T>(string message, string json, string queue)
        {
            T myObject = JsonConvert.DeserializeObject<T>(message);

            CreateNewTaskAsync(message, myObject, queue);

        }

        private async Task CreateNewTaskAsync<T>(string message, T? myObject, string queue)
        {
            string dbname = "AllDailyDuties";
            string containername = "AgendaService";
            
            // Convert to list of ID's
            List<string> list = new List<string>((IEnumerable<string>)myObject);
            Database database = await _client.CreateDatabaseIfNotExistsAsync(dbname);
            Container container = database.GetContainer(containername);

            // Loop over list to generate query: c.id IN('x-x-x-x','y-y-y-y', ...)
            string query = "SELECT * FROM c WHERE c.id IN (" + string.Join(",", list.Select(x => $"'{x}'")) + ")";
            FeedIterator<dynamic> resultSet = container.GetItemQueryIterator<dynamic>(query);
            List<dynamic> itemList = new List<dynamic>();
            while (resultSet.HasMoreResults)
            {
                Microsoft.Azure.Cosmos.FeedResponse<dynamic> response = await resultSet.ReadNextAsync();
                itemList.AddRange(response);
            }
            // Create List of objects with response
            List<AgendaItem> agendaList = new List<AgendaItem>();
            List<Models.Shared.User> users = new List<Models.Shared.User>();
            List<DateTime> suggestedDates = new List<DateTime>();
            foreach (JObject item in itemList)
            {
                Models.Shared.User user = new Models.Shared.User(
                    item["User"]["Id"].Value<Guid>(),
                    item["User"]["Username"].Value<string>(),
                    item["User"]["Email"].Value<string>()
                    );
                users.Add(user);
                suggestedDates.Add(item["Json"]["ScheduledAt"].Value<DateTime>());
            }
            Console.WriteLine(users);
            Console.WriteLine(suggestedDates);


        }
    }
}
