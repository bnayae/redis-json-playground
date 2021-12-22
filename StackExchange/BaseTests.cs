using System.Diagnostics;
using System.Runtime.CompilerServices;

using NRediSearch;

using StackExchange.Redis;

using Xunit;
using Xunit.Abstractions;

// docker run -p 6379:6379 -it --rm --name redis-mod redislabs/redismod:latest

// https://developer.redis.com/develop/dotnet/redis-om-dotnet/simple-text-queries#full-text-search

namespace RedisJsonPlayground
{
    public abstract class BaseTests: IDisposable
    {
        protected readonly ITestOutputHelper _output;

        public BaseTests(ITestOutputHelper output)
        {
            _output = output;
            _muxer = GetConnection(output);
            Db = _muxer.GetDatabase();
        }

        ~BaseTests() => Dispose();
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _muxer?.Dispose();
        }

        private IConnectionMultiplexer _muxer;
        protected IDatabase Db { get; }


        protected async Task<Client> GetClientAsync(string indexName)
        {
            var client = new Client(indexName, Db);
            var wasReset = await ResetAsync (client);
            return client;
        }


        private static IConnectionMultiplexer GetConnection(ITestOutputHelper output)
        {
            var options = new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" },
                AllowAdmin = true,
                ConnectTimeout = 2000,
                SyncTimeout = 15000,
            };

            try
            {
                var conn = ConnectionMultiplexer.Connect(options);
                return conn;
            }
            catch (RedisConnectionException ex)
            {
                output.WriteLine(ex.GetBaseException().Message);
                throw;
            }
        }

        protected async Task<bool> ResetAsync(Client client)
        {
            _output.WriteLine("Resetting index");
            try
            {
                var result = await client.DropIndexAsync(); // tests create them
                _output.WriteLine($"  Result: {result}");
                return result;
            }
            catch (RedisServerException ex)
            {
                if (string.Equals("Unknown Index name", ex.Message, StringComparison.InvariantCultureIgnoreCase))
                {
                    _output.WriteLine("  Unknown index name");
                    return true;
                }
                if (string.Equals("no such index", ex.Message, StringComparison.InvariantCultureIgnoreCase))
                {
                    _output.WriteLine("  No such index");
                    return true;
                }
                else
                {
                    throw;
                }
            }
        }


    }
}