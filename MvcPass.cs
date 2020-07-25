using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace REST_test
{
    public class MvcPass
    {
        public string Guid { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
        public string PersonPatronymic { get; set; }
        public string PassportNumber { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }


        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
        }

        public MvcPass()
        {
            PersonName = Randomizer.RandomString();
            PersonSurname = Randomizer.RandomString();
            PersonPatronymic = Randomizer.RandomString();
            PassportNumber = Randomizer.RandomPassportRus();
        }
    }
}
