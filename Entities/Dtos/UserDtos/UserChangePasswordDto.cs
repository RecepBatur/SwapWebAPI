using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.UserDtos
{
    public class UserChangePasswordDto : IDto
    {
        //public int UserId { get; set; }
        public string CurrentPassword  { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordAgain { get; set; }

    }
}
