using FTM.API.Helpers;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        public DummyController() { }

        [HttpGet("/")]
        [AllowAnonymous]
        public async Task<String> GetDummy()
        {
            return "This is dummy api using get method";
        }

        [HttpGet("/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<String> GetDummyAdmin()
        {
            return "This is dummy Admin api using get method";
        }

        [HttpGet("/user")]
        [Authorize(Roles = "User")]
        public async Task<String> GetDummyUser()
        {
            return "This is dummy User api using get method";
        }

        [HttpGet("/authorization/member/view")]
        [FTAuthorize(MethodType.VIEW, FeatureType.MEMBER)]
        public async Task<String> GetDummyAuthorizationView()
        {
            return "This is dummy Authorization API Member - Member View";
        }

        [HttpGet("/authorization/fund/view")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<String> GetDummyAuthorizationFundView()
        {
            return "This is dummy Authorization API Member - Fund View";
        }

        [HttpGet("/authorization/owner")]
        [FTAuthorizeOwner]
        public async Task<String> GetDummyOwnerAuthorization()
        {
            return "This is dummy Authorization API Owner";
        }
    }
}
