using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	internal class QEBitmap
	{
		public static Db4objects.Db4o.Internal.Fieldindex.QEBitmap ForQE(QE qe)
		{
			bool[] bitmap = new bool[4];
			qe.IndexBitMap(bitmap);
			return new Db4objects.Db4o.Internal.Fieldindex.QEBitmap(bitmap);
		}

		private QEBitmap(bool[] bitmap)
		{
			_bitmap = bitmap;
		}

		private bool[] _bitmap;

		public virtual bool TakeGreater()
		{
			return _bitmap[QE.GREATER];
		}

		public virtual bool TakeEqual()
		{
			return _bitmap[QE.EQUAL];
		}

		public virtual bool TakeSmaller()
		{
			return _bitmap[QE.SMALLER];
		}
	}
}
