﻿using AmeisenBotUtilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace AmeisenBotLogger
{
    /// <summary>
    /// LogLevels
    /// </summary>
    public enum LogLevel
    {
        VERBOSE,
        DEBUG,
        WARNING,
        ERROR
    }

    /// <summary>
    /// Class to store a log entry within
    /// </summary>
    public class AmeisenLogEntry
    {
        public string functionName;
        public int id;
        public LogLevel loglevel;
        public string msg;
        public object originClass;
        public string timestamp;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append($"[{id}]");
            sb.Append($"[{timestamp}]");
            sb.Append($"[{loglevel.ToString()}]\t");
            sb.Append($"[{originClass.ToString()}:");
            sb.Append($"{functionName}] - ");
            sb.Append(msg);

            return sb.ToString();
        }
    }

    public class AmeisenLogger
    {
        public string currentUsername;

        /// <summary>
        /// Initialize/Get the instance of our singleton
        /// </summary>
        /// <returns>AmeisenLogger instance</returns>
        public static AmeisenLogger Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AmeisenLogger();
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Add an entry to the log
        /// </summary>
        /// <param name="loglevel">LogLevel of this Log-Message</param>
        /// <param name="msg">...</param>
        /// <param name="self">Class that calls it / string with class name</param>
        /// <param name="functionName">
        /// Function name that this is called by, no need to set this manually
        /// </param>
        /// <returns>The AmeisenLogEntry</returns>
        public AmeisenLogEntry Log(LogLevel loglevel, string msg, object self, [CallerMemberName]string functionName = "")
        {
            AmeisenLogEntry logEntry = new AmeisenLogEntry
            {
                loglevel = loglevel,
                id = logcount,
                timestamp = DateTime.Now.ToLongTimeString(),
                msg = msg,
                originClass = self,
                functionName = functionName
            };

            if (loglevel >= activeLogLevel)
            {
                logcount++;
                entries.Enqueue(logEntry);
            }
            return logEntry;
        }

        public void RefreshLogName()
        {
            string newLogName = $"{DateTime.Now.ToString("dd-MM-yyyy")}_{DateTime.Now.ToString("HH-mm")}-{currentUsername}.txt";

            if (File.Exists(logPath + logName))
            {
                if (File.Exists(logPath + newLogName))
                {
                    File.Delete(logPath + newLogName);
                }

                File.Move(logPath + logName, logPath + newLogName);
            }

            logName = newLogName;
        }

        /// <summary>
        /// Set the LogLevel that is going to be saved in the logs
        /// </summary>
        /// <param name="logLevel">LogLevel to save to the logfile</param>
        public void SetActiveLogLevel(LogLevel logLevel) => activeLogLevel = logLevel;

        /// <summary>
        /// Stop the logging thread, dont forget it!
        /// </summary>
        public void StopLogging() => loggingActive = false;

        private static readonly object padlock = new object();
        private static AmeisenLogger instance;
        private readonly string logPath = AppDomain.CurrentDomain.BaseDirectory + "/logs/";
        private LogLevel activeLogLevel;
        private ConcurrentQueue<AmeisenLogEntry> entries;
        private int logcount = 0;
        private bool loggingActive;
        private Thread loggingThread;
        private string logName;

        private AmeisenLogger()
        {
            activeLogLevel = LogLevel.WARNING; // Default to avoid spam
            loggingActive = true;
            entries = new ConcurrentQueue<AmeisenLogEntry>();
            loggingThread = new Thread(new ThreadStart(WorkOnQueue));
            loggingThread.Start();
            if (currentUsername == null || currentUsername == "")
            {
                currentUsername = Utils.GenerateRandonString(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }

            logName = $"{DateTime.Now.ToString("dd-MM-yyyy")}_{DateTime.Now.ToString("HH-mm")}-{currentUsername}.txt";
        }

        private void SaveLogToFile(AmeisenLogEntry entry)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            File.AppendAllText(logPath + logName, entry.ToString() + Environment.NewLine);
        }

        private void WorkOnQueue()
        {
            while (loggingActive || !entries.IsEmpty)
            {
                if (entries.TryDequeue(out AmeisenLogEntry currentEntry))
                {
                    SaveLogToFile(currentEntry);
                }
                Thread.Sleep(10);
            }
        }
    }
}