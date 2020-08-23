﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{

    public class RestClientClassAttribute : KnifeAttribute
    {
        public string DefaultBaseAddress { get; set; }
        public Type InjectType { get; set; }
        public RestClientClassAttribute() : base(typeof(RestClient))
        {

        }
        public RestClientClassAttribute(string defaultBaseAddress) : base(typeof(RestClient))
        {
            this.DefaultBaseAddress = defaultBaseAddress;
        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));

            var injectType = this.InjectType ?? DeduceInjectType(declareType);
            if (injectType != null)
            {
                services.AddTransient(injectType, (sp) => sp.GetService(declareType));
            }
            var method = typeof(RestClientClassAttribute).GetMethod(nameof(RegisteHttpClientAndRestInfo), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });
        }
        private void RegisteHttpClientAndRestInfo<T>(IServiceCollection services)
           where T : class
        {
            services.AddHttpClient<T>();
            services.AddSingleton<RestInfo<T>>((sp) =>
            {
                var options = sp.GetRequiredService<ApiServicesOptions>();
                if (options.Services.TryGetValue(typeof(T).FullName, out ServiceOptions serviceOptions))
                {
                    return new RestInfo<T>
                    {
                        BaseAddress = serviceOptions.BaseAddress,
                        Headers = serviceOptions.Headers,
                    };
                }
                return new RestInfo<T>()
                {
                };
            });
        }

        private Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length != 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}'.");
            }
            return allInterfaces.FirstOrDefault();
        }
    }
}
