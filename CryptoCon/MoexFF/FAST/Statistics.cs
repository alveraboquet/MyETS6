using System;
using System.Collections.Generic;

namespace Moex.FAST
{
    public class Statistics
    //internal class Statistics
    {
        public Statistics(DateTime dateTime, int connectors)
        {
            this.connectors = new List<ConnectorStatistics>(connectors);
            for (int index = 0; index < connectors; index++)
            {
                this.connectors.Add(new ConnectorStatistics());
            }
            this.Clear(dateTime);
        }

        public void Add(Statistics statistics)
        {
            this.Processed += statistics.Processed;
            for (int index = 0; index < this.connectors.Count; index++)
            {
                this.connectors[index].Add(statistics.connectors[index]);
            }
            this.HistoricalReplays += statistics.HistoricalReplays;
        }

        public void Clear(DateTime dateTime)
        {
            this.Started = dateTime;
            this.Processed = 0;
            for (int index = 0; index < this.connectors.Count; index++)
            {
                this.connectors[index].Clear();
            }
            this.HistoricalReplays = 0;
        }

        public DateTime Started { get; private set; }

        public uint Processed { get; private set; }

        public class ConnectorStatistics
        {
            public ConnectorStatistics()
            {
                this.Clear();
            }

            public void Add(ConnectorStatistics connectorStatistics)
            {
                this.Messages = connectorStatistics.Messages;
                this.Received += connectorStatistics.Received;
                this.Processed += connectorStatistics.Processed;
                this.Skipped += connectorStatistics.Skipped;
                this.Losses += connectorStatistics.Losses;
            }

            public void Clear()
            {
                this.Messages = 0;
                this.Received = 0; // Нет возможности независимого подсчета полученных сообщений.
                this.Processed = 0;
                this.Skipped = 0;
                this.Losses = 0;
            }

            public void SetMessages(int messages)
            {
                this.Messages = messages;
            }
            public int Messages { get; private set; } // Очередь (нестатистический параметр для отладки).

            public uint Received { get; private set; }
            public int Processed { get; private set; }
            public uint Skipped { get; private set; }
            public uint Losses { get; private set; }

            public void MessageProcessed()
            {
                //this.Received++;
                this.Processed++;
            }

            public void MessageSkipped()
            {
                //this.Received++;
                this.Skipped++;
            }

            public void MessageLost()
            {
                //this.Received++;
                this.Losses++;
            }
        }

        private List<ConnectorStatistics> connectors { get; set; }

        public IReadOnlyList<ConnectorStatistics> Connectors => this.connectors;

        public uint HistoricalReplays { get; private set; }

        public void MessageProcessed(bool afterHistoricalReplay)
        {
            this.Processed++;
            if (afterHistoricalReplay)
            {
                this.HistoricalReplays++;
            }
        }

        public void Log(string title, NLog.Logger? logger)
        {
            if (logger != null)
            {
                logger.Info($"{title}: {this.Started.ToString()}, processed {this.Processed}, historical replays {this.HistoricalReplays}.");
                if (this.Connectors.Count >= 2)
                {
                    logger.Info(string.Format("\tSnapshot A/B: processed {0}/{1}, skipped {2}/{3}, losses {4}/{5}, messages {6}/{7}.",
                        this.Connectors[0].Processed, this.Connectors[1].Processed,
                        this.Connectors[0].Skipped, this.Connectors[1].Skipped,
                        this.Connectors[0].Losses, this.Connectors[1].Losses,
                        this.Connectors[0].Messages, this.Connectors[1].Messages));
                }
                if (this.Connectors.Count == 4)
                {
                    logger.Info(string.Format("\tIncremental A/B: processed {0}/{1}, skipped {2}/{3}, losses {4}/{5}, messages {6}/{7}.",
                        this.Connectors[2].Processed, this.Connectors[3].Processed,
                        this.Connectors[2].Skipped, this.Connectors[3].Skipped,
                        this.Connectors[2].Losses, this.Connectors[3].Losses,
                        this.Connectors[2].Messages, this.Connectors[3].Messages));
                }
            }
        }
    }
}
