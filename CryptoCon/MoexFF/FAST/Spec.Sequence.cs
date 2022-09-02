using System.Collections.Generic;
using System.Xml;

namespace Moex.FAST
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        public class Sequence : TemplateItem
        {
            public Sequence(XmlNode sequenceNode, List<string> errors) : base(sequenceNode)
            {
                this.items = null;
                if (sequenceNode.ChildNodes.Count > 0)
                {
                    this.items = new List<TemplateItem>(sequenceNode.ChildNodes.Count);
                    foreach (XmlNode itemNode in sequenceNode.ChildNodes)
                    {
                        string name = itemNode.Name.ToLower();
                        FieldType fieldType = Spec.FieldTypeFromString(name);
                        if (fieldType != FieldType.UNKNOWN)
                        {
                            Field field = new Field(fieldType, itemNode);
                            if ((field.Name != "") && (field.ID != 0))
                            {
                                this.items.Add(field);
                            }
                            else
                            {
                                errors.Add($"{this.GetType().Name} \"{this.Name}\": invalid field \"{field.Name}\"({field.ID}).");
                            }
                        }
                        else if (name == "sequence")
                        {
                            Sequence sequence = new Sequence(itemNode, errors);
                            if (sequence.Name != "")
                            {
                                this.items.Add(sequence);
                            }
                            else
                            {
                                errors.Add($"{this.GetType().Name} \"{this.Name}\": invalid sequence.");
                            }
                        }
                        else if (!name.Contains("#comment"))
                        {
                            errors.Add($"{this.GetType().Name} \"{this.Name}\": invalid item \"{name}\".");
                        }
                    }
                }
            }

            private List<TemplateItem>? items { get; set; }

            public IReadOnlyList<TemplateItem>? Items => this.items;
        }
    }
}
