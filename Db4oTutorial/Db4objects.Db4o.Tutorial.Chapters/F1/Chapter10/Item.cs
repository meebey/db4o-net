/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4odoc.Tutorial.F1.Chapter11
{
    public class Item
    {
        private static readonly string Load = "LOAD__________________________";

        private static readonly string Update = "ccccc";

        public string _string;

        public Item(string str)
        {
            _string = str;
        }

        public static object NewItem(int i)
        {
            return new Db4objects.Db4o.Bench.Crud.Item(Load + i);
        }

        public virtual void Change()
        {
            _string = _string + Update;
        }
    }
}
