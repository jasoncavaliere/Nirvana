﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TechFu.Nirvana.CQRS.Util;
using TechFu.Nirvana.Util.Extensions;

namespace TechFu.Nirvana.Configuration
{


    public static class NirvanaSetup
    {
        public static string UiNotificationHubName { get; set; } = "UiNotifications";
        public static string AssemblyFolder { get; set; }
        public static string ControllerAssemblyName { get; set; }
        public static string ControllerRootNamespace { get; set; }
        public static Type RootType { get; internal set; }
        public static Type AggregateAttributeType { get; internal set; }
        public static string TaskIdentifierProperty { get; internal set; }
        public static  Func<string, object, bool> AttributeMatchingFunction { get; internal set; }
        public static  string[] AssemblyNameReferences { get; internal set; }
        public static ControllerType[] ControllerTypes { get; internal set; }
        public static Func<Type, Object> GetService { get; internal set; }

        //processing configuration
        public static MediationStrategy CommandMediationStrategy { get; internal  set; } = MediationStrategy.InProcess;
        public static MediationStrategy QueryMediationStrategy { get; internal  set; } = MediationStrategy.InProcess;
        public static MediationStrategy UiNotificationMediationStrategy { get; internal  set; } = MediationStrategy.None;


        //Configuration for task processor applications
        public static MediationStrategy RecieverMediationStrategy { get; internal set; } = MediationStrategy.None;

        //How to handle sub calls within this app 
        // FOr instance, we get a call from a command processor via web
        // we want to call this in proc and syncronously
        //somewhere in the command we want to call a child command  - this may or may not be something 
        //we want to run in proc
        public static ChildMediationStrategy ChildMediationStrategy { get; internal  set; } = ChildMediationStrategy.Synchronous;


        //Called On Configuration build
        public static string[] RootNames { get; internal set; }
        public static IDictionary<string, NirvanaTypeDefinition[]> QueryTypes{ get; internal set; }
        public static IDictionary<string, NirvanaTypeDefinition[]> CommandTypes{ get; internal set; }
        public static IDictionary<string, NirvanaTypeDefinition[]> UiNotificationTypes{ get; internal set; }


        //TODO - replace CqrsUtils.GetRootTypeName  with this and use it in that function to speed up
        //public static IDictionary<Type, string> TypeRootNames{ get; internal set; }


        public static NirvanaConfigurationHelper Configure()
        {
            return new NirvanaConfigurationHelper();
        }

        public static string ShowSetup()
        {
            var builder = new StringBuilder();
            var propertyInfos = typeof(NirvanaSetup).GetProperties(BindingFlags.Public | BindingFlags.Static);
            var fieldInfos = typeof(NirvanaSetup).GetFields(BindingFlags.Public | BindingFlags.Static);

            propertyInfos.ForEach(x =>
            {

                builder.AppendLine($"{x.Name} : {x.GetValue(null)}");
            });
            fieldInfos.ForEach(x =>
            {
                builder.AppendLine($"{x.Name} : {x.GetValue(null)}");
            });

            return builder.ToString();

        }


        public static NirvanaTypeDefinition FindTypeDefinition(Type getType)
        {
            //TODO - faster here?
            var type = CheckTypes(getType, CommandTypes);
            if (type != null)
            {
                return type;
            }

            type = CheckTypes(getType,UiNotificationTypes);
            if (type != null)
            {
                return type;
            }
            return CheckTypes(getType, QueryTypes);
        }

        private static NirvanaTypeDefinition CheckTypes(Type getType, IDictionary<string, NirvanaTypeDefinition[]> nirvanaTypeDefinitionses)
        {
            return nirvanaTypeDefinitionses.Keys.Select(key => LookForType(nirvanaTypeDefinitionses[key], getType)).FirstOrDefault(type => type != null);
        }

        private static NirvanaTypeDefinition LookForType(NirvanaTypeDefinition[] commandType, Type getType)
        {
            return commandType.FirstOrDefault(x => x.Matches(getType));
        }
    }

    public class NirvanaTypeDefinition
    {
        public Type TaskType { get; set; }
        public Type NirvanaActionType { get; set; }
        public string UniqueName { get; set; }
        public string TypeCorrelationId { get; set; }

        public bool Matches(Type testType)
        {
            return testType == TaskType;
        }
    }
}