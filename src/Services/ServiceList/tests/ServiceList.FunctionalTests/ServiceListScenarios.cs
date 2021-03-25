using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ServiceList.FunctionalTests
{
    public class ServiceListScenarios
       : ServiceListScenariosBase
    {
        [Fact]
        public async Task Get_get_all_servicelistitems_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.Items());

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_servicelistitem_by_id_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.ItemById(1));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_servicelistitem_by_id_and_response_bad_request_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.ItemById(int.MinValue));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_get_servicelistitem_by_id_and_response_not_found_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.ItemById(int.MaxValue));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_get_servicelistitem_by_name_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.ItemByName(".NET"));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_paginated_servicelistitem_by_name_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                const bool paginated = true;
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.ItemByName(".NET", paginated));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_get_paginated_servicelist_items_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                const bool paginated = true;
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.Items(paginated));

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
