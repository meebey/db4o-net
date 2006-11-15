
using System.Globalization;

using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class CultureInfoTestCase : AbstractDb4oTestCase
	{
		public CultureInfo frfr;
		public CultureInfo invariant;

		protected override void Store()
		{
			CultureInfoTestCase test = new CultureInfoTestCase();
			test.frfr = new CultureInfo("fr-FR");
			test.invariant = CultureInfo.InvariantCulture;
			
			Db().Set(test);
		}
		
		public void TestRetrieveCultureInfo()
		{
			CultureInfoTestCase test = (CultureInfoTestCase) RetrieveOnlyInstance(typeof(CultureInfoTestCase));
			Assert.IsNotNull(test);
			
			Assert.IsNotNull(test.frfr);
			Assert.AreEqual("fr-FR", test.frfr.Name);
			Assert.AreEqual("fr", test.frfr.TwoLetterISOLanguageName);
			
			Assert.AreEqual(CultureInfo.InvariantCulture, test.invariant);
		}
	}
}
