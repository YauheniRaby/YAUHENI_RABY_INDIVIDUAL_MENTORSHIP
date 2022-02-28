using System.Collections.Generic;
using ConsoleApp.Command;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Services.Abstract;

namespace ConsoleApp.Extensions
{
    public static class CommandExtensions
    {
        public static void FillCommands(this IList<ICommand> commands, IUserCommunicateService userCommunicateService)
        {
            commands.Add(new CurrentlyWeatherCommand(userCommunicateService));
            commands.Add(new ExitCommand());
            commands.Add(new ExitCommand());
        }
    }
}
