// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectType.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using DEHPCommon.MappingEngine.Utilities;

    using NLog;

    public class ObjectType : DynamicObject
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger(typeof(ObjectType));

        public Dictionary<string, (Type Type, object Instance)> Properties { get; }= new Dictionary<string, (Type Type, object Instance)>();

        public Type InternalType { get; set; }

        public IMappingRule[] MappingRules { get; }
        
        public ObjectType(object baseObject, params IMappingRule[] properties)
        {
            baseObject = baseObject ??
                              throw new ArgumentNullException(nameof(baseObject), $"{nameof(baseObject)} cannot be null");

            this.MappingRules = properties;

            foreach (var property in properties)
            {
                this.Build(baseObject, property);
            }
        }

        /// <summary>
        /// Builds the properties of this represented <see cref="object"/>
        /// </summary>
        /// <param name="baseObject">The <see cref="object"/></param>
        /// <param name="property"></param>
        private void Build(object baseObject, IMappingRule property)
        {
            var type = baseObject.GetType();

            var value = type.GetProperty(property.DstPropertyName)?.GetValue(baseObject, null) ??
                        type.GetProperties().FirstOrDefault(x => x.Name == property.DstPropertyName);

            if (!MappingUtility.IsSimpleType(property.InternalType))
            {
                this.Properties.Add(property.DstPropertyName, (value?.GetType(), new ObjectType(value, property.NestedRules)));
            }
            else if (value?.GetType() == property.InternalType)
            {
                this.Properties.Add(property.DstPropertyName, (property.InternalType, value));
            }
            else
            {
                this.logger.Info($"The object of type {nameof(type)} could not be resolved or it's value could not be computed");
                throw new TypeInitializationException(type.FullName, null);
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (this.Properties.ContainsKey(binder.Name))
            {
                var type = this.Properties[binder.Name].Type;
                
                if (value.GetType() == type)
                {
                    this.Properties[binder.Name] = (type, value);
                    return true;
                }

                this.logger.Info($"Value {value} is not of type {type.Name}");
                throw new TypeInitializationException(type.FullName, null);
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Properties[binder.Name].Instance;
            return true;
        }
    }
}
