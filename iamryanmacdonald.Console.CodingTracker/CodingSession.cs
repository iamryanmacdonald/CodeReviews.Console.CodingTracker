namespace iamryanmacdonald.Console.CodingTracker;

internal class CodingSession
{
    internal int Duration { get; set; }
    internal DateTime EndTime { get; set; }
    internal int Id { get; set; }
    internal DateTime StartTime { get; set; }

    internal static int CalculateDuration(DateTime startTime, DateTime endTime)
    {
        return (int)(endTime - startTime).TotalSeconds;
    }
}