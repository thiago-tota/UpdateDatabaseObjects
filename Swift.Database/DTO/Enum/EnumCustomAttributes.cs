using System.Collections.Generic;
using System.Linq;

namespace Swift.Database.DTO.Enum
{
    public static class EnumCustomAttributes
    {
        public static List<string> GetValue<T>(this T objectEnum)
        {
            List<string> result = new List<string>();

            foreach (var item in typeof(T).GetField(objectEnum.ToString()).CustomAttributes)
            {
                result.Add(item.ConstructorArguments.FirstOrDefault().Value.ToString());
            }

            return result;
        }
    }
}
