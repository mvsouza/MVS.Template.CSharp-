using Autofac;
using MVS.Template.CSharp.Application.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MVS.Template.CSharp.Infrastructure
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register all the Command classes (they implement IAsyncRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(SolveCalculusCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));


            builder.Register<SingleInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.Register<MultiInstanceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();

                return t =>
                {
                    var resolved = (IEnumerable<object>)componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
                    return resolved;
                };
            });
            
        }
    }
}
