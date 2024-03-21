using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AuthentiactionWebApi.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly JwtTokenHandler _jwtTokenHandler;
        public AccountController(JwtTokenHandler jWTTokenHandler)
        {
            _jwtTokenHandler = jWTTokenHandler;
        }

        [HttpPost]
        public ActionResult<AuthenticationResponse> Authenticate([FromBody] AuthenticationRequest authenticateRequest)
        {
            var authentiactionResponse = _jwtTokenHandler.GenerateJwtToken(authenticateRequest);
            if (authentiactionResponse == null) return Unauthorized();
            return Ok(authentiactionResponse);
        
        
        }
        
    }
}
