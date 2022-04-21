﻿using System;
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
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //.AddUserSecrets<Program>()
            //.Build()
            //IConfiguration config = new ConfigurationBuilder()

            var t = Environment.GetEnvironmentVariable("Key");

            var configuration = new FileConfig();
            configRoot.Bind(configuration);
            
            
            
            
            configuration.ApiConfig = new WeatherApiConfiguration();
            configuration.AppConfig = new AppConfiguration();

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