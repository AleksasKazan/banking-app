using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Models.Request;
using Domain.Clients.Firebase.Models;
using Domain.Clients.Firebase.Options;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Domain.Clients.Firebase
{
    public class FireBaseClient : IFireBaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseOptions _firebaseOptions;

        public FireBaseClient(HttpClient httpClient, IOptions<FirebaseOptions> firebaseOptions)
        {
            _httpClient = httpClient;
            _firebaseOptions = firebaseOptions.Value;

        }

        public async Task<SignUpResponse> SignUp(UserSignUpRequestModel user)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:signUp?key={_firebaseOptions.ApiKey}";
            var post = new SignUpRequest
            {
                Email = user.Email,
                Password = user.Password
            };

            var response = await _httpClient.PostAsJsonAsync(url, post);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SignUpResponse>();
            }

            var firebaseError = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            throw new FirebaseException(firebaseError.Error.Message, firebaseError.Error.StatusCode);
        }

        public async Task<SignInResponse> SignIn(UserSignInRequestModel user)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:signInWithPassword?key={_firebaseOptions.ApiKey}";
            var post = new SignInRequest
            {
                Email = user.Email,
                Password = user.Password,
            };

            var response = await _httpClient.PostAsJsonAsync(url, post);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SignInResponse>();
            }

            var firebaseError = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            throw new FirebaseException(firebaseError.Error.Message, firebaseError.Error.StatusCode);
        }

        public async Task<ErrorResponse> DeleteAccount(string idToken)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:delete?key={_firebaseOptions.ApiKey}";
            var post = new DeleteRequest
            {
                IdToken = idToken
            };
            var postJson = JsonSerializer.Serialize(post);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(postJson, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<ErrorResponse>();

        }

        public async Task<PasswordResetResponse> PasswordReset(PasswordResetRequestModel user)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:sendOobCode?key={_firebaseOptions.ApiKey}";
            var post = new ResetPasswordRequest
            {
                Email = user.Email,
                //requestType = "PASSWORD_RESET"
                RequestType = user.RequestType
            };
            var postJson = JsonSerializer.Serialize(post);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(postJson, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<PasswordResetResponse>();

        }

        public async Task<ChangeEmailResponse> ChangeEmail(string email, string idToken)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:update?key={_firebaseOptions.ApiKey}";
            var post = new ChangeEmailRequest
            {
                IdToken = idToken,
                Email = email,
            };
            var postJson = JsonSerializer.Serialize(post);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(postJson, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<ChangeEmailResponse>();

        }

        public async Task<ChangePasswordResponse> ChangePassword(string newPassword, string idToken)
        {
            var url = $"{_firebaseOptions.BaseAddress}v1/accounts:update?key={_firebaseOptions.ApiKey}";
            var post = new ChangePasswordRequest
            {
                Password = newPassword,
                IdToken = idToken,
            };
            var postJson = JsonSerializer.Serialize(post);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_httpClient.BaseAddress, url),
                Content = new StringContent(postJson, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<ChangePasswordResponse>();

        }
    }
}
