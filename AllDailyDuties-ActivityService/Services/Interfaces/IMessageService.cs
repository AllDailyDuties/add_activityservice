namespace AllDailyDuties_ActivityService.Services.Interfaces
{
    public interface IMessageService
    {
        Task CreateObject<T>(string message, string json, string queue);
    }
}
