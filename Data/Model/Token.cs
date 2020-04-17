
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Model
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public long ExpireToken { get; set; }
        public string RefreshToken { get; set; }
        public long ExpireRefreshToken { get; set; }
        public IdentityUser Username { get; set; }
    }
}
