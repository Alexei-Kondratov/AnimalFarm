﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Data
{
    public interface ITransactionManager
    {
        ITransaction CreateTransaction();
    }
}
