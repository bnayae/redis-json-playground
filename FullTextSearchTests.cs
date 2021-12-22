using System.Diagnostics;

using Redis.OM;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;

using RedisJsonPlayground.Entities;

using Xunit;

// docker run -p 6379:6379 -it --rm --name redis-mod redislabs/redismod:latest

// https://developer.redis.com/develop/dotnet/redis-om-dotnet/simple-text-queries#full-text-search

namespace RedisJsonPlayground
{
    public class FullTextSearchTests
    {
        private readonly RedisConnectionProvider _provider;
        private readonly IRedisCollection<Person> _people;


        public FullTextSearchTests()
        {
            _provider = new RedisConnectionProvider("redis://localhost:6379");
            _provider.Connection.DropIndex(typeof(Person));
            _provider.Connection.CreateIndex(typeof(Person));
            _people = _provider.RedisCollection<Person>();
        }

        private async ValueTask<string[]> InsertAsync()
        {
            string[] result = await Task.WhenAll(
                                    _people.InsertAsync(new Person
                                    {
                                        FirstName = "James",
                                        LastName = "bond",
                                        Age = 50,
                                        Email = "bondjamesbond@email.com",
                                        Tags = "agent,hero",
                                        Desc= "does it matter",
                                        Address = new Address 
                                        {
                                            City = "London",
                                            Street = "Wellington Square",
                                            House = "30",
                                            Zip="12345"
                                        }
                                    }),
                                    _people.InsertAsync(new Person
                                    {
                                        FirstName = "Harry",
                                        LastName = "Potter",
                                        Age = 14,
                                        Email = "potter@email.com",
                                        Tags = "wizard,hero",
                                        Desc= "It's going to shock you",
                                        Address = new Address 
                                        {
                                            City = "London ",
                                            Street = "Privet Drive",
                                            House = "4",
                                            Zip="795483"
                                        }
                                    }),
                                    _people.InsertAsync(new Person
                                    {
                                        FirstName = "HARY",
                                        LastName = "poter",
                                        Age = 14,
                                        Email = "poter@mock.com",
                                        Tags = "mock",
                                        Desc= "go away from it",
                                        Address = new Address 
                                        {
                                            City = "L'ondon",
                                            Street = "Privet Drive",
                                            House = "5",
                                            Zip="8888"
                                        }
                                    }),
                                    _people.InsertAsync(new Person
                                    {
                                        FirstName = "ARY",
                                        LastName = "poterr",
                                        Age = 14,
                                        Email = "ary@mock.com",
                                        Tags = "mock",
                                        Desc= "something went wrong",
                                        Address = new Address 
                                        {
                                            City = "L_ondon",
                                            Street = "Privet Drive",
                                            House = "7",
                                            Zip="7777"
                                        }
                                    }),
                                    _people.InsertAsync(new Person
                                    {
                                        FirstName = "Mr",
                                        LastName = "Smith",
                                        Age = 14,
                                        Email = "smith@email.com",
                                        Tags = "villain",
                                        Desc= "something to do",
                                        Address = new Address 
                                        {
                                            City = "Matrix ",
                                            Street = "anywhere",
                                            House = "2050",
                                            Zip="0110100111"
                                        }
                                    })
                                );
            return result;


        }

        [Fact]
        public async Task SearchTest()
        {
            string[] ids = await InsertAsync();
            try
            {
                var y = await _provider.RedisCollection<Person>().ToImmutableArrayAsync();
                var x = await _people.ToImmutableArrayAsync();
                var people = await _people.Where(p => p.FirstName == "HARRY").ToImmutableArrayAsync();
                var people1 = await _people.Where(p => p.FirstName == "H'ARRY").ToImmutableArrayAsync();
                var people2 = await _people.Where(p => p.FirstName == "HAry").ToImmutableArrayAsync();


            }
            finally
            {
                foreach (var id in ids)
                {
                    _provider.Connection.Unlink(id);
                }                
            }
        }
        

        [Fact]
        public async Task StemmingTest()
        {
            string[] ids = await InsertAsync();
            try
            {
                var y = await _provider.RedisCollection<Person>().ToImmutableArrayAsync();
                var x = await _people.ToImmutableArrayAsync();
                var people = await _people.Where(p => p.Desc == "go").ToImmutableArrayAsync();
                var people1 = await _people.Where(p => p.Desc == "going").ToImmutableArrayAsync();
                var people2 = await _people.Where(p => p.Desc == "goes").ToImmutableArrayAsync();

            }
            finally
            {
                foreach (var id in ids)
                {
                    _provider.Connection.Unlink(id);
                }                
            }
        }
    }
}