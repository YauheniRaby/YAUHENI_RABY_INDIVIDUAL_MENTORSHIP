using System.Threading.Tasks;
using ConsoleApp.Configuration;
using ConsoleApp.Configuration.Abstract;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Ninject;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main()
        {
            var configRoot = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            var configuration = new FileConfig() { AppConfiguration = new AppConfiguration() };
            configRoot.Bind(configuration);

            await StartUserCommunication(GetRegistrarDependencies(configuration));
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
            ninjectKernel.AddValidators(config);
            ninjectKernel.Bind<IConfig>().ToConstant(config);

            return ninjectKernel;
        }
    }
}