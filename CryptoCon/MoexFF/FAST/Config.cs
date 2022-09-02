using System;
using System.Collections.Generic;
using System.Xml;

namespace Moex.FAST
{
    public partial class Config : XMLResource
    //internal partial class Config : XMLResource
    {
        public Config(string resourceName) : base(resourceName)
        {
            this.datafeeds = new Dictionary<string, Datafeed>(0);
        }

        protected override void GetData(XmlDocument document)
        {
            if ((document.ChildNodes != null) && (document.ChildNodes.Count == 1) &&
                (document.ChildNodes.Item(0)?.Name.ToLower() == "configuration"))
            {
                XmlNode? root = document.ChildNodes.Item(0);
                if (root != null)
                {
                    foreach (XmlNode child in root.ChildNodes)
                    {
                        string childName = child.Name.ToLower();
                        bool isASTS = childName == "channel";
                        bool isSPECTRA = childName == "marketdatagroup";
                        if (!isASTS && !isSPECTRA)
                        {
                            continue;
                        }
                        string datafeedID = "";
                        if (isASTS)
                        {
                            datafeedID = XMLResource.GetAttribute(child, "id") ?? "UNKNOWN";
                        }
                        if (isSPECTRA)
                        {
                            datafeedID = XMLResource.GetAttribute(child, "feedtype") ?? "UNKNOWN";
                            if (this.datafeeds.ContainsKey(datafeedID) == false)
                            {
                                this.datafeeds.Add(datafeedID, new Datafeed(datafeedID));
                            }
                        }
                        if ((child.ChildNodes != null) && (child.ChildNodes.Count == 1) &&
                            (child.ChildNodes.Item(0)?.Name.ToLower() == "connections"))
                        {
                            XmlNode? connectionsNode = child.ChildNodes.Item(0);
                            if (connectionsNode != null)
                            {
                                foreach (XmlNode connectionNode in connectionsNode.ChildNodes)
                                {
                                    if (isASTS)
                                    {
                                        string type = (XMLResource.GetAttribute(connectionNode, "type", "feed-type") ?? "").ToLower();
                                        string fullDatafeedID = datafeedID;
                                        if (type.Contains("historical"))
                                        {
                                            fullDatafeedID = datafeedID + "-historical";
                                        }
                                        else if (type.Contains(' '))
                                        {
                                            fullDatafeedID = datafeedID + "-" + type.Substring(0, type.IndexOf(' '));
                                        }
                                        if (this.datafeeds.ContainsKey(fullDatafeedID) == false)
                                        {
                                            this.datafeeds.Add(fullDatafeedID, new Datafeed(fullDatafeedID));
                                        }
                                        this.datafeeds[fullDatafeedID].AddASTSConnection(connectionNode, type);
                                    }
                                    if (isSPECTRA)
                                    {
                                        this.datafeeds[datafeedID].AddSPECTRAConnection(connectionNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void WriteInfo()
        {
            Console.WriteLine($"{this.datafeeds.Count} datafeeds.");
            int count = 0;
            foreach (var datafeed in this.datafeeds)
            {
                count++;
                Console.WriteLine($"{count}) {datafeed.Value.ToString()}");
            }
        }

        public Datafeed? GetDatafeed(string name)
        {
            if (this.datafeeds.ContainsKey(name))
            {
                return this.datafeeds[name];
            }
            return null;
        }
        private Dictionary<string, Datafeed> datafeeds;

        public IReadOnlyDictionary<string, Datafeed> Datafeeds => this.datafeeds;
    }
}
