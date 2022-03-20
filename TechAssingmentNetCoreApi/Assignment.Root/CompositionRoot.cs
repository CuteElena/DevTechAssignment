using Assignment.Business;
using Assignment.Business.Impl;
using Assignment.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechAssignmentWebApi.Domain.DataAccess.Impl;

namespace Assignment.Root
{
    public class CompositionRoot
    {
        public static void InjectDependencies(IServiceCollection services)
        {

            services.AddScoped(typeof(IFileDataAccess), typeof(FileDataAccess));
            services.AddScoped<IFileBusinessLogic, FileBusinessLogic>();

        }
    }
}
