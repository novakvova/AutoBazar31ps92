using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBazar.Entities
{
    public sealed class UserRefreshToken 
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long UserId { get; set; }
        public DbUser User { get; set; }
    }
}
