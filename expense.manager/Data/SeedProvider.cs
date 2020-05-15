using System;
using System.Collections.Generic;
using System.Text;

namespace expense.manager.Data
{
    public static class SeedProvider
    {


        public static List<InitialCategory> GetDefaultCategories()
        {
            var result = new List<InitialCategory>()
            {
                InitialCategory.Create(AppContent.Food,
                    new List<InitialCategory>()
                    {
                        InitialCategory.Create(AppContent.Grossery),
                        InitialCategory.Create(AppContent.Restaurant)
                    }

                ),
                InitialCategory.Create(AppContent.Entertainment,
                    new List<InitialCategory>()
                    {
                        InitialCategory.Create(AppContent.Movies),
                        InitialCategory.Create(AppContent.Bars)
                    }

                ),

                InitialCategory.Create(AppContent.Ponctual,
                    new List<InitialCategory>()
                    {
                        InitialCategory.Create(AppContent.Trips),
                        InitialCategory.Create(AppContent.Gifts)
                    }

                ),

                InitialCategory.Create(AppContent.Recurring,
                    new List<InitialCategory>()
                    {
                        InitialCategory.Create(AppContent.Housing),
                        InitialCategory.Create(AppContent.Bills)
                    }

                ),

            };

            return result;
        }



    }

    public class InitialCategory
    {
        public string Name { get; set; }

        public List<InitialCategory> SubCategories { get; set; }

        public static InitialCategory Create(string name, List<InitialCategory> subCategories = null)
        {
            return new InitialCategory()
            {
                Name = name,
                SubCategories = subCategories
            };
        }
    }
}
