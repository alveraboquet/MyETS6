using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProviderContract
{
    public static class Utils
    {
        private static readonly DateTime unixTime;

        static Utils()
        {
            unixTime = new DateTime(1970, 1, 1);
        }
        /// <summary>
        /// Преобразует date в Timestamp секунды
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Timestamp в секундах от 01.01.1970</returns>
        public static long DateTimeToLongSeconds(DateTime date)
        {
            long seconds = (long)(date - unixTime).TotalSeconds;
            return seconds;
        }
        /// <summary>
        /// Преобразует date в Timestamp миллисекунды
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Timestamp в миллисекундах от 01.01.1970</returns>
        public static long DateTimeToLongMilliseconds(DateTime date)
        {
            long milliseconds = (long)(date - unixTime).TotalMilliseconds;
            return milliseconds;
        }

        /// <summary>
        /// Текущий Timestamp в миллисекундах
        /// </summary>
        /// <returns>Timestamp в миллисекундах от 01.01.1970</returns>
        public static long GetCurrentTimestampMilliseconds()
        {
            return DateTimeToLongMilliseconds(DateTime.UtcNow);
        }

        /// <summary>
        /// Текущий Timestamp в секундах
        /// </summary>
        /// <returns>Timestamp в секундах от 01.01.1970</returns>
        public static long GetCurrentTimestampSeconds()
        {
            return DateTimeToLongSeconds(DateTime.UtcNow);
        }

        /// <summary>
        /// Выполняет шифрование inputBytes с ключом keyByte в SHA256 
        /// </summary>
        /// <param name="keyByte">Ключ шифрования</param>
        /// <param name="inputBytes">Данные для шифрования</param>
        /// <returns>Массив зашифрованных данных</returns>
        public static byte[] CreateHMACSHA256(byte[] keyByte, byte[] inputBytes)
        {
            using (var hmac = new HMACSHA256(keyByte))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return hashValue;
            }
        }

        /// <summary>
        /// Выполняет преобразование массива байт в HEX-строку
        /// </summary>
        /// <param name="ba">Исходный массива байт</param>
        /// <returns>HEX-строка</returns>
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Разбивает коллекцию на коллекцию списков по partSize элементов каждый.
        /// </summary>
        /// <typeparam name="T">Тип данных элементов в списке</typeparam>
        /// <param name="list">Исходный список</param>
        /// <param name="partSize">Количество элементов в групп</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(this IEnumerable<T> list, int partSize)
        {
            var pos = 0;
            while (list.Skip(pos).Any())
            {
                yield return list.Skip(pos).Take(partSize).ToList();
                pos += partSize;
            }
        }
    }
}
