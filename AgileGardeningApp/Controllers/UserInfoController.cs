using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgileGardeningApp.ServiceRepository.Interface;

namespace AgileGardeningApp.Controllers
{
    [RoutePrefix("api/v1")]
    public class UserInfoController : ApiController
    {
        private readonly IUserInfoService _userInfoService;

        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        public class LoginInput
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [Route("Login")]
        [HttpPost]
        public HttpResponseMessage LoginUser([FromBody] LoginInput input)
        {
            try
            {
                var loginToken = _userInfoService.LoginUser(input.Email, input.Password);
                return Request.CreateResponse(HttpStatusCode.OK, new { token = loginToken });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { ex_message = ex.Message, ex_stack = ex.StackTrace, ex_inner_ex = ex.InnerException?.Message });
            }
        }
    }
}
