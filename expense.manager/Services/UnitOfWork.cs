using System;
using System.Collections.Generic;
using System.Text;
using expense.manager.Data;

namespace expense.manager.Services
{
    public class UnitOfWork: IUnitOfWork
    {

        private readonly ExpenseManagerContext _currentContext;
        public IRepository Repository { get; }

        public UnitOfWork(ExpenseManagerContext context)
        {
            _currentContext = context;
            Repository = new Repository(context);
        }
        public int Complete()
        {
            return _currentContext.SaveChanges();
        }

        public void Dispose()
        {
            _currentContext?.Dispose();
        }
    }
}
