﻿using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Vehicles.Pn.Abstractions;
using Vehicles.Pn.Infrastructure.Data;
using Vehicles.Pn.Infrastructure.Data.Factories;
using Vehicles.Pn.Services;

namespace Vehicles.Pn
{
    public class EformVehiclesPlugin : IEformPlugin
    {
        public string GetName() => "Microting Vehicles plugin";
        public string ConnectionStringName() => "EFormVehiclesPnConnection";
        public string PluginPath() => PluginAssembly().Location;

        public Assembly PluginAssembly()
        {
            return typeof(EformVehiclesPlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IVehiclesService, VehiclesService>();
            services.AddSingleton<IVehicleLocalizationService, VehicleLocalizationService>();
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<VehiclesPnDbContext>(o => o.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(PluginAssembly().FullName)));

            var contextFactory = new VehiclesPnContextFactory();
            var context = contextFactory.CreateDbContext(new[] {connectionString});
            context.Database.Migrate();
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
        }

        public MenuModel HeaderMenu()
        {
            var result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                Name = "Vehicles",
                E2EId = "",
                Link = "/plugins/vehicles-pn"
            });
            return result;
        }
        
        public void SeedDatabase(string connectionString)
        {
        }
    }
}
