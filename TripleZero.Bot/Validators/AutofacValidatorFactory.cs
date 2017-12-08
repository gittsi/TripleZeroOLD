using Autofac;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Bot.Infrastructure
{
    //public class AutofacValidatorFactory : ValidatorFactoryBase
    //{
    //    private readonly IComponentContext _context;

    //    public AutofacValidatorFactory(IComponentContext context)
    //    {
    //        _context = context;
    //    }

    //    public override IValidator CreateInstance(Type validatorType)
    //    {
    //        object instance;
    //        if (_context.TryResolve(validatorType, out instance))
    //        {
    //            var validator = instance as IValidator;
    //            return validator;
    //        }

    //        return null;
    //    }
    //}
    public class AutofacValidatorFactory2 : ValidatorFactoryBase
    {
        private readonly IComponentContext _context;

        public AutofacValidatorFactory2(IComponentContext context)
        {
            _context = context;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            object instance;
            if (_context.TryResolve(validatorType, out instance))
            {
                var validator = instance as IValidator;
                return validator;
            }

            return null;
        }
    }


    public class AutofacValidatorFactory : IValidatorFactory
    {
        private readonly IComponentContext container;

        public AutofacValidatorFactory(IComponentContext container)
        {
            this.container = container;
        }

        public IValidator<T> GetValidator<T>()
        {
            return (IValidator<T>)GetValidator(typeof(T));
        }

        public IValidator GetValidator(Type type)
        {
            var genericType = typeof(IValidator<>).MakeGenericType(type);
            object validator;
            if (container.TryResolve(genericType, out validator))
                return (IValidator)validator;

            return null;
        }

       
    }

    //public class AutofacValidatorFactory2 : ValidatorFactoryBase
    //{
    //    private readonly IContainer container;

    //    public AutofacValidatorFactory2(IContainer container)
    //    {
    //        this.container = container;
    //    }

    //    public override IValidator CreateInstance(Type validatorType)
    //    {
    //        IValidator validator = container.ResolveOptionalKeyed<IValidator>(validatorType);
    //        return validator;
    //    }
    //}


    //public class ValidatorFactory : ValidatorFactoryBase
    //{
    //    private readonly IComponentContext context;

    //    public ValidatorFactory(IComponentContext context)
    //    {
    //        this.context = context;
    //    }

    //    public override IValidator CreateInstance(Type validatorType)
    //    {
    //        return context.Resolve(validatorType) as IValidator;
    //    }
    //}
}
