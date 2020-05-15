using System.Collections.Generic;
using System.Threading.Tasks;
using expense.manager.Models;
using expense.manager.ViewModels.PageModels;

namespace expense.manager.Services
{
    public interface IExpenseManagerService
    {

        Task<IEnumerable<Category>> GetAllCategories();
        Task<int> AddExpense(Expense expense);


        Task<bool> DeleteExpense(Expense expense);
            
        Task<bool> DeleteCategory(Category category);
        Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, int level=0);
        Task<IEnumerable<Expense>> GetExpensesRecap(string monthId, int level = -1);
        Task<Category> GetCategory(int id);
        Task<IEnumerable<Tag>> GetTags(bool computeAmmounts = true);
        Task<int> AddTag(Tag newTag);
        Task<bool> DeleteTag(Tag newTag);
        Task<double?> GetAmmountSpentForTag(int id);
        Task<IEnumerable<Expense>> GetExpensesByTag(int tagId);
        Task<IEnumerable<Tag>> GetTagsForExpense(int expenseId);
        Task UntagExpense(Tag tag, Expense expense);
        Task<int> AddCategory(Category category, CategoryEditionTracker editionTracker, string currentMonthId);
        IEnumerable<Currency> GetCurrencies();
        Task<Currency> GetCurrency(string code);
        Task<double?> GetSpecifiedBudget(Category category, string monthId);
        Task PersistPreviousMonthBudget(string monthId);
    }
}