﻿// <auto-generated />
using AccReporting.Server.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace AccReporting.Server.OptimizedModels
{
    [DbContext(typeof(ApplicationDbContext))]
    public partial class ApplicationDbContextModel : RuntimeModel
    {
        static ApplicationDbContextModel()
        {
            var model = new ApplicationDbContextModel();
            model.Initialize();
            model.Customize();
            _instance = model;
        }

        private static ApplicationDbContextModel _instance;
        public static IModel Instance => _instance;

        partial void Initialize();

        partial void Customize();
    }
}
