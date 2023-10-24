using System;
using System.Runtime.CompilerServices;

namespace FanControl.AcerAspireTC895;

public struct BytePercentage
{
    public byte ByteValue { get;  private set; }

    public float PercentageValue
    {
        readonly get => (float) ByteValue / byte.MaxValue * 100;
        set => ByteValue = Convert.ToByte(byte.MaxValue * value.Clamp(0, byte.MaxValue) / 100);
    }

    public static implicit operator byte(BytePercentage value) => value.ByteValue;
    public static implicit operator BytePercentage(byte value) => new() {ByteValue = value};
    public static implicit operator float(BytePercentage value) => value.PercentageValue;
    public static implicit operator BytePercentage(float value) => new() {PercentageValue = value};
}

file static class MathExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T value, T min, T max)
        where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable =>
        value.CompareTo(min) < 0
            ? min
            : value.CompareTo(max) > 0
                ? max
                : value;
}