using Core.Entities;
using Entities.Dtos.PagingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.UserDtos
{
    public class UserGetAllPagingDto : IDto
    {
        public List<UserGetAllDto> Data { get; set; }
        public PagingDto Paging { get; set; }
    }
}
