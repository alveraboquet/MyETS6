using System;
using System.Collections.Generic;
using System.Xml;

namespace Moex.FIX
{
    public partial class Spec : XMLResource
    //internal partial class Spec : XMLResource
    {
        public Spec(string resourceName) : base(resourceName)
        {
        }

        protected override void GetData(XmlDocument document)
        {
            if ((document.ChildNodes != null) && (document.ChildNodes.Count == 1) &&
                (document.ChildNodes.Item(0)?.Name.ToLower() == "fix"))
            {
                XmlNode? root = document.ChildNodes.Item(0);
                if (root != null)
                {
                    foreach (XmlNode child in root.ChildNodes)
                    {
                        string name = child.Name.ToLower();
                        Console.WriteLine($"Node {name}.");
                        if (name == "header")
                        {
                            //
                        }
                        else if (name == "trailer")
                        {
                            //
                        }
                        else if (name == "messages")
                        {
                            //
                        }
                        else if (name == "components")
                        {
                            //
                        }
                        if (name == "fields")
                        {
                            this.CreateFields(child);
                        }
                    }
                }
            }
        }

        protected override void WriteInfo()
        {
        }

        private Dictionary<int, Field>? fieldsByNumber;
        private Dictionary<string, Field>? fieldsByName;
    }
}
