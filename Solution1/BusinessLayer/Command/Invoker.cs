﻿using System.Threading.Tasks;
using BusinessLayer.Command.Abstract;

namespace BusinessLayer.Command
{
    public class Invoker : IInvoker
    {
        public Task<T> RunAsync<T>(ICommand<T> command)
        {
            return command.ExecuteAsync();
        }
    }
}