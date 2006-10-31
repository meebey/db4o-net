/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Drawing;
using Db4objects.Db4o.Config;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
#if !CF_1_0 && !CF_2_0
	/// <summary>
	/// Summary description for CsImage.
	/// </summary>
	public class CsImage : AbstractDb4oTestCase
	{

        private static String IMAGE_PATH = "C:\\CO.jpg";

        Image image;
        Bitmap bitmap;

        protected override void  Configure(Db4objects.Db4o.Config.IConfiguration config)
        {
            base.Configure(config);
            config.ObjectClass(typeof(Image)).Translate(new TNull());
            config.ObjectClass(typeof(Bitmap)).Translate(new TSerializable());
        }

//        ovpublic void StoreOne(){
//            image = Image.FromFile(IMAGE_PATH);
//            bitmap = (Bitmap)Image.FromFile(IMAGE_PATH);
//        }

        public void _Test()
        {
            // TODO: implement this one
        }

	}
#endif
}
