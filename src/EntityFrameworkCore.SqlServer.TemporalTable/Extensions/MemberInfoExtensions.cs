using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Reflection
{
    internal static class MemberInfoExtensions
    {
        public static string GetSimpleMemberName(this MemberInfo member)
        {
            var name = member.Name;
            var index = name.LastIndexOf('.');
            return index >= 0 ? name.Substring(index + 1) : name;
        }
    }
}
