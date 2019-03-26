using System;

namespace Common.CSharp
{
    public static class CommonUtils
    {
        public static bool ContainType(object[] arr, Type type)
        {
            foreach (var o in arr)
            {
                if (o.GetType() == type)
                    return true;
            }

            return false;
        }

        public static bool IsTypeEqual(Type type1, Type type2)
        {
            if (type1 == type2)
                return true;

            if (type1 == null || type2 == null)
                return false;

            return type2.IsAssignableFrom(type1);
        }
    }
}