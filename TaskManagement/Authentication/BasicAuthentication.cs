using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;
        public BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> optionsMonitor,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            ISystemClock clock,
            ApplicationDbContext context) : base(optionsMonitor, loggerFactory, urlEncoder, clock)
        {
            _context = context;
        }

        
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the request has an Authorization header
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized"); // Return 401
            }

            // Store the Authorization header as a string
            string header = Request.Headers["Authorization"].ToString();

            // Check if the header is empty or does not start with "Basic"
            if(string.IsNullOrEmpty(header) || !header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized"); // Return 401
            }

            // Remove "Basic " to obtain the token
            string token = header.Substring(6);

            // Decode token from Base64 
            string tokenDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            // Split the decoded token into username and password
            string[] credentials = tokenDecoded.Split(':');

            if (credentials?.Length != 2)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string username = credentials[0];
            string password = credentials[1];

            // Find the user with the given username in the db
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            
            if (user == null)
            {
                return AuthenticateResult.Fail("Unauthorized"); // Return 401
            }

            // Check if the password matches the hashed password in the db
            var hasher = new PasswordHasher<User>();
            var hasherVerified = hasher.VerifyHashedPassword(user, user.Password, password);

            if(hasherVerified != PasswordVerificationResult.Success)
            {
                return AuthenticateResult.Fail("Unauthorized"); // Return 401
            }

            // Creat claims for the user
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            AuthenticationTicket ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
            



        }
    }
}
