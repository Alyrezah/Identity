namespace Identity.Core.Application
{
    public class CommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string>? ErrorMessages { get; set; }
    }

    public static class CommandMessages
    {
        public const string CommandSuccess = "The operation was successful";
        public const string CommandFailer = "The operation failed";
    }
}
