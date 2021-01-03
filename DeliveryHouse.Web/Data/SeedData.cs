using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Enums;
using DeliveryHouse.Web.Data.Entities;
using DeliveryHouse.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Data
{
    public class SeedData
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedData(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoryPoductAsync();
            await CheckRoleAsync();
            await CheckUserAsync("Yoshi", "Shimizu", "yoshimizu2015@gmail.com", "999 999 999", "Urb. Lima D-12 Casa Grande", UserType.Admin);
        }

        private async Task<User> CheckUserAsync(string firsName, string lastName, string email, string phone, string address, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);

            if (user == null)
            {
                user = new User
                {
                    FirstName = firsName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    UserType = userType,
                    City = _context.Cities.FirstOrDefault()
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmatioTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRoleAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCategoryPoductAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category
                {
                    Name = "Electrodomésticos",
                    ImageCategory = $"~/images/categories/categoria electrodomesticos.jfif",
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Name = "Refrigeradora Family Hub",
                            ImageProduct = $"~/images/products/refrigeradora family hub.png",
                            Description = "Abastécete y almacena todos tus alimentos de forma ordenada con un espacio de almacenamiento" +
                                          " de ↑600 litros*. La exclusiva tecnología SpaceMax ™ permite que las paredes sean mucho más" +
                                          " finas con un aislamiento de alta eficiencia. Por lo tanto, crea más espacio de almacenamiento" +
                                          " sin aumentar las dimensiones externas ni comprometer la eficiencia energética.",
                            IsActive = true,
                            Price = 6400.99m
                        },
                        new Product
                        {
                            Name = "Cocina Coldex",
                            ImageProduct = $"~/images/products/cocina.jpg",
                            Description = "Lúcete con las mejores comidas y postres gracias a Coldex. La cocina CX 602 / FP101S11SA cuentan" +
                                          " con un total de 4 quemadores de distintos tamaños que se adaptarán a todas tus recetas." +
                                          " Tiene además un amplio horno de 55 litros de capacidad, para que puedas engreír a tu familia" +
                                          " con exquisitas y saludables recetas horneadas.",
                            IsActive = true,
                            Price = 3400.99m
                        }
                    }
                });
                _context.Categories.Add(new Category
                {
                    Name = "Electros",
                    ImageCategory = $"~/images/categories/categoria electros.jfif",
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Name = "Marciano de Lúcma",
                            ImageProduct = $"~/images/products/lucma.jpg",
                            Description = "Es temporada de lúcuma, una fruta deliciosa que se presta para hacer varios postres y este helado " +
                                          "rico y fácil.. Ingredientes 02 unidades lúcuma 1 tz leche frappé al gusto Azúcar o miel",
                            IsActive = true,
                            Price = 1.00m
                        },
                        new Product
                        {
                            Name = "Marciano de Coco",
                            ImageProduct = $"~/images/products/coco.jfif",
                            Description = "Refréscate en estos días de calor, aprende a preparar unos deliciosos bolis caseros." +
                                          " Puedes usar la ...Ingredientes 1 liter de leche 3 cucharadas de esencia de vainilla " +
                                          "(o el saborizante de tu preferencia) 0.75 de taza de azúcar",
                            IsActive = true,
                            Price = 1.00m
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Perú",
                    ImageCountry = $"~/images/countries/bandera peruana.png",
                    Departments = new List<Department>
                    {
                        new Department
                        {
                            Name = "La Libertad",
                            Cities = new List<City>
                            {
                                new City
                                {
                                    Name = "Trujillo"
                                },
                                new City
                                {
                                    Name = "La Esperanza"
                                },
                                new City
                                {
                                    Name = "El Porvenir"
                                }
                            }
                        },
                        new Department
                        {
                            Name = "Ancash",
                            Cities = new List<City>
                            {
                                new City
                                {
                                    Name = "Huaráz"
                                },
                                new City
                                {
                                    Name = "Catác"
                                },
                                new City
                                {
                                    Name = "Caráz"
                                }
                            }
                        }
                    }

                });
                _context.Countries.Add(new Country
                {
                    Name = "Ecuador",
                    ImageCountry = $"~/images/countries/bandera ecuador.png",
                    Departments = new List<Department>
                    {
                        new Department
                        {
                            Name = "Chimborazo",
                            Cities = new List<City>
                            {
                                new City
                                {
                                    Name = "Riobamba"
                                },
                                new City
                                {
                                    Name = "Cantones"
                                }
                            }
                        },
                        new Department
                        {
                            Name = "Pichincha",
                            Cities = new List<City>
                            {
                                new City
                                {
                                    Name = "Quito"
                                }
                            }
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}