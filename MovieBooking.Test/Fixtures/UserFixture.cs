using Moq;
using MovieBooking.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBooking.Test.Fixtures
{
    public static class UserFixture
    {
        [ExcludeFromCodeCoverage]
        public static UserDto ReturnUser()
        {
            return new()
            {
                FirstName = "Rishabh",
                LastName = "Sinha",
                Email = "rishabh@gmail.com",
                LoginId = "Rishabh",
                Password = "Pass",
                Contact = "9123412212"
            };
        }

        [ExcludeFromCodeCoverage]
        public static Tuple<string, string> ReturnLoginIdPassword()
        {
            return Tuple.Create<string, string>("Mohan", "WelcomeFSE1");
        }
    }
}
