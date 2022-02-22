using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Extensions;
using ConsoleApp.Configuration;
using ConsoleApp.Extension;
using ConsoleApp.Service.Abstract;
using Microsoft.Extensions.Logging;
using Ninject;
using static ConsoleApp.Constants;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();

            var userCommunicationService = ninjectKernel.Get<IUserCommunicationService>();

            while (true)
            {
                await userCommunicationService.Communication();
            }
        }
    }
}
