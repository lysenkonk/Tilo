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

            //Category lingue = new Category("Нижнее бельё и домашняя одежда", null);


            //if (context.Categories.FirstOrDefault(p => p.Name == "Боди") == null)
            //{
            //    context.Categories.Add(new Category("Боди", lingue));
            //}
            //if (context.Categories.FirstOrDefault(p => p.Name == "Пеньюары") == null)
            //{
            //    context.Categories.Add(new Category("Пеньюары", lingue));
            //}
            //if (context.Categories.FirstOrDefault(p => p.Name == "Ролевое бельё") == null)
            //{
            //    context.Categories.Add(new Category("Ролевое бельё", lingue));
            //}
            //if (context.Categories.FirstOrDefault(p => p.Name == "Пижамы") == null)
            //{
            //    context.Categories.Add(new Category("Пижамы", lingue));
            //}

            //if (context.Categories.FirstOrDefault(p => p.Name == "Трусики") == null)
            //{
            //    context.Categories.Add(new Category("Трусики", lingue));
            //}

            //if (context.Categories.FirstOrDefault(p => p.Name == "Бра") == null)
            //{
            //    context.Categories.Add(new Category("Бра", lingue));
            //}
            //if (context.Categories.FirstOrDefault(p => p.Name == "Аксессуары") == null)
            //{
            //    context.Categories.Add(new Category("Аксессуары", lingue));
            //}
            //if (context.Categories.FirstOrDefault(p => p.Name == "Подарочные сертификаты") == null)
            //{
            //    context.Categories.Add(new Category("Подарочные сертификаты", lingue));
            //}

            context.SaveChanges();


            if (!context.Products.Any())
            {
                //context.Products.AddRange(
                //    new Product
                //    {
                //        Name = "Magnolia",
                //        Category = context.Categories.FirstOrDefault(p => p.Name == "Suit") ?? new Category("Suit"),
                //        Color = "white",
                //        Price = 350,
                //        Sizes = new List<Size> { new Size ("S") },
                //        Description = "Форма низа — бразильяна. Низ выполнен из кружева спереди и стрейч-тюля сзади. Цвет: жемчужно - розовый Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //        Images = new List<FileModel> { new FileModel("1.jpg") }
                //    },

                //     new Product
                //     {
                //         Name = "Magnolia",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "black",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("75A") },
                //         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("19.jpg") }
                //     },
                //     new Product
                //     {
                //       Name = "Moss",
                //       Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //       Color = "green",
                //       Price = 350,
                //         Sizes = new List<Size> { new Size("S") },
                //         Description = "Форма низа — бразильяна. Низ выполнен из кружева спереди и стрейч-тюля сзади. Цвет: жемчужно - розовый Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //       Images = new List<FileModel> { new FileModel("39.jpg") }

                //     },

                //     new Product
                //     {
                //         Name = "Moss",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "pink",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("80 B") },
                //         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("1.jpg") }

                //     },

                //    new Product
                //     {
                //       Name = "Unnamed 2.0 Black",
                //        Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //        Color = "green",
                //        Price = 350,
                //        Sizes = new List<Size> { new Size("XS") },
                //        Description = "Высокий низ, изготовленный из прозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //        Images = new List<FileModel> { new FileModel("19.jpg") }

                //    },

                //     new Product
                //     {
                //         Name = "Unnamed 2.0 Black",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "beige",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("80 C") },
                //         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("39.jpg") }

                //     },
                //     new Product
                //     {
                //       Name = "Black Rose",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //         Color = "black",
                //         Price = 350,
                //         Sizes = new List<Size> { new Size("M") },
                //         Description = "Форма низа — бесшовная бразильяна. Низ выполнен из полупрозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("19.jpg") }

                //     },

                //     new Product
                //     {
                //         Name = "Black Rose",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "black",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("80 A") },
                //         Description = "Классическая модель бра на кости, изготовлена из прозрачного стрейч-тюля. Фурнитура — металл, цвет — чёрный. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("1.jpg") }

                //     }
                //     ,
                //     new Product
                //     {
                //       Name = "Moss",
                //       Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //       Color = "green",
                //       Price = 350,
                //         Sizes = new List<Size> { new Size("L") },
                //         Description = "Форма низа — бразильяна. Низ выполнен из кружева спереди и стрейч-тюля сзади. Цвет: жемчужно - розовый Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //       Images = new List<FileModel> { new FileModel("19.jpg") }

                //     },

                //     new Product
                //     {
                //         Name = "Moss",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "pink",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("75 C") },
                //         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("19.jpg") }

                //     },

                //    new Product
                //     {
                //       Name = "Unnamed 2.0 Black",
                //        Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //        Color = "green",
                //        Price = 350,
                //        Sizes = new List<Size> { new Size("80 C") },
                //        Description = "Высокий низ, изготовленный из прозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //        Images = new List<FileModel> { new FileModel("39.jpg") }

                //    },

                //     new Product
                //     {
                //         Name = "Unnamed 2.0 Black",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "beige",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("80 C") },
                //         Description = "Классическая модель бра на кости с чашкой из трех частей и кружевным фестоном. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("1.jpg") }

                //     },
                //     new Product
                //     {
                //       Name = "Black Rose",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Бра") ?? new Category("Бра"),
                //         Color = "black",
                //         Price = 350,
                //         Sizes = new List<Size> { new Size("80 C") },
                //         Description = "Форма низа — бесшовная бразильяна. Низ выполнен из полупрозрачного стрейч-тюля. Форма низа — стринги. Цвет: черный Материал: 70 % — полиэстер, 20 % — хлопок, 10 % — эластан Подкладка: 100 % хлопок Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("19.jpg") }

                //     },

                //     new Product
                //     {
                //         Name = "Black Rose",
                //         Category = context.Categories.FirstOrDefault(p => p.Name == "Трусики") ?? new Category("Трусики"),
                //         Color = "black",
                //         Price = 1350,
                //         Sizes = new List<Size> { new Size("75 C") },
                //         Description = "Классическая модель бра на кости, изготовлена из прозрачного стрейч-тюля. Фурнитура — металл, цвет — чёрный. Фурнитура — металл, цвет — серебро.Цвет: жемчужно - розовый Материал: 70 % — полиэсте 20 % — хлопок,"
                //         + "10 % — эластан" + "Ручная стирка, в холодной воде",
                //         Images = new List<FileModel> { new FileModel("39.jpg") }
                //     }
                //     );
                context.SaveChanges();
            }
        }
    }
}
