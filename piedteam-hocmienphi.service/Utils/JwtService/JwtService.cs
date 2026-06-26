using System.Security.Claims;

namespace piedteam_hocmienphi.service.Utils.JwtService;

public class JwtService
{
    public static string GenerateToken(IEnumerable<Claim> claims, JwtOptions options)
    {
        //claim đại diện payload trong jwt
        return "";
    }
}