//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Tilo.Models
//{
//    public class SampleContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//        public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            // получаем конфигурацию из файла appsettings.json
//            ConfigurationBuilder builder = new ConfigurationBuilder();
//            builder.SetBasePath(Directory.GetCurrentDirectory());
//            builder.AddJsonFile("appsettings.json");
//            IConfigurationRoot config = builder.Build();

//            // получаем строку подключения из файла appsettings.json
//            string connectionString = config.GetConnectionString("DefaultConnection");
//            optionsBuilder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));
//            return new ApplicationDbContext(optionsBuilder.Options);
//        }
//    }
//}
