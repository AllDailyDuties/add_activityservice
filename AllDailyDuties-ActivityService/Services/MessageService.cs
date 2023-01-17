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
            string title = "";
            string activity = "";

            while (resultSet.HasMoreResults)
            {
                Microsoft.Azure.Cosmos.FeedResponse<dynamic> response = await resultSet.ReadNextAsync();
                itemList.AddRange(response);
                title = itemList[0]["Json"]["Title"];
                activity = itemList[0]["Json"]["Activity"];
            }
            // Create List of objects with response
            List<AgendaItem> agendaList = new List<AgendaItem>();
            List<Models.Shared.User> users = new List<Models.Shared.User>();
            List<DateTime> suggestedDates = new List<DateTime>();
            foreach (JObject item in itemList)
            {
                string mainId = item["id"].Value<string>();
                string id = item["User"]["UserId"].Value<string>();
                Guid guid = Guid.Parse(id);
                Models.Shared.User user = new Models.Shared.User(
                    guid,
                    item["User"]["Username"].Value<string>(),
                    item["User"]["Email"].Value<string>()
                    );
                users.Add(user);
                suggestedDates.Add(item["Json"]["ScheduledAt"].Value<DateTime>());
                //await container.DeleteItemAsync<dynamic>(mainId, new PartitionKey(mainId.ToString()));
                ItemResponse<dynamic> response = await container.ReadItemAsync<dynamic>(mainId, new PartitionKey(mainId.ToString()));
                dynamic items = response.Resource;

                // Update the 'Status' property
                items.Status = "AssignedNew";

                // Replace the document
                response = await container.ReplaceItemAsync<dynamic>(items, mainId, new PartitionKey(mainId.ToString()));
            }

            Console.WriteLine(users);
            var count = suggestedDates.Count;
            double temp = 0D;
            for (int i = 0; i < count; i++)
            {
                temp += suggestedDates[i].Ticks / (double)count;
            }
            var average = new DateTime((long)temp);
            Guid newId = Guid.NewGuid();
            AgendaItem agendaItem = new AgendaItem(newId, title, activity, users, DateTime.Now, average);
            //agendaList.Add();
            string dbname2 = "AllDailyDuties";
            string containername2 = "ActivityService";

            Database database2 = await _client.CreateDatabaseIfNotExistsAsync(dbname2);
            Container container2 = database2.GetContainer(containername2);
            dynamic dbEntry = await container2.CreateItemAsync<dynamic>(
            item: agendaItem,
            partitionKey: new PartitionKey(newId.ToString()));

        }
    }
}
