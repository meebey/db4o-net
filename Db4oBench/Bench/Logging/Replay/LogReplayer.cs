/* Copyright (C) 2004 - 2007 db4objects Inc. http://www.db4o.com */
using System;
using System.IO;
using System.Collections;
using System.Text;

using Sharpen.Util;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Foundation;

using Db4objects.Db4o.Bench.Logging.Replay.Commands;

namespace Db4objects.Db4o.Bench.Logging.Replay
{
    public class LogReplayer
    {
        private String _logFilePath;
        private IoAdapter _io;
        private ISet _commands;
        private Hashtable _counts;

        public LogReplayer(String logFilePath, IoAdapter io, ISet commands)
        {
            _logFilePath = logFilePath;
            _io = io;
            _commands = commands;
            _counts = new Hashtable();
            foreach (object com in commands)
            {
                _counts[com] = (Int64)0;
            }
        }

        public LogReplayer(String logFilePath, IoAdapter io): this(logFilePath, io, LogConstants.AllEntries())
        {
            
        }


        public void ReplayLog()
        {
            PlayCommandList(ReadCommandList());
        }

        public List4 ReadCommandList()
        {
            List4 list = null;
            StreamReader reader = new StreamReader(_logFilePath);
            String line = null;
            while ((line = reader.ReadLine()) != null)
            {
                IIoCommand ioCommand = ReadLine(line);
                if (ioCommand != null)
                {
                    list = new List4(list, ioCommand);
                }
            }
            reader.Close();
            return list;
        }

        public void PlayCommandList(List4 commandList)
        {
            while (commandList != null)
            {
                IIoCommand ioCommand = (IIoCommand)commandList._element;
                ioCommand.Replay(_io);
                commandList = commandList._next;
            }
        }


        private IIoCommand ReadLine(String line)
        {
            String commandName;
            if ((commandName = AcceptedCommandName(line)) != null)
            {
                IncrementCount(commandName);
                return CommandForLine(line);
            }
            return null;
        }

        private String AcceptedCommandName(String line)
        {
            if (line.Length == 0)
            {
                return null;
            }
            foreach (String commandName in _commands)
            {
                if (line.StartsWith(commandName))
                {
                    return commandName;
                }
            }
            return null;
        }

        private IIoCommand CommandForLine(String line)
        {
            if (line.StartsWith(LogConstants.ReadEntry))
            {
                int length = Parameter(LogConstants.ReadEntry, line);
                return new ReadCommand(length);
            }
            if (line.StartsWith(LogConstants.WriteEntry))
            {
                int length = Parameter(LogConstants.WriteEntry, line);
                return new WriteCommand(length);
            }
            if (line.StartsWith(LogConstants.SyncEntry))
            {
                return new SyncCommand();
            }
            if (line.StartsWith(LogConstants.SeekEntry))
            {
                int address = Parameter(LogConstants.ReadEntry, line);
                return new SeekCommand(address);
            }

            return null;
        }


        private int Parameter(String command, String line)
        {
            return Parameter(command.Length, line);
        }

        private int Parameter(int start, String line)
        {
            return Int32.Parse(line.Substring(start));
        }

        private void IncrementCount(String key)
        {
            long count = (Int64)_counts[key];
            _counts[key] = (Int64)(count + 1);
        }

        public Hashtable OperationCounts()
        {
            return _counts;
        }
    }
}

