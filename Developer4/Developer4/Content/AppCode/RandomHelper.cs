using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Content.AppCode
{
    public class RandomHelper
    {
        private static Random random = new Random();
        private static Random randomNumber = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomNumber(int length)
        {
            const string chars = "0123456789";
            string result = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var result2 = 0;
            try
            {
                result2 = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                result2 = RandomNumber(length);
            }

            return result2;
        }

        public static int RandomNumberNoZeros(int length)
        {
            const string chars = "123456789";
            string result = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var result2 = 0;
            try
            {
                result2 = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                result2 = RandomNumber(length);
            }

            return result2;
        }

        public static string GenerateGUID()
        {
            var guid = Guid.NewGuid().ToString();
            return guid;
        }
    }
}