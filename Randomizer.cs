using System;
using System.Collections.Generic;
using System.Text;

namespace REST_test
{
    public static class Randomizer
    {
        public static Random rand = new Random();

        public static string RandomString(int len = 20)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var res = "";

            for (int i = 0; i < len; i++)
            {
                res += chars[rand.Next(0, chars.Length)];
            }
            return (res);
        }

        public static string RandomPassportRus()
        {
            var chars = "0123456789";
            var res = "";

            for (int i = 0; i < 10; i++)
            {
                res += chars[rand.Next(0, chars.Length)];
            }
            res = res.Insert(4, " ");

            return res;
        }
    }
}
