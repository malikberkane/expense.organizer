using System;
using expense.manager.Utils;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace expense.manager.Data
{
    public class ExpenseManagerContext : DbContext
    {

        public DbSet<ExpenseData> Expenses { get; set; }
        public DbSet<CategoryData> ExpenseCategories { get; set; }
        public DbSet<TagData> ExpenseTags { get; set; }
        public DbSet<ExpenseTagRelationData> ExpenseTagRelations { get; set; }

        public DbSet<BudgetData> Budgets { get; set; }





        #region Private implementation

        protected string DatabasePath { get; set; }

        public ExpenseManagerContext()
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                DatabasePath = DependencyService.Get<IFileHelper>().GetConnection();
            }

            EnsureAndMigrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseTagRelationData>()
                .HasKey(c => new { c.TagId, c.ExpenseId });

            
            modelBuilder.Entity<ExpenseData>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<CategoryData>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<BudgetData>()
                .HasKey(c => new { c.CategoryId, c.MonthId });

            var initialDataset = SeedProvider.GetDefaultCategories();

            var id = 1;
            foreach (var initialCategory in initialDataset)
            {
                modelBuilder.Entity<CategoryData>().HasData(new CategoryData()
                {
                    Id = id,
                    Name = initialCategory.Name,

                });

                var parentId = id;
                id++;

                if (initialCategory.SubCategories == null || initialCategory.SubCategories.Count == 0) continue;
                foreach (var subcategory in initialCategory.SubCategories)
                {
                    modelBuilder.Entity<CategoryData>().HasData(new CategoryData()
                    {
                        Id = id,
                        ParentId = parentId,
                        Name = subcategory.Name,

                    });

                    id++;


                }
            }

            var expenseId = 1;
            Random random = new Random();

            Random randomDay = new Random();



            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    var date = new DateTime(2020, random.Next(1, 12), randomDay.Next(1, 30));
                    modelBuilder.Entity<ExpenseData>().HasData(new ExpenseData
                    {
                        Id = expenseId,
                        CreationDate = date,
                        ExpenseLabel = $"depense {j}",
                        CategoryId = 0,
                        MonthId = date.ToMonthId(),
                        Ammount = random.NextDouble() * 10,


                    });

                    expenseId++;

                }

            }










        }
    




    

        public void EnsureAndMigrate()
        {
            try
            {
                this.Database.EnsureCreated();
                this.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                optionsBuilder.UseSqlite($"Filename=test.db");
                
            }
            else
            {
                optionsBuilder.UseSqlite($"Filename={DatabasePath}");

            }

        }

        #endregion
    }
}