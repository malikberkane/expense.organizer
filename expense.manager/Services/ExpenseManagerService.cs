using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace expense.manager.Services
{
    public class ExpenseManagerService : IExpenseManagerService
    {
        public IRepository Repository => DependencyService.Get<IRepository>();


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
            return await Repository.GetSpecialBudget(category.Id, monthId);
        }

        public async Task<Category> GetCategory(int id)
        {
            var result = (await Repository.GetCategory(id))?.Map<CategoryData, Category>();
            return result;
        }

        public async Task<IEnumerable<Expense>> GetExpensesRecap(string monthId, int level = -1)
        {

            return (await Repository.GetExpenses(monthId, level))?.Select(expense => expense.Map<ExpenseData, Expense>()).OrderByDescending(e => e.CreationDate).ToList();


        }


        public async Task<IEnumerable<Tag>> GetTagsForExpense(int expenseId)
        {
            var tags = await Repository.GetTagsByExpenseId(expenseId);

            return tags?.Select(t => t.Map<TagData, Tag>());


        }

        public async Task<IEnumerable<Expense>> GetExpensesByTag(int tagId)
        {

            return (await Repository.GetExpenseFromTagAsync(tagId))?.Select(expense => expense.Map<ExpenseData, Expense>()).ToList();


        }



        public async Task<int> AddExpense(Expense expense)
        {

            var dataObject = expense.Map<Expense, ExpenseData>();
            dataObject.MonthId = expense.CreationDate.ToMonthId();
            var expenseId = await Repository.AddExpense(dataObject);
            await Repository.AddTagsToExpense(expenseId, expense.TagList.Select(t => t.Id));
            expense.Id = expenseId;


            return expenseId;



        }


        public async Task<int> AddCategory(Category category, CategoryEditionTracker editionTracker, string currentMonthId)
        {
            var newCategoryId = await Repository.AddCategory(category.Map<Category, CategoryData>());

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
                await Repository.AddOrUpdateSpecialBuget(category.Map<Category, CategoryData>(),currentMonthId, editionTracker.SpecialBudget.final.Value);

            }


            category.Id = newCategoryId;

            return newCategoryId;
        }


        public async Task PersistPreviousMonthBudget(string monthId)
        {
            await Repository.PersistPreviousMonthBudget(monthId);
        }


        private async Task UpdateParentRecurringBudgets(CategoryEditionTracker editionTracker)
        {
            var parentCategs = await GetParentCateg(editionTracker.ParentCategory.final);


            foreach (var categ in parentCategs)
            {
                var newRecurringBudget = categ.RecurringBudget - editionTracker.RecurringBudget.initial.GetValueOrDefault() + editionTracker.RecurringBudget.final.GetValueOrDefault();

                if (newRecurringBudget >= 0)
                {
                    categ.RecurringBudget = newRecurringBudget;
                }


                await Repository.AddCategory(categ.Map<Category, CategoryData>());


            }


        }

        private async Task TransferParentBudgets(Category category, CategoryEditionTracker editionTracker)
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


                await Repository.AddCategory(categ.Map<Category, CategoryData>());





            }
            foreach (var categ in newParentCategs)
            {
                var newRecurringBudget = categ.RecurringBudget + category.RecurringBudget;

                if (newRecurringBudget > 0)
                {
                    categ.RecurringBudget = newRecurringBudget;
                }

                await Repository.AddCategory(categ.Map<Category, CategoryData>());

            }
        }

        public async Task<bool> DeleteExpense(Expense expense)
        {

            await Repository.DeleteExpense(expense.Map<Expense, ExpenseData>());

            expense.PreviousCategId = expense.CategoryId;
            return true;


        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return (await Repository.GetAllCategories())?.Select(category => category.Map<CategoryData, Category>()).ToList();
        }


        [Time]
        public async Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, int level = 0)
        {

            return await Repository.GetCategoriesRecap(monthId, level);

        }


        public async Task<int> AddTag(Tag newTag)
        {
            return await Repository.AddTag(newTag.Map<Tag, TagData>());

        }

        public Task UntagExpense(Tag tag, Expense expense)
        {

            return Repository.SupressExpenseTagRelation(expense.Map<Expense, ExpenseData>(), tag.Map<Tag, TagData>());

        }


        public async Task<bool> DeleteTag(Tag newTag)
        {
            await Repository.DeleteTag(newTag.Map<Tag, TagData>());

            return true;

        }


        public async Task<IEnumerable<Tag>> GetTags(bool computeAmmounts = true)
        {
            var alltags = (await Repository.GetAllTags())?.Select(tag => tag.Map<TagData, Tag>()).ToList();

            if (computeAmmounts)
            {
                foreach (Tag tag in alltags)
                {
                    tag.AmmountSpent = (await Repository.GetExpenseFromTagAsync(tag.Id))?.Sum(expense => expense.Ammount);
                }
            }

            return alltags;
        }


        public async Task<double?> GetAmmountSpentForTag(int id)
        {

            var expenses = await Repository.GetExpenseFromTagAsync(id);



            return expenses.Sum(e => e.Ammount);
        }




        private async Task<IEnumerable<Category>> GetParentCateg(Category category)
        {

            var result = new List<Category>();
            int currentId = category.Id;
            CategoryData currentCateg;
            while (currentId != 0)
            {
                currentCateg = await Repository.GetCategory(currentId);
                if (currentCateg != null)
                {
                    result.Add(currentCateg.Map<CategoryData, Category>());
                    currentId = currentCateg.ParentId;

                }

            }
            return result;
        }






        public async Task<bool> DeleteCategory(Category category)
        {
            try
            {


                await Repository.DeleteCategory(category.Map<Category,CategoryData>());
                var oldParentCategs = await GetParentCateg(category);

                foreach (var categ in oldParentCategs)
                {
                    var newRecurringBudget = categ.RecurringBudget - category.RecurringBudget;

                    if (newRecurringBudget >= 0)
                    {
                        categ.RecurringBudget = newRecurringBudget;
                    }

                 
                }

                return true;

            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}