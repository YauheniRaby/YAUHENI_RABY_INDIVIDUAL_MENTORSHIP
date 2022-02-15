﻿using BusinessLayer.DTOs;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IWeatherServise
    {
        string GetByCityName(string CityName);
    }
}
