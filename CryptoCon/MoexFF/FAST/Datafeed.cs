using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moex.FAST
{
    public class Datafeed
    //internal class Datafeed
    {
        public Datafeed(Config.Datafeed config, Spec? spec, bool addStatistics = false,
            bool consoleLog = false, bool snapshotConsoleLog = false, bool incrementalConsoleLog = false)
        {
            this.config = config;
            this.snapshotConnectors = new List<DataConnector>();
            if (this.config.SnapshotConnection != null)
            {
                if (this.config.SnapshotConnection.AConnector != null)
                {
                    this.snapshotConnectors.Add(new DataConnector("Snapshot-A",
                        this.config.SnapshotConnection.AConnector, consoleLog : consoleLog));
                }
                if (this.config.SnapshotConnection.BConnector != null)
                {
                    this.snapshotConnectors.Add(new DataConnector("Snapshot-A",
                        this.config.SnapshotConnection.BConnector, consoleLog: consoleLog));
                }
            }
            this.incrementalConnectors = new List<DataConnector>();
            if (this.config.IncrementalConnection != null)
            {
                if (this.config.IncrementalConnection.AConnector != null)
                {
                    this.incrementalConnectors.Add(new DataConnector("Incremental-A",
                        this.config.IncrementalConnection.AConnector, consoleLog : consoleLog));
                }
                if (this.config.IncrementalConnection.BConnector != null)
                {
                    this.incrementalConnectors.Add(new DataConnector("Incremental-B",
                        this.config.IncrementalConnection.BConnector, consoleLog : consoleLog));
                }
            }
            this.decoder = null;
            if (spec != null)
            {
                this.decoder = new Decoder(spec);
            }
            this.addStatistics = addStatistics;
            this.consoleLog = consoleLog;
            this.snapshotConsoleLog = snapshotConsoleLog;
            this.incrementalConsoleLog = incrementalConsoleLog;
        }

        private Config.Datafeed config { get; set; }

        private List<DataConnector> snapshotConnectors { get; set; }
        private List<DataConnector> incrementalConnectors { get; set; }

        private struct ProcessStage
        {
            public ProcessStage(IReadOnlyList<DataConnector> dataConnectors, Decoder? decoder, bool consoleLog)
            {
                this.dataConnectors = dataConnectors;
                this.decoder = decoder;
                this.consoleLog = consoleLog;
                this.dataConnectorIndex = 0;
                this.currentMessageNumber = 0;
                this.errorCount = 0;
                this.nextMessageNumber = 0;
            }

            private IReadOnlyList<DataConnector> dataConnectors { get; set; }

            private Decoder? decoder { get; set; }

            private bool consoleLog { get; set; }

            private int dataConnectorIndex { get; set; }

            private int currentMessageNumber { get; set; }

            private int errorCount { get; set; }
            private int nextMessageNumber { get; set; }

            private void ProcessMessage(int messageNumber, bool afterHistoricalReplay, Statistics? statistics)
            {
                this.currentMessageNumber = messageNumber;
                if (this.consoleLog)
                {
                    if (afterHistoricalReplay == true)
                    {
                        Console.WriteLine($"Get message number {this.currentMessageNumber} (after historical replay).");
                    }
                    else
                    {
                        Console.WriteLine($"Get message number {this.currentMessageNumber}.");
                    }
                }
                statistics?.MessageProcessed(afterHistoricalReplay);
            }

            private void ProcessMessage(DataConnector.MessageInfo messageInfo)
            {
                if (this.decoder != null)
                {
                    Console.WriteLine($"Decode message {messageInfo.Number}({messageInfo.Data?.Length}):");
                    if (messageInfo.Data != null)
                    {
                        this.decoder.Process(messageInfo.Data);
                    }
                }
            }

            public bool Step(Statistics? statistics, int shiftDataConnectorIndex)
            {
                if (this.dataConnectorIndex == this.dataConnectors.Count)
                {
                    this.dataConnectorIndex = 0;
                }
                var message = this.dataConnectors[this.dataConnectorIndex].GetFirstMessage(
                    this.currentMessageNumber, statistics?.Connectors[this.dataConnectorIndex + shiftDataConnectorIndex]);
                bool received = message.Number != Int32.MaxValue;
                if (received)
                {
                    if ((message.Number == (this.currentMessageNumber + 1)) || // Increment message number.
                        (this.currentMessageNumber == 0) ||                    // Init volume...
                        (message.Number == 1))                                 // Init snapshot... 
                    {
                        this.ProcessMessage(message.Number, false, statistics);
                        this.ProcessMessage(message);
                        this.dataConnectors[this.dataConnectorIndex].FirstMessageProcessed(
                            statistics?.Connectors[this.dataConnectorIndex + shiftDataConnectorIndex]);
                        this.errorCount = 0;
                    }
                    else if (message.Number > (this.currentMessageNumber + 1))
                    {
                        if (this.consoleLog)
                        {
                            Console.WriteLine($"Error: message number {message.Number} (current {this.currentMessageNumber}).");
                        }
                        if (this.errorCount == 0)
                        {
                            this.nextMessageNumber = message.Number;
                        }
                        else
                        {
                            this.nextMessageNumber = Math.Min(message.Number, this.nextMessageNumber);
                        }
                        this.errorCount++;
                        if (this.errorCount == this.dataConnectors.Count)
                        {
                            int startMessageNumber = this.currentMessageNumber + 1;
                            int finishMessageNumber = this.nextMessageNumber - 1;
                            if (startMessageNumber > finishMessageNumber)
                            {
                                startMessageNumber = 1;
                            }
                            if (this.consoleLog)
                            {
                                Console.WriteLine($"Need historical replay {startMessageNumber}-{finishMessageNumber}.");
                            }
                            for (int messageNumber = startMessageNumber; messageNumber <= finishMessageNumber; messageNumber++)
                            {
                                this.ProcessMessage(messageNumber, true, statistics);
                                //this.ProcessMessage(message);
                            }
                            this.errorCount = 0;
                        }
                    }
                }
                this.dataConnectorIndex++;
                return received;
            }
        }

        public void Process(CancellationToken cancellationToken, NLog.Logger? statisticsdLogger, NLog.Logger? exceptionsLogger)
        {
            List<Task> tasks = new List<Task>();
            foreach (var dataConnector in this.snapshotConnectors)
            {
                tasks.Add(dataConnector.Process(cancellationToken, exceptionsLogger));
            }
            foreach (var dataConnector in this.incrementalConnectors)
            {
                tasks.Add(dataConnector.Process(cancellationToken, exceptionsLogger));
            }

            this.totalStatistics = null;
            this.lastMinuteStatistics = null;
            this.currentStatistics = null;
            if (this.addStatistics)
            {
                DateTime startDateTime = DateTime.Now;
                this.currentStatistics = new Statistics(startDateTime, tasks.Count);
                this.lastMinuteStatistics = new Statistics(startDateTime, tasks.Count);
                this.totalStatistics = new Statistics(startDateTime, tasks.Count);
            }

            ProcessStage snapshotProcessStage = new ProcessStage(this.snapshotConnectors, this.decoder, this.snapshotConsoleLog);
            ProcessStage incrementalProcessStage = new ProcessStage(this.incrementalConnectors, this.decoder, this.incrementalConsoleLog);
            while (!cancellationToken.IsCancellationRequested)
            {
                int snapshotProcessSteps = 0;
                while (snapshotProcessStage.Step(this.currentStatistics, 0))
                {
                    snapshotProcessSteps++;
                    if (snapshotProcessSteps == 1000)
                    {
                        break;
                    }
                }
                int incrementalProcessSteps = 0;
                while (incrementalProcessStage.Step(this.currentStatistics, this.snapshotConnectors.Count))
                {
                    incrementalProcessSteps++;
                    if (incrementalProcessSteps == 1000)
                    {
                        break;
                    }
                }
                this.ApplyStatistics(statisticsdLogger);
                if ((snapshotProcessSteps == 0) && (incrementalProcessSteps == 0))
                {
                    if (this.consoleLog)
                    {
                        Console.WriteLine("Sleep...");
                    }
                    Thread.Sleep(1);
                }
            }
        }

        private Decoder? decoder { get; set; }

        private bool addStatistics { get; set; }
        private Statistics? totalStatistics { get; set; }
        private Statistics? lastMinuteStatistics { get; set; }
        private Statistics? currentStatistics { get; set; }

        private bool consoleLog { get; set; }
        private bool snapshotConsoleLog { get; set; }
        private bool incrementalConsoleLog { get; set; }

        private void ApplyStatistics(NLog.Logger? logger)
        {
            if (this.addStatistics && (this.currentStatistics != null))
            {
                DateTime currentDateTime = DateTime.Now;
                if ((currentDateTime.Minute != this.currentStatistics.Started.Minute) &&
                    (this.lastMinuteStatistics != null) && (this.totalStatistics != null))
                {
                    this.totalStatistics.Add(this.currentStatistics);
                    this.lastMinuteStatistics.Clear(this.currentStatistics.Started);
                    this.lastMinuteStatistics.Add(this.currentStatistics);
                    this.currentStatistics.Clear(currentDateTime);
                    if (logger != null)
                    {
                        lock (logger)
                        {
                            this.totalStatistics.Log($"Total statistics for \"{this.config.Name}\"", logger);
                            this.lastMinuteStatistics.Log($"LastMinute statistics for \"{this.config.Name}\"", logger);
                        }
                    }
                }
            }
        }
    }
}
