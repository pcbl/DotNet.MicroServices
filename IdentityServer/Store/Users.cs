using IdentityModel;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Store
{
    internal class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser> {
            new TestUser {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "admin",
                Password = "test",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "admin@test.com"),
                    new Claim(JwtClaimTypes.Role, "admin")
                }
            }
        };
        }
    }
}
