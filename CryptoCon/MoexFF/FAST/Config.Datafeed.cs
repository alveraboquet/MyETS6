using System;
using System.Text;
using System.Xml;

namespace Moex.FAST
{
    public partial class Config : XMLResource
    //internal partial class Config : XMLResource
    {
        public class UDPConnection
        {
            public UDPConnection(string name)
            {
                this.Name = name;
                this.AConnector = null;
                this.BConnector = null;
            }

            public string Name { get; private set; }

            public UDPConnector? AConnector { get; private set; }
            public UDPConnector? BConnector { get; private set; }

            public void AddConnector(XmlNode connectorNode, string feed)
            {
                if (feed == "a")
                {
                    this.AConnector = new UDPConnector(connectorNode);
                }
                if (feed == "b")
                {
                    this.BConnector = new UDPConnector(connectorNode);
                }
            }
        }

        public sealed class Datafeed
        {
            public Datafeed(string name)
            {
                this.Name = name;
                this.SnapshotConnection = null;
                this.IncrementalConnection = null;
                this.HistoricalReplay = null;
            }

            public string Name { get; private set; }

            public UDPConnection? SnapshotConnection { get; private set; }
            public UDPConnection? IncrementalConnection { get; private set; }

            public TCPConnector? HistoricalReplay { get; private set; }

            public void AddASTSConnection(XmlNode connectionNode, string type)
            {
                string protocol = (XMLResource.GetChild(connectionNode, "protocol")?.InnerText ?? "").ToLower();
                if (protocol == "tcp/ip")
                {
                    this.HistoricalReplay = new TCPConnector(connectionNode);
                }
                else if (protocol == "udp/ip")
                {
                    UDPConnection? connection = null;
                    if (type.Contains("snapshot"))
                    {
                        connection = this.SnapshotConnection = this.SnapshotConnection ?? new UDPConnection("Snapshot");
                    }
                    else if (type.Contains("replay"))
                    {
                        connection = this.SnapshotConnection = this.SnapshotConnection ?? new UDPConnection("Replay");
                    }
                    else if (type.Contains("incremental"))
                    {
                        connection = this.IncrementalConnection = this.IncrementalConnection ?? new UDPConnection("Incremental");
                    }
                    else if (type.Contains("status"))
                    {
                        connection = this.IncrementalConnection = this.IncrementalConnection ?? new UDPConnection("Status");
                    }
                    if (connection != null)
                    {
                        foreach (XmlNode child in connectionNode.ChildNodes)
                        {
                            string name = child.Name.ToLower();
                            if (name == "feed")
                            {
                                string value = (XMLResource.GetAttribute(child, "id") ?? "").ToLower();
                                if ((value == "a") || (value == "b"))
                                {
                                    connection.AddConnector(child, value);
                                }
                            }
                        }
                    }
                }
            }

            public void AddSPECTRAConnection(XmlNode connectionNode)
            {
                string type = (XMLResource.GetChild(connectionNode, "type")?.InnerText ?? "").ToLower();
                string protocol = (XMLResource.GetChild(connectionNode, "protocol")?.InnerText ?? "").ToLower();
                if (type.Contains("historical") && (protocol == "tcp/ip"))
                {
                    this.HistoricalReplay = new TCPConnector(connectionNode);
                }
                else if (protocol == "udp/ip")
                {
                    string feed = (XMLResource.GetChild(connectionNode, "feed")?.InnerText ?? "").ToLower();
                    if (type.Contains("snapshot"))
                    {
                        this.SnapshotConnection = this.SnapshotConnection ?? new UDPConnection("Snapshot");
                        this.SnapshotConnection.AddConnector(connectionNode, feed);
                    }
                    else if (type.Contains("replay"))
                    {
                        this.SnapshotConnection = this.SnapshotConnection ?? new UDPConnection("Replay");
                        this.SnapshotConnection.AddConnector(connectionNode, feed);
                    }
                    else if (type.Contains("incremental"))
                    {
                        this.IncrementalConnection = this.IncrementalConnection ?? new UDPConnection("Incremental");
                        this.IncrementalConnection.AddConnector(connectionNode, feed);
                    }
                }
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"{this.Name}:");
                if (this.SnapshotConnection != null)
                {
                    stringBuilder.Append($"\n\t{this.SnapshotConnection.Name} - ");
                    if (this.SnapshotConnection.AConnector != null)
                    {
                        stringBuilder.Append($"A: {this.SnapshotConnection.AConnector.ToString()}, ");
                    }
                    if (this.SnapshotConnection.BConnector != null)
                    {
                        stringBuilder.Append($"B: {this.SnapshotConnection.BConnector.ToString()}");
                    }
                }
                if (this.IncrementalConnection != null)
                {
                    stringBuilder.Append($"\n\t{this.IncrementalConnection.Name} - ");
                    if (this.IncrementalConnection.AConnector != null)
                    {
                        stringBuilder.Append($"A: {this.IncrementalConnection.AConnector.ToString()}, ");
                    }
                    if (this.IncrementalConnection.BConnector != null)
                    {
                        stringBuilder.Append($"B: {this.IncrementalConnection.BConnector.ToString()}");
                    }
                }
                if (this.HistoricalReplay != null)
                {
                    stringBuilder.Append($"\n\tHistorical - {this.HistoricalReplay.ToString()}");
                }
                return stringBuilder.ToString();
            }
        }
    }
}
