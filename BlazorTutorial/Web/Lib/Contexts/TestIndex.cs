
using System;
using System.Collections.Generic;
using System.Dynamic;
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

            EventComposition comp = composition = new CompositionBuilder()
                .WithEventComposition<EventComposition>(componentType).Item;

            return builder =>
            {
                new FragmentBuilder(builder).DoBuild(
                    (sequence, build) => { build.OpenComponent(sequence, componentType); },
                    (sequence, build) => { build.AddAttribute(sequence, "Context", comp); },
                    (sequence, build) => { build.AddAttribute(sequence, "Id", id); },
                    (sequence, build) => { build.CloseComponent(); });
            };
        }

        public TestIndex()
        {
            TestFrag = CreateComponent(typeof(TestComp), out var composition, out string tag);

            //create data context for the component being generated
            //associated it via tag.
            //Decoupling logic from the component.

            KeyValuePair<string, dynamic> ComponentDataSource = new(tag, new{count = 0});
            
            composition.ConformToEventSource(new
            {
                OnMouseUp = (Action<object>) (_ =>
                {
                    ComponentDataSource = new(tag, new {count = ComponentDataSource.Value.count + 1});
                }),

                OnMouseDown = (Action<object>) (_ => { Console.WriteLine("mouse clicked: "+ComponentDataSource.Value.count); }),
            });

            composition.Receive = o =>
            {
                composition.AssignInterfaceProperties(o);
                Console.WriteLine("doing");
            };
        }
    }
}