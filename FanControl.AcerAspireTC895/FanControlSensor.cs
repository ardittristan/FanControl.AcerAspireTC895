using FanControl.Plugins;

namespace FanControl.AcerAspireTC895;

public class FanControlSensor : IPluginControlSensor
{
    public const byte ReadAddress = 0x2C;
    public const byte WriteAddress = 0xC0;
    public const int ApplyInterval = 50;

    public string Id => "AcerAspireTC895FanControlSensor";
    public string Name => "Acer Aspire TC-895 Fan Control Sensor";
    public float? Value => _value;
    private BytePercentage? _value;

    private bool _running;
    private BytePercentage _setLevel = 0x01;

    private void Run()
    {
        _running = true;
        new Thread(Action).Start();
        return;

        void Action()
        {
            while (_running)
            {
                Plugin.ECWrite(WriteAddress, _setLevel);
                Thread.Sleep(ApplyInterval);
            }
        }
    }

    public void Set(float val)
    {
        _setLevel = val;
        if (!_running)
            Run();
    }

    public void Update() =>
        _value = Plugin.ECRead(ReadAddress);

    public void Reset() => _running = false;
}