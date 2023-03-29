﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace TatBlog.WebApi.Validations
{
    public static class FluentValidationDependencyInjection
    {
        public static WebApplicationBuilder ConfigureFluentValidation(
            this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly() );
            return builder;
        }
    }
}
