using System;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib.Components
{
    public interface IMouseCallbacks
    {
        public virtual Action<MouseEventArgs> OnMouseUp
        {
            get => throw new NotImplementedException();
            set { }
        }
        public virtual Action<MouseEventArgs> OnMouseDown
        {
            get => throw new NotImplementedException();
            set { }
        }
    }
}