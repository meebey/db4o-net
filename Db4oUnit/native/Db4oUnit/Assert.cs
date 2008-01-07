using System;

namespace Db4oUnit
{
	public delegate void CodeBlock();

	public partial class Assert
	{
		public static void Expect(System.Type exception, CodeBlock block)
		{
			Assert.Expect(exception, new DelegateCodeBlock(block));
		}

		private class DelegateCodeBlock : ICodeBlock
		{
			private readonly CodeBlock _block;

			public DelegateCodeBlock(CodeBlock block)
			{
				_block = block;
			}

			public void Run()
			{
				_block();
			}
		}
	}
}
