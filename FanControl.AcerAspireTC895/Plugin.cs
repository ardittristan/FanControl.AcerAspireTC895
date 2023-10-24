using System;
using System.IO;
using System.Threading;
using FanControl.Plugins;
using StagWare.FanControl.Plugins;

namespace FanControl.AcerAspireTC895;

// ReSharper disable once ClassNeverInstantiated.Global
public class Plugin : IPlugin
{
    public string Name => "Acer Aspire TC-895 Fan";

    public void Initialize() =>
        ec = LoadEC();

    public void Load(IPluginSensorsContainer container) =>
        container.ControlSensors.Add(new FanControlSensor());

    public void Close() =>
        ec = null;

    static Plugin()
    {
        NbfcResolver.Setup(AppDomain.CurrentDomain);

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            if (ec == null)
                return;
            try
            {
                ec.ReleaseLock();
                ec.Dispose();
            }
            catch
            {
                // ignored
            }
        };
    }

    // ReSharper disable once InconsistentNaming
    private static IEmbeddedController? ec;

    // ReSharper disable once InconsistentNaming ParameterHidesMember
    internal static byte ECRead(byte register, IEmbeddedController? ec = null, bool verbose = false)
    {
        byte b = 0;

        if (ec == null)
            AccessEcSynchronized(cb);
        else
            AccessEcSynchronized(cb, ec);

        return b;

        // ReSharper disable once InconsistentNaming ParameterHidesMember VariableHidesOuterVariable
        void cb(IEmbeddedController ec)
        {
            b = ec.ReadByte(register);
            if (verbose)
                Console.WriteLine($"{b} (0x{b:X2})");
        }
    }

    // ReSharper disable once InconsistentNaming ParameterHidesMember
    internal static void ECWrite(byte register, byte value, IEmbeddedController? ec = null)
    {
        if (ec == null)
            AccessEcSynchronized(cb);
        else
            AccessEcSynchronized(cb, ec);
        return;

        // ReSharper disable once InconsistentNaming ParameterHidesMember VariableHidesOuterVariable
        void cb(IEmbeddedController ec)
        {
            ec.WriteByte(register, value);
        }
    }

    // ReSharper disable once InconsistentNaming
    private static IEmbeddedController? LoadEC()
    {
        FanControlPluginLoader<IEmbeddedController> ecLoader = new(Path.Combine(NbfcResolver.Path, "Plugins"));

        if (ecLoader.FanControlPlugin == null)
        {
            Console.Error.WriteLine("Could not load EC plugin. Try to run with elevated privileges.");
            return null;
        }

        ecLoader.FanControlPlugin.Initialize();

        if (ecLoader.FanControlPlugin.IsInitialized)
            return ecLoader.FanControlPlugin;

        Console.Error.WriteLine("EC initialization failed. Try to run with elevated privileges.");
        ecLoader.FanControlPlugin.Dispose();

        return null;
    }

    private static void AccessEcSynchronized(Action<IEmbeddedController> callback)
    {
        if (ec != null)
            AccessEcSynchronized(callback, ec);
        else
            using (ec = LoadEC())
                if (ec != null)
                    AccessEcSynchronized(callback, ec);
    }

    // ReSharper disable once ParameterHidesMember
    private static void AccessEcSynchronized(Action<IEmbeddedController> callback, IEmbeddedController ec)
    {
        if (ec.AcquireLock(200))
            try
            {
                callback(ec);
            }
            finally
            {
                ec.ReleaseLock();
            }
        else
            Console.Error.WriteLine("Could not acquire EC lock");
    }

    public static void Test()
    {
#if DEBUG
        using (ec = LoadEC())
        {
            if (ec == null)
                return;

            Console.WriteLine("reading");

            while (true)
            {
                ECRead(0xC0, ec, true);
                ECWrite(0xC0, 0x01, ec);
                ECRead(0xC0, ec, true);
                Thread.Sleep(10);
            }
        }
#endif
    }
}