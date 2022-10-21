using DTO;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace TrackerWeb
{
    public static class Helper
    {
        public static string GetChanges<TModel>(TModel antiguo, TModel nuevo, IEnumerable<string> props = null)
        {
            if (props == null)
            {
                props = new List<string>();
            }

            var ret = "";
            Type type = typeof(TModel);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in properties.Where(x => !props.Contains(x.Name)))
            {
                if (prop.PropertyType.IsGenericType || prop.PropertyType == typeof(string))
                {
                        var i = prop.GetValue(antiguo);
                        var f = prop.GetValue(nuevo);
                        if (!object.Equals(i, f))
                        {
                            ret += $"{prop.Name} cambia de {i} a {f}" + System.Environment.NewLine;
                        }
                }
            }
            return ret;
        }
    }
}
