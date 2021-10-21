using System;
using System.Text.Json.Serialization;

namespace Domain.Clients.Firebase.Models
{
    public class SignInRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken => true;
    }

    public class SignUpRequest : SignInRequest
    {
    }

    public class DeleteRequest
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }

    public class ResetPasswordRequest
    {
        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class ChangeEmailRequest
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}

