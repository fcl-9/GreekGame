namespace GreekGame.API.BackgroundServices;

internal class InvalidEventTypeException : Exception
{
    public InvalidEventTypeException(string failedToParseRandomEventType)
    {
    }
}
