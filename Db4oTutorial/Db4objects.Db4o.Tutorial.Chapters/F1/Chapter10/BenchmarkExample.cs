/* Copyright (C) 2008  db4objects Inc.  http://www.db4o.com */
using System;
using System.IO;
using Sharpen.Util;
using System.Collections.Generic;
using System.Text;

using Db4objects.Db4o.Bench.Delaying;
using Db4objects.Db4o.Bench.Logging;
using Db4objects.Db4o.Bench.Logging.Replay;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tutorial.F1.Chapter10;

namespace Db4objects.Db4o.Tutorial.F1.Chapter10
{
    public class BenchmarkExample
    {
        private static int _objectCount = 10;

        private static String _resultsFile2 = "db4o-IoBenchmark-results-10-slower.log";
        private static String _resultsFile1 = "db4o-IoBenchmark-results-10.log";

        private static String _doubleLine = "=============================================================";

        private static String _singleLine = "-------------------------------------------------------------";

        private static String _dbFileName = "IoBenchmark.db4o";

        private static Delays _delays = null;


        public static void Main(String[] args)
        {
            //RunNormal();
            RunDelayed();
        }

        public static void RunNormal()
        {
            PrintDoubleLine();
            RandomAccessFileAdapter rafAdapter = new RandomAccessFileAdapter();
            IoAdapter ioAdapter = new LoggingIoAdapter(rafAdapter, "test.log");

            System.Console.WriteLine("Running db4o IoBenchmark");
            PrintDoubleLine();
            // Run a target application and log its I/O access pattern
            RunTargetApplication(_objectCount);
            // Replay the recorded I/O operations once to prepare a database file.
            PrepareDbFile(_objectCount);
            // Replay the recorded I/O operations a second time. 
            // Operations are grouped by command type (read, write, seek, sync), 
            // and the total time executing all operations of a specific command type is measured. 
            RunBenchmark(_objectCount);
        }

        public static void RunDelayed()
        {
            PrintDoubleLine();
            System.Console.WriteLine("Running db4o IoBenchmark");
            PrintDoubleLine();
            // Write sample slow data to the test file
            PrepareDelayedFile(_resultsFile2);
            // calculate the delays to match the slower device
            ProcessResultsFiles(_resultsFile1, _resultsFile2);
            // Run a target application and log its I/O access pattern
            RunTargetApplication(_objectCount);
            // Replay the recorded I/O operations once to prepare a database file.
            PrepareDbFile(_objectCount);
            // Replay the recorded I/O operations a second time. 
            // Operations are grouped by command type (read, write, seek, sync), 
            // and the total time executing all operations of a specific command type is measured. 
            RunBenchmark(_objectCount);
        }

        public static void PrepareDelayedFile(String fileName)
        {
            TextWriter @out = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write));
            @out.Write("-------------------------------------------------------------\r\n" +
                    "db4o IoBenchmark results with 10 items\r\n" +
                    "-------------------------------------------------------------\r\n" +
                    "\r\n" +
                    "Results for READ \r\n" +
                    "> operations executed: 74\r\n" +
                    "> time elapsed: 1 ms\r\n" +
                    "> operations per millisecond: 74\r\n" +
                    "> average duration per operation: 0.01361351351351351 ms\r\n" +
                    "READ 13613 ns\r\n" +
                    "\r\n" +
                    "Results for WRITE \r\n" +
                    "> operations executed: 139\r\n" +
                    "> time elapsed: 2 ms\r\n" +
                    "> operations per millisecond: 69.5\r\n" +
                    "> average duration per operation: 0.014388489208633093 ms\r\n" +
                    "WRITE 14388 ns\r\n" +
                    "\r\n" +
                    "Results for SYNC \r\n" +
                    "> operations executed: 40\r\n" +
                    "> time elapsed: 80.4 ms\r\n" +
                    "> operations per millisecond: 0.4975124378109452\r\n" +
                    "> average duration per operation: 2.01 ms\r\n" +
                    "SYNC 2010000 ns\r\n" +
                    "\r\n" +
                    "Results for SEEK \r\n" +
                    "> operations executed: 213\r\n" +
                    "> time elapsed: 2 ms\r\n" +
                    "> operations per millisecond: 106.5\r\n" +
                    "> average duration per operation: 0.00938967136150234 ms\r\n" +
                    "SEEK 9389 ns\r\n");
            @out.Close();
        }

        public static void RunTargetApplication(int itemCount)
        {
            // Stage 1: running the application to record IO access 
            System.Console.WriteLine("Running target application ...");
            // Any custom application can be used instead
            new CrudApplication().Run(itemCount);
        }


        public static void PrepareDbFile(int itemCount)
        {
            // Stage 2:Replay the recorded IO to prepare a database file
            System.Console.WriteLine("Preparing DB file ...");
            DeleteFile(_dbFileName);
            IoAdapter rafFactory = new RandomAccessFileAdapter();
            IoAdapter raf = rafFactory.Open(_dbFileName, false, 0, false);
            // Use the file with the recorded operations from stage 1
            LogReplayer replayer = new LogReplayer("simplecrud_" + itemCount + ".log", raf);
            try
            {
                replayer.ReplayLog();
            }
            catch (IOException e)
            {
                ExitWithError("Error reading I/O operations log file");
            }
            finally
            {
                raf.Close();
            }
        }


        public static void RunBenchmark(int itemCount)
        {
            // Stage 3: Replay the recorded IO grouping command types
            System.Console.WriteLine("Running benchmark ...");
            DeleteBenchmarkResultsFile(itemCount);
            TextWriter @out = new StreamWriter(new FileStream(ResultsFileName(itemCount), FileMode.Append, FileAccess.Write));
            PrintRunHeader(itemCount, @out);
            // run all commands: READ_ENTRY, WRITE_ENTRY, SYNC_ENTRY, SEEK_ENTRY
            for (int i = 0; i < LogConstants.AllConstants.Length; i++)
            {
                String currentCommand = LogConstants.AllConstants[i];
                BenchmarkCommand(currentCommand, itemCount, _dbFileName, @out);
            }
            @out.Close();
            DeleteFile(_dbFileName);
            DeleteCrudLogFile(itemCount);
        }


        public static void BenchmarkCommand(String command, int itemCount, String dbFileName, TextWriter @out)
        {
            HashSet commands = CommandSet(command);
            IoAdapter io = IoAdapter(dbFileName);
            LogReplayer replayer = new LogReplayer(CrudApplication.LogFileName(itemCount), io, commands);
            List4 commandList = replayer.ReadCommandList();

            StopWatch watch = new StopWatch();
            watch.Start();
            replayer.PlayCommandList(commandList);
            watch.Stop();
            io.Close();

            long timeElapsed = watch.Elapsed();
            long operationCount = (long)replayer.OperationCounts()[command];
            PrintStatisticsForCommand(@out, command, timeElapsed, operationCount);
        }


        public static IoAdapter IoAdapter(String dbFileName)
        {
            if (Delayed())
            {
                return DelayingIoAdapter(dbFileName);
            }

            IoAdapter rafFactory = new RandomAccessFileAdapter();
            return rafFactory.Open(dbFileName, false, 0, false);
        }


        public static IoAdapter DelayingIoAdapter(String dbFileName)
        {
            IoAdapter rafFactory = new RandomAccessFileAdapter();
            IoAdapter delFactory = new DelayingIoAdapter(rafFactory, _delays);
            return delFactory.Open(dbFileName, false, 0, false);
        }


        public static void ProcessResultsFiles(String resultsFile1, String resultsFile2)
        {
            System.Console.WriteLine("Delaying:");
            try
            {
                DelayCalculation calculation = new DelayCalculation(resultsFile1, resultsFile2);
                calculation.ValidateData();
                if (!calculation.IsValidData())
                {
                    ExitWithError("> Result files are invalid for delaying!");
                }
                _delays = calculation.CalculatedDelays();
                System.Console.WriteLine("> Required delays:");
                System.Console.WriteLine("> " + _delays);
                System.Console.WriteLine("> Adjusting delay timer to match required delays...");
                calculation.AdjustDelays(_delays);
                System.Console.WriteLine("> Adjusted delays:");
                System.Console.WriteLine("> " + _delays);
            }
            catch (IOException e)
            {
                ExitWithError("> Could not open results file(s)!\n" +
                            "> Please check the file name settings in IoBenchmark.properties.");
            }
        }


        public static void ExitWithError(String error)
        {
            System.Console.WriteLine(error + "\n Aborting execution!");
            Environment.Exit(0);
        }

        public static String ResultsFileName(int itemCount)
        {
            String fileName = "db4o-IoBenchmark-results-" + itemCount;
            if (Delayed())
            {
                fileName += "-delayed";
            }
            fileName += ".log";
            return fileName;
        }

        public static bool Delayed()
        {
            return _delays != null;
        }

        public static HashSet CommandSet(String command)
        {
            HashSet commands = new HashSet();
            commands.Add(command);
            return commands;
        }

        public static void DeleteBenchmarkResultsFile(int itemCount)
        {
            DeleteFile(ResultsFileName(itemCount));
        }

        public static void DeleteCrudLogFile(int itemCount)
        {
            DeleteFile("simplecrud_" + itemCount + ".log");
        }

        public static void DeleteFile(String fileName)
        {
            File.Delete(fileName);
        }


        public static void PrintRunHeader(int itemCount, TextWriter @out)
        {
            Output(@out, _singleLine);
            Output(@out, "db4o IoBenchmark results with " + itemCount + " items");
            System.Console.WriteLine("Statistics written to " + ResultsFileName(itemCount));
            Output(@out, _singleLine);
            Output(@out, "");
        }

        public static void PrintStatisticsForCommand(TextWriter @out, String currentCommand, long timeElapsed, long operationCount)
        {
            double avgTimePerOp = (double)timeElapsed / (double)operationCount;
            double opsPerMs = (double)operationCount / (double)timeElapsed;
            double nanosPerMilli = Math.Pow(10, 6);

            String output = "Results for " + currentCommand + "\r\n" +
                            "> operations executed: " + operationCount + "\r\n" +
                            "> time elapsed: " + timeElapsed + " ms\r\n" +
                            "> operations per millisecond: " + opsPerMs + "\r\n" +
                            "> average duration per operation: " + avgTimePerOp + " ms\r\n" +
                            currentCommand + (int)(avgTimePerOp * nanosPerMilli) + " ns\r\n";

            Output(@out, output);
            System.Console.WriteLine(" ");
        }

        public static void Output(TextWriter @out, String text)
        {
            @out.WriteLine(text);
            System.Console.WriteLine(text);
        }


        public static void PrintDoubleLine()
        {
            System.Console.WriteLine(_doubleLine);
        }


    }


}
