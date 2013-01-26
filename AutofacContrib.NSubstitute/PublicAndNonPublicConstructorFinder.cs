using System;
using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace AutofacContrib.NSubstitute
{
    internal class PublicAndNonPublicConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            var finder = new BindingFlagsConstructorFinder(BindingFlags.Public | BindingFlags.NonPublic);
            return finder.FindConstructors(targetType);
        }
    }
}
