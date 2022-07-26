using JetBrains.Annotations;

namespace JsonApiDotNetCore.Configuration;

[PublicAPI]
public sealed class PageNumber : IEquatable<PageNumber>
{
    public static readonly PageNumber ValueZero = new(0);

    public int ZeroBasedValue { get; }

    public PageNumber(int zeroBasedValue)
    {
        if (zeroBasedValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(zeroBasedValue));
        }

        ZeroBasedValue = zeroBasedValue;
    }

    public bool Equals(PageNumber? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ZeroBasedValue == other.ZeroBasedValue;
    }

    public override bool Equals(object? other)
    {
        return Equals(other as PageNumber);
    }

    public override int GetHashCode()
    {
        return ZeroBasedValue.GetHashCode();
    }

    public override string ToString()
    {
        return ZeroBasedValue.ToString();
    }
}
