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
                    PhoneNumber = "8044812273"
                };

                var result = await this.userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await this.userHelper.AddUserToRoleAsync(user, "Admin");
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