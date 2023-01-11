namespace AllDailyDuties_ActivityService.Models.Shared
{
    public class AgendaItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Activity { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ScheduledAt { get; set; }

        public AgendaItem(Guid id, string title, string activity, User user, DateTime createdAt, DateTime scheduledAt)
        {
            Id = id;
            Title = title;
            Activity = activity;
            User = user;
            CreatedAt = createdAt;
            ScheduledAt = scheduledAt;
        }

    }
}
