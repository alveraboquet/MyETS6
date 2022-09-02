using System.Collections.Generic;
using System.Xml;

namespace Moex.FIX
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        //private class Component : Group
        //{
        /*
        public Component(XmlNode groupNode, List<string> errors)
        {
            this.Name = XMLResource.GetAttribute(groupNode, "name");
            this.fields = null;
            if (groupNode.ChildNodes.Count > 0)
            {
                this.fields = new Dictionary<string, string>(groupNode.ChildNodes.Count);
                foreach (XmlNode fieldNode in groupNode.ChildNodes)
                {
                    if (fieldNode.Name.ToLower() == "field")
                    {
                        string fieldName = XMLResource.GetAttribute(fieldNode, "field");
                        string fieldRequired = XMLResource.GetAttribute(fieldNode, "required");
                        if (fieldRequired == "")
                        {
                            fieldRequired = "N";
                        }
                        if ((fieldName != "") && !this.fields.ContainsKey(fieldName))
                        {
                            this.fields[fieldName] = fieldRequired;
                        }
                        else
                        {
                            errors.Add($"Group \"{this.Name}\": field \"{fieldName}\" already added.");
                        }
                    }
                }
            }
        }
        */

        //private Dictionary<string, Group>? groups { get; set; }

        //public IReadOnlyList<string, Group>? Groups => this.groups;
        //}
    }
}
