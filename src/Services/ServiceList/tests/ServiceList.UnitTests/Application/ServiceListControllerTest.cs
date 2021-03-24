using Microsoft.AspNetCore.Mvc;
using ServiceListAPI;
using ServiceListAPI.Controllers;
using ServiceListAPI.Infrastructure;
using ServiceListAPI.Model;
using ServiceListAPI.ViewModel;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ServiceListAPI.IntegrationEvents;
using AutoFixture;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.ServiceList.Application
{
    public class ServiceListControllerTest
    {
        private readonly DbContextOptions<ServiceListContext> _dbOptions;

        public ServiceListControllerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<ServiceListContext>()
                .UseInMemoryDatabase(databaseName: "service-list-in-memory")
                .Options;

            using (var dbContext = new ServiceListContext(_dbOptions))
            {
                dbContext.AddRange(GetServiceListItems());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_ServiceLists_items_success()
        {
            //Arrange
            var pageSize = 10;
            var pageIndex = 1;

            var expectedItemsInPage = 10;
            var expectedTotalItems = 20;

            var serviceListContext = new ServiceListContext(_dbOptions);
            var settings = new TestServiceListSettings();

            var integrationServicesMock = new Mock<IServiceListIntegrationEventService>();

            //Act
            var serviceListController = new ServiceListController(serviceListContext, settings, integrationServicesMock.Object);
            var actionResult = await serviceListController.ItemsAsync(pageSize, pageIndex);

            //Assert
            var okResult = actionResult as OkObjectResult;
            Assert.IsType<PaginatedItemsViewModel<ServiceListItem>>(okResult.Value);
            var page = Assert.IsAssignableFrom<PaginatedItemsViewModel<ServiceListItem>>(okResult.Value);
            Assert.Equal(expectedTotalItems, page.Count);
            Assert.Equal(pageIndex, page.PageIndex);
            Assert.Equal(pageSize, page.PageSize);
            Assert.Equal(expectedItemsInPage, page.Data.Count());
        }

        private List<ServiceListItem> GetServiceListItems()
        {
            Fixture fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture.CreateMany<ServiceListItem>(20).ToList();
        }
    }

    public class TestServiceListSettings : IOptionsSnapshot<ServiceListSettings>
    {
        public ServiceListSettings Value => new ServiceListSettings
        {
            AzureStorageEnabled = true
        };

        public ServiceListSettings Get(string name) => Value;
    }
}
