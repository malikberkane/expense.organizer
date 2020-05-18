using System;
using System.Collections.Generic;
using System.Text;
using expense.manager.Data;

namespace expense.manager.Services
{
    public interface IUnitOfWork: IDisposable
    {
        IRepository Repository { get; }

        int Complete();
    }
}
