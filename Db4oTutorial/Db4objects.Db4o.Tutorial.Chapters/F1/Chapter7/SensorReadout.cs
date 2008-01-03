using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{
    public class SensorReadout : IActivatable
    {
        private readonly DateTime _time;
        private readonly Car _car;
        private readonly String _description;
        private SensorReadout _next;

        [Transient] 
        private IActivator _activator;

        protected SensorReadout(DateTime time, Car car, String description)
        {
            this._time = time;
            this._car = car;
            this._description = description;
            this._next = null;
        }

        public Car Car
        {
            get
            {
				Activate(ActivationPurpose.Read);
                return _car;
            }
        }

        public DateTime Time
        {
            get
            {
                Activate(ActivationPurpose.Read);
                return _time;
            }
        }

        public String Description
        {
            get
            {
				Activate(ActivationPurpose.Read);
                return _description;
            }
        }

        public SensorReadout Next
        {
            get
            {
				Activate(ActivationPurpose.Read);
                return _next;
            }
        }

        public void Append(SensorReadout readout)
        {
            Activate(ActivationPurpose.Write);
            if (_next == null)
            {
                _next = readout;
            }
            else
            {
                _next.Append(readout);
            }
        }

        public int CountElements()
        {
			Activate(ActivationPurpose.Read);
            return (_next == null ? 1 : _next.CountElements() + 1);
        }

        public override String ToString()
        {
			Activate(ActivationPurpose.Read);
            return String.Format("{0} : {1} : {2}", _car, _time, _description);
        }

        public void Activate(ActivationPurpose purpose)
        {
            if (_activator != null)
            {
                _activator.Activate(purpose);
            }
        }

        public void Bind(IActivator activator)
        {
            if (_activator != null || activator == null)
            {
                throw new InvalidOperationException();
            }
            _activator = activator;
        }
    }
}