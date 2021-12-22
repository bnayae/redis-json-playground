using System.Diagnostics;

using NRediSearch;

using StackExchange.Redis;

using Xunit;
using Xunit.Abstractions;
using System.Text.Json;

using static Weknow.Text.Json.Constants;

using static NRediSearch.Client;

// docker run -p 6379:6379 -it --rm --name redis-mod redislabs/redismod:latest

// https://developer.redis.com/develop/dotnet/redis-om-dotnet/simple-text-queries#full-text-search

namespace RedisJsonPlayground
{
    public class FullTextSearchTests : BaseTests
    {
        public FullTextSearchTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Json_SingleProp_Index_Test()
        {
            string idx = "idx:tests:a";
            Client cl = await GetClientAsync(idx);
            IndexDefinition defenition = new IndexDefinition(prefixes: new string[] { "king:" }, type: IndexDefinition.IndexType.Json);
            Schema sc = new Schema()
                                .AddTextField("$.name");
            Assert.True(await cl.CreateIndexAsync(sc, new ConfiguredIndexOptions(defenition)));

            await Db.ExecuteAsync("JSON.SET", "king:1", ".", "{\"name\": \"henry\"}");
            await Db.ExecuteAsync("JSON.SET", "king:2", ".", "{\"name\": \"james\"}");

            // Query
            SearchResult res = cl.Search(new Query("henry"));

            Assert.Equal(1, res.TotalResults);
            Assert.Equal("king:1", res.Documents[0].Id);
            Assert.Equal("{\"name\":\"henry\"}", res.Documents[0]["json"]);
        }

        [Fact]
        public async Task Json_GeneralSearch_Test()
        {
            string idx = "idx:tests:b";
            Client cl = await PrepareAsync(idx);

            // Query
            SearchResult res = await cl.SearchAsync(new Query("going"));

            Assert.Equal(2, res.TotalResults);
            Assert.Equal("person:2", res.Documents[0].Id);
            RedisValue doc = res.Documents[0]["json"];
            var person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("poter@mock.com", person.Email);
            
            Assert.Equal("person:1", res.Documents[1].Id);
            doc = res.Documents[1]["json"];
            person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("potter@email.com", person.Email);
        }       
        
        [Fact]
        public async Task Json_PropSearch_Test()
        {
            string idx = "idx:tests:c";
            Client cl = await PrepareAsync(idx);

            // Query
            SearchResult res = await cl.SearchAsync(new Query("@desc:going") { WithPayloads = true });

            Assert.Equal(2, res.TotalResults);
            Assert.Equal("person:2", res.Documents[0].Id);
            RedisValue doc = res.Documents[0]["json"];
            var person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("poter@mock.com", person.Email);
            
            Assert.Equal("person:1", res.Documents[1].Id);
            doc = res.Documents[1]["json"];
            person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("potter@email.com", person.Email);
        }

        [Fact]
        public async Task Json_NestProps_Index_Test()
        {
            string idx = "idx:tests:c";
            Client cl = await PrepareAsync(idx);

            // Query
            SearchResult res = await cl.SearchAsync(new Query("lonDon"));

            Assert.Equal(2, res.TotalResults);
            Assert.Equal("person:2", res.Documents[0].Id);
            RedisValue doc = res.Documents[0]["json"];
            var person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("poter@mock.com", person.Email);
            
            Assert.Equal("person:1", res.Documents[1].Id);
            doc = res.Documents[1]["json"];
            person = JsonSerializer.Deserialize<Person>(doc.ToString(), SerializerOptions);
            Assert.Equal("potter@email.com", person.Email);
        }

        private async Task<Client> PrepareAsync(string idx)
        {
            Client cl = await GetClientAsync(idx);
            IndexDefinition defenition = new IndexDefinition(prefixes: new[] { "person:" }, type: IndexDefinition.IndexType.Json);
            Schema sc = new Schema()
                                .AddSortableTextField(FieldName.Of("$.firstName"), unNormalizedForm: true)
                                .AddSortableTextField("$.desc")
                                .AddSortableTextField("$.address.city");
            Assert.True(await cl.CreateIndexAsync(sc, new ConfiguredIndexOptions(defenition)));

            var people = GetPeople();
            int i = 0;
            foreach (var p in people)
            {
                await Db.ExecuteAsync("JSON.SET", $"person:{i++}", ".", p.Serialize(SerializerOptions));
            }

            return cl;
        }

        private Person[] GetPeople()
        {
            Person[] result = {
                                new Person
                                {
                                    FirstName = "James",
                                    LastName = "bond",
                                    Age = 50,
                                    Email = "bondjamesbond@email.com",
                                    Tags = "agent,hero",
                                    Desc = "does it matter",
                                    Address = new Address
                                    {
                                        City = "London",
                                        Street = "Wellington Square",
                                        House = "30",
                                        Zip = "12345"
                                    }
                                },
                                new Person
                                {
                                    FirstName = "Harry",
                                    LastName = "Potter",
                                    Age = 14,
                                    Email = "potter@email.com",
                                    Tags = "wizard,hero",
                                    Desc = "go away from it",
                                    Address = new Address
                                    {
                                        City = "london ",
                                        Street = "Privet Drive",
                                        House = "4",
                                        Zip = "795483"
                                    }
                                },
                                new Person
                                {
                                    FirstName = "HARY",
                                    LastName = "poter",
                                    Age = 14,
                                    Email = "poter@mock.com",
                                    Tags = "mock",
                                    Desc = "It's going to shock you",
                                    Address = new Address
                                    {
                                        City = "L'ondon",
                                        Street = "Privet Drive",
                                        House = "5",
                                        Zip = "8888"
                                    }
                                },
                                new Person
                                {
                                    FirstName = "ARY",
                                    LastName = "poterr",
                                    Age = 14,
                                    Email = "ary@mock.com",
                                    Tags = "mock",
                                    Desc = "something went wrong",
                                    Address = new Address
                                    {
                                        City = "L_ondon",
                                        Street = "Privet Drive",
                                        House = "7",
                                        Zip = "7777"
                                    }
                                },
                                new Person
                                {
                                    FirstName = "Mr",
                                    LastName = "Smith",
                                    Age = 14,
                                    Email = "smith@email.com",
                                    Tags = "villain",
                                    Desc = "something to do",
                                    Address = new Address
                                    {
                                        City = "Matrix ",
                                        Street = "anywhere",
                                        House = "2050",
                                        Zip = "0110100111"
                                    }
                                }
                            };
            return result;


        }

    }
}