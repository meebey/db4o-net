using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{
    public class Pilot : IActivatable
    {
        private readonly String _name;
        private int _points;
        
        [Transient]
        private IActivator _activator;

        public Pilot(String name,int points) 
        {
            this._name=name;
            this._points=points;
        }

        public int Points
        {
            get
            {
                Activate();
                return _points;
            }

            set
            {
                Activate();
                _points += value;
            }
        }

        public String Name
        {
            get
            {
                Activate();
                return _name;
            }
        }

        public override string  ToString()
        {
            Activate();
            return String.Format("{0}/{1}", _name, _points);
        }
        
        public void Activate() 
        {
            if(_activator != null) 
            {
                _activator.Activate();
            }
        }

        public void Bind(IActivator activator) 
        {
            if(_activator != null || activator == null) 
            {
                throw new InvalidOperationException();
            }
            _activator = activator;
        }
    }
}