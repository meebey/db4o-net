using System;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{
    public class TemperatureSensorReadout : SensorReadout
    {
        private readonly double _temperature;

        public TemperatureSensorReadout(DateTime time, Car car, string description, double temperature)
            : base(time, car, description)
        {
            this._temperature = temperature;
        }

        public double Temperature
        {
            get
            {
                Activate();
                return _temperature;
            }
        }

        public override String ToString()
        {
            Activate();
            return string.Format("{0} temp : {1}", base.ToString(), _temperature);
        }
    }
}