using expense.manager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace expense.manager.Data
{
    public interface IRepository
    {
     

        Task<int> AddCategory(CategoryData category);

        Task AddBudget(BudgetData budget);
        Task<double?> GetSpecialBudget(int categoryId, string monthId);
        Task<int> AddExpense(ExpenseData expense);
        Task AddExpenseTagRelation(int expenseId, int tagId);
        Task<int> AddTag(TagData tag);
        Task DeleteCategory(CategoryData category);
        Task DeleteExpense(ExpenseData expense);
        Task DeleteExpenseTagRelation(int expenseId);
        Task DeleteTag(TagData tag);
        Task<IEnumerable<CategoryData>> GetAllCategories();
        Task<IEnumerable<ExpenseData>> GetAllExpenses();
        Task<IEnumerable<TagData>> GetAllTags();
        Task<IEnumerable<CategoryData>> GetCategoriesLevel(int level);
        Task<CategoryData> GetCategory(int categoryId);
        Task<ExpenseData> GetExpense(int expenseId);
        Task<IEnumerable<ExpenseData>> GetExpenseFromTagAsync(int tagId);
        Task<List<ExpenseData>> GetExpenses(string monthId, int categoryId=-1);
        Task<TagData> GetTagById(int tagId);
        Task<IEnumerable<TagData>> GetTagsByExpenseId(int expenseId);
        Task SupressExpenseTagRelation(ExpenseData expense, TagData tag);
        Task<double> GetSumForCategory(int categId, string monthId);
        Task UpdateCategorySum(SumForCategoryData sumData);
        Task<double> GetSumForCategoryRaw(string monthId, int categoryId);
        Task UpdateRecurringBudgets(IEnumerable<int> categIds, double value);
        Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, int level = 0);
        Task AddTagsToExpense(int expenseId, IEnumerable<int> tagsIds);
        Task AddOrUpdateSpecialBuget(CategoryData category, string monthId, double value);
        Task RemoveSpecialBudget(int categoryId, string monthId);
        Task PersistPreviousMonthBudget(string monthId);
    }
}