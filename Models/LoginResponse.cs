using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class LoginResponse
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
