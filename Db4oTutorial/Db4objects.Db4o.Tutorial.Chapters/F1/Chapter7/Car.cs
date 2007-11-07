using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{

public class Car : IActivatable 
{
    private readonly String _model;
    private Pilot _pilot;
    private SensorReadout _history;
    
    [Transient]
    private IActivator _activator;

    public Car(String model) 
    {
        this._model=model;
        this._pilot=null;
        this._history=null;
    }

    public Pilot Pilot
    {
        get
        {
            Activate();
            return _pilot;
        }

        set
        {
            Activate();
            this._pilot = value;
        }
    }
    
    public Pilot PilotWithoutActivation
    {
        get { return _pilot; }
    }

    public String Model
    {
        get
        {
            Activate();
            return _model;
        }
    }
    
    public SensorReadout History
    {
        get
        {
            Activate();
            return _history;
        }
    }
    
    public void snapshot() 
    {
        Activate();
        AppendToHistory(new TemperatureSensorReadout(DateTime.Now,this,"oil", PollOilTemperature()));
        AppendToHistory(new TemperatureSensorReadout(DateTime.Now, this, "water", PollWaterTemperature()));
        AppendToHistory(new PressureSensorReadout(DateTime.Now, this, "oil", PollOilPressure()));
    }

    protected double PollOilTemperature() 
    {
        Activate();
        return 0.1* CountHistoryElements();
    }

    protected double PollWaterTemperature() 
    {
        Activate();
        return 0.2* CountHistoryElements();
    }

    protected double PollOilPressure() 
    {
        return 0.3* CountHistoryElements();
    }

    public override String ToString() 
    {
        Activate();
        return string.Format("{0}[{1}]/{2}", _model, _pilot, CountHistoryElements());
    }
    
    private int CountHistoryElements() 
    {
        Activate();
        return (_history==null ? 0 : _history.CountElements());
    }
    
    private void AppendToHistory(SensorReadout readout) 
    {
        Activate();
        if(_history==null) 
        {
            _history=readout;
        }
        else 
        {
            _history.Append(readout);
        }
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