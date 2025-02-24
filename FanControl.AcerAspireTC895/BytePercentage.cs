namespace FanControl.AcerAspireTC895;

public struct BytePercentage
{
    public byte ByteValue { get; private set; }

    public float PercentageValue
    {
        readonly get => (float) ByteValue / byte.MaxValue * 100;
        set => ByteValue = Convert.ToByte(byte.MaxValue * Math.Clamp(value, 0, byte.MaxValue) / 100);
    }

    public static implicit operator byte(BytePercentage value) => value.ByteValue;
    public static implicit operator BytePercentage(byte value) => new() {ByteValue = value};
    public static implicit operator float(BytePercentage value) => value.PercentageValue;
    public static implicit operator BytePercentage(float value) => new() {PercentageValue = value};
}

#if !NETCOREAPP2_0_OR_GREATER
file static class Math
{
    public static float Clamp(float value, float min, float max) =>
        min > max
            ? throw new ArgumentException($"'{min}' cannot be greater than {max}.")
            : value < min
                ? min
                : value > max
                    ? max
                    : value;
}
#endif