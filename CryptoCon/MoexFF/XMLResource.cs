using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System;

namespace Moex
{
    public abstract class XMLResource
    //internal abstract class XMLResource
    {
        protected XMLResource(string resourceName)
        {
            this.resourceName = resourceName;

            this.Errors = new List<string>();
        }

        public void Load(bool writeInfo)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resources = assembly.GetManifestResourceNames();
            string resourceFullName = "";
            foreach (var resource in resources)
            {
                if (resource.Contains(resourceName))
                {
                    resourceFullName = resource;
                    break;
                }
            }

            if (resourceFullName == "")
            {
                this.Errors.Add($"Resource \"{resourceName}\" not found.");
                return;
            }

            var stream = assembly.GetManifestResourceStream(resourceFullName);
            if (stream == null)
            {
                this.Errors.Add($"Can't load resource \"{resourceFullName}\".");
                return;
            }

            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(stream);
                this.GetData(document);
                if (writeInfo)
                {
                    this.WriteInfo();
                }
            }
            catch (Exception exception)
            {
                this.Errors.Add($"Exception \"{exception.Message}\".");
            }
            if (writeInfo)
            {
                this.WriteErrors();
            }
        }

        private string resourceName { get; set; }

        protected abstract void GetData(XmlDocument document);

        protected List<string> Errors { get; private set; }

        protected abstract void WriteInfo();

        public void WriteErrors()
        {
            Console.WriteLine($"{this.Errors.Count} errors.");
            for (int index = 0; index < this.Errors.Count; index++)
            {
                Console.WriteLine($"{index + 1}) {this.Errors[index]}");
            }
        }

        public static XmlNode? GetChild(XmlNode? node, string childName)
        {
            if (node != null)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.ToLower() == childName)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        public static string? GetAttribute(XmlNode? node, string attributeName)
        {
            if ((node != null) && (node.Attributes != null))
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name.ToLower() == attributeName)
                    {
                        return attribute.Value;
                    }
                }
            }
            return null;
        }

        public static string? GetAttribute(XmlNode? node, string childName, string attributeName)
        {
            XmlNode? child = XMLResource.GetChild(node, childName);
            if (child != null)
            {
                return XMLResource.GetAttribute(child, attributeName);
            }
            return null;
        }
    }
}
