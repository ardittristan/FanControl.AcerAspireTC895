using LibreHardwareMonitor.Hardware.Motherboard.Lpc.EC;

namespace FanControl.AcerAspireTC895;

// ReSharper disable once InconsistentNaming
internal class WinEC : WindowsEmbeddedControllerIO
{
    private const int MaxRetries = 5;

    public byte ReadByte(byte register) => ReadLoop<byte>(register, ReadByteOp);

    public void WriteByte(byte register, byte value) => WriteLoop(register, value, WriteByteOp);

    private static TResult ReadLoop<TResult>(byte register, ReadOp<TResult> op) where TResult : new()
    {
        TResult result = new();

        for (int i = 0; i < MaxRetries; i++)
            if (op(register, out result))
                return result;

        return result;
    }

    private static void WriteLoop<TValue>(byte register, TValue value, WriteOp<TValue> op)
    {
        for (int i = 0; i < MaxRetries; i++)
            if (op(register, value))
                return;
    }

    private delegate bool ReadOp<TParam>(byte register, out TParam p);

    private delegate bool WriteOp<in TParam>(byte register, TParam p);
}