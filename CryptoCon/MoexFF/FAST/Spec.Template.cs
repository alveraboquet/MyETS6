using System;
using System.Collections.Generic;
using System.Xml;

namespace Moex.FAST
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        public class TemplateItem
        {
            protected TemplateItem(XmlNode itemNode)
            {
                this.Name = XMLResource.GetAttribute(itemNode, "name") ?? "";
                string? presence = XMLResource.GetAttribute(itemNode, "presence");
                this.IsOptional = presence == "optional";
                this.IsMandatory = presence == "mandatory";
            }

            public string Name { get; private set; }

            // Presence.
            public bool IsMandatory { get; private set; }
            public bool IsOptional { get; private set; }
        }

        public sealed class Template : Sequence
        {
            public Template(XmlNode templateNode, List<string> errors) : base(templateNode, errors)
            {
                this.ID = 0;
                uint id = 0;
                if (UInt32.TryParse(XMLResource.GetAttribute(templateNode, "id"), out id))
                {
                    this.ID = id;
                }
            }

            public uint ID { get; private set; }

            public override string ToString() => $"Template \"{this.Name}\"({this.ID}), {(this.Items == null ? 0 : this.Items.Count)} items.";
        }
    }
}
