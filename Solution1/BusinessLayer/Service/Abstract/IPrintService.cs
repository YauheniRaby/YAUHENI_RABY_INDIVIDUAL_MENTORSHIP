using BusinessLayer.DTOs;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service.Abstract
{
    public interface IPrintService
    {
        string Print(WeatherDTO weatherDTO);
    }
}
