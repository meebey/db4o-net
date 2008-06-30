/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Util;
using Db4oUnit;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.CLI1.CrossPlatform
{
	class CrossplatformTestCase : ITestLifeCycle
	{
#if !CF
		private const int HOST_PORT = 3739;
		private const string USER_NAME = "db4o_cpt";
		private const string USER_PWD = "test";

		private const string JOE_NAME = "Joe";
		private const string WOODY_NAME = "Woody";

		private static readonly Person[] persons = new Person[]
			                				{
			                					new Person("Viktor Navorski", 2004),
			                					new Person(JOE_NAME, 1),
			                					new Person("Carl Hanratty", 2002),
			                					new Person(WOODY_NAME, 1995),
			                					new Person("Woody Car", 2006),
			                					new Person("Joe", 2),
			                				};

		public void TestQueryOnEmptyDatabaseDoesntThrows()
		{
			AssertQuery(
				new Person[0],
				FindAllJoes,
				"I doubt you'd name your baby like this, but who knows? ;)");
		}

		public void TestDotnetClient()
		{
			AssertEvents();

			//AssertSort();

			AddPersons(persons);

			AssertQuery(
				Array.FindAll(persons, FindAllJoes), 
				FindAllJoes, 
				JOE_NAME);

			AssertDelete();

			AssertQueryFromJavaClient();

			AssertInsertFromJavaClient("Michael Sullivan", 2002);
		}

		private void AssertInsertFromJavaClient(string name, int year)
		{
			CompileJava(JavaClientQuery);

			string insertResult = JavaServices.java(JavaClientQuery.MainClassName, HOST_PORT.ToString(), USER_NAME, USER_PWD, name, year.ToString());
			Assert.AreEqual("", insertResult);

			AssertExactlyOneInstance(name, year);
		}

		private void AssertExactlyOneInstance(string name, int year)
		{
			Iterator4Assert.AreEqual(
				new Person[] { new Person(name, year) },
				SodaQueryByName(name).GetEnumerator());
		}

		private void AssertQueryFromJavaClient()
		{
			Iterator4Assert.AreEqual(
							Array.FindAll(
									persons, 
									delegate(Person candidate) { return candidate.Name.StartsWith(WOODY_NAME);}),
							RunJavaQuery(WOODY_NAME));
		}

		private IEnumerator RunJavaQuery(string tbf)
		{
			CompileJava(JavaClientQuery);

			string queryResult = JavaServices.java(JavaClientQuery.MainClassName, HOST_PORT.ToString(), USER_NAME, USER_PWD, tbf);
			Assert.IsGreater(0, queryResult.Length);

			return ParseJavaClientResults(queryResult);
		}

		private void CompileJava(JavaSnippet snippet)
		{
			JavaServices.CompileJavaCode(snippet.MainClassFile, snippet.SourceCode);
		}

		private static IEnumerator ParseJavaClientResults(string result)
		{
			IList<Person> personList = new List<Person>();

			TextReader reader = new StringReader(result);
			string line ;
			while ((line = reader.ReadLine())  != null)
			{
				Match match = Regex.Match(line, @"(.+)/(.+)\Z");
				personList.Add(new Person(match.Groups[1].Captures[0].Value, Int32.Parse(match.Groups[2].Captures[0].Value)));
			}

			return personList.GetEnumerator();
		}

		private void AssertQuery(Person[] expected, Predicate<Person> predicate, string name)
		{
			AssertNativeQuery(expected, predicate);
			AssertSodaQuery(expected, name);
			AssertQBEQuery(expected, new Person(name, 0));
		}

		private void AssertNativeQuery(Person[] expected, Predicate<Person> predicate)
		{
			Iterator4Assert.AreEqual(expected, _db.Query(predicate).GetEnumerator());
		}

		private void AssertQBEQuery(Person[] expected, Person template)
		{
			Iterator4Assert.AreEqual(expected, _db.QueryByExample(template).GetEnumerator());
		}

		private void AssertSodaQuery(Person[] expected, string name)
		{
			ICollection actual = SodaQueryByName(name);
			Assert.AreEqual(expected.Length, actual.Count);
			Iterator4Assert.AreEqual(expected, actual.GetEnumerator());
		}

		private void AssertDelete()
		{
			AssertDelete(JOE_NAME, Array.FindAll(persons, FindAllJoes).Length);
			
			DeleteAll();
			
			AssertNotFound(WOODY_NAME);

			_db.Ext().Purge();
			
			AddPersons(persons);
		}

		private void AssertDelete(string name, int count)
		{
			AssertStoredCount(name, count);
			Delete(SodaQueryByName(name));
			AssertNotFound(name);
		}

		private void AssertEvents()
		{
			DeleteAll();

/*			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(_db);


			eventRegistry.Created += delegate { createdCount++; };
			eventRegistry.Deleted += delegate { deletedCount++; };
 */

			int deletedCount = 0;
			int createdCount = 0;

			RegisterEvents(
				delegate { createdCount++; },
				delegate { deletedCount++; });

			AddPersons(persons);
			DeleteAll();

			Assert.AreEqual(persons.Length, createdCount);
			//Assert.AreEqual(persons.Length, _deletedCount);
			TestPlatform.EmitWarning("Delete events still not working.");
		}

		private void RegisterEvents(ObjectEventHandler createdHandler, ObjectEventHandler  deletedHandler)
		{
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(_db);

			eventRegistry.Created += createdHandler;
			eventRegistry.Deleted += deletedHandler;
		}

		private void AssertSort()
		{
			Person[] expectedOrder = (Person[])persons.Clone();
			Array.Sort(expectedOrder, Person.SortByYear);

			Iterator4Assert.AreEqual(
				expectedOrder, 
				_db.Query(Person.SortByYear).GetEnumerator());
		}

		private void AssertStoredCount(string name, int count)
		{
			ICollection personList = SodaQueryByName(name);
			Assert.IsNotNull(personList);

			Assert.AreEqual(count, personList.Count);
		}

		private void AssertNotFound(string name)
		{
			AssertStoredCount(name, 0);
		}

		private void Delete(ICollection toBeDeleted)
		{
			foreach (Object item in toBeDeleted)
			{
				_db.Delete(item);
			}

			_db.Commit();
		}

		private void DeleteAll()
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (Person));
			Delete(query.Execute());
		}

		private ICollection SodaQueryByName(string name)
		{
			IQuery query = _db.Query();
			query.Constrain(typeof (Person));
			query.Descend("_name").Constrain(name);
			
			return query.Execute();
		}

		private void AddPersons(IEnumerable<Person> toBeAdded)
		{
			foreach(Person person in toBeAdded)
			{
				_db.Store(person);
			}

			_db.Commit();
		}

		private static bool FindAllJoes(Person candidate)
		{
			return candidate.Name == JOE_NAME;
		}

		private static IConfiguration Config()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.Add(new JavaSupport());

			AddAlias(config, "com.db4o.crossplatform.test.server.StartServer$StopServer", typeof(StopServer));
			AddAlias(config, "com.db4o.crossplatform.test.server.StartServer$Person", typeof(Person));
			//config.AddAlias((new TypeAlias("com.db4o.crossplatform.test.server.StartServer$SortByYear", "Db4objects.Db4o.Tests.CLI1.CrossPlatform.Person+SortByYearImpl, Db4objects.Db4o.Tests")));

			return config;
		}

		private static void AddAlias(IConfiguration config, string storedType, Type runtimeType)
		{
			config.AddAlias(new TypeAlias(storedType, TypeReference.FromType(runtimeType).ToString()));
		}

		private void Connect()
		{
			while (true)
			{
				try
				{
					_db = Db4oFactory.OpenClient(Config(), "localhost", HOST_PORT, USER_NAME, USER_PWD);
					break;
				}
				catch (SocketException se)
				{
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		private void StartJavaServer()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				JavaServices.CompileJavaCode(JavaServerCode.MainClassFile, JavaServerCode.SourceCode);
				string output = JavaServices.java(JavaServerCode.MainClassName, HOST_PORT.ToString(), USER_NAME, USER_PWD);
				Assert.AreEqual("", output);
			});
		}

		private void ShutdownJavaServer()
		{
			if (_db != null)
			{
				IMessageSender messageSender = _db.Ext().Configure().ClientServer().GetMessageSender();
				messageSender.Send(new StopServer());
			}
		}

		public void SetUp()
		{
			if (!JavaServices.CanRunJavaCompatibilityTests())
			{
				throw new TestException("Fail to run cross platform tests", null);
			}

			StartJavaServer();

			Connect();
		}

		public void TearDown()
		{
			ShutdownJavaServer();
		}

		private IEventRegistry _eventRegistry;
		private IObjectContainer _db;

		private readonly JavaSnippet JavaServerCode = new JavaSnippet("com.db4o.crossplatform.test.server.StartServer",
@"package com.db4o.crossplatform.test.server;

import java.io.*;
import com.db4o.query.*; 
import com.db4o.*;
import com.db4o.foundation.io.File4;
import com.db4o.foundation.io.Path4;
import com.db4o.messaging.MessageContext;
import com.db4o.messaging.MessageRecipient;

public class StartServer implements MessageRecipient  {
	private boolean stop = false;

	public static void main(String[] args) throws IOException {
		new StartServer().runServer(args);
	}

	public void runServer(String[] args) throws IOException {
		if (args.length < 3) {
			System.err.println(""Db4o (java) server usage: StartServer host_port user password"");
		}
			
		synchronized (this) {
			String databaseFile = Path4.combine(Path4.getTempPath(),""CrossPlatformJavaServer.odb""); 
			File4.delete(databaseFile);

			ObjectServer db4oServer = Db4o.openServer(databaseFile, Integer.parseInt(args[0]));
			db4oServer.grantAccess(args[1], args[2]);
			
			db4oServer.ext().configure().clientServer().setMessageRecipient(this);

			Thread.currentThread().setName(this.getClass().getName());

			try {
				int count = 0;
				while(!stop && count < 240) {
					this.wait(500);
					count++;
				}

			} catch (Exception e) {
				e.printStackTrace();
			}
			db4oServer.close();
		}
	}

	@Override
	public void processMessage(MessageContext con, Object message) {
		if (message instanceof StopServer) {
			close();
		}
	}
	
	public void close() {
		synchronized (this) {
			stop = true;
			this.notify();
		}
	}

	public static class StopServer {
		private int _id;
	}

	public static class Person {
		public String _name;
		public int _year;
    }

	public static class SortByYear implements QueryComparator {
		public int compare(Object first, Object second) {
			Person lhs = (Person) first;
			Person rhs = (Person) second;

			return lhs._year - rhs._year;
		}
	}
}");

		private readonly JavaSnippet JavaClientQuery = new JavaSnippet("com.db4o.crossplatform.test.client.ClientCrossPlatform",

@"package com.db4o.crossplatform.test.client;

import com.db4o.*;
import com.db4o.query.Query;
import com.db4o.config.Configuration;

public class ClientCrossPlatform {
	public static void main(String[] args) {
		if (args.length < 4) {
			System.err.println(""Java client query: invalid # of arguments. Expected: 4 (port user passwd name [year]) got "" + args.length);
			return;
		}
		
		ObjectContainer db = Db4o.openClient(config(), ""localhost"", Integer.parseInt(args[0]), args[1], args[2]);
		
		if (args.length == 4) {	
			System.out.print(queryPersons(db, args[3]));	
		}
		else {
			insertPerson(db, args[3], Integer.parseInt(args[4]));	
		}
			
		db.close();
	}

	private static String queryPersons(ObjectContainer db, String tbq) {
		StringBuffer output = new StringBuffer();
		for (Object person : getPersons(db, tbq)) output.append(person + ""\n"");
		return output.toString();
	}

	private static void insertPerson(ObjectContainer db, String name, int year) {
		db.set(new Person(name, year));
	}

	private static Configuration config() {
		Configuration config = Db4o.newConfiguration();
		
		config.addAlias(new com.db4o.config.TypeAlias(""com.db4o.crossplatform.test.server.StartServer$Person"", Person.class.getName()));
		return config;
	}

	private static ObjectSet getPersons(ObjectContainer db, String name) {
		Query query = db.query();
	
		query.constrain(Person.class);
		query.descend(""_name"").constrain(name).like();
		
		return query.execute();
	}

	public static class Person {
		public String _name;
		public int _year;

        public Person(String name, int year) {
			_name = name;
			_year = year;
		}

		public Person() {
		}

		public String toString() {
			return _name + ""/"" + _year;
		}		
	}
}");
#else
		public void SetUp()
		{
		}

		public void TearDown()
		{
		}
#endif
	}

	internal class StopServer
	{
		public int _id;
	}
}
