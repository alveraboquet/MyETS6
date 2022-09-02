using System;
using System.Collections.Generic;
using System.Xml;

namespace Moex.FAST
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        public Spec(string resourceName) : base(resourceName)
        {
            this.templatesByNumber = new Dictionary<uint, Template>(0);
        }

        protected override void GetData(XmlDocument document)
        {
            if ((document.ChildNodes != null) && (document.ChildNodes.Count == 1) &&
                (document.ChildNodes.Item(0)?.Name.ToLower() == "templates"))
            {
                XmlNode? root = document.ChildNodes.Item(0);
                if (root != null)
                {
                    this.templatesByNumber = new Dictionary<uint, Template>(root.ChildNodes.Count);
                    foreach (XmlNode child in root.ChildNodes)
                    {
                        string name = child.Name.ToLower();
                        if (name == "template")
                        {
                            Template template = new Template(child, this.Errors);
                            if ((template.Name != "") && (template.ID != 0))
                            {
                                if (!this.templatesByNumber.ContainsKey(template.ID))
                                {
                                    this.templatesByNumber.Add(template.ID, template);
                                }
                                else
                                {
                                    this.Errors.Add($"Template {template.ID} already added.");
                                }
                            }
                            else
                            {
                                this.Errors.Add($"Template \"{template.Name}\"({template.ID}) invalid.");
                            }
                        }
                    }
                }
            }
        }

        protected override void WriteInfo()
        {
            Console.WriteLine($"{this.templatesByNumber.Count} templates.");
            int count = 0;
            foreach (var template in this.templatesByNumber)
            {
                Console.WriteLine($"{++count}) {template.Value.ToString()}");
            }
        }

        public Template? GetTemplate(uint number)
        {
            if (this.templatesByNumber.ContainsKey(number))
            {
                return this.templatesByNumber[number];
            }
            return null;
        }
        private Dictionary<uint, Template> templatesByNumber;
    }
}
