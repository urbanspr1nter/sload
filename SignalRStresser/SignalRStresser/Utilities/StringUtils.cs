using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRStresser.Utilities
{
    class StringUtils
    {
        public static string CollectionToString<T>(IEnumerable<T> collection, string delimiter)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("");

            foreach(var element in collection)
            {
                sb.Append(element.ToString());
                sb.Append(delimiter);
            }
           
            return sb.ToString();
        }

    }
}
