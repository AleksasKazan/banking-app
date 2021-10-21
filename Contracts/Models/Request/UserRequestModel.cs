using System;
using System.Text.Json.Serialization;

namespace Contracts.Models.Request
{
    public class UserRequestModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken => true;
    }

    public class UserSignUpRequestModel : UserRequestModel
    {
        public string UserName { get; set; }
    }

    public class UserSignInRequestModel : UserRequestModel
    {
    }

    public class UserDeleteRequestModel
    {
        public string IdToken { get; set; }
    }

    public class PasswordResetRequestModel
    {
        public string Email { get; set; }

        public string RequestType => "PASSWORD_RESET";
    }

    public class ChangeEmailRequestModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string IdToken { get; set; }

        public string UserName { get; set; }

    }

    public class ChangePasswordRequestModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string IdToken { get; set; }

        public string UserName { get; set; }

    }
}
