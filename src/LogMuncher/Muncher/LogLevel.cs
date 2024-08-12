namespace LogMuncher.Muncher;

internal class LogLevel(string value)
{
    readonly string _value = value;

    public static implicit operator string(LogLevel @in)
    {
        return @in._value;
    }

    public static implicit operator LogLevel(string @in)
    {
        return new LogLevel(@in);
    }

    public float GetLogWeight()
    {
        return _value.ToLowerInvariant() switch
        {
            "message" => 0f,
            "info" => 0f,
            "debug" => 1f,
            "warning" => 5f,
            "error" => 10f,
            "fatal" => 15f,
            _ => 0f,
        };
    }

    public override string ToString()
    {
        return _value.Trim();
    }
}