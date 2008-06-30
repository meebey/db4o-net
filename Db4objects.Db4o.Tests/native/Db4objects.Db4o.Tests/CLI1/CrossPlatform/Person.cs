using System.Collections;
using System.Collections.Generic;

namespace Db4objects.Db4o.Tests.CLI1.CrossPlatform
{
	public class Person
	{
		private static IComparer<Person> _sortByYear = new SortByYearImpl();

		private string _name;
		private int _year;

		public Person(string name, int year)
		{
			_name = name;
			_year = year;
		}

		public string Name
		{
			get { return _name; }
		}

		public int Year
		{
			get { return _year; }
		}

		public override bool Equals(object obj)
		{
			Person candidate = (Person) obj;
			if (candidate == null) return false;
			if (candidate.GetType() != GetType()) return false;

			return _name == candidate.Name && _year == candidate.Year;
		}

		public static IComparer<Person> SortByYear
		{
			get
			{
				return _sortByYear;
			}
		}

		public override string ToString()
		{
			return _name + "/" + _year;
		}

		private sealed class SortByYearImpl : IComparer<Person>
		{
			#region IComparer<Person> Members

			public int Compare(Person lhs, Person rhs)
			{
				return lhs._year - rhs._year;
			}

			#endregion
		}
	}
}
