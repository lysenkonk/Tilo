using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilo.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            if(!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Magnolia",
                        Category = new Category("Трусы"),
                        Price = 350,
                        Description = "Форма низа — бразильяна. Низ выполнен из кружева спереди и стрейч-тюля сзади. Цвет: жемчужно - розовый Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде"
                    },

                     new Product
                     {
                         Name = "Magnolia",
                         Category = new Category("Бра"),
                         Price = 1350,
                         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                         + "10 % — эластан" + "Ручная стирка, в холодной воде"
                     },
                     new Product
                     {
                       Name = "Moss",
                       Category = new Category("Трусы"),
                       Price = 350,
                       Description = "Форма низа — бразильяна. Низ выполнен из кружева спереди и стрейч-тюля сзади. Цвет: жемчужно - розовый Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде"
                      },

                     new Product
                     {
                         Name = "Moss",
                         Category = new Category("Бра"),
                         Price = 1350,
                         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                         + "10 % — эластан" + "Ручная стирка, в холодной воде"
                     },

                    new Product
                     {
                       Name = "Unnamed 2.0 Black",
                       Category = new Category("Трусы"),
                       Price = 350,
                       Description = "Высокий низ, изготовленный из прозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде"
                    },

                     new Product
                     {
                         Name = "Unnamed 2.0 Black",
                         Category = new Category("Бра"),
                         Price = 1350,
                         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                         + "10 % — эластан" + "Ручная стирка, в холодной воде"
                     },
                     new Product
                     {
                       Name = "Black Rose",
                       Category = new Category("Трусы"),
                       Price = 350,
                       Description = "Форма низа — бесшовная бразильяна. Низ выполнен из полупрозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде"
                     },

                     new Product
                     {
                         Name = "Black Rose",
                         Category = new Category("Бра"),
                         Price = 1350,
                         Description = "Классическая модель бра на кости, изготовлена из прозрачного стрейч-тюля. Фурнитура — металл, цвет — чёрный. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                         + "10 % — эластан" + "Ручная стирка, в холодной воде"
                     }
                     );
                context.SaveChanges();
            }
        }
    }
}
