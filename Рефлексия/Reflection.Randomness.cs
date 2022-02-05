using System;
using System.Linq;
using System.Reflection;

namespace Reflection.Randomness
{
    public class Generator<T> where T : new()
    {
        private static PropertyInfo[] properties;
        private static FromDistribution[] distributions;

        static Generator()
        {
            properties = typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(FromDistribution), true).Length > 0)
                .ToArray();

            distributions = properties
                .Select(p => (FromDistribution)Attribute.GetCustomAttribute(p, typeof(FromDistribution)))
                .ToArray();
            if (properties.Length != distributions.Length)
                throw new Exception();
        }

        public T Generate(Random rnd)
        {
            T obj = new T();

            for (var i = 0; i < properties.Length; i++)
            {
                var d = distributions[i];
                
                try
                {
                    var distribution = (IContinuousDistribution)Activator
                        .CreateInstance(d.DistributionType, d.Parameters);
                    properties[i].SetValue(obj, distribution.Generate(rnd));
                }
                catch
                {
                    throw new ArgumentException(d.DistributionType.Name);
                }
            }
            return obj;
        }
    }

    public class FromDistribution : Attribute
    {
        public Type DistributionType { get; set; }
        public  object[] Parameters { get; set; }

        public FromDistribution(Type distributionType, params object[] parameters)
        {
            this.DistributionType = distributionType;
            this.Parameters = parameters;
        }
    }
}