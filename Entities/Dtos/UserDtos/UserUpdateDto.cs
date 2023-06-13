using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

    }
}
