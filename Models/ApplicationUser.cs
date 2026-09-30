using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }

        public int LoginLogId { get; internal set; }

        public int ClinicId { get; set; }

        private string _displayName;
        [StringLength(20)]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    return this.UserName;
                else return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }
    }
}
