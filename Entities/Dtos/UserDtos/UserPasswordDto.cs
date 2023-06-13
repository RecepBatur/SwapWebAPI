using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.UserDtos
{
    public class UserPasswordDto : IDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public byte[] NewPassword { get; set; }
        public byte[] NewPasswordAgain { get; set; }
    }
}
