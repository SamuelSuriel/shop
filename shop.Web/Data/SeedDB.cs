namespace shop.Web.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Helpers;

    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private readonly Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            await this.userHelper.CheckRoleAsync("Admin");
            await this.userHelper.CheckRoleAsync("Customer");

            if (!this.context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Santo Domingo" });
                cities.Add(new City { Name = "Santiago" });
                cities.Add(new City { Name = "Punta Cana" });

                this.context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Republica Dominicana"
                });

                await this.context.SaveChangesAsync();
            }

            //Add user
            var user = await this.userHelper.GetUserByEmailAsync("samueldc29@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Samuel",
                    LastName = "Ramirez",
                    Email = "samueldc29@gmail.com",
                    UserName = "samueldc29@gmail.com",
                    Address = "Calle Primera Calle Segunda",
                    PhoneNumber = "809 481 2273",
                    CityId = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await this.userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await this.userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await this.userHelper.GenerateEmailConfirmationTokenAsync(user);
                await this.userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await this.userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await this.userHelper.AddUserToRoleAsync(user, "Admin");
            }

            // Add products
            if (!this.context.Products.Any())
            {
                this.AddProduct("AirPods", user);
                this.AddProduct("Blackmagic eGPU Pro", user);
                this.AddProduct("iPad Pro", user);
                this.AddProduct("iMac", user);
                this.AddProduct("iPhone X", user);
                this.AddProduct("iWatch Series 4", user);
                await this.context.SaveChangesAsync();
            }
        }
        private void AddProduct(string name, User user)
        {
            this.context.Products.Add(new Product
            {
                Name = name,
                Price = this.random.Next(1000),
                IsAvailabe = true,
                Stock = this.random.Next(100),
                User = user,
                ImageUrl = $"~/Images/Products/{name}.png"
            });

        }
    }
}