/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang
{
    public class ClassNotFoundException : System.Exception
    {
        public ClassNotFoundException()
        {
        }

        public ClassNotFoundException(string message) : base(message)
        {
        }
    	
    	public ClassNotFoundException(string message, Exception cause) : base(message, cause)
    	{
    	}
    }
}
