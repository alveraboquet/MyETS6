using System.Collections.Generic;
using System.Xml;

namespace Moex.FIX
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        private class Group
        {
            public Group(XmlNode groupNode, List<string> errors)
            {
                this.Name = XMLResource.GetAttribute(groupNode, "name") ?? "";
                this.fields = null;
                if (groupNode.ChildNodes.Count > 0)
                {
                    this.fields = new List<string>(groupNode.ChildNodes.Count);
                    this.fieldsByName = new Dictionary<string, KeyValuePair<string, Field?>>(groupNode.ChildNodes.Count);
                    foreach (XmlNode fieldNode in groupNode.ChildNodes)
                    {
                        if (fieldNode.Name.ToLower() == "field")
                        {
                            string fieldName = XMLResource.GetAttribute(fieldNode, "field") ?? "";
                            string fieldRequired = XMLResource.GetAttribute(fieldNode, "required") ?? "N";
                            if ((fieldName != "") && !this.fieldsByName.ContainsKey(fieldName))
                            {
                                this.fields.Add(fieldName);
                                this.fieldsByName[fieldName] = new KeyValuePair<string, Field?>(fieldRequired, null);
                            }
                            else
                            {
                                errors.Add($"{this.GetType().Name} \"{this.Name}\": field \"{fieldName}\" already added.");
                            }
                        }
                    }
                }
            }

            public string Name { get; private set; }

            private List<string>? fields { get; set; }
            private Dictionary<string, KeyValuePair<string, Field?>>? fieldsByName { get; set; }

            public IReadOnlyList<string>? Fields => this.fields;
        }

        private Group CreateGroup(XmlNode groupNode, List<string> errors)
        {
            Group group = new Group(groupNode, errors);
            return group;
        }
    }
}
