using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace ProviderContract.Services.JsonConverters
{
    /// <summary>
    /// Конвертирует строку в элемент перечисления.
    /// Если элемент не найден, то возвращает элемент с индексом 0.
    /// </summary>
    public class EnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // По умолчанию элемент перечисления с нулевым индексом
            int index = 0;

            // Получаем допустимые значения
            var values = objectType.GetEnumValues();

            for (int i = 1; i < values.Length; i++)
            {
                //Получаем атрибуты элемента перечисления
                var attributes = objectType.GetMember(values.GetValue(i).ToString())[0]
                    .GetCustomAttributes(typeof(EnumMemberAttribute), false);

                // Если атрибутов нет, пропускаем элемент
                if (attributes.Length == 0)
                {
                    continue;
                }

                // Если значение атрибута совпадает с полученным значением, то возвращаем индекс элемента
                var description = ((EnumMemberAttribute)attributes[0]).Value;
                if (description.Equals(reader.Value.ToString()))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
