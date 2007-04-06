using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	public class IxTraverser
	{
		private IxPath i_appendHead;

		private IxPath i_appendTail;

		private IxPath i_greatHead;

		private IxPath i_greatTail;

		internal IIndexable4 i_handler;

		private IxPath i_smallHead;

		private IxPath i_smallTail;

		internal bool[] i_take;

		private void Add(IVisitor4 visitor, IxPath a_previousPath, IxPath a_great, IxPath
			 a_small)
		{
			AddPathTree(visitor, a_previousPath);
			if (a_great != null && a_small != null && a_great.CarriesTheSame(a_small))
			{
				Add(visitor, a_great, a_great.i_next, a_small.i_next);
				return;
			}
			AddGreater(visitor, a_small);
			AddSmaller(visitor, a_great);
		}

		private void AddAll(IVisitor4 visitor, Tree a_tree)
		{
			if (a_tree != null)
			{
				((IxTree)a_tree).Visit(visitor, null);
				AddAll(visitor, a_tree._preceding);
				AddAll(visitor, a_tree._subsequent);
			}
		}

		private void AddGreater(IVisitor4 visitor, IxPath a_path)
		{
			if (a_path != null)
			{
				if (a_path.i_next == null)
				{
					AddSubsequent(visitor, a_path);
				}
				else
				{
					if (a_path.i_next.i_tree == a_path.i_tree._preceding)
					{
						AddSubsequent(visitor, a_path);
					}
					else
					{
						AddPathTree(visitor, a_path);
					}
					AddGreater(visitor, a_path.i_next);
				}
			}
		}

		private void AddPathTree(IVisitor4 visitor, IxPath a_path)
		{
			if (a_path != null)
			{
				a_path.Add(visitor);
			}
		}

		private void AddPreceding(IVisitor4 visitor, IxPath a_path)
		{
			AddPathTree(visitor, a_path);
			AddAll(visitor, a_path.i_tree._preceding);
		}

		private void AddSmaller(IVisitor4 visitor, IxPath a_path)
		{
			if (a_path != null)
			{
				if (a_path.i_next == null)
				{
					AddPreceding(visitor, a_path);
				}
				else
				{
					if (a_path.i_next.i_tree == a_path.i_tree._subsequent)
					{
						AddPreceding(visitor, a_path);
					}
					else
					{
						AddPathTree(visitor, a_path);
					}
					AddSmaller(visitor, a_path.i_next);
				}
			}
		}

		private void AddSubsequent(IVisitor4 visitor, IxPath a_path)
		{
			AddPathTree(visitor, a_path);
			AddAll(visitor, a_path.i_tree._subsequent);
		}

		private int CountGreater(IxPath a_path, int a_sum)
		{
			if (a_path.i_next == null)
			{
				return a_sum + CountSubsequent(a_path);
			}
			if (a_path.i_next.i_tree == a_path.i_tree._preceding)
			{
				a_sum += CountSubsequent(a_path);
			}
			else
			{
				a_sum += a_path.CountMatching();
			}
			return CountGreater(a_path.i_next, a_sum);
		}

		private int CountPreceding(IxPath a_path)
		{
			return Tree.Size(a_path.i_tree._preceding) + a_path.CountMatching();
		}

		private int CountSmaller(IxPath a_path, int a_sum)
		{
			if (a_path.i_next == null)
			{
				return a_sum + CountPreceding(a_path);
			}
			if (a_path.i_next.i_tree == a_path.i_tree._subsequent)
			{
				a_sum += CountPreceding(a_path);
			}
			else
			{
				a_sum += a_path.CountMatching();
			}
			return CountSmaller(a_path.i_next, a_sum);
		}

		private int CountSpan(IxPath a_previousPath, IxPath a_great, IxPath a_small)
		{
			if (a_great == null)
			{
				if (a_small == null)
				{
					return a_previousPath.CountMatching();
				}
				return CountGreater(a_small, a_previousPath.CountMatching());
			}
			else
			{
				if (a_small == null)
				{
					return CountSmaller(a_great, a_previousPath.CountMatching());
				}
			}
			if (a_great.CarriesTheSame(a_small))
			{
				return CountSpan(a_great, a_great.i_next, a_small.i_next);
			}
			return a_previousPath.CountMatching() + CountGreater(a_small, 0) + CountSmaller(a_great
				, 0);
		}

		private int CountSubsequent(IxPath a_path)
		{
			return Tree.Size(a_path.i_tree._subsequent) + a_path.CountMatching();
		}

		private void DelayedAppend(IxTree a_tree, int a_comparisonResult, int[] lowerAndUpperMatch
			)
		{
			if (i_appendHead == null)
			{
				i_appendHead = new IxPath(this, null, a_tree, a_comparisonResult, lowerAndUpperMatch
					);
				i_appendTail = i_appendHead;
			}
			else
			{
				i_appendTail = i_appendTail.Append(a_tree, a_comparisonResult, lowerAndUpperMatch
					);
			}
		}

		private void FindBoth()
		{
			if (i_greatTail.i_comparisonResult == 0)
			{
				FindSmallestEqualFromEqual((IxTree)i_greatTail.i_tree._preceding);
				ResetDelayedAppend();
				FindGreatestEqualFromEqual((IxTree)i_greatTail.i_tree._subsequent);
			}
			else
			{
				if (i_greatTail.i_comparisonResult < 0)
				{
					FindBoth1((IxTree)i_greatTail.i_tree._subsequent);
				}
				else
				{
					FindBoth1((IxTree)i_greatTail.i_tree._preceding);
				}
			}
		}

		private void FindBoth1(IxTree a_tree)
		{
			if (a_tree != null)
			{
				int res = a_tree.Compare(null);
				int[] lowerAndUpperMatch = a_tree.LowerAndUpperMatch();
				i_greatTail = i_greatTail.Append(a_tree, res, lowerAndUpperMatch);
				i_smallTail = i_smallTail.Append(a_tree, res, lowerAndUpperMatch);
				FindBoth();
			}
		}

		private void FindNullPath1(IxPath[] headTail)
		{
			if (headTail[1].i_comparisonResult == 0)
			{
				FindGreatestNullFromNull(headTail, (IxTree)headTail[1].i_tree._subsequent);
			}
			else
			{
				if (headTail[1].i_comparisonResult < 0)
				{
					FindNullPath2(headTail, (IxTree)headTail[1].i_tree._subsequent);
				}
				else
				{
					FindNullPath2(headTail, (IxTree)headTail[1].i_tree._preceding);
				}
			}
		}

		private void FindNullPath2(IxPath[] headTail, IxTree tree)
		{
			if (tree != null)
			{
				int res = tree.Compare(null);
				headTail[1] = headTail[1].Append(tree, res, tree.LowerAndUpperMatch());
				FindNullPath1(headTail);
			}
		}

		private void FindGreatestNullFromNull(IxPath[] headTail, IxTree tree)
		{
			if (tree != null)
			{
				int res = tree.Compare(null);
				DelayedAppend(tree, res, tree.LowerAndUpperMatch());
				if (res == 0)
				{
					headTail[1] = headTail[1].Append(i_appendHead, i_appendTail);
					ResetDelayedAppend();
				}
				if (res > 0)
				{
					FindGreatestNullFromNull(headTail, (IxTree)tree._preceding);
				}
				else
				{
					FindGreatestNullFromNull(headTail, (IxTree)tree._subsequent);
				}
			}
		}

		public virtual int FindBounds(object a_constraint, IxTree a_tree)
		{
			if (a_tree != null)
			{
				i_handler = a_tree.Handler();
				i_handler.PrepareComparison(a_constraint);
				int res = a_tree.Compare(null);
				i_greatHead = new IxPath(this, null, a_tree, res, a_tree.LowerAndUpperMatch());
				i_greatTail = i_greatHead;
				i_smallHead = (IxPath)i_greatHead.ShallowClone();
				i_smallTail = i_smallHead;
				FindBoth();
				int span = 0;
				if (i_take[QE.EQUAL])
				{
					span += CountSpan(i_greatHead, i_greatHead.i_next, i_smallHead.i_next);
				}
				if (i_take[QE.SMALLER])
				{
					IxPath head = i_smallHead;
					while (head != null)
					{
						span += head.CountPreceding(i_take[QE.NULLS]);
						head = head.i_next;
					}
				}
				if (i_take[QE.GREATER])
				{
					IxPath head = i_greatHead;
					while (head != null)
					{
						span += head.CountSubsequent();
						head = head.i_next;
					}
				}
				return span;
			}
			return 0;
		}

		public virtual int FindBoundsExactMatch(object a_constraint, IxTree a_tree)
		{
			i_take = new bool[] { false, false, false, false };
			i_take[QE.EQUAL] = true;
			return FindBounds(a_constraint, a_tree);
		}

		private void FindGreatestEqualFromEqual(IxTree a_tree)
		{
			if (a_tree != null)
			{
				int res = a_tree.Compare(null);
				DelayedAppend(a_tree, res, a_tree.LowerAndUpperMatch());
				if (res == 0)
				{
					i_greatTail = i_greatTail.Append(i_appendHead, i_appendTail);
					ResetDelayedAppend();
				}
				if (res > 0)
				{
					FindGreatestEqualFromEqual((IxTree)a_tree._preceding);
				}
				else
				{
					FindGreatestEqualFromEqual((IxTree)a_tree._subsequent);
				}
			}
		}

		private void FindSmallestEqualFromEqual(IxTree a_tree)
		{
			if (a_tree != null)
			{
				int res = a_tree.Compare(null);
				DelayedAppend(a_tree, res, a_tree.LowerAndUpperMatch());
				if (res == 0)
				{
					i_smallTail = i_smallTail.Append(i_appendHead, i_appendTail);
					ResetDelayedAppend();
				}
				if (res < 0)
				{
					FindSmallestEqualFromEqual((IxTree)a_tree._subsequent);
				}
				else
				{
					FindSmallestEqualFromEqual((IxTree)a_tree._preceding);
				}
			}
		}

		private void ResetDelayedAppend()
		{
			i_appendHead = null;
			i_appendTail = null;
		}

		public virtual void VisitAll(IVisitor4 visitor)
		{
			if (i_take[QE.EQUAL])
			{
				if (i_greatHead != null)
				{
					Add(visitor, i_greatHead, i_greatHead.i_next, i_smallHead.i_next);
				}
			}
			if (i_take[QE.SMALLER])
			{
				IxPath head = i_smallHead;
				while (head != null)
				{
					head.AddPrecedingToCandidatesTree(visitor);
					head = head.i_next;
				}
			}
			if (i_take[QE.GREATER])
			{
				IxPath head = i_greatHead;
				while (head != null)
				{
					head.AddSubsequentToCandidatesTree(visitor);
					head = head.i_next;
				}
			}
		}

		public virtual void VisitPreceding(FreespaceVisitor visitor)
		{
			if (i_smallHead != null)
			{
				i_smallHead.VisitPreceding(visitor);
			}
		}

		public virtual void VisitSubsequent(FreespaceVisitor visitor)
		{
			if (i_greatHead != null)
			{
				i_greatHead.VisitSubsequent(visitor);
			}
		}

		public virtual void VisitMatch(FreespaceVisitor visitor)
		{
			if (i_smallHead != null)
			{
				i_smallHead.VisitMatch(visitor);
			}
		}
	}
}
