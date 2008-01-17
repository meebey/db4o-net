/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Reflection;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA.Sample;

namespace Db4objects.Db4o.Tests.Common.TA.Sample
{
	public class SampleTestCase : AbstractDb4oTestCase, IOptOutTA, IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new SampleTestCase().RunAll();
		}

		private long customerID;

		private long countryID;

		private Db4oUUID customerUUID;

		private Db4oUUID countryUUID;

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
			config.GenerateUUIDs(ConfigScope.Globally);
		}

		protected override void Store()
		{
			Customer customer = new Customer();
			customer._name = "db4objects";
			Address address = new Address();
			customer._addresses = new Address[] { address };
			Country country = new Country();
			address._country = country;
			address._firstLine = "Suite 350";
			State state = new State();
			country._states = new State[] { state };
			state._name = "California";
			City city = new City();
			state._cities = new City[] { city };
			Store(customer);
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			object customer = RetrieveOnlyInstance(typeof(Customer));
			object country = RetrieveOnlyInstance(typeof(Country));
			customerID = Db().GetID(customer);
			countryID = Db().GetID(country);
			customerUUID = Db().GetObjectInfo(customer).GetUUID();
			countryUUID = Db().GetObjectInfo(country).GetUUID();
			Reopen();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestRetrieveNonActivatable()
		{
			CheckGraphActivation((Customer)RetrieveOnlyInstance(typeof(Customer)));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestRetrieveActivatable()
		{
			CheckGraphActivation((Country)RetrieveOnlyInstance(typeof(Country)));
		}

		public virtual void TestPeekPersisted()
		{
			Run(new _IActAndAssert_75(this));
		}

		private sealed class _IActAndAssert_75 : SampleTestCase.IActAndAssert
		{
			public _IActAndAssert_75(SampleTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object ActOnRoot(object obj)
			{
				return this._enclosing.Db().PeekPersisted(obj, int.MaxValue, true);
			}

			public void AssertOnLeaves(object obj)
			{
				Assert.IsNotNull(obj);
				Assert.IsFalse(this._enclosing.Db().IsStored(obj));
			}

			private readonly SampleTestCase _enclosing;
		}

		public virtual void TestFullActivation()
		{
			Run(new _IActAndAssert_87(this));
		}

		private sealed class _IActAndAssert_87 : SampleTestCase.IActAndAssert
		{
			public _IActAndAssert_87(SampleTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object ActOnRoot(object obj)
			{
				this._enclosing.Db().Activate(obj, int.MaxValue);
				return obj;
			}

			public void AssertOnLeaves(object obj)
			{
				Assert.IsNotNull(obj);
				Assert.IsTrue(this._enclosing.Db().IsStored(obj));
			}

			private readonly SampleTestCase _enclosing;
		}

		public virtual void TestRefresh()
		{
			Run(new _IActAndAssert_100(this));
		}

		private sealed class _IActAndAssert_100 : SampleTestCase.IActAndAssert
		{
			public _IActAndAssert_100(SampleTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object ActOnRoot(object obj)
			{
				this._enclosing.Db().Activate(obj, int.MaxValue);
				IEnumerator i = this._enclosing.IterateGraphStringFields(obj);
				while (i.MoveNext())
				{
					SampleTestCase.FieldOnObject fieldOnObject = (SampleTestCase.FieldOnObject)i.Current;
					fieldOnObject._field.Set(fieldOnObject._object, "modified");
				}
				this._enclosing.Db().Refresh(obj, int.MaxValue);
				return obj;
			}

			public void AssertOnLeaves(object obj)
			{
				IEnumerator i = this._enclosing.IterateStringFieldsOnObject(obj);
				while (i.MoveNext())
				{
					SampleTestCase.FieldOnObject fieldOnObject = (SampleTestCase.FieldOnObject)i.Current;
					Assert.AreNotEqual("modified", fieldOnObject._field.Get(fieldOnObject._object));
				}
			}

			private readonly SampleTestCase _enclosing;
		}

		public virtual void TestDeactivate()
		{
			Run(new _IActAndAssert_122(this));
		}

		private sealed class _IActAndAssert_122 : SampleTestCase.IActAndAssert
		{
			public _IActAndAssert_122(SampleTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object ActOnRoot(object obj)
			{
				this._enclosing.Db().Activate(obj, int.MaxValue);
				IEnumerator graph = this._enclosing.IterateGraph(obj);
				this._enclosing.Db().Deactivate(obj, int.MaxValue);
				return graph;
			}

			public void AssertOnLeaves(object obj)
			{
				this._enclosing.AssertIsDeactivated(obj);
			}

			private readonly SampleTestCase _enclosing;
		}

		public virtual void TestGetById()
		{
			AssertIsDeactivated(CountryByID());
			AssertIsDeactivated(CustomerByID());
		}

		public virtual void TestGetByUUID()
		{
			AssertIsDeactivated(Db().GetByUUID(countryUUID));
			AssertIsDeactivated(Db().GetByUUID(customerUUID));
		}

		public virtual IEnumerator IterateGraphStringFields(object obj)
		{
			return new CompositeIterator4(new _MappingIterator_146(this, IterateGraph(obj)));
		}

		private sealed class _MappingIterator_146 : MappingIterator
		{
			public _MappingIterator_146(SampleTestCase _enclosing, IEnumerator baseArg1) : base
				(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				return this._enclosing.IterateStringFieldsOnObject(current);
			}

			private readonly SampleTestCase _enclosing;
		}

		internal virtual Customer CustomerByID()
		{
			return (Customer)Db().GetByID(customerID);
		}

		internal virtual Country CountryByID()
		{
			return (Country)Db().GetByID(countryID);
		}

		internal virtual IEnumerator IterateGraph(object obj)
		{
			// A small evil multimethod hack to have "Do What I mean" behaviour. 
			if (obj is IEnumerator)
			{
				return (IEnumerator)obj;
			}
			if (obj is Customer)
			{
				return IterateGraph((Customer)obj);
			}
			return IterateGraph((Country)obj);
		}

		private IEnumerator IterateGraph(Customer customer)
		{
			return new CompositeIterator4(new IEnumerator[] { IterateGraph(customer._addresses
				[0]._country), new ArrayIterator4(new object[] { customer._addresses[0]._country
				, customer._addresses[0], customer }) });
		}

		private IEnumerator IterateGraph(Country country)
		{
			return new ArrayIterator4(new object[] { country._states[0]._cities[0], country._states
				[0], country });
		}

		/// <exception cref="Exception"></exception>
		private void CheckGraphActivation(Customer customer)
		{
			AssertIsActivated(customer);
			AssertIsNotNull(customer, "_name");
			AssertIsNotNull(customer, "_addresses");
			Address address = customer._addresses[0];
			AssertIsActivated(address);
			AssertIsNotNull(address, "_firstLine");
			AssertIsNotNull(address, "_country");
			CheckGraphActivation(address._country);
		}

		/// <exception cref="Exception"></exception>
		private void CheckGraphActivation(Country country)
		{
			AssertIsDeactivated(country);
			AssertIsNull(country, "_states");
			State state = country.GetState("94403");
			AssertIsActivated(state);
			AssertIsNotNull(country, "_states");
			AssertIsNotNull(state, "_name");
			AssertIsNotNull(state, "_cities");
			City city = state._cities[0];
			Assert.IsNotNull(city);
			AssertIsDeactivated(city);
			AssertIsNull(city, "_name");
		}

		internal virtual void AssertIsDeactivated(object obj)
		{
			IEnumerator i = IterateFieldValues(obj);
			while (i.MoveNext())
			{
				Assert.IsNull(i.Current);
			}
			Assert.IsFalse(Db().IsActive(obj));
			Assert.IsTrue(Db().IsStored(obj));
		}

		private void AssertIsActivated(object obj)
		{
			IEnumerator i = IterateFieldValues(obj);
			while (i.MoveNext())
			{
				Assert.IsNotNull(i.Current);
			}
			Assert.IsTrue(Db().IsActive(obj));
			Assert.IsTrue(Db().IsStored(obj));
		}

		internal virtual IEnumerator IterateStringFieldsOnObject(object obj)
		{
			IReflectClass stringClass = Reflector().ForClass(typeof(string));
			return new _MappingIterator_236(this, stringClass, obj, IteratePersistentFields(obj
				));
		}

		private sealed class _MappingIterator_236 : MappingIterator
		{
			public _MappingIterator_236(SampleTestCase _enclosing, IReflectClass stringClass, 
				object obj, IEnumerator baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.stringClass = stringClass;
				this.obj = obj;
			}

			protected override object Map(object current)
			{
				IReflectField field = (IReflectField)current;
				if (field.GetFieldType() != stringClass)
				{
					return MappingIterator.Skip;
				}
				return new SampleTestCase.FieldOnObject(field, obj);
			}

			private readonly SampleTestCase _enclosing;

			private readonly IReflectClass stringClass;

			private readonly object obj;
		}

		private IEnumerator IterateFieldValues(object obj)
		{
			return new _MappingIterator_248(this, obj, IteratePersistentFields(obj));
		}

		private sealed class _MappingIterator_248 : MappingIterator
		{
			public _MappingIterator_248(SampleTestCase _enclosing, object obj, IEnumerator baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.obj = obj;
			}

			protected override object Map(object current)
			{
				IReflectField field = (IReflectField)current;
				try
				{
					return field.Get(obj);
				}
				catch (Exception e)
				{
					throw new Db4oException(e);
				}
			}

			private readonly SampleTestCase _enclosing;

			private readonly object obj;
		}

		private IEnumerator IteratePersistentFields(object obj)
		{
			IReflectClass claxx = Reflector().ForObject(obj);
			IReflectField[] fields = claxx.GetDeclaredFields();
			return new _MappingIterator_263(this, new ArrayIterator4(fields));
		}

		private sealed class _MappingIterator_263 : MappingIterator
		{
			public _MappingIterator_263(SampleTestCase _enclosing, ArrayIterator4 baseArg1) : 
				base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				IReflectField field = (IReflectField)current;
				if (field.IsTransient() || field.IsStatic())
				{
					return MappingIterator.Skip;
				}
				return field;
			}

			private readonly SampleTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		private void AssertIsNull(object obj, string fieldName)
		{
			Assert.IsTrue(FieldIsNull(obj, fieldName));
		}

		/// <exception cref="Exception"></exception>
		private void AssertIsNotNull(object obj, string fieldName)
		{
			Assert.IsFalse(FieldIsNull(obj, fieldName));
		}

		/// <exception cref="Exception"></exception>
		private bool FieldIsNull(object obj, string fieldName)
		{
			Type clazz = obj.GetType();
			FieldInfo field = Sharpen.Runtime.GetDeclaredField(clazz, fieldName);
			Assert.IsNotNull(field);
			return field.GetValue(obj) == null;
		}

		private void Run(SampleTestCase.IActAndAssert actAndAssert)
		{
			Run(actAndAssert, CustomerByID());
			Run(actAndAssert, CountryByID());
		}

		private void Run(SampleTestCase.IActAndAssert actAndAssert, object obj)
		{
			IEnumerator i = IterateGraph(actAndAssert.ActOnRoot(obj));
			while (i.MoveNext())
			{
				actAndAssert.AssertOnLeaves(i.Current);
			}
		}

		public class FieldOnObject
		{
			public readonly IReflectField _field;

			public readonly object _object;

			public FieldOnObject(IReflectField field, object obj)
			{
				_field = field;
				_object = obj;
			}
		}

		public interface IActAndAssert
		{
			object ActOnRoot(object obj);

			void AssertOnLeaves(object obj);
		}
	}
}
