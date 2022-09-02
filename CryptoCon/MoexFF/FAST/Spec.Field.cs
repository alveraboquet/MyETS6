using System;
using System.Text;
using System.Xml;

namespace Moex.FAST
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        public enum FieldType
        {
            UNKNOWN     = 0,
            STRING      = 1,
            INT32       = 2,
            INT64       = 3,
            UINT32      = 4,
            UINT64      = 5,
            DECIMAL     = 6,
            LENGTH      = 7,
            BYTE_VECTOR = 8
        }

        public static FieldType FieldTypeFromString(string name)
        {
            switch (name)
            {
                case "string":
                    return FieldType.STRING;
                case "int32":
                    return FieldType.INT32;
                case "int64":
                    return FieldType.INT64;
                case "uint32":
                    return FieldType.UINT32;
                case "uint64":
                    return FieldType.UINT64;
                case "decimal":
                    return FieldType.DECIMAL;
                case "length":
                    return FieldType.LENGTH;
                case "bytevector":
                    return FieldType.BYTE_VECTOR;
                default:
                    return FieldType.UNKNOWN;
            }
        }

        public sealed class Field : TemplateItem
        {
            public Field(FieldType fieldType, XmlNode fieldNode) : base(fieldNode)
            {
                this.Type = fieldType;
                this.ID = 0;
                int id = 0;
                if (Int32.TryParse(XMLResource.GetAttribute(fieldNode, "id"), out id))
                {
                    this.ID = id;
                }
                this.IsUnicode = XMLResource.GetAttribute(fieldNode, "charset") == "unicode";
                this.Constant = "";
                this.IsConstant = false;
                this.Default = "";
                this.IsDefault = false;
                this.IsCopy = false;
                this.Increment = 1;
                this.IsIncrement = false;
                this.Delta = "";
                this.IsDelta = false;
                if (fieldNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in fieldNode.ChildNodes)
                    {
                        string nodeName = node.Name.ToLower();
                        if (nodeName == "constant")
                        {
                            this.Constant = XMLResource.GetAttribute(node, "value") ?? "";
                            this.IsConstant = true;
                        }
                        else if (nodeName == "default")
                        {
                            this.Default = XMLResource.GetAttribute(node, "value") ?? "";
                            this.IsDefault = true;
                        }
                        else if (nodeName == "copy")
                        {
                            this.IsCopy = true;
                        }
                        else if (nodeName == "increment")
                        {
                            int increment = 0;
                            if (Int32.TryParse(XMLResource.GetAttribute(node, "value") ?? "", out increment) == true)
                            {
                                this.Increment = increment;
                            }
                            this.IsIncrement = true;
                        }
                        else if (nodeName == "delta")
                        {
                            this.Delta = XMLResource.GetAttribute(node, "value") ?? "";
                            this.IsDelta = true;
                        }
                    }
                }
            }

            public FieldType Type { get; private set; }

            public int ID { get; private set; }

            public bool IsUnicode { get; private set; }

            public string Constant { get; private set; }
            public bool IsConstant { get; private set; }

            public string Default { get; private set; }
            public bool IsDefault { get; private set; }

            public bool IsCopy { get; private set; }

            public int Increment { get; private set; }
            public bool IsIncrement { get; private set; }

            public string Delta { get; private set; }
            public bool IsDelta { get; private set; }


            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"Field \"{this.Name}\"({this.ID}), type \"{this.Type}\", ");
                stringBuilder.Append($"presence {(this.IsMandatory ? "mandatory" : "optional")}");
                if (this.IsUnicode)
                {
                    stringBuilder.Append(", unicode");
                }
                if (this.IsConstant)
                {
                    stringBuilder.Append($", constant \"{this.Constant}\"");
                }
                if (this.IsDefault)
                {
                    stringBuilder.Append($", default \"{this.Default}\"");
                }
                if (this.IsCopy)
                {
                    stringBuilder.Append(", copy");
                }
                if (this.IsIncrement)
                {
                    stringBuilder.Append($", increment \"{this.Increment}\"");
                }
                if (this.IsDelta)
                {
                    stringBuilder.Append($", delta \"{this.Delta}\"");
                }
                stringBuilder.Append(".");
                return stringBuilder.ToString();
            }
        }
    }
}
