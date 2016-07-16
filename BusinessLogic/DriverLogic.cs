using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Business_Logic.Entities;
using Business_Logic.Dtos;
using Business_Logic.Helpers;
using Newtonsoft.Json;

namespace Business_Logic
{
    public class DriverLogic : baseLogic
    {
        public List<Driver> GetList()
        {
            return DB.Drivers.ToList();
        }
    }
}
