using System.Text.Json.Serialization;

namespace Domain.Clients.Firebase.Models
{
    public class SignInResponse
    {
        public string IdToken { get; set; }

        public string Email { get; set; }

        [JsonPropertyName("localId")]
        public string FirebaseId { get; set; }
    }

    public class SignUpResponse : ErrorResponse
    {
        public string IdToken { get; set; }

        public string Email { get; set; }

        [JsonPropertyName("localId")]
        public string FirebaseId { get; set; }
    }

    public class PasswordResetResponse
    {
        public string Email { get; set; }
    }

    public class ChangeEmailResponse : SignInResponse
    {
    }

    public class ChangePasswordResponse : SignInResponse
    {
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }
    }
}
