namespace AllDailyDuties_ActivityService.Models.Shared
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public User(Guid userId, string username, string email)
        {
            UserId = userId;
            UserName = username;
            Email = email;
        }
    }
}
