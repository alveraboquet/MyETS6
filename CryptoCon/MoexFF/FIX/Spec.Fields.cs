using System.Collections.Generic;
using System.Xml;
using System;

namespace Moex.FIX
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        private class Field
        {
            public Field(XmlNode fieldNode)
            {
                this.Number = 0;
                int number = 0;
                if (Int32.TryParse(XMLResource.GetAttribute(fieldNode, "number"), out number))
                {
                    this.Number = number;
                }
                this.Name = XMLResource.GetAttribute(fieldNode, "name") ?? "";
                this.Type = (XMLResource.GetAttribute(fieldNode, "type") ?? "").ToUpper()   ;
                this.enumValues = null;
                if (fieldNode.ChildNodes.Count > 0)
                {
                    this.enumValues = new List<string>(fieldNode.ChildNodes.Count);
                    foreach (XmlNode valueNode in fieldNode.ChildNodes)
                    {
                        if (valueNode.Name.ToLower() == "value")
                        {
                            this.enumValues.Add(XMLResource.GetAttribute(valueNode, "enum") ?? "");
                        }
                    }
                }
            }

            public int Number { get; private set; }
            public string Name { get; private set; }
            public string Type { get; private set; }

            private List<string>? enumValues { get; set; }

            public IReadOnlyList<string>? EnumValues => this.enumValues;

            public bool CheckValue(string value)
            {
                if (this.enumValues == null)
                {
                    return true;
                }
                foreach (string enumValue in enumValues)
                {
                    if (enumValue == value)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void CreateFields(XmlNode fieldsNode)
        {
            this.fieldsByNumber = new Dictionary<int, Field>(fieldsNode.ChildNodes.Count);
            this.fieldsByName = new Dictionary<string, Field>(fieldsNode.ChildNodes.Count);
            foreach (XmlNode fieldNode in fieldsNode.ChildNodes)
            {
                Field field = new Field(fieldNode);
                if ((field.Number != 0) && (field.Name != ""))
                {
                    if (!this.fieldsByNumber.ContainsKey(field.Number) && !this.fieldsByName.ContainsKey(field.Name))
                    {
                        this.fieldsByNumber.Add(field.Number, field);
                        this.fieldsByName.Add(field.Name, field);
                    }
                    else
                    {
                        this.Errors.Add($"Field \"{field.Name}\"({field.Number}) already added.");
                    }
                }
                else
                {
                    this.Errors.Add($"Invalid field: \"{fieldNode.ToString()}\"");
                }
            }
        }
    }
}
