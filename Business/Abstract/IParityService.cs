using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos.ParityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IParityService
    {
        IDataResult<bool> ParityAdd(ParityAddedDto parity);
        Task<IDataResult<List<Root>>> ParityGetList();

    }
}
