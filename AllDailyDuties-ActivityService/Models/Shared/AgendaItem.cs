namespace AllDailyDuties_ActivityService.Models.Shared
{
    public class AgendaItem
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Activity { get; set; }
        public List<User> Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledAt { get; set; }

        public AgendaItem(Guid _id, string title, string activity, List<User> users, DateTime createdAt, DateTime scheduledAt)
        {
            id = _id;
            Title = title;
            Activity = activity;
            Users = users;
            CreatedAt = createdAt;
            ScheduledAt = scheduledAt;
        }

    }
}
