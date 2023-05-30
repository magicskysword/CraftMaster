using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CraftMaster;

public static class MiscUtil
{
    public static string Format(this string format, params object[] args)
    {
        return string.Format(format, args);
    }
    
    public static bool HasContent(this IList list)
    {
        return list != null && list.Count > 0;
    }
}