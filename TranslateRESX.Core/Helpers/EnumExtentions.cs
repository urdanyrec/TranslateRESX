using System;
using System.ComponentModel;
using System.Reflection;

namespace TranslateRESX.Core.Helpers
{
    public static class EnumExtentions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
                DescriptionAttribute[] array = (DescriptionAttribute[])customAttributes;
                return (array.Length > 0) ? array[0].Description : value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static T GetEnumValueByDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"No enum value with description '{description}' found in {type.Name}");
        }
    }
}
