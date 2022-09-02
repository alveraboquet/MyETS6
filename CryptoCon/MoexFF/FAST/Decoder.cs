using System;
using System.Collections.Generic;
using System.Text;

namespace Moex.FAST
{
    internal partial class Decoder
    {
        public Decoder(Spec spec)
        {
            this.spec = spec;
        }

        private const byte FAST_STOP_BIT =  0b1000_0000;
        private const byte FAST_DATA_BITS = 0b0111_1111;
        private const byte FAST_SIGN_BIT =  0b0100_0000;
        private const int FAST_DATA_BITS_COUNT = 7;
        private const byte FAST_PRESENCE_MASK = 0b0100_0000;

        private Spec spec { get; set; }

        private class PresenceMap
        {
            public PresenceMap(byte[] buffer, ref int bufferIndex)
            {
                this.buffer = buffer;
                this.length = 0;
                for (bufferIndex = 4; bufferIndex < this.buffer.Length; )
                {
                    this.length++;
                    bufferIndex++;
                    if ((this.buffer[3 + this.length] & Decoder.FAST_STOP_BIT) != 0)
                    {
                        break;
                    }
                }
                this.currentIndex = 0;
                this.currentMask = Decoder.FAST_PRESENCE_MASK;
                this.savedValues = new Dictionary<string, string>();
                Console.Write($"Presence map ({this.length}):");
                for (int index = 0; index < (4 + this.length); index++)
                {
                    Console.Write($"   {this.buffer[index]:X}");
                }
                Console.WriteLine();
            }

            private byte[] buffer { get; set; }

            private int length { get; set; }

            private int currentIndex { get; set; }
            private byte currentMask { get; set; }

            public bool CheckCurrentField()
            {
                if (this.currentIndex == this.length)
                {
                    return false;
                }
                bool result = ((this.buffer[4 + this.currentIndex] & this.currentMask) != 0);
                this.currentMask >>= 1;
                if (this.currentMask == 0x00)
                {
                    this.currentIndex++;
                    this.currentMask = Decoder.FAST_PRESENCE_MASK;
                }
                return result;
            }

            public void SaveValue(string name, string value)
            {
                if (this.savedValues.ContainsKey(name))
                {
                    this.savedValues[name] = value;
                }
                else
                {
                    this.savedValues.Add(name, value);
                }
            }
            public string? GetValue(string name)
            {
                if (this.savedValues.ContainsKey(name))
                {
                    return this.savedValues[name];
                }
                else
                {
                    return null;
                }
            }
            private Dictionary<string, string> savedValues;
        }

        public void Process(byte[] buffer)
        {
            int bufferIndex = 0;
            PresenceMap presenceMap = new PresenceMap(buffer, ref bufferIndex);
            uint templateID = Decoder.GetUInt32(null, buffer, ref bufferIndex);
            Console.WriteLine($"Template ID {templateID}...");
            var template = this.spec.GetTemplate(templateID);
            if (template != null)
            {
                Console.WriteLine($"Template \"{template.Name}\".");
                for (int index = 0; index < template.Items?.Count; index++)
                {
                    this.ProcessItem(template.Items[index], buffer, ref bufferIndex, presenceMap);
                }
            }
            else
            {
                Console.WriteLine("UNKNOWN TEMPLATE.");
            }
        }

        private void ProcessItem(Spec.TemplateItem item, byte[] buffer, ref int bufferIndex, PresenceMap presenceMap)
        {
            Console.WriteLine($"Process item {item.Name}...");
            if (item is Spec.Field)
            {
                var field = (Spec.Field)item;
                if (field.IsConstant)
                {
                    string value = field.Constant;
                    Console.WriteLine($"\tConstant value \"{value}\".");
                }
                else if (field.IsDefault)
                {
                    //bool presence = presenceMap.CheckCurrentField();
                    bool presence = false;
                    Console.WriteLine($"Default presence {presence}");
                    string value = field.Default;
                    if (presence)
                    {
                        value = this.ProcessField(field, buffer, ref bufferIndex, presenceMap);
                    }
                    Console.WriteLine($"\tDefault value \"{value}\".");
                }
                else if (field.IsCopy)
                {
                    Console.WriteLine($"\tCopy NJI...");
                }
                else if (field.IsIncrement)
                {
                    bool presence = presenceMap.CheckCurrentField();
                    Console.WriteLine($"Incremental presence {presence}");
                    string value = "";
                    if (presence)
                    {
                        value = this.ProcessField(field, buffer, ref bufferIndex, presenceMap);
                        presenceMap.SaveValue(field.Name, value);
                    }
                    else
                    {
                        value = presenceMap.GetValue(field.Name) ?? "0";
                        int intValue = 0;
                        Int32.TryParse(value, out intValue);
                        intValue++;
                        value = intValue.ToString();
                        presenceMap.SaveValue(field.Name, value);
                    }
                    Console.WriteLine($"\tIncremental value \"{value}\".");
                }
                else
                {
                    this.ProcessField(field, buffer, ref bufferIndex, presenceMap);
                }
            }
            else if (item is Spec.Sequence)
            {
                var sequence = (Spec.Sequence)item;
                Console.WriteLine($"Sequence {sequence.Name}({sequence.Items?.Count} items).");
                Console.WriteLine($"Buffer index {bufferIndex}... {buffer[bufferIndex + 0]:X} {buffer[bufferIndex + 1]:X} {buffer[bufferIndex + 2]:X}");
                Console.WriteLine($"Buffer index {bufferIndex}... {buffer[bufferIndex + 3]:X} {buffer[bufferIndex + 4]:X} {buffer[bufferIndex + 5]:X}");
                Console.WriteLine($"Buffer index {bufferIndex}... {buffer[bufferIndex + 6]:X} {buffer[bufferIndex + 7]:X} {buffer[bufferIndex + 8]:X}");
                if (sequence.Items?.Count > 0)
                {
                    var length = sequence.Items[0] as Spec.Field;
                    if ((length != null) && (length.Type == Spec.FieldType.LENGTH))
                    {
                        uint items = Decoder.GetUInt32(length, buffer, ref bufferIndex);
                        Console.WriteLine($"\tSequence LENGTH {items}");
                        for (int index = 0; index < items; index++)
                        {
                            Console.WriteLine($"\tSequence item {index + 1}...");
                            for (int itemIndex = 1; itemIndex < sequence.Items.Count; itemIndex++)
                            {
                                this.ProcessItem(sequence.Items[itemIndex], buffer, ref bufferIndex, presenceMap);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"\tType \"{item.GetType()}\".");
            }
            Console.WriteLine($"Buffer index {bufferIndex}...");
            //Console.WriteLine($"Buffer index {bufferIndex}... {buffer[bufferIndex]:X} {buffer[bufferIndex + 1]:X} {buffer[bufferIndex + 2]:X}");
        }

        private string ProcessField(Spec.Field field, byte[] buffer, ref int bufferIndex, PresenceMap presenceMap)
        {
            if (field.Type == Spec.FieldType.STRING)
            {
                string result = Decoder.GetString(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet STRING \"{result}\".");
                return result;
            }
            else if (field.Type == Spec.FieldType.INT32)
            {
                int result = Decoder.GetInt32(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet INT32 {result}.");
                return result.ToString();
            }
            else if (field.Type == Spec.FieldType.INT64)
            {
                long result = Decoder.GetInt64(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet INT64 {result}.");
                return result.ToString();
            }
            else if (field.Type == Spec.FieldType.UINT32)
            {
                uint result = Decoder.GetUInt32(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet UINT32 {result}.");
                return result.ToString();
            }
            else if (field.Type == Spec.FieldType.UINT64)
            {
                ulong result = Decoder.GetUInt64(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet UINT64 {result}.");
                return result.ToString();
            }
            else if (field.Type == Spec.FieldType.DECIMAL)
            {
                decimal result = Decoder.GetDecimal(field, buffer, ref bufferIndex);
                Console.WriteLine($"\tGet DECIMAL {result}.");
                return result.ToString();
            }
            else if (field.Type == Spec.FieldType.BYTE_VECTOR)
            {
                byte[] result = Decoder.GetByteVector(field, buffer, ref bufferIndex);
                string stringResult = Encoding.UTF8.GetString(result);
                Console.WriteLine($"\tGet BYTE VECTOR {stringResult}({result.Length}).");
                return stringResult;
            }
            else
            {
                Console.WriteLine($"\tType \"{field.Type}\".");
                return "UNKNOWN FIELD";
            }
        }
    }
}
