using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tp.FastDataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait(new CancellationToken());
        }




        private static async Task RunAsync()
        {
            using (var client = GetHttpClient())
            {
                var feature = await ModifyResource(client, "Features", new
                {
                    Name = "Test Feature",
                    Project = new
                    {
                        Id = 169156
                    }
                });

                var featureId = feature["Id"].Value<int>();

                var story = await ModifyResource(client, "UserStories", new
                {
                    Name = "test add",
                    Project = new
                    {
                        Id = 169156
                    },
                    Feature = new
                    {
                        Id = featureId
                    }
                });

                var storyId = story["Id"].Value<int>();

                var task1 = await ModifyResource(client, "Tasks", new
                {
                    Name = "test1",
                    //EffortToDo = 4,
                    //TimeRemain = 3,
                    RoleEfforts = new []
                    {
                        new
                        {
                            Role = new
                            {
                                Id = 1
                            },
                            Effort = 6,
                            TimeRemain = 3
                        }
                    },
                    UserStory = new
                    {
                        Id = storyId
                    }
                });

                //await ModifyResource(client, "UserStories", new
                //{
                //    Id = storyId,
                //    EffortToDo = 5
                //});


                //await ModifyResource(client, "Tasks", new
                //{
                //    Id = task1["Id"].Value<int>(),
                //    Effort = 4
                //});

                //await ModifyResource(client, "RoleEfforts", new
                //{
                //    Effort = 4,
                //    Assignable = new
                //    {
                //        Id = task1["Id"].Value<int>()
                //    }
                //});

                //await UpdateResource(client, "UserStories", new
                //{
                //    Id = result["Id"].Value<int>(),
                //    TimeRemain = 5
                //});
            }
        }




        private static async Task<JObject> ModifyResource(HttpClient client, string resourceName, dynamic fields)
        {
            using (var content = new StringContent(JsonConvert.SerializeObject(fields)))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(resourceName, content);
                var str = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(str);
                }

                return JObject.Parse(str);
            }
        }

        private static HttpClient GetHttpClient()
        {
            var result = new HttpClient
            {
                BaseAddress  = new Uri("http://localhost/TP.Investigation/api/v1/"),
            };
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes("admin:admin");
            result.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


            return result;
        }
    }
}
