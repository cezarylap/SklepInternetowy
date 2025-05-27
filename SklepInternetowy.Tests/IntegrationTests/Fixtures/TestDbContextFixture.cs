using System;
using Microsoft.EntityFrameworkCore;
using ProductSklepInternetowyUI.Data; // <- dostosuj namespace

namespace SklepInternetowy.Tests.IntegrationTests.Fixtures
{
    public class TestDbContextFixture : IDisposable
    {
        public ApplicationDbContext Context { get; }

        public TestDbContextFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;
            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}