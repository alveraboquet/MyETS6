using NLog;
using NLog.Config;
using NLog.Targets;

namespace Moex
{
    public class NLogUtils
    {
        private NLogUtils()
        {
        }

        public static Logger GetLogger(string targetName, string logsDirectory, string prefix)
        {
            LoggingConfiguration configuration = LogManager.Configuration;
            if (configuration == null)
            {
                configuration = new LoggingConfiguration();
            }

            FileTarget target = new FileTarget();
            target.Name = targetName;
            target.FileName = logsDirectory + prefix + "${shortdate}.log";
            target.Layout = "${longdate} [${level}] (${callsite}) ${message}";
            target.ArchiveFileName = logsDirectory + prefix + "{#}.log";
            target.ArchiveNumbering = ArchiveNumberingMode.Date;
            target.ArchiveEvery = FileArchivePeriod.Day;
            target.ArchiveDateFormat = "yyyy-MM-dd";
            target.MaxArchiveFiles = 5;
            configuration.AddTarget(targetName, target);
            LoggingRule rule = new LoggingRule(targetName, LogLevel.Debug, target);
            configuration.LoggingRules.Add(rule);

            LogManager.Configuration = configuration;

            return LogManager.GetLogger(targetName);
        }

        public static Logger GetLogger(string targetName, string logsDirectory)
        {
            LoggingConfiguration configuration = LogManager.Configuration;
            if (configuration == null)
            {
                configuration = new LoggingConfiguration();
            }

            FileTarget target = new FileTarget();
            target.Name = targetName;
            target.FileName = logsDirectory + targetName + ".log";
            target.Layout = "${longdate} [${level}] (${callsite}) ${message}";
            configuration.AddTarget(targetName, target);

            LoggingRule rule = new LoggingRule(targetName, LogLevel.Debug, target);
            configuration.LoggingRules.Add(rule);

            LogManager.Configuration = configuration;

            return LogManager.GetLogger(targetName);
        }

        public static Logger GetEmptyLogger(string targetName, string logsDirectory)
        {
            LoggingConfiguration configuration = LogManager.Configuration;
            if (configuration == null)
            {
                configuration = new LoggingConfiguration();
            }

            FileTarget target = new FileTarget();
            target.Name = targetName;
            target.FileName = logsDirectory + targetName + ".log";
            target.Layout = "${message}";
            configuration.AddTarget(targetName, target);

            LoggingRule rule = new LoggingRule(targetName, LogLevel.Debug, target);
            configuration.LoggingRules.Add(rule);

            LogManager.Configuration = configuration;

            return LogManager.GetLogger(targetName);
        }
    }
}
