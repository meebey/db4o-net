/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// This class provides static constants for the query evaluation
	/// modes that db4o supports.
	/// </summary>
	/// <remarks>
	/// This class provides static constants for the query evaluation
	/// modes that db4o supports.
	/// <br /><br /><b>For detailed documentation please see
	/// <see cref="IQueryConfiguration.EvaluationMode">IQueryConfiguration.EvaluationMode
	/// 	</see>
	/// </b>
	/// </remarks>
	public class QueryEvaluationMode
	{
		private readonly string _id;

		private QueryEvaluationMode(string id)
		{
			_id = id;
		}

		/// <summary>Constant for immediate query evaluation.</summary>
		/// <remarks>
		/// Constant for immediate query evaluation. The query is executed fully
		/// when Query#execute() is called.
		/// <br /><br /><b>For detailed documentation please see
		/// <see cref="IQueryConfiguration.EvaluationMode">IQueryConfiguration.EvaluationMode
		/// 	</see>
		/// </b>
		/// </remarks>
		public static readonly Db4objects.Db4o.Config.QueryEvaluationMode IMMEDIATE = new 
			Db4objects.Db4o.Config.QueryEvaluationMode("IMMEDIATE");

		/// <summary>Constant for snapshot query evaluation.</summary>
		/// <remarks>
		/// Constant for snapshot query evaluation. When Query#execute() is called,
		/// the query processor chooses the best indexes, does all index processing
		/// and creates a snapshot of the index at this point in time. Non-indexed
		/// constraints are evaluated lazily when the application iterates through
		/// the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// resultset of the query.
		/// <br /><br /><b>For detailed documentation please see
		/// <see cref="IQueryConfiguration.EvaluationMode">IQueryConfiguration.EvaluationMode
		/// 	</see>
		/// </b>
		/// </remarks>
		public static readonly Db4objects.Db4o.Config.QueryEvaluationMode SNAPSHOT = new 
			Db4objects.Db4o.Config.QueryEvaluationMode("SNAPSHOT");

		/// <summary>Constant for lazy query evaluation.</summary>
		/// <remarks>
		/// Constant for lazy query evaluation. When Query#execute() is called, the
		/// query processor only chooses the best index and creates an iterator on
		/// this index. Indexes and constraints are evaluated lazily when the
		/// application iterates through the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// resultset of the query.
		/// <br /><br /><b>For detailed documentation please see
		/// <see cref="IQueryConfiguration.EvaluationMode">IQueryConfiguration.EvaluationMode
		/// 	</see>
		/// </b>
		/// </remarks>
		public static readonly Db4objects.Db4o.Config.QueryEvaluationMode LAZY = new Db4objects.Db4o.Config.QueryEvaluationMode
			("LAZY");

		private static readonly Db4objects.Db4o.Config.QueryEvaluationMode[] MODES = new 
			Db4objects.Db4o.Config.QueryEvaluationMode[] { Db4objects.Db4o.Config.QueryEvaluationMode
			.IMMEDIATE, Db4objects.Db4o.Config.QueryEvaluationMode.SNAPSHOT, Db4objects.Db4o.Config.QueryEvaluationMode
			.LAZY };

		/// <summary>internal method, ignore please.</summary>
		/// <remarks>internal method, ignore please.</remarks>
		public virtual int AsInt()
		{
			for (int i = 0; i < MODES.Length; i++)
			{
				if (MODES[i] == this)
				{
					return i;
				}
			}
			throw new InvalidOperationException();
		}

		/// <summary>internal method, ignore please.</summary>
		/// <remarks>internal method, ignore please.</remarks>
		public static Db4objects.Db4o.Config.QueryEvaluationMode FromInt(int i)
		{
			return MODES[i];
		}

		public override string ToString()
		{
			return _id;
		}
	}
}
