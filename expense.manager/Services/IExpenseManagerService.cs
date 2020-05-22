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
        Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, Category parentCategory=null);

        Task AddOrUpdateSpecialBudget(Category category, double budget, string monthId);

        Task UpdateParentRecurringBudgets(Category parentCateg, double? oldBudget, double newBudget);

        Task UpdateParentCategsBudgets(Category oldParentCateg, Category newParentCateg, double budgetToTransfer);
        Task<IEnumerable<Expense>> GetExpensesRecap(string monthId, int categoryId = -1);
        Task<Category> GetCategory(int id);
        Task<IEnumerable<Tag>> GetTags(bool computeAmmounts = true);
        Task<int> AddTag(Tag newTag);
        Task<bool> DeleteTag(Tag newTag);
        Task<IEnumerable<Expense>> GetExpensesByTag(int tagId);
        Task<IEnumerable<Tag>> GetTagsForExpense(int expenseId);
        Task UntagExpense(Tag tag, Expense expense);
        Task<int> AddOrUpdateCategory(Category category);
        IEnumerable<Currency> GetCurrencies();
        Task<Currency> GetCurrency(string code);
        Task<double?> GetSpecifiedBudget(Category category, string monthId);
        Task PersistPreviousMonthBudget(string monthId);
    }
}