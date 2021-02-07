using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWFA
{
     class Utility
     {
          public static string DanPropertyList(object obj)
          {
               // adapted from: https://stackoverflow.com/a/16932448
               var props = obj.GetType().GetProperties();
               var sb = new StringBuilder();
               foreach (var p in props)
               {
                    sb.AppendLine(p.Name + ": " + p.GetValue(obj, null));
               }
               return sb.ToString();
          }

          public static string Base64Encode(string plainText)
          {
               var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
               return System.Convert.ToBase64String(plainTextBytes);
          }

          public static string Base64Decode(string base64EncodedData)
          {
               var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
               return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
          }
     }
}
