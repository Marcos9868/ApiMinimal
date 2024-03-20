using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMinimal.Models;

namespace ApiMinimal.Services
{
    public interface ITokenService
    {
        string GenerateToken(string Key, string Issuer, string Audience, User user);
    }
}