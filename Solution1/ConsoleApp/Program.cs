using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Command;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using Ninject;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main()
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();

            var userCommunicationService = ninjectKernel.Get<IUserCommunicateService>();

            while (true)
            {
               await userCommunicationService.MenuAsync();
            }
        }
    }
}
