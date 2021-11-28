
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using BlazorTutorial.Web.Lib.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib.Contexts
{
    public class FragmentBuilder
    {
        public int Sequence { get; set; } = 0;
        
        public RenderTreeBuilder RenderBuilder { get; set; }

        public FragmentBuilder(RenderTreeBuilder builder)
        {
            RenderBuilder = builder;
        }

        public FragmentBuilder Operate(Action<int, RenderTreeBuilder> action)
        {
            action(Sequence, RenderBuilder);
            Sequence++;
            return this;
        }

        public void DoBuild(params Action<int, RenderTreeBuilder>[] actions)
        {
            foreach (var action in actions) Operate(action);
        }
        
    }

    public class TestIndex
    {
        public RenderFragment TestFrag { get; set; }

        public RenderFragment CreateComponent(Type componentType, out EventComposition composition, out string uid)
        {
            string id = uid = Guid.NewGuid().ToString();

            var comp = composition = new CompositionBuilder()
                .WithEventComposition(componentType).AssignInject(typeof(string), "testName").Item as EventComposition;
            
            return builder =>
            {
                new FragmentBuilder(builder).DoBuild(
                    (sequence, build) => { build.OpenComponent(sequence, componentType); },
                    (sequence, build) => { build.AddAttribute(sequence, "Context", comp); },
                    (sequence, build) => { build.AddAttribute(sequence, "Id", id); },
                    (sequence, build) => { build.CloseComponent(); });
            };

        }
        
        public class MethodProp
        {
            
            public Action<object> DoThing { get; set; }
            
            public void MethodObj(object a)
            {
                DoThing(a);
            }
        }

        public TestIndex()
        {
            
            var holdType = new CompositionBuilder().CreateComponentFrom(typeof(TestComp))
                .AssignInject(typeof(PageViewManager), "testProp").Item as TestComp;
            
            TestFrag = CreateComponent(holdType.GetType(), out var composition, out string tag);

            //Could be PageViewManager instead.
            Dictionary<string, int> ComponentDataSource = new () {{tag,0}};
            
            composition.ConformToEventSource(new
            {
                OnMouseUp = (Action<object>) (_ => { ComponentDataSource[tag] += 1; }),
                OnMouseDown = (Action<object>) (_ => { Console.WriteLine("mouse clicked"); }),
                TestString = null as string
            });

            //called in the component's OnParametersSetAsync
            composition.Receive = o =>
            {
                composition.AssignInterfaceProperties(o);
                var propTest = o.GetType().GetProperty("testProp").GetValue(o) as PageViewManager;
                Console.WriteLine((o as IKeyCallbacks).TestString);
                Console.WriteLine(propTest.PageViewMap.Count);
            };
        }
    }
}