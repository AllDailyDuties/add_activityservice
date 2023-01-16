using StackExchange.Redis;

namespace AllDailyDuties_ActivityService.Middleware.Messaging
{
    public class RedisConnection
    {
        private static Lazy<ConnectionMultiplexer> Lazy = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("redis-0.redis.default.svc.cluster.local:6379,password=1_1qwerty");
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