using System;

namespace BlazorTutorial.Web.Lib.Components
{
    public interface IReceivable
    {
        public Action<object> Receive { get; set; }
    }
}