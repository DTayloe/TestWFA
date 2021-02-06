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
     }
}
