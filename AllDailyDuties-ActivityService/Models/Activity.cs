using AllDailyDuties_ActivityService.Models.Shared;

namespace AllDailyDuties_ActivityService.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public List<AgendaItem> AgendaItem { get; set; }
        public DateTime DefinitiveDate { get; set; }

    }
}
