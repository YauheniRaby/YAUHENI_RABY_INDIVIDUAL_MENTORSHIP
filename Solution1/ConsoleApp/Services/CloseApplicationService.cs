using ConsoleApp.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Services
{
    public class CloseApplicationService : ICloseApplicationService
    {
        public Task<bool> Exit()
        {
            return Task.FromResult(false);
        }
    }
}
