using AutoMapper;
using MovieBooking.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Test.Setups
{
    public static class Setup
    {
        public static IMapper SetupAutoMapper()
        {
            var applicationMapping = new ApplicationMapper();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(applicationMapping));
            return new Mapper(configuration);
        }
    }
}
