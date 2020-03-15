using AutoBazar.BLL.DTO;
using AutoBazar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBazar.BLL.Abstract
{
    public interface IJWTTokenService
    {
        string CreateToken(DbUser user);
        string CreateRefreshToken(DbUser user);
        Task<TokensDTO> RefreshAuthToken(string oldAuthToken, string refreshToken);
    }
}
