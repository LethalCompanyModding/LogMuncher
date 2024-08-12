namespace LogMuncher.Muncher;

internal class LogLevel(string value)
{
    readonly string _value = value;
    public const float BoringPenalty = 5f;

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
            /*=================================================================
                Goals of these specific weights:

                *message, info and debug will be dropped almost all the time

                *warning will be dropped if it contains no matches because
                warnings are valid in undesired states that the code can
                recover from

                *error and fatal will never be dropped but sorted by matches
                or lack thereof

            =================================================================*/
            "message" => 0f,
            "info" => 0f,
            "debug" => 0f,
            "warning" => 5f,
            "error" => 10f,
            "fatal" => 20f,
            _ => 0f,
        };
    }

    public override string ToString()
    {
        return _value.Trim();
    }
}
