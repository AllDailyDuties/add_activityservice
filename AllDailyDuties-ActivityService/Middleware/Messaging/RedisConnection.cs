using StackExchange.Redis;

namespace AllDailyDuties_ActivityService.Middleware.Messaging
{
    public class RedisConnection
    {
        private static Lazy<ConnectionMultiplexer> Lazy = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("localhost:6379");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return Lazy.Value;
            }
        }
    }
}