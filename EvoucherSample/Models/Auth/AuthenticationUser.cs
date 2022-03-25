using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EvoucherSample.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace EvoucherSample.Models.Auth
{
    public class AuthenticationUser : IdentityUser
    {
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
