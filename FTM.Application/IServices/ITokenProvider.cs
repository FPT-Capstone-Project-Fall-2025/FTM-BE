using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface ITokenProvider
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        string GenerateThirdPartyJwtToken(IEnumerable<Claim> claims, DateTime expires);
        string GetIssuer();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
