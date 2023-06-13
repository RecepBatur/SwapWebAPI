using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.PagingDtos
{
    public class PagingFilterDto : IDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
