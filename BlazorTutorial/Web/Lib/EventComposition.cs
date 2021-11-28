using System;
using System.Linq;
using System.Reflection;
using BlazorTutorial.Web.Lib.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib
{
    public class EventComposition : IReceivable
    {
        public ContextInterop ContextInterop { get; set; }

        public void OnInit(BackingInterop backingInterop) =>
            ContextInterop = backingInterop.ContextInterop;
        
        public Action<object> Receive { get; set; }

        public void AssignInterfaceProperties(object matcher)
        {
            var matchInterfaces = matcher.GetType().GetInterfaces();
            var theseInterfaces = GetType().GetInterfaces();
            
            foreach (var face in theseInterfaces.Where(matchInterfaces.Contains))
                foreach (var props in face.GetProperties())
                    props.SetValue(matcher, props.GetValue(this));
        }

        public void ConformToEventSource(object source)
        {
            
            foreach (var props in this.GetType().GetProperties())
            {
                var obj = props.GetValue(this);
                var prop = this.GetType().GetProperty(props.Name);
                
                if (obj is not null) prop.SetValue(source, obj);
            }
        }
    }
}