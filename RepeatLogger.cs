/*
namespace LogMuncher;

internal static class RepeatLogger
{
    private static int Repeats = 0;
    private static int LastInHash = 0;

    public static void WriteLogLine(object Message)
    {

        //Check if this is a repeat message
        int TempHash = Message.GetHashCode();

        if (TempHash == LastInHash)
        {
            //Adjust the message and reset the cursor to the previous line
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
            Repeats++;
            Message = $"{Message} x{Repeats}";
        }
        else
        {
            Repeats = 0;
            LastInHash = TempHash;
        }

        //Write the message at the current cursor
        Console.WriteLine(Message);
    }
}
*/
