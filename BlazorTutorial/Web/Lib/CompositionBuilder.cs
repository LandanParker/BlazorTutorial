using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Schema;
using BlazorTutorial.Web.Lib.Components;
using BlazorTutorial.Web.Lib.Contexts;
using Microsoft.AspNetCore.Components;

namespace BlazorTutorial.Web.Lib
{

    public class BuildCascade
    {

        public BuildCascade(TypeBuilder t)
        {
            ItemType = t;
        }
        
        public BuildCascade TryPickInterface<I>(out I iface)
        {
            iface = (I) Item;
            return this;
        }

        public BuildCascade DoOtherthing()
        {
            return this;
        }

        public BuildCascade CreateType()
        {
            _Item ??= Activator.CreateInstance(ItemType.CreateType());
            return this;
        }

        public BuildCascade AssignInject(Type tp, string propertyName)
        {
            CompositionBuilder.AssignInject(ItemType, tp, propertyName);
            return this;
        }
        
        public TypeBuilder ItemType { get; set; }
        
        private object _Item { get; set; }
        
        public object Item
        {
            get => CreateType()._Item;
            set => _Item = value;
        }
        
    }
    
    public class CompositionBuilder
    {

        public BuildCascade CreateComponentFrom(Type componentType)
        {
            var assemblyName = new Guid().ToString();
            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            ModuleBuilder mb = ab.DefineDynamicModule("Module");

            var type = mb.DefineType(nameof(CompositionBuilder) + "_" + componentType.Name, TypeAttributes.Public, componentType);
            
            return new BuildCascade(type);
        }
        
        public static void AssignInject(TypeBuilder tBuilder, Type tp, string propertyName)
        {
            var field = tBuilder.DefineField("_" + tp.Name.ToLower(), tp, FieldAttributes.Private);

            var property = tBuilder.DefineProperty(propertyName, PropertyAttributes.None, tp, new Type[0]);

            ConstructorInfo classCtorInfo = typeof(InjectAttribute).GetConstructors()[0];
            
            CustomAttributeBuilder myCABuilder = 
                new CustomAttributeBuilder(classCtorInfo, new object[] {});
            
            property.SetCustomAttribute(myCABuilder);

            var getter = tBuilder.DefineMethod("get_" + tp.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual,
                tp, new Type[0]);
            var setter = tBuilder.DefineMethod("set_" + tp.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null,
                new Type[] {tp});
            var getGenerator = getter.GetILGenerator();
            var setGenerator = setter.GetILGenerator();
            //get Emit
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, field);
            getGenerator.Emit(OpCodes.Ret);
            
            //set Emit
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Stfld, field);
            setGenerator.Emit(OpCodes.Ret);
            property.SetGetMethod(getter);
            property.SetSetMethod(setter);
        }
        
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

        public BuildCascade WithEventComposition<Exten, EventContainingType>(params Type[] types) => 
            WithEventCompositionB(
                typeof(EventContainingType)
                    .GetInterfaces()
                    .Where(e=>!e.IsAssignableFrom(typeof(ComponentBase))).ToArray()
            );
        
        public BuildCascade WithEventComposition(Type eventContainingType, params Type[] types) => 
            WithEventCompositionB(
                eventContainingType
                    .GetInterfaces()
                    .Where(e=>!e.IsAssignableFrom(typeof(ComponentBase))).ToArray()
            );

        public BuildCascade WithEventCompositionB(params Type[] types)
        {
            var assemblyName = new Guid().ToString();
            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            ModuleBuilder mb = ab.DefineDynamicModule("Module");

            var type = mb.DefineType(GetType().Name + "_" + nameof(EventComposition), TypeAttributes.Public, typeof(EventComposition));
            
            foreach (var tp in types)
                DoInterfaceBuild(type, tp);
            return new BuildCascade(type);
        }
    }
}