using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBazar.BLL.DTO
{
    public class TokensDTO
    {
        public string Token { get; }
        public string RefreshToken { get; }
        public TokensDTO(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
