using System;
using System.Text;

namespace Moex.FAST
{
    internal partial class Decoder
    {
        private static string GetString(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            if ((field != null) && field.IsUnicode)
            {
                return Encoding.UTF8.GetString(Decoder.GetByteVector(field, buffer, ref bufferIndex));
            }
            int startIndex = bufferIndex;
            int finishIndex = bufferIndex;
            byte data = buffer[bufferIndex++];
            while ((data & Decoder.FAST_STOP_BIT) == 0)
            {
                finishIndex++;
                data = buffer[bufferIndex++];
            }
            buffer[finishIndex] &= Decoder.FAST_DATA_BITS;
            if (buffer[finishIndex] == 0x00)
            {
                finishIndex--;
            }
            if (buffer[startIndex] == 0x00)
            {
                return "";
            }
            return Encoding.ASCII.GetString(buffer, startIndex, finishIndex + 1 - startIndex);
        }

        private static int GetInt32(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetInt32((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static int GetInt32(bool optional, byte[] buffer, ref int bufferIndex)
        {
            byte data = buffer[bufferIndex++];
            int result = data & Decoder.FAST_DATA_BITS;
            bool negative = (data & Decoder.FAST_SIGN_BIT) != 0;
            if (negative == true)
            {
                unchecked
                {
                    result |= (int)0xff_ff_ff_80;
                }
            }
            while ((data & Decoder.FAST_STOP_BIT) == 0)
            {
                result <<= Decoder.FAST_DATA_BITS_COUNT;
                data = buffer[bufferIndex++];
                result |= (int)(data & Decoder.FAST_DATA_BITS);
            }
            if (optional && !negative)
            {
                result--;
            }
            return result;
        }

        private static long GetInt64(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetInt64((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static long GetInt64(bool optional, byte[] buffer, ref int bufferIndex)
        {
            byte data = buffer[bufferIndex++];
            long result = data & Decoder.FAST_DATA_BITS;
            bool negative = (data & Decoder.FAST_SIGN_BIT) != 0;
            if (negative == true)
            {
                unchecked
                {
                    result |= (long)0xff_ff_ff_ff_ff_ff_ff_80;
                }
            }
            while ((data & Decoder.FAST_STOP_BIT) == 0)
            {
                result <<= Decoder.FAST_DATA_BITS_COUNT;
                data = buffer[bufferIndex++];
                result |= (long)(data & Decoder.FAST_DATA_BITS);
            }
            if (optional && !negative)
            {
                result--;
            }
            return result;
        }

        private static uint GetUInt32(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetUInt32((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static uint GetUInt32(bool optional, byte[] buffer, ref int bufferIndex)
        {
            byte data = buffer[bufferIndex++];
            uint result = (uint)(data & Decoder.FAST_DATA_BITS);
            while ((data & Decoder.FAST_STOP_BIT) == 0)
            {
                result <<= Decoder.FAST_DATA_BITS_COUNT;
                data = buffer[bufferIndex++];
                result |= (uint)(data & Decoder.FAST_DATA_BITS);
            }
            if (optional && (result > 0))
            {
                result--;
            }
            return result;
        }

        private static ulong GetUInt64(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetUInt64((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static ulong GetUInt64(bool optional, byte[] buffer, ref int bufferIndex)
        {
            byte data = buffer[bufferIndex++];
            ulong result = (ulong)(data & Decoder.FAST_DATA_BITS);
            while ((data & Decoder.FAST_STOP_BIT) == 0)
            {
                result <<= Decoder.FAST_DATA_BITS_COUNT;
                data = buffer[bufferIndex++];
                result |= (uint)(data & Decoder.FAST_DATA_BITS);
            }
            if (optional && (result > 0))
            {
                result--;
            }
            return result;
        }

        private static decimal GetDecimal(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetDecimal((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static decimal GetDecimal(bool optional, byte[] buffer, ref int bufferIndex)
        {
            int exponent = Decoder.GetInt32(false, buffer, ref bufferIndex);
            if (optional)
            {
                if (exponent == 0)
                {
                    return 0.0m;
                }
                exponent--;
            }
            decimal mantissa = Decoder.GetInt64(false, buffer, ref bufferIndex);
            decimal result = mantissa;
            for (int index = 0; index < Math.Abs(exponent); index++)
            {
                if (exponent > 0)
                {
                    result *= 10;
                }
                else if (exponent < 0)
                {
                    result /= 10;
                }
            }
            return result;
        }

        private static byte[] GetByteVector(Spec.Field? field, byte[] buffer, ref int bufferIndex)
        {
            return Decoder.GetByteVector((field == null) ? false : field.IsOptional, buffer, ref bufferIndex);
        }

        private static byte[] GetByteVector(bool optional, byte[] buffer, ref int bufferIndex)
        {
            uint bytes = Decoder.GetUInt32(false, buffer, ref bufferIndex);
            if (optional && (bytes > 0))
            {
                bytes--;
            }
            byte[] vector = new byte[bytes];
            for (int index = 0; index < bytes; index++)
            {
                vector[index] = buffer[bufferIndex++];
            }
            return vector;
        }
    }
}
