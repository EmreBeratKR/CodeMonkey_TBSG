namespace CommandSystem
{
    public enum CommandStatus
    {
        Ok,
        NotFound = 404,
        NotSelected,
        BadSelection,
        NotEnoughCommandPoint,
        Blocked,
        InvalidPath,
        TooFarAway,
        TargetNotFound,
        FriendlyFire
    }


    public static class CommandStatusExtensions
    {
        public static string AsString(this CommandStatus commandStatus)
        {
            return commandStatus switch
            {
                CommandStatus.NotFound => "Bad\nSelection!",
                CommandStatus.NotSelected => "No\nCommand!",
                CommandStatus.BadSelection => "Bad\nSelection"!,
                CommandStatus.NotEnoughCommandPoint => "Not Enough\nCommand Point!",
                CommandStatus.Blocked => "Blocked!",
                CommandStatus.InvalidPath => "Invalid\nPath!",
                CommandStatus.TooFarAway => "Too\nFar Away!",
                CommandStatus.TargetNotFound => "There is\nNo Target!",
                CommandStatus.FriendlyFire => "Friendly\nFire!",
                _ => "OK!"
            };
        }
    }
}