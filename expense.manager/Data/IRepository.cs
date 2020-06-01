using expense.manager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace expense.manager.Data
{
    public interface IRepository
    {
     

        Task<int> AddCategory(CategoryData category);
        Task<double?> GetSpecialBudget(int categoryId, string monthId);
        Task<int> AddExpense(ExpenseData expense);
        Task<int> AddTag(TagData tag);
        Task DeleteCategory(CategoryData category);
        Task DeleteExpense(ExpenseData expense);

        Task<IEnumerable<ExpenseData>> GetAllExpenses();

        Task<IEnumerable<ExpenseData>> GetPagedExpense(Expression<Func<ExpenseData, bool>> predicate,
                                         int page, int pageSize);


        Task DeleteTag(TagData tag);

        Task<IEnumerable<CategoryData>> GetAllCategories();
        Task DeleteExpenses(Expression<Func<ExpenseData, bool>> predicate);

        Task DeleteSpecialBudget(CategoryData category, string monthId);
        Task<IEnumerable<TagData>> GetAllTags();
        Task<CategoryData> GetCategory(int categoryId);
        Task<IEnumerable<ExpenseData>> GetExpenseFromTagAsync(int tagId);
        Task<IEnumerable<ExpenseData>> GetExpenses(Expression<Func<ExpenseData, bool>> predicate);

        Task<double> SumExpenses(Expression<Func<ExpenseData, bool>> predicate);

        Task<IEnumerable<TagData>> GetTagsByExpenseId(int expenseId);
        Task SupressExpenseTagRelation(ExpenseData expense, TagData tag);
        Task<IEnumerable<CategoryData>> GetCategories(Expression<Func<CategoryData, bool>> predicate);
        Task AddTagsToExpense(int expenseId, IEnumerable<int> tagsIds);
        Task AddOrUpdateSpecialBuget(CategoryData category, string monthId, double newBudget);
        Task PersistPreviousMonthBudget(string monthId);
    }
}