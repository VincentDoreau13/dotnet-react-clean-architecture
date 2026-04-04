using System.Reflection;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Domain.Common;

public abstract class Enumeration(int id, string name) : IComparable
{
    public string Name { get; } = name;

    public int Id { get; } = id;

    public int CompareTo(object? obj)
    {
        return obj is Enumeration other ? Id.CompareTo(other.Id) : 1;
    }

    public override string ToString()
    {
        return Name;
    }

    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
        where TEnumeration : Enumeration
    {
        return typeof(TEnumeration).GetFields(BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Select(field => field.GetValue(null))
            .Cast<TEnumeration>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        bool typeMatches = GetType() == obj.GetType();
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        return Math.Abs(firstValue.Id - secondValue.Id);
    }

    public static TEnumeration FromValue<TEnumeration>(int value)
        where TEnumeration : Enumeration
    {
        return Parse<TEnumeration, int>(value, "value", enumeration => enumeration.Id == value);
    }

    public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
        where TEnumeration : Enumeration
    {
        return Parse<TEnumeration, string>(displayName, "display name", enumeration => enumeration.Name == displayName);
    }

    private static TEnumeration Parse<TEnumeration, TValue>(TValue value, string description, Func<TEnumeration, bool> predicate)
        where TEnumeration : Enumeration
    {
        TEnumeration? matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

        if (matchingItem is null)
            throw new FunctionalException($"'{value}' is not a valid {description} in {typeof(TEnumeration).Name}");

        return matchingItem;
    }

    public static bool operator ==(Enumeration left, Enumeration right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Enumeration left, Enumeration right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration left, Enumeration right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumeration left, Enumeration right)
    {
        return left?.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}
