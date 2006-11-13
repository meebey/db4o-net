using System;

namespace Db4objects.Db4o.Tutorial.F1.Chapter4
{   
    public class TemperatureSensorReadout : SensorReadout
    {
        double _temperature;

        public TemperatureSensorReadout(DateTime time, Car car, string description, double temperature)
            : base(time, car, description)
        {
            _temperature = temperature;
        }
        
        public double Temperature
        {
            get
            {
                return _temperature;
            }
        }
        
        override public string ToString()
        {
            return string.Format("{0} temp: {1}", base.ToString(), _temperature);
        }
    }
}
