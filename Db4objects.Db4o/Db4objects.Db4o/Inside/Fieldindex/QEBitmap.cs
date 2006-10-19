namespace Db4objects.Db4o.Inside.Fieldindex
{
	internal class QEBitmap
	{
		public static Db4objects.Db4o.Inside.Fieldindex.QEBitmap ForQE(Db4objects.Db4o.QE
			 qe)
		{
			bool[] bitmap = new bool[4];
			qe.IndexBitMap(bitmap);
			return new Db4objects.Db4o.Inside.Fieldindex.QEBitmap(bitmap);
		}

		private QEBitmap(bool[] bitmap)
		{
			_bitmap = bitmap;
		}

		private bool[] _bitmap;

		public virtual bool TakeGreater()
		{
			return _bitmap[Db4objects.Db4o.QE.GREATER];
		}

		public virtual bool TakeEqual()
		{
			return _bitmap[Db4objects.Db4o.QE.EQUAL];
		}

		public virtual bool TakeSmaller()
		{
			return _bitmap[Db4objects.Db4o.QE.SMALLER];
		}
	}
}
