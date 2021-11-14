using System;
using BlazorTutorial.Web.Lib.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib.Contexts
{
    public class EventComposition : IReceivable
    {
        public Action<object> Receive { get; set; }
    }
}