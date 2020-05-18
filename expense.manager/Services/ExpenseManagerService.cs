using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using expense.manager.Data;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.PageModels;
using MethodTimer;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace expense.manager.Services
{
    public class ExpenseManagerService : IExpenseManagerService
    {
        public IEnumerable<Currency> GetCurrencies()
        {

            var result = new List<Currency>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(ExpenseManagerService)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("expense.manager.Common-Currency.json");

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
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var result = await unitOfWork.Repository.GetSpecialBudget(category.Id, monthId);
                unitOfWork.Complete();
                return result;
            }
        }

        public async Task<Category> GetCategory(int id)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                return (await unitOfWork.Repository.GetCategory(id))?.Map<CategoryData, Category>();

            }
        }

        public async Task<IEnumerable<Expense>> GetExpensesRecap(string monthId, int categoryId = -1)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                return (await unitOfWork.Repository.GetExpenses(e => e.MonthId == monthId && e.CategoryId == categoryId))?.Select(expense => expense.Map<ExpenseData, Expense>()).OrderByDescending(e => e.CreationDate).ToList();

            }



        }

        public async Task<IEnumerable<Tag>> GetTagsForExpense(int expenseId)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var tags = await unitOfWork.Repository.GetTagsByExpenseId(expenseId);

                return tags?.Select(t => t.Map<TagData, Tag>());
            }

        }

        public async Task<IEnumerable<Expense>> GetExpensesByTag(int tagId)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                return (await unitOfWork.Repository.GetExpenseFromTagAsync(tagId))
                    ?.Select(expense => expense.Map<ExpenseData, Expense>()).ToList();
            }

        }

        public async Task<int> AddExpense(Expense expense)
        {

            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var dataObject = expense.Map<Expense, ExpenseData>();
                dataObject.MonthId = expense.CreationDate.ToMonthId();
                var expenseId = await unitOfWork.Repository.AddExpense(dataObject);
                await unitOfWork.Repository.AddTagsToExpense(expenseId, expense.TagList.Select(t => t.Id));
                expense.Id = expenseId;

                unitOfWork.Complete();


                return expenseId; 
            }



        }

        public async Task<int> AddCategory(Category category, CategoryEditionTracker editionTracker, string currentMonthId)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var newCategoryId = await unitOfWork.Repository.AddCategory(category.Map<Category, CategoryData>());

                if (!editionTracker.ParentCategory.initial.Equals(editionTracker.ParentCategory.final))
                {
                    await TransferParentBudgets(category, editionTracker);
                }


                if (editionTracker.RecurringBudget.initial != editionTracker.RecurringBudget.final)
                {
                    await UpdateParentRecurringBudgets(editionTracker);
                }


                if (editionTracker.SpecialBudget.initial != editionTracker.SpecialBudget.final)
                {
                    await unitOfWork.Repository.AddOrUpdateSpecialBuget(category.Map<Category, CategoryData>(),
                        currentMonthId, editionTracker.SpecialBudget.final.Value);
                }

                category.Id = newCategoryId;

                unitOfWork.Complete();
                return newCategoryId;

            }
        }

        public async Task PersistPreviousMonthBudget(string monthId)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                await unitOfWork.Repository.PersistPreviousMonthBudget(monthId);
                unitOfWork.Complete();

            }
        }

        private async Task UpdateParentRecurringBudgets(CategoryEditionTracker editionTracker)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var parentCategs = await GetParentCateg(editionTracker.ParentCategory.final);

                foreach (var categ in parentCategs)
                {
                    var newRecurringBudget = categ.RecurringBudget -
                                             editionTracker.RecurringBudget.initial.GetValueOrDefault() +
                                             editionTracker.RecurringBudget.final.GetValueOrDefault();

                    if (newRecurringBudget >= 0)
                    {
                        categ.RecurringBudget = newRecurringBudget;
                    }


                    await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());


                }
            }

        }

        private async Task TransferParentBudgets(Category category, CategoryEditionTracker editionTracker)
        {

            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var oldParentCategs = await GetParentCateg(editionTracker.ParentCategory.initial);
                var newParentCategs = await GetParentCateg(editionTracker.ParentCategory.final);


                foreach (var categ in oldParentCategs)
                {
                    var newRecurringBudget = categ.RecurringBudget - category.RecurringBudget;

                    if (newRecurringBudget >= 0)
                    {
                        categ.RecurringBudget = newRecurringBudget;
                    }


                    await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());





                }

                foreach (var categ in newParentCategs)
                {
                    var newRecurringBudget = categ.RecurringBudget + category.RecurringBudget;

                    if (newRecurringBudget > 0)
                    {
                        categ.RecurringBudget = newRecurringBudget;
                    }

                    await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());

                }

            }
        }

        public async Task<bool> DeleteExpense(Expense expense)
        {

            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                await unitOfWork.Repository.DeleteExpense(expense.Map<Expense, ExpenseData>());

                expense.PreviousCategId = expense.CategoryId;

                unitOfWork.Complete();
                return true;

            }

        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {

            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                return (await unitOfWork.Repository.GetAllCategories())
                    ?.Select(category => category.Map<CategoryData, Category>()).ToList();

            }
        }


        [Time] 
        public async Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, int level = 0)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var categories =
                    (await unitOfWork.Repository.GetCategories(n => n.ParentId == level))?.Select(n =>
                        n.Map<CategoryData, Category>()).ToList();

                if (categories == null)
                {
                    return new List<Category>();

                }

                //await PopulateChildrenCategories(categories, unitOfWork);
                foreach (var category in categories)
                {

                    //var flattenedCategoriesIds = category.ChildrenCategories
                    //    .SelectManyRecursive(l => l.ChildrenCategories).Select(c => c.Id);
                    //category.AmmountSpent =
                    //    (await unitOfWork.Repository.GetExpenses(e =>
                    //        e.MonthId == monthId && flattenedCategoriesIds.Contains(e.CategoryId))).Sum(e => e.Ammount);

                    category.Budget = await unitOfWork.Repository.GetSpecialBudget(category.Id, monthId) ??
                                      category.RecurringBudget;
                }


                unitOfWork.Complete();
                return categories;
            }

        }

        [Time]
        private async Task PopulateChildrenCategories(IEnumerable<Category> categories, IUnitOfWork currentunitOfWork) 
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var enumerable = categories.ToList();
                enumerable.ForEach(
                    n =>

                        n.ChildrenCategories = unitOfWork.Repository.GetCategories(c => c.ParentId == n.Id).Result
                            .Select(m => m.Map<CategoryData, Category>()).ToList()

                );

                foreach (var category in enumerable)
                {
                    await PopulateChildrenCategories(category.ChildrenCategories, currentunitOfWork);
                }

            }
        }


        public async Task<int> AddTag(Tag newTag)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                return await unitOfWork.Repository.AddTag(newTag.Map<Tag, TagData>());

            }
        }

        public Task UntagExpense(Tag tag, Expense expense)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {

                return unitOfWork.Repository.SupressExpenseTagRelation(expense.Map<Expense, ExpenseData>(),
                    tag.Map<Tag, TagData>());
            }
        }


        public async Task<bool> DeleteTag(Tag newTag)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                await unitOfWork.Repository.DeleteTag(newTag.Map<Tag, TagData>());

                return true;
            }

        }


        public async Task<IEnumerable<Tag>> GetTags(bool computeAmmounts = true)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var alltags = (await unitOfWork.Repository.GetAllTags())?.Select(tag => tag.Map<TagData, Tag>())
                    .ToList();

                if (alltags == null || !computeAmmounts) return alltags;

                foreach (var tag in alltags)
                {
                    tag.AmmountSpent =
                        (await unitOfWork.Repository.GetExpenseFromTagAsync(tag.Id))?.Sum(expense => expense.Ammount);
                }

                return alltags;
            }
        }

   
        private async Task<IEnumerable<Category>> GetParentCateg(Category category)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                var result = new List<Category>();
                var currentId = category.Id;
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
        }

        public async Task<bool> DeleteCategory(Category category)
        {
            using (var unitOfWork = new UnitOfWork(new ExpenseManagerContext()))
            {
                try
                {

                    await unitOfWork.Repository.DeleteCategory(category.Map<Category, CategoryData>());

                    var flattenedChildren = category.ChildrenCategories.SelectManyRecursive(l => l.ChildrenCategories);

                    foreach (var categ in flattenedChildren)
                    {
                        await unitOfWork.Repository.DeleteCategory(categ.Map<Category, CategoryData>());
                    }

                    var oldParentCategs = await GetParentCateg(category);
                    foreach (var categ in oldParentCategs)
                    {
                        var newRecurringBudget = categ.RecurringBudget - category.RecurringBudget;

                        if (newRecurringBudget >= 0)
                        {
                            categ.RecurringBudget = newRecurringBudget;
                        }

                        await unitOfWork.Repository.AddCategory(categ.Map<Category, CategoryData>());
                    }

                    return true;

                }
                catch (Exception)
                {

                    return false;
                }
                finally
                {
                    unitOfWork.Complete();
                }

            }

        }
    }
}