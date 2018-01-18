namespace CK.Glouton.Service.Common
{
    [System.Flags]
    public enum Operation
    {
        EqualTo = 1 << 0,
        NotEqualTo = 1 << 1,
        In = 1 << 2,
        Contains = 1 << 3,
        StartsWith = 1 << 4,
        EndsWith = 1 << 5,
        GreaterThan = 1 << 6,
        GreaterThanOrEqualTo = 1 << 7,
        LessThan = 1 << 8,
        LessThanOrEqualTo = 1 << 9,
        IsNotNull = 1 << 10
    }
}