﻿using System.Net.Http;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using BusinessLayer.Vlidators;
using ConsoleApp.AutoMap;
using ConsoleApp.Configuration;
using ConsoleApp.Services;
using ConsoleApp.Services.Abstract;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ninject;

namespace ConsoleApp.Extensions
{
    public static class RegisterDependencies
    {
        public static void AddServices(this IKernel ninjectKernel)
        {
            ninjectKernel.Bind<IUserCommunicateService>().To<UserCommunicateService>();
            ninjectKernel.Bind<IWeatherServiсe>().To<WeatherService>();
            ninjectKernel.Bind<IWeatherApiService>().To<WeatherApiService>()
                .WithConstructorArgument("httpClient", new HttpClient());
            ninjectKernel.Bind<IMapper>().To<Mapper>()
                .WithConstructorArgument("configurationProvider", MapperConfig.GetConfiguration());
            ninjectKernel.Bind<ILogger>().ToMethod(x => LoggerConfiguration.GetConfiguration<Program>());
        }

        public static void AddValidators(this IKernel ninjectKernel)
        {
            ninjectKernel.Bind<IValidator<ForecastWeatherRequestDTO>>().To<ForecastWeatherRequestDTOValidator>();
        }
    }
}
