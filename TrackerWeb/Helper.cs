using DTO;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace TrackerWeb
{
    public static class Helper
    {

        public static string GetChanges(Aviso model1, Aviso model2)
        {
            var ret = "";
            Aviso changed = Difference<Aviso>(model1, model2);
            var props = GetPropertiesNotNull(changed);
            foreach (PropertyInfo pi in props.Where(x=>x.Name!= "usuario_modificacion")) {
                if (pi.Name != "DESCRIPCION")
                {
                //    ret += $"Cambia Descripción" + System.Environment.NewLine;
                //}
                //else
                //{
                    ret += $"{pi.Name} cambia de {pi.GetValue(model1)} a {pi.GetValue(model2)}" + System.Environment.NewLine;
                }
            }
            return ret;
        }

        public static TModel Difference<TModel>(this TModel model1, TModel model2) where TModel : new()
        {
            static bool AreEqual(object obj1, object obj2) =>
                (obj1 is null && obj2 is null) ||
                obj1.Equals(obj2);

            Type type = typeof(TModel);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            TModel result = properties.Aggregate(new TModel(), (seed, property) =>
            {
                var value1 = property.GetValue(model1);
                var value2 = property.GetValue(model2);

                if (!AreEqual(value1, value2))
                {
                    property.SetValue(seed, value1);
                }

                return seed;
            });

            return result;
        }

        public static List<PropertyInfo> GetPropertiesNotNull<TModel>(TModel model)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(model);
                    if (!string.IsNullOrEmpty(value))
                    {
                        ret.Add(pi);
                    }
                }
            }
            return ret;
        }
    }
}
