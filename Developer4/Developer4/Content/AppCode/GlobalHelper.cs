using SharedAssemblies.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Developer4.Content.AppCode
{
    public class GlobalHelper
    {
        public static string StrToHex(string str)
        {
            char[] values = str.ToCharArray();
            string result = String.Empty;
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                result += String.Format("{0:X}", value);
            }

            return result;
        }

        public static string HexToStr(string HexValue)
        {
            string StrValue = "";
            while (HexValue.Length > 0)
            {
                try
                {
                    StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                    HexValue = HexValue.Substring(2, HexValue.Length - 2);
                }
                catch
                {
                    break;
                }

            }
            return StrValue;
        }
    }
}