﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.CoinDtos
{
    public class CoinListDto : IDto
    {
        public string CoinName { get; set; }

    }
}