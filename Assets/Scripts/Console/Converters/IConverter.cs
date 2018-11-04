namespace IngameConsole
{
    public interface IConverter
    {
        object AttemptConversion(params string[] rawParameters);
    }
    public interface IConverter<T> : IConverter
    {
        new T AttemptConversion(params string[] rawParameters);
    }
}
