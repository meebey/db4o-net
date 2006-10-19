/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang {

#if CF_1_0
    /// <summary>
    /// holds a pair of short and long assembly name to help Type.ForName()<br />
    /// Instances of this class are stored to the db4o database files.
    /// </summary>
    /// 
    public class AssemblyNameHint {

        public String shortName;
        private String longName;

        public AssemblyNameHint() {
        }

        public AssemblyNameHint(String shortName, String longName) {
            this.shortName = shortName;
            this.longName = longName;
        }

		public string LongName	{
			get {
				return longName;
			}
			set {
				if (null == value && null != longName) 	{
					throw new ArgumentNullException("LongName");
				}
				longName = value;
			}
		}
    }
#endif
}

