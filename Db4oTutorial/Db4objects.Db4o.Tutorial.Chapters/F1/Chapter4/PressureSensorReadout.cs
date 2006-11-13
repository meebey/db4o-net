using System;

namespace Db4objects.Db4o.Tutorial.F1.Chapter4
{
    public class PressureSensorReadout : SensorReadout
    {
        double _pressure;
        
        public PressureSensorReadout(DateTime time, Car car, string description, double pressure)
            : base(time, car, description)
        {
            _pressure = pressure;
        }
        
        public double Pressure
        {
            get
            {
                return _pressure;
            }
        }
        
        override public string ToString()
        {
            return string.Format("{0} pressure: {1}", base.ToString(), _pressure);
        }
    }
}
