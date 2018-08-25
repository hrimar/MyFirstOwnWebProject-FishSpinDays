using FishSpinDays.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishSpinDays.Tests.Mocks
{
    public static class MockDbContext
    {
        public static FishSpinDaysDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<FishSpinDaysDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            // 1.1. Moke DB:
            return new FishSpinDaysDbContext(options);
        }
    }
}
