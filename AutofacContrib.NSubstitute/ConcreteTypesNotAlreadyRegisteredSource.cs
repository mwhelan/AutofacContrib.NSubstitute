using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace AutofacContrib.NSubstitute
{
    /// <summary>
    /// Provides registrations on-the-fly for any concrete type not already registered with
    /// the container, having a public or non-public constructor.
    /// </summary>
    public class ConcreteTypesNotAlreadyRegisteredSource : IRegistrationSource
    {
        readonly Func<Type, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteTypesNotAlreadyRegisteredSource"/> class.
        /// </summary>
        public ConcreteTypesNotAlreadyRegisteredSource()
            : this(t => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteTypesNotAlreadyRegisteredSource"/> class.
        /// </summary>
        /// <param name="predicate">A predicate that selects types the source will register.</param>
        public ConcreteTypesNotAlreadyRegisteredSource(Func<Type, bool> predicate)
        {
            _predicate = Enforce.ArgumentNotNull(predicate, "predicate");
        }

        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used
        /// by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (registrationAccessor == null)
            {
                throw new ArgumentNullException("registrationAccessor");
            }
            var ts = service as TypedService;
            if (ts == null ||
                !ts.ServiceType.IsClass ||
                ts.ServiceType.IsSubclassOf(typeof(Delegate)) ||
                ts.ServiceType.IsAbstract ||
                !_predicate(ts.ServiceType) ||
                registrationAccessor(service).Any())
                return Enumerable.Empty<IComponentRegistration>();

            return new[]
            {
                RegistrationBuilder
                    .ForType(ts.ServiceType)
                    .FindConstructorsWith(new PublicAndNonPublicConstructorFinder())
                    .CreateRegistration()
            };
        }

        /// <summary>
        /// Gets whether the registrations provided by this source are 1:1 adapters on top
        /// of other components (I.e. like Meta, Func or Owned.)
        /// </summary>
        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "ConcreteTypesNotAlreadyRegisteredSource";
        }
    }

    static class Enforce
    {
        /// <summary>
        /// Enforce that an argument is not null. Returns the
        /// value if valid so that it can be used inline in
        /// base initialiser syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns><paramref name="value"/></returns>
        public static T ArgumentNotNull<T>(T value, string name)
            where T : class
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (value == null)
                throw new ArgumentNullException(name);

            return value;
        }
    }
}
