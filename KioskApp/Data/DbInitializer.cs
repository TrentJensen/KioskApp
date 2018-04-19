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
            if(!context.Products.Any())
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
                            VendorId = 1
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
                            VendorId = 1
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
                            VendorId = 1
                        },
                        new Product
                        {
                            Name = "Dusting Mitt Test",
                            Description = "Thick terrycloth dusting mitt",
                            Price = 18.47M,
                            VendorCost = 12.00M,
                            Color = ColorEnum.Green,
                            Image = @"dusting-mitt-green.jpg",
                            UnitsInStock = 5,
                            IsLimitedEdition = false,
                            VendorId = 2
                        }
                    );
                    context.SaveChanges();
            }
        }
    }
}
