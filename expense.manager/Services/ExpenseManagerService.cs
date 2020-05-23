using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using expense.manager.Data;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Utils;
using MethodTimer;
using Newtonsoft.Json;

namespace expense.manager.Services
{
    public class ExpenseManagerService : IExpenseManagerService
    {
        public IEnumerable<Currency> GetCurrencies()
        {

            var result = new List<Currency>();
            var assembly = typeof(ExpenseManagerService).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("expense.manager.Resources.Common-Currency.json");

            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                var rootobject = JsonConvert.DeserializeObject<Rootobject>(json);
                if (rootobject?.currencies != null && rootobject.currencies.Any())
                {
                    result.AddRange(rootobject.currencies);
                }
            }

            return result;
        }

        public Task<Currency> GetCurrency(string code)
        {
            return Task.FromResult(GetCurrencies().SingleOrDefault(c => c.cc == code));
        }

        public async Task<double?> GetSpecifiedBudget(Category category, string monthId)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var result = await unitOfWork.Repository.GetSpecialBudget(category.Id, monthId);
            unitOfWork.Complete();
            return result;
        }

        public async Task<Category> GetCategory(int id)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            return (await unitOfWork.Repository.GetCategory(id))?.Map<CategoryData, Category>();
        }

        public async Task<IEnumerable<Expense>> GetExpensesRecap(string monthId, int categoryId = -1)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            if (categoryId == -1)
            {
                return (await unitOfWork.Repository.GetExpenses(e => e.MonthId == monthId))?.Select(expense => expense.Map<ExpenseData, Expense>()).OrderByDescending(e => e.CreationDate).ToList();
            }
            return (await unitOfWork.Repository.GetExpenses(e => e.MonthId == monthId && e.CategoryId == categoryId))?.Select(expense => expense.Map<ExpenseData, Expense>()).OrderByDescending(e => e.CreationDate).ToList();
        }

        public async Task<IEnumerable<Tag>> GetTagsForExpense(int expenseId)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var tags = await unitOfWork.Repository.GetTagsByExpenseId(expenseId);

            return tags?.Select(t => t.Map<TagData, Tag>());
        }

        public async Task<IEnumerable<Expense>> GetExpensesByTag(int tagId)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            return (await unitOfWork.Repository.GetExpenseFromTagAsync(tagId))
                ?.Select(expense => expense.Map<ExpenseData, Expense>()).ToList();
        }

        public async Task AddOrUpdateExpense(Expense expense)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var dataObject = expense.Map<Expense, ExpenseData>();
            dataObject.MonthId = expense.CreationDate.ToMonthId();
            var expenseId = await unitOfWork.Repository.AddExpense(dataObject);
            await unitOfWork.Repository.AddTagsToExpense(expenseId, expense.TagList.Select(t => t.Id));
            unitOfWork.Complete();
        }

        public async Task AddOrUpdateCategory(Category category)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());

            var categoryToAdd = category.Map<Category, CategoryData>();
            categoryToAdd.ParentId = category.ParentCategory?.Id ?? 0;
            await unitOfWork.Repository.AddCategory(categoryToAdd);

            unitOfWork.Complete();
        }


    
        public async Task AddOrUpdateSpecialBudget(Category category, double budget, string monthId)
        {

            using var unitOfWork= new UnitOfWork(new ExpenseManagerContext());

            await unitOfWork.Repository.AddOrUpdateSpecialBuget(category.Map<Category, CategoryData>(),
                monthId, budget); 
            unitOfWork.Complete();
        }

        public async Task DeleteSpecialBudget(Category category, string monthId)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());

            await unitOfWork.Repository.DeleteSpecialBudget(category.Map<Category, CategoryData>(), monthId);
            unitOfWork.Complete();
        }

        public async Task PersistPreviousMonthBudget(string monthId)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            await unitOfWork.Repository.PersistPreviousMonthBudget(monthId);
            unitOfWork.Complete();
        }

        public async Task UpdateParentRecurringBudgets(Category parentCateg, double? oldBudget, double newBudget)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var parentCategs = await GetParentCateg(parentCateg, includeCurrent:true);

            foreach (var categ in parentCategs)
            {
                var newRecurringBudget = categ.RecurringBudget -
                                         oldBudget +
                                         newBudget;

                if (newRecurringBudget.HasValue && newRecurringBudget >= 0)
                {
                    categ.RecurringBudget = newRecurringBudget.Value;
                }


                await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());


            }

            unitOfWork.Complete();
        }

        public async Task UpdateParentCategsBudgets(Category oldParentCateg, Category newParentCateg, double budgetToTransfer)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var oldParentCategs = await GetParentCateg(oldParentCateg, includeCurrent:true);
            var newParentCategs = await GetParentCateg(newParentCateg, includeCurrent:true);


            foreach (var categ in oldParentCategs)
            {
                var newRecurringBudget = categ.RecurringBudget - budgetToTransfer;

                if (newRecurringBudget >= 0)
                {
                    categ.RecurringBudget = newRecurringBudget;
                }


                await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());





            }

            foreach (var categ in newParentCategs)
            {
                var newRecurringBudget = categ.RecurringBudget + budgetToTransfer;

                if (newRecurringBudget > 0)
                {
                    categ.RecurringBudget = newRecurringBudget;
                }

                await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());

            }

            unitOfWork.Complete();
        }

        public async Task<bool> DeleteExpense(Expense expenseToDelete)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            await unitOfWork.Repository.DeleteExpense(expenseToDelete.Map<Expense, ExpenseData>());

            expenseToDelete.PreviousCategId = expenseToDelete.CategoryId;

            unitOfWork.Complete();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var categories =
                (await unitOfWork.Repository.GetCategories(n => n.ParentId == 0))?.Select(n =>
                    n.Map<CategoryData, Category>()).ToList();

            if (categories == null)
            {
                return new List<Category>();

            }

            await PopulateChildrenCategories(categories, unitOfWork);

            unitOfWork.Complete();

            return categories;
        }


        [Time] 
        public async Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, Category parentCategory=null)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var parentId = parentCategory?.Id ?? 0;

            var categories =
                (await unitOfWork.Repository.GetCategories(n => n.ParentId == parentId))?.Select(n =>
                    n.Map<CategoryData, Category>()).ToList();

            if (categories == null)
            {
                return new List<Category>();

            }

            foreach (var category in categories)
            {
                category.ParentCategory = parentCategory;
            }
            

              

            await PopulateChildrenCategories(categories, unitOfWork);
            foreach (var category in categories)
            {

                var flattenedCategoriesIds = category.ChildrenCategories
                    .SelectManyRecursive(l => l.ChildrenCategories).Select(c => c.Id);
                  
                category.AmmountSpent =
                    (await unitOfWork.Repository.SumExpenses(e =>
                        e.MonthId == monthId && (flattenedCategoriesIds.Contains(e.CategoryId) || e.CategoryId==category.Id)));

                category.Budget = await unitOfWork.Repository.GetSpecialBudget(category.Id, monthId) ??
                                  category.RecurringBudget;
            }


            unitOfWork.Complete();
            return categories;
        }

        [Time]
        private async Task PopulateChildrenCategories(IEnumerable<Category> categories, IUnitOfWork currentunitOfWork) 
        {
            
                var enumerable = categories.ToList();
                enumerable.ForEach(
                    n =>

                        n.ChildrenCategories = currentunitOfWork.Repository.GetCategories(c => c.ParentId == n.Id).Result
                            .Select(m =>
                            {
                                var child= m.Map<CategoryData, Category>();
                                child.ParentCategory = n;
                                return child;
                            }).ToList()

                );

                foreach (var category in enumerable)
                {
                    await PopulateChildrenCategories(category.ChildrenCategories, currentunitOfWork);
                }

            
        }


        public async Task<int> AddTag(Tag newTag)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var result= await unitOfWork.Repository.AddTag(newTag.Map<Tag, TagData>());
            unitOfWork.Complete();

            return result;

        }

        public Task UntagExpense(Tag tag, Expense expense)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var result= unitOfWork.Repository.SupressExpenseTagRelation(expense.Map<Expense, ExpenseData>(),
                tag.Map<Tag, TagData>());
            unitOfWork.Complete();

            return result;
        }


        public async Task<bool> DeleteTag(Tag newTag)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            await unitOfWork.Repository.DeleteTag(newTag.Map<Tag, TagData>());
            unitOfWork.Complete();
            return true;
        }


        public async Task<IEnumerable<Tag>> GetTags(bool computeAmmounts = true)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var alltags = (await unitOfWork.Repository.GetAllTags())?.Select(tag => tag.Map<TagData, Tag>())
                .ToList();

            if (alltags == null || !computeAmmounts) return alltags;

            foreach (var tag in alltags)
            {
                tag.AmmountSpent =
                    (await unitOfWork.Repository.GetExpenseFromTagAsync(tag.Id))?.Sum(expense => expense.Ammount) ?? 0;
            }

            unitOfWork.Complete();

            return alltags;
        }


        private async Task<IEnumerable<Category>> GetParentCateg(Category category, bool includeCurrent = false)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            var result = new List<Category>();
            var currentId = (includeCurrent ? category?.Id: category.ParentCategory?.Id) ?? default;
            while (currentId != 0)
            {
                var currentCateg = await unitOfWork.Repository.GetCategory(currentId);
                if (currentCateg != null)
                {
                    result.Add(currentCateg.Map<CategoryData, Category>());
                    currentId = currentCateg.ParentId;

                }
            }

            return result;
        }

        public async Task<bool> DeleteCategory(Category categoryToDelete)
        {
            using var unitOfWork = new UnitOfWork(new ExpenseManagerContext());
            try
            {

                await unitOfWork.Repository.DeleteCategory(categoryToDelete.Map<Category, CategoryData>());

                var flattenedChildren = categoryToDelete.ChildrenCategories.SelectManyRecursive(l => l.ChildrenCategories);

                foreach (var categ in flattenedChildren)
                {
                    await unitOfWork.Repository.DeleteCategory(categ.Map<Category, CategoryData>());
                    await unitOfWork.Repository.DeleteExpenses(n => n.CategoryId == categ.Id);

                }

                var oldParentCategs = await GetParentCateg(categoryToDelete);
                foreach (var categ in oldParentCategs)
                {
                    var newRecurringBudget = categ.RecurringBudget - categoryToDelete.RecurringBudget;

                    if (newRecurringBudget >= 0)
                    {
                        categ.RecurringBudget = newRecurringBudget;
                    }

                    await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());
                }


                await unitOfWork.Repository.DeleteExpenses(n => n.CategoryId == categoryToDelete.Id);
                unitOfWork.Complete();

                return true;


            }
            catch (Exception)
            {

                return false;
            }
            


        }
    }
}