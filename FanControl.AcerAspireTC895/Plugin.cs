using FanControl.Plugins;
using LibreHardwareMonitor.Hardware;

namespace FanControl.AcerAspireTC895;

// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : IPlugin
{
    public string Name => "Acer Aspire TC-895 Fan";

    public void Initialize() =>
        (_computer ??= new Computer()).Open();

    public void Load(IPluginSensorsContainer container) =>
        container.ControlSensors.Add(new FanControlSensor());

    public void Close() => _computer?.Close();

    private Computer? _computer;

    // ReSharper disable once InconsistentNaming
    internal static byte ECRead(byte register, bool verbose = false)
    {
        using WinEC ec = new();

        byte b = ec.ReadByte(register);

        if (verbose)
            Console.WriteLine($"{b} (0x{b:X2}) | {register}");

        return b;
    }

    // ReSharper disable once InconsistentNaming
    internal static void ECWrite(byte register, byte value)
    {
        using WinEC ec = new();

        ec.WriteByte(register, value);
    }

    public static void Test()
    {
#if DEBUG
        (new Computer()).Open();

        Console.WriteLine("reading");

        while (true)
        {
            ECRead(0xC0, true);
            ECWrite(0xC0, 0x01);
            ECRead(0xC0, true);
            ECRead(FanControlSensor.ReadAddress, true);
            Thread.Sleep(10);
        }
#endif
    }
}