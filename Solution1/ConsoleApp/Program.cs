using System.Threading.Tasks;
using BusinessLayer.Configuration.Abstract;
using ConsoleApp.Configuration;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using Ninject;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main()
        {
            await StartUserCommunication(GetRegistrarDependencies(new FileConfig()));
        }

        public static async Task StartUserCommunication(IKernel ninjectKernel)
        {
            var userCommunicationService = ninjectKernel.Get<IUserCommunicateService>();
            while (await userCommunicationService.CommunicateAsync())
            {
            }

            return;
        }

        public static StandardKernel GetRegistrarDependencies(IConfig config)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();
            ninjectKernel.AddValidators();
            ninjectKernel.Bind<IConfig>().ToConstant(config);

            return ninjectKernel;
        }
    }
}