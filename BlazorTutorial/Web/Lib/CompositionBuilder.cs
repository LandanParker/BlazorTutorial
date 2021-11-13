using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Schema;

namespace BlazorTutorial.Web.Lib
{

    public class BuildCascade
    {

        public BuildCascade PickInterface<I>(out I iface)
        {
            iface = (I) Item;
            return this;
        }

        public object Item { get; set; }
    }
    

    public class CompositionBuilder
    {

        public BuildCascade WithEventComposition<Exten>(params Type[] types)
        {
            var assemblyName = new Guid().ToString();
            AssemblyName aName = new AssemblyName(assemblyName);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            ModuleBuilder mb = ab.DefineDynamicModule("Module");

            var type = mb.DefineType(GetType().Name + "_" + typeof(Exten).Name, TypeAttributes.Public, typeof(Exten));

            foreach (var tp in types)
            {
                type.AddInterfaceImplementation(tp);
                foreach (var v in tp.GetProperties())
                {
                    var field = type.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
                    var property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                    var getter = type.DefineMethod("get_" + v.Name,
                        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual,
                        v.PropertyType, new Type[0]);
                    var setter = type.DefineMethod("set_" + v.Name,
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
                    type.DefineMethodOverride(getter, v.GetGetMethod());
                    type.DefineMethodOverride(setter, v.GetSetMethod());
                }
            }

            return new BuildCascade
            {
                Item = (Exten) Activator.CreateInstance(type.CreateType())
            };

        }
    }
}