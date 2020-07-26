using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace REST_test
{
    public class GeneralHttpRequest
    {
        /// <summary> URI получения животных по статусу </summary>
        public string PetStatusUri { get; set; } = "https://petstore.swagger.io/v2/pet/findByStatus?status=";
        /// <summary> URI добавления животного </summary>
        public string PetCreateUri { get; } = "https://petstore.swagger.io/v2/pet";
        /// <summary>  Получение животного по ID </summary>
        public string PetGetUri { get; set; } = $"https://petstore.swagger.io/v2/pet/";
        /// <summary> Создание заказа на животное </summary>
        public string PetOrderCreateUri { get; } = "https://petstore.swagger.io/v2/store/order";
        /// <summary> Получение заказа на животное по ID заказа </summary>
        public string PetGetOrderUri { get; set; } = $"https://petstore.swagger.io/v2/store/order/";
        /// <summary> Распарсенный JSON на заказ животного </summary>
        public OrderPet JsonOrderPet = new OrderPet
        {
            Id = 0,
            PetId = 5,
            Quantity = 1,
            Shipdate = "2020-04-02T00:00:24.272+0000",
            Status = "placed",
            Complete = true
        };
        /// <summary> Распарсенный JSON получения информации о животном </summary>
        public PetInfo JsonPetInfo = new PetInfo
        {
            Id = 0,
            Name = "bobik",
            PhotoUrls = new List<string> { "string" },
            Category = new PetInfo.CategoryClass
            {
                Id = 0,
                Name = "string"
            },
            Tags = new List<PetInfo.Tag>
            {
                new PetInfo.Tag
                {
                    Id = 0,
                    Name = "string"
                }
            }
        };


        public class OrderPet
        {
            public long Id { get; set; }
            public long PetId { get; set; }
            public int Quantity { get; set; }
            public string Shipdate { get; set; }
            public string Status { get; set; }
            public bool Complete { get; set; }
        }
        public class PetInfo
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public List<string> PhotoUrls { get; set; }
            public CategoryClass Category { get; set; }
            public List<Tag> Tags { get; set; }

            public class CategoryClass
            {
                public long Id { get; set; }
                public string Name { get; set; }
            }

            public class Tag
            {
                public long Id { get; set; }
                public string Name { get; set; }
            }
        }
        /// <summary> Метод работы с веб-запросами в общем виде </summary>
        /// <param name="method">Метод запроса</param>
        /// <param name="url">URI адрес запроса</param>
        /// <param name="json">Опциональный json для POST запроса</param>
        /// <returns> Сообщение с ответом </returns>
        public async Task<HttpResponseMessage> Request(string method, string url, string json = "")
        {
            using (var client = new HttpClient())
            {
                var x = new StringContent(json, Encoding.UTF8, "application/json");
                var response = method switch
                {
                    ("get") => await client.GetAsync(url),
                    ("post") => await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")),
                    ("put") => await client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json")),
                    ("delete") => await client.DeleteAsync(url),
                    _ => throw new Exception("Указан неверный тип запроса"),
                };
                return response;
            }

        }
    }
}
