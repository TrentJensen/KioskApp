using KioskApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(Categories.Select(c => c.Value));
            }

            context.SaveChanges();

            if (!context.Products.Any())
            {
                context.Products.AddRangeAsync
                    (
                        new Product
                        {
                            Name = "EnviroCloth",
                            Description = "An Anti-microbial microfiber cloth for everything.",
                            Price = 17.99M,
                            VendorCost = 11.70M,
                            Color = ColorEnum.Graphite,
                            Image = @"envirocloth-graphite.jpg",
                            IsLimitedEdition = false,
                            UnitsInStock = 10,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 1
                        },
                        new Product
                        {
                            Name = "Body Pack",
                            Description = "A 3-pack body cleaning microfiber cloth.",
                            Price = 19.99M,
                            VendorCost = 13.00M,
                            Color = ColorEnum.Tranquil,
                            Image = @"three-pack-tranquil.jpg",
                            IsLimitedEdition = false,
                            UnitsInStock = 9,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 3
                        },
                        new Product
                        {
                            Name = "Dusting Mitt",
                            Description = "Thick terrycloth dusting mitt",
                            Price = 18.47M,
                            VendorCost = 12.00M,
                            Color = ColorEnum.Green,
                            Image = @"dusting-mitt-green.jpg",
                            UnitsInStock = 3,
                            IsLimitedEdition = false,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 4
                        },
                        new Product
                        {
                            Name = "Blue Diamond Bathroom Cleaner",
                            Description = "Cleaning enzyme for bathrooms",
                            Price = 29.99M,
                            VendorCost = 19.50M,
                            Color = ColorEnum.None,
                            Image = @"blue-diamond-bathroom-cleaner.jpg",
                            UnitsInStock = 0,
                            IsLimitedEdition = false,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 4
                        },
                        new Product
                        {
                            Name = "Cleaning Paste",
                            Description = "Non-abrasive miracle paste",
                            Price = 29.99M,
                            VendorCost = 19.50M,
                            Color = ColorEnum.None,
                            Image = @"cleaning-paste.jpg",
                            UnitsInStock = 3,
                            IsLimitedEdition = false,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 5
                        },
                        new Product
                        {
                            Name = "Kids Pet to Dry Kitten",
                            Description = "Hand drying microfiber for children",
                            Price = 19.99M,
                            VendorCost = 13.00M,
                            Color = ColorEnum.Graphite,
                            Image = @"kids-pet-to-dry-kitten.jpg",
                            UnitsInStock = 3,
                            IsLimitedEdition = false,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 7
                        },
                        new Product
                        {
                            Name = "Envirowand",
                            Description = "A wand to dust hard-to-reach places",
                            Price = 31.99M,
                            VendorCost = 20.80M,
                            Color = ColorEnum.None,
                            Image = @"mop-base-large.jpg",
                            UnitsInStock = 0,
                            IsLimitedEdition = false,
                            VendorId = 1,
                            VendorGuid = "3a9a4cff-7471-450b-9ded-a8b3ef89e926",
                            CategoryId = 1
                        }
                    );
            }

			context.SaveChanges();
		}

        private static Dictionary<string, Category> _categories;

        public static Dictionary<string, Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    var genresList = new Category[]
                    {
                        new Category {CategoryName = "Microfiber"},
                        new Category {CategoryName = "Floor Systems"},
                        new Category {CategoryName = "Bath and Body Care"},
                        new Category {CategoryName = "Home Essentials"},
                        new Category {CategoryName = "Kitchen Cleaning"},
                        new Category {CategoryName = "Personal Care"},
                        new Category {CategoryName = "Family"}
                    };

                    _categories = new Dictionary<string, Category>();

                    foreach (Category genre in genresList)
                    {
                        Categories.Add(genre.CategoryName, genre);
                    }
                }
                return _categories;
            }
        }
    }
}
