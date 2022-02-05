using Ddd.Taxi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ddd.Infrastructure
{
    public class ValueType<T>
    {
        protected readonly List<PropertyInfo> properties;

        public ValueType()
        {
            properties = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)  
                .ToList();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            foreach (var property in properties)
            {
                var thisValue = property.GetValue(this );
                var objectValue = property.GetValue(obj);
                if (thisValue == null && objectValue == null)
                    return true;
                if (!thisValue.Equals(objectValue))
                    return false;
            }
            return true;
        }

        public override int GetHashCode() 
        {
            int hash = 0;
            unchecked
            {
                foreach (var property in properties)
                {
                    hash += (property.GetValue(this).GetHashCode()
                            * property.Name.GetHashCode()) * property.GetHashCode();
                }
            }
            return hash;
        }

        public bool Equals(PersonName name) => Equals((object)name);

        public override string ToString()
        {
            return GetType().Name + "(" + string
                .Join("; ", properties
                .OrderBy((x) => x.Name)
                .Select((x, y) => x.Name + ": " + x.GetValue(this))) + ")";
        }
    }
}