using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnimalFarm.Data
{
    public interface IAzureTableTransaction : ITransaction
    {
        void AddOperation(string tableName, TableOperation operation);
    }
}
