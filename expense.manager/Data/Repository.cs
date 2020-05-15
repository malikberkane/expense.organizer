using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace expense.manager.Data
{
    public class Repository : IRepository
    {

        private ExpenseManagerContext CurrentContext => new ExpenseManagerContext();

        #region Expense Management

        public async Task<IEnumerable<ExpenseData>> GetAllExpenses()
        {
            using (var context = CurrentContext)
            {
                return await context.Expenses.ToListAsync();
            }

        }

        public async Task<List<ExpenseData>> GetExpenses(string monthId, int categoryId = -1)
        {
            using (var context = CurrentContext)
            {
                var monthIdFilterResult = await context.Expenses.Where(t => t.MonthId == monthId).ToListAsync();

                if (categoryId == -1)
                {
                    return monthIdFilterResult;
                }

                return monthIdFilterResult.Where(item => item.CategoryId == categoryId).ToList();

            }


        }

        private async Task<List<int>> GetChildrenCategoriesId(int id)
        {
            using (var context = CurrentContext)
            {
                var result = new List<int>();
                var subcategories = context.ExpenseCategories.Where(c => c.ParentId == id);
                result.AddRange(subcategories.Select(n => n.Id));
                foreach (var item in subcategories)
                {
                    result.AddRange(await GetChildrenCategoriesId(item.Id));

                }
                return result;

            }

        }

        private async Task<List<int>> GetChildrenRecursive(int id, IEnumerable<CategoryData> list)
        {
            var result = new List<int>();
            var subcategories = list.Where(c => c.ParentId == id);
            result.AddRange(subcategories.Select(n => n.Id));
            foreach (var item in subcategories)
            {
                result.AddRange(await GetChildrenCategoriesId(item.Id));

            }
            return result;

        }

        public async Task<IEnumerable<Category>> GetCategoriesRecap(string monthId, int level = 0)
        {
            using (var context = CurrentContext)
            {

                return context.ExpenseCategories.Where(ec => ec.ParentId == level).AsEnumerable().Select(c =>
                     {

                         var categ = c.Map<CategoryData, Category>();
                         var list = GetChildrenRecursive(c.Id, context.ExpenseCategories).Result;
                         list.Add(categ.Id);
                         categ.AmmountSpent = context.Expenses.Where(e => list.Contains(e.CategoryId) && monthId == e.MonthId).Sum(a => a.Ammount);
                         categ.Budget = context.Budgets.SingleOrDefault(b => b.CategoryId == categ.Id && b.MonthId == monthId)?.BudgetAmmount ?? categ.RecurringBudget;
                         return categ;
                     }).ToList();





            }




        }

        public async Task<double> GetSumForCategoryRaw(string monthId, int categoryId)
        {
            using (var context = CurrentContext)
            {
                var list = await GetChildrenCategoriesId(categoryId);
                list.Add(categoryId);
                return context.Expenses.Where(e => e.MonthId == monthId && list.Contains(e.CategoryId)).Sum(n => n.Ammount);
            }
        }

        public async Task<IEnumerable<ExpenseData>> GetExpenseFromTagAsync(int tagId)
        {
            using (var context = CurrentContext)
            {
                var ids = context.ExpenseTagRelations.Where(n => n.TagId == tagId).Select(p => p.ExpenseId).ToList();
                return await context.Expenses.Where(n => ids.Contains(n.Id)).ToListAsync();
            }



        }





        public async Task<ExpenseData> GetExpense(int expenseId)
        {
            using (var context = CurrentContext)
            {
                return await context.Expenses.SingleAsync(t => t.Id == expenseId);

            }
        }


        public Task DeleteExpense(ExpenseData expense)
        {
            using (var context = CurrentContext)
            {
                context.Expenses.Remove(expense);

                var itemsToRemove = context.ExpenseTagRelations.Where(n => n.ExpenseId == expense.Id);
                foreach (var item in itemsToRemove)
                {
                    context.ExpenseTagRelations.Remove(item);
                }

                return context.SaveChangesAsync();
            }
        }

        public async Task<int> AddExpense(ExpenseData expense)
        {
            using (var context = CurrentContext)
            {
                if (expense.Id == 0)
                {

                    await context.Expenses.AddAsync(expense);

                }
                else
                {
                    context.Entry(expense).State = EntityState.Modified;
                }


                await context.SaveChangesAsync();

                return expense.Id;
            }




            //await DeleteExpenseTagRelation(expense.Id);

            //foreach (var tagId in tagIds)
            //{
            //    await AddExpenseTagRelation(expense.Id, tagId);
            //}


            //return await  Context.SaveChangesAsync();

        }



        public async Task<IEnumerable<TagData>> GetAllTags()
        {
            using (var context = CurrentContext)
            {
                return await context.ExpenseTags.ToListAsync();

            }

        }

        public async Task<TagData> GetTagById(int tagId)
        {
            using (var context = CurrentContext)
            {
                return await context.ExpenseTags.SingleAsync(n => n.Id == tagId);

            }

        }

        public async Task<IEnumerable<TagData>> GetTagsByExpenseId(int expenseId)
        {
            using (var context = CurrentContext)
            {
                var tagsId = context.ExpenseTagRelations.Where(n => n.ExpenseId == expenseId).Select(p => p.TagId);
                return await context.ExpenseTags.Where(n => tagsId.Contains(n.Id)).ToListAsync();
            }


        }






        public Task DeleteTag(TagData tag)
        {
            using (var context = CurrentContext)
            {
                context.ExpenseTags.Remove(tag);
                var itemsToRemove = context.ExpenseTagRelations.Where(n => n.TagId == tag.Id);
                foreach (var item in itemsToRemove)
                {
                    context.ExpenseTagRelations.Remove(item);
                }
                return context.SaveChangesAsync();
            }

        }

        #endregion






        public async Task<int> AddTag(TagData tag)
        {
            using (var context = CurrentContext)
            {
                if (tag.Id == 0)
                {
                    context.ExpenseTags.AddAsync(tag);
                }

                else
                {

                    context.Entry(tag).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();

                return tag.Id;
            }

        }

        #region Category Management

        public async Task<IEnumerable<CategoryData>> GetAllCategories()
        {
            using (var context = CurrentContext)
            {
                return await context.ExpenseCategories.ToListAsync();

            }
        }

        public async Task<IEnumerable<CategoryData>> GetCategoriesLevel(int level)
        {
            using (var context = CurrentContext)
            {
                return await context.ExpenseCategories.Where(c => c.ParentId == level).ToListAsync();


            }


        }

        public async Task<CategoryData> GetCategory(int categoryId)
        {
            using (var context = CurrentContext)
            {
                return await context.ExpenseCategories.SingleAsync(t => t.Id == categoryId);

            }
        }

        public Task DeleteCategory(CategoryData category)
        {
            using (var context = CurrentContext)
            {
                var childIds = GetChildrenCategoriesId(category.Id);
                childIds.Result.Insert(0, category.Id);
                foreach (var categId in childIds.Result)
                {
                    var categToRemove = context.ExpenseCategories.SingleOrDefault(n => n.Id == categId);

                    if (categToRemove != null)
                    {
                        context.ExpenseCategories.Remove(categToRemove);
                    }



                    var expensesToRemove = context.Expenses.Where(n => n.CategoryId == categId);

                    var budgetsToRemove = context.Budgets.Where(n => n.CategoryId == categId);

                    foreach (var expense in expensesToRemove)
                    {
                        context.Expenses.Remove(expense);

                    }

                    foreach (var budget in budgetsToRemove)
                    {
                        context.Budgets.Remove(budget);

                    }
                }







                return context.SaveChangesAsync();

            }

        }



        public async Task<int> AddCategory(CategoryData category)
        {
            using (var context = CurrentContext)
            {
                if (category.Id == 0)
                {
                    await context.ExpenseCategories.AddAsync(category);
                }

                else
                {
                    context.ExpenseCategories.Update(category);
                }


                await context.SaveChangesAsync();

                return category.Id;
            }
        }


        public async Task AddBudget(BudgetData budget)
        {
            using (var context = CurrentContext)
            {
                var budgetToUpdate = context.Budgets.SingleOrDefault(b => b.CategoryId == budget.CategoryId && b.MonthId == budget.MonthId);

                if (budgetToUpdate != null)
                {
                    budgetToUpdate.BudgetAmmount = budget.BudgetAmmount;
                }
                else
                {
                    await context.Budgets.AddAsync(budget);
                }

                await context.SaveChangesAsync();

            }
        }

        public async Task UpdateRecurringBudgets(IEnumerable<int> categIds, double value)
        {
            using (var context = CurrentContext)
            {
                var categsToUpdate = context.ExpenseCategories.Where(c => categIds.Contains(c.Id));

                foreach (var categ in categsToUpdate)
                {
                    categ.RecurringBudget += value;
                    context.ExpenseCategories.Update(categ);
                }

                await context.SaveChangesAsync();

            }
        }




        public async Task RemoveSpecialBudget(int categoryId, string monthId)
        {
            using (var context = CurrentContext)
            {

                var budgetToRemove = context.Budgets.SingleOrDefault(b => b.MonthId == monthId && b.CategoryId == categoryId);

                if (budgetToRemove != null)
                {
                    context.Budgets.Remove(budgetToRemove);

                }

                await context.SaveChangesAsync();

            }

        }

        public Task<double?> GetSpecialBudget(int categoryId, string monthId)
        {
            using (var context = CurrentContext)
            {
                var result = context.Budgets.SingleOrDefault(b => b.CategoryId == categoryId && b.MonthId == monthId);

                return Task.FromResult(result?.BudgetAmmount);

            }
        }

        public Task AddTagsToExpense(int expenseId, IEnumerable<int> tagsIds)
        {
            using (var context = CurrentContext)
            {
                var itemsToRemove = context.ExpenseTagRelations.Where(n => n.ExpenseId == expenseId);

                itemsToRemove.ForEachAsync(i =>
                {
                    context.ExpenseTagRelations.Remove(i);
                });


                foreach (var tagId in tagsIds)
                {
                    context.ExpenseTagRelations.Add(new ExpenseTagRelationData { ExpenseId = expenseId, TagId = tagId });
                }

                return context.SaveChangesAsync();
            }

        }

        public Task AddExpenseTagRelation(int expenseId, int tagId)
        {
            using (var context = CurrentContext)
            {
                if (context.ExpenseTagRelations.Any(n => n.TagId == tagId && n.ExpenseId == expenseId))
                    return Task.CompletedTask;

                context.ExpenseTagRelations.Add(new ExpenseTagRelationData()
                {
                    ExpenseId = expenseId,
                    TagId = tagId,
                });

                return context.SaveChangesAsync();
            }

        }

        public async Task UpdateCategorySum(SumForCategoryData sumData)
        {

            using (var context = CurrentContext)
            {
                if (!context.Sums.Any(n => n.CategoryId == sumData.CategoryId && n.MonthId == sumData.MonthId))
                {

                    await context.Sums.AddAsync(sumData);

                }
                else
                {
                    try
                    {
                        context.Sums.Update(sumData);

                    }
                    catch (Exception ex)
                    {

                    }

                }


                await context.SaveChangesAsync();

            }

        }


        public async Task AddOrUpdateSpecialBuget(CategoryData category, string monthId, double newBudget)
        {

            using (var context = CurrentContext)
            {
                if (!context.Budgets.Any(n => n.CategoryId == category.Id && n.MonthId == monthId))
                {

                    await context.Budgets.AddAsync(new BudgetData
                    {
                        CategoryId = category.Id,
                        BudgetAmmount = newBudget,
                        MonthId = monthId
                    });

                }
                else
                {
                    var budgetToUpdate = context.Budgets.Single(n => n.CategoryId == category.Id && n.MonthId == monthId);
                    budgetToUpdate.BudgetAmmount = newBudget;
                    context.Budgets.Update(budgetToUpdate);

                }


                await context.SaveChangesAsync();

            }

        }



        public async Task PersistPreviousMonthBudget(string monthId)
        {
            using (var context = CurrentContext)
            {
                foreach (var categ in context.ExpenseCategories)
                {
                    var specialbudget = context.Budgets.SingleOrDefault(n => n.CategoryId == categ.Id && n.MonthId == monthId);

                    if (specialbudget == null && categ.RecurringBudget!=0)
                    {
                        await context.Budgets.AddAsync(new BudgetData
                        {
                            CategoryId = categ.Id,
                            BudgetAmmount = categ.RecurringBudget,
                            MonthId = monthId
                        });
                    }

                }


                await context.SaveChangesAsync();

            }

        }





        public Task<double> GetSumForCategory(int categId, string monthId)
        {
            using (var context = CurrentContext)
            {
                var sumData = context.Sums.SingleOrDefault(n => n.CategoryId == categId && n.MonthId == monthId);

                if (sumData == null)
                {
                    return Task.FromResult(0.0);
                }

                return Task.FromResult(sumData.Ammount);

            }
        }

        public Task DeleteExpenseTagRelation(int expenseId)
        {
            using (var context = CurrentContext)
            {
                var itemsToRemove = context.ExpenseTagRelations.Where(n => n.ExpenseId == expenseId);

                foreach (var item in itemsToRemove)
                {
                    context.ExpenseTagRelations.Remove(item);
                }

                return context.SaveChangesAsync();
            }

        }

        public Task SupressExpenseTagRelation(ExpenseData expense, TagData tag)
        {

            using (var context = CurrentContext)
            {
                var itemsToRemove = context.ExpenseTagRelations.Where(n => n.ExpenseId == expense.Id && n.TagId == tag.Id);

                foreach (var item in itemsToRemove)
                {
                    context.ExpenseTagRelations.Remove(item);
                }

                return context.SaveChangesAsync();
            }

        }




        #endregion


    }


}