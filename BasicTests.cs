using System.Diagnostics;

using Redis.OM;

using RedisJsonPlayground.Entities;

using Xunit;

// docker run -p 6379:6379 -it --rm --name redis-mod redislabs/redismod:latest

namespace RedisJsonPlayground
{
    public class BasicTests
    {
        private readonly RedisConnectionProvider _provider;


        public BasicTests()
        {
            _provider = new RedisConnectionProvider("redis://localhost:6379");
            _provider.Connection.DropIndex(typeof(Customer));
            _provider.Connection.CreateIndex(typeof(Customer));
        }


        [Fact]
        public async Task InsertAndQueryTest()
        {
            var customers = _provider.RedisCollection<Customer>();

            // Insert customer
            var k1 = await customers.InsertAsync(new Customer()
            {
                FirstName = "James",
                LastName = "bond",
                Age = 50,
                Email = "bondjamesbond@email.com"
            });

            var k2 = await customers.InsertAsync(new Customer()
            {
                FirstName = "James",
                LastName = "Torry",
                Age = 68,
                Email = "torry@email.com"
            });

            // Insert customer
            var k3 = await customers.InsertAsync(new Customer()
            {
                FirstName = "Bob",
                LastName = "Bond",
                Age = 90,
                Email = "bob@email.com"
            });


            try
            {

                // Find all customers whose last name is "Bond"
                var res1 = await customers.Where(x => x.LastName == "Bond").ToImmutableArrayAsync();

                Assert.Equal(2, res1.Length);
                Assert.Contains(res1, m => m.Email == "bob@email.com");
                Assert.Contains(res1, m => m.Email == "bondjamesbond@email.com");

                // Find all customers whose last name is Bond OR whose age is greater than 65
                var res2 = await customers.Where(x => x.Age > 65).ToImmutableArrayAsync();
                Assert.Equal(2, res2.Length);
                Assert.Contains(res2, m => m.Email == "bob@email.com");
                Assert.Contains(res2, m => m.Email == "torry@email.com");

                // Find all customers whose last name is Bond AND whose first name is James
                var res3 = await customers.Where(x => x.LastName == "Bond" && x.FirstName == "James").ToImmutableArrayAsync();
                Assert.Single(res3);
                Assert.Contains(res3, m => m.Email == "bondjamesbond@email.com");

                var res4 = await customers.Where(x => x.FirstName == "JAMES").ToImmutableArrayAsync();

                Assert.Equal(2, res4.Length);
                Assert.Contains(res4, m => m.Email == "torry@email.com");
                Assert.Contains(res4, m => m.Email == "bondjamesbond@email.com");
            }
            finally
            {
                _provider.Connection.Unlink(k1);
                _provider.Connection.Unlink(k2);
                _provider.Connection.Unlink(k3);
            }
        }

        [Fact]
        public async Task ConstraintTest()
        {
            var customers = _provider.RedisCollection<Customer>();

            var k1 = await customers.InsertAsync(new Customer()
            {
                FirstName = "James",
                LastName = "bond",
                Age = 50,
                Email = "bondjamesbond@email.com"
            });


            try
            {
                var k2 = await customers.InsertAsync(new Customer()
                {
                    FirstName = "James-clone",
                    LastName = "bond-clone",
                    Age = 100,
                    Email = "bondjamesbond@email.com"
                });

                var res1 = await customers.FindByIdAsync(k2);
                Assert.Equal(k1, k2);
                Assert.Equal("James-clone", res1?.FirstName);

                var k3 = await _provider.Connection.SetAsync(new Customer()
                {
                    FirstName = "James-x",
                    LastName = "bond-x",
                    Age = 100,
                    Email = "bondjamesbond@email.com"
                });

                var res2 = await customers.FindByIdAsync(k2);
                Assert.Equal("James-x", res2?.FirstName);
            }
            finally
            {
                _provider.Connection.Unlink(k1);
            }
        }

        [Fact]
        public async Task TagsTest()
        {
            var customers = _provider.RedisCollection<Customer>();

            var k1 = await customers.InsertAsync(new Customer()
            {
                FirstName = "James",
                LastName = "bond",
                Age = 50,
                Email = "bondjamesbond@email.com",
                Tags = "agent,hero"
            });
            var k2 = await customers.InsertAsync(new Customer()
            {
                FirstName = "Harry",
                LastName = "Poter",
                Age = 14,
                Email = "poter@email.com",
                Tags = "wizard,hero"
            });
            var k3 = await customers.InsertAsync(new Customer()
            {
                FirstName = "Mr",
                LastName = "Smith",
                Age = 14,
                Email = "smith@email.com",
                Tags = "villain"
            });


            try
            {
                var res1 = await customers.Where(c => c.Tags == "agent" || c.Tags == "villain").ToImmutableArrayAsync();
                Assert.Equal(2, res1.Length);
                Assert.Contains(res1, m => m.Email == "bondjamesbond@email.com");
                Assert.Contains(res1, m => m.Email == "smith@email.com");

                var res2 = await customers.Where(c => c.Tags == "wizard" && c.Tags == "hero").ToImmutableArrayAsync();
                Assert.Single(res2);
                Assert.Contains(res2, m => m.Email == "poter@email.com");

            }
            finally
            {
                _provider.Connection.Unlink(k1);
                _provider.Connection.Unlink(k2);
                _provider.Connection.Unlink(k3);
            }
        }
    }
}