/* Copyright (C) 2004-2006   db4objects Inc.   http://www.db4o.com */

namespace Db4oTools {

	using System;
	
	using Sharpen.Lang;

	using Db4objects.Db4o;
	using Db4objects.Db4o.Ext;
	using Db4objects.Db4o.Foundation;

    /**
     * prints statistics about a database file to System.out.
     * <br>
     * <br>Pass the database file path as an argument.
     * <br>
     * <br>This class is not part of db4o.dll. It is delivered
     * as sourcecode in the path ../com/db4o/tools/<br><br>
     */
    public class Statistics {
      
        /**
         * the main method that runs the statistics.
         * @param String[] a String array of length 1, with the name
         * of the database file as element 0.
         */
        public static void Main(String[] args) {
            if (args == null || args.Length != 1) {
                Console.WriteLine("Usage: java Db4objects.Db4o.Tools.Statistics <database filename>");
            } else {
                new Statistics().Run(args[0]);
            }
        }
      
        public void Run(String filename) {
            if (new Sharpen.IO.File(filename).Exists()) {
                IObjectContainer con1 = null;
                try { 
                    con1 = Db4o.OpenFile(filename);
                    PrintHeader("STATISTICS");
                    Console.WriteLine("File: " + filename);
                    PrintStats(con1, filename);
                    con1.Close();
                      
                }  catch (Exception e) { 
                    Console.WriteLine("Statistics failed for file: \'" + filename + "\'");
                    Console.WriteLine(e.Message);
                    Sharpen.Runtime.PrintStackTrace(e);
                                         
                }
            } else {
                Console.WriteLine("File not found: \'" + filename + "\'");
            }
        }
      
        private void PrintStats(IObjectContainer con, String filename) {
            Tree unavailable = new TreeString(REMOVE);
            Tree noConstructor = new TreeString(REMOVE);
            IStoredClass[] internalClasses = con.Ext().StoredClasses();
            for (int i1 = 0; i1 < internalClasses.Length; i1++) {
                try { 
                    Class clazz1 = Class.ForName(internalClasses[i1].GetName());
                    try { 
                        clazz1.NewInstance();
                                
                    }  catch (Exception th) { 
                        noConstructor = noConstructor.Add(new TreeString(internalClasses[i1].GetName()));
                    }
                }  catch (Exception t) { 
                    unavailable = unavailable.Add(new TreeString(internalClasses[i1].GetName()));
                                         
                }
            }
            unavailable = unavailable.RemoveLike(new TreeString(REMOVE));
            noConstructor = noConstructor.RemoveLike(new TreeString(REMOVE));
            if (unavailable != null) {
                PrintHeader("UNAVAILABLE");
                unavailable.Traverse(new StatisticsPrintKey());
            }
            if (noConstructor != null) {
                PrintHeader("NO PUBLIC CONSTRUCTOR");
                noConstructor.Traverse(new StatisticsPrintKey());
            }
            PrintHeader("CLASSES");
            Console.WriteLine("Number of objects per class:");
            if (internalClasses.Length > 0) {
                Tree all1 = new TreeStringObject(internalClasses[0].GetName(), internalClasses[0]);
                for (int i1 = 1; i1 < internalClasses.Length; i1++) {
                    all1 = all1.Add(new TreeStringObject(internalClasses[i1].GetName(), internalClasses[i1]));
                }
                all1.Traverse(new StatisticsPrintNodes());
            }
            PrintHeader("SUMMARY");
            Console.WriteLine("File: " + filename);
            Console.WriteLine("Stored classes: " + internalClasses.Length);
            if (unavailable != null) {
                Console.WriteLine("Unavailable classes: " + unavailable.Size());
            }
            if (noConstructor != null) {
                Console.WriteLine("Classes without public constructors: " + noConstructor.Size());
            }
            Console.WriteLine("Total number of objects: " + (ids.Size() - 1));
        }
      
        private void PrintHeader(String str) {
            int starcount = (39 - str.Length) / 2;
            string stars = "";
            for (int i1 = 0; i1 < starcount; i1++) {
                stars += "*";
            }
            Console.WriteLine("\n\n" + stars + " " + str + " " + stars);
        }

        internal static TreeInt ids = new TreeInt(0);
        private static String REMOVE = "XXxxREMOVExxXX";
    }

    internal class StatisticsPrintKey : IVisitor4{
        public void Visit(Object obj){
            Console.WriteLine(((TreeString)obj)._key);           
        }
    }

    internal class StatisticsPrintNodes : IVisitor4{
        public void Visit(Object obj){
            TreeStringObject node = (TreeStringObject)obj;
            long[] newIDs = ((IStoredClass)node._object).GetIDs();
            for (int j = 0; j < newIDs.Length; j ++) {
                if (Statistics.ids.Find(new TreeInt((int)newIDs[j])) == null) {
                    Statistics.ids = (TreeInt)Statistics.ids.Add(new TreeInt((int)newIDs[j]));
                }
            }
            Console.WriteLine(node._key + ": " + newIDs.Length);
        }

    }
}