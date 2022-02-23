﻿using System.Threading.Tasks;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using Ninject;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();

            var userCommunicationService = ninjectKernel.Get<IUserCommunicateService>();

            while (true)
            {
                await userCommunicationService.CommunicateAsync();
            }
        }
    }
}
