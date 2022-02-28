﻿using System;
using System.Threading.Tasks;
using ConsoleApp.Command.Abstract;

namespace ConsoleApp.Command
{
    public class ExitCommand : ICommand
    {
        public Task Execute()
        {
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }
}
