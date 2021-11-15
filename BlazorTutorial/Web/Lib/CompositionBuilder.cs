using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Schema;
using BlazorTutorial.Web.Lib.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTutorial.Web.Lib
{

    public class BuildCascade<Exten>
    {

        public BuildCascade<Exten> TryPickInterface<I>(out I iface) where I : Exten
        {
            iface = (I) Item;
            return this;
        }

        public BuildCascade<Exten> DoOtherthing()
        {
            return this;
        }

        public Exten Item { get; set; }
    }
    
    public class CompositionBuilder
    {

        private void DoInterfaceBuild(TypeBuilder tBuilder, Type tp)
        {
            tBuilder.AddInterfaceImplementation(tp);
            foreach (var v in tp.GetProperties())
            {
                var field = tBuilder.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
                var property = tBuilder.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                var getter = tBuilder.DefineMethod("get_" + v.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual,
                    v.PropertyType, new Type[0]);
                var setter = tBuilder.DefineMethod("set_" + v.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null,
                    new Type[] {v.PropertyType});
                var getGenerator = getter.GetILGenerator();
                var setGenerator = setter.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, field);
                getGenerator.Emit(OpCodes.Ret);
                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Stfld, field);
                setGenerator.Emit(OpCodes.Ret);
                property.SetGetMethod(getter);
                property.SetSetMethod(setter);
                tBuilder.DefineMethodOverride(getter, v.GetGetMethod());
                tBuilder.DefineMethodOverride(setter, v.GetSetMethod());
            }
        }
        
        //public BuildCascade WithEventComposition<Exten>()

        public BuildCascade<Exten> WithEventComposition<Exten, EventContainingType>(params Type[] types) => 
            WithEventCompositionB<Exten>(
                typeof(EventContainingType)
                    .GetInterfaces()
                    .Where(e=>!e.IsAssignableFrom(typeof(ComponentBase))).ToArray()
            );
        
        public BuildCascade<Exten> WithEventComposition<Exten>(Type eventContainingType, params Type[] types) => 
            WithEventCompositionB<Exten>(
                eventContainingType
                    .GetInterfaces()
                    .Where(e=>!e.IsAssignableFrom(typeof(ComponentBase))).ToArray()
            );

        public BuildCascade<Exten> WithEventCompositionB<Exten>(params Type[] types)
        {
            var assemblyName = new Guid().ToString();
            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            ModuleBuilder mb = ab.DefineDynamicModule("Module");

            var type = mb.DefineType(GetType().Name + "_" + typeof(Exten).Name, TypeAttributes.Public, typeof(Exten));
            
            foreach (var tp in types)
                DoInterfaceBuild(type, tp);

            return new BuildCascade<Exten>
            {
                Item = (Exten) Activator.CreateInstance(type.CreateType())
            };
        }
    }
}