using System.Xml;
using System;
using System.Xml.Linq;

namespace Moex.FAST
{
    public partial class Config : XMLResource
    //internal partial class Config : XMLResource
    {
        public abstract class Connector
        {
            protected Connector(XmlNode connectorNode)
            {
                this.IP = XMLResource.GetChild(connectorNode, "ip")?.InnerText ?? "";
                this.Port = 0;
                int port = 0;
                if (Int32.TryParse(XMLResource.GetChild(connectorNode, "port")?.InnerText, out port))
                {
                    this.Port = port;
                }
            }

            public string IP { get; private set; }
            public int Port { get; private set; }
        }

        public sealed class UDPConnector : Connector
        {
            public UDPConnector(XmlNode connectorNode) : base(connectorNode)
            {
                this.SrcIP = XMLResource.GetChild(connectorNode, "src-ip")?.InnerText ?? "";
            }

            public string SrcIP { get; private set; }

            public override string ToString() => $"UDP {this.SrcIP}/{this.IP}:{this.Port}";
        }

        public sealed class TCPConnector : Connector
        {
            public TCPConnector(XmlNode connectorNode) : base(connectorNode)
            {
                this.IP2 = "";
                if (connectorNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in connectorNode.ChildNodes)
                    {
                        string nodeName = node.Name.ToLower();
                        if ((nodeName == "ip") && (this.IP != node.InnerText))
                        {
                            this.IP2 = node.InnerText;
                        }
                    }
                }
            }

            public string IP2 { get; private set; }

            public override string ToString() => $"TCP {this.IP}/{this.IP2}:{this.Port}";
        }
    }
}
