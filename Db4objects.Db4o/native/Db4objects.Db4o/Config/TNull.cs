﻿/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o {

	/// <exclude />
    public class TNull : IObjectTranslator {

        public void OnActivate(IObjectContainer objectContainer, object obj, object members){
        }

        public Object OnStore(IObjectContainer objectContainer, object obj){
            return null;
        }

        public System.Type StoredClass(){
            return typeof(object);
        }
    }
}