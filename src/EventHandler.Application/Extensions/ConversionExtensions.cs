namespace EventHandler.Application.Extensions
{
    public static class ConversionExtensions
    {
        public static bool IsSafeToConvertIntToUShort(this int valueToConvert) =>
            valueToConvert <= ushort.MaxValue && valueToConvert >= 0;
    }
}
