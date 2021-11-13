using System;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib.Components
{
    public interface IKeyCallbacks
    {
        public virtual Action<KeyboardEventArgs> OnKeyUp
        {
            get => throw new NotImplementedException();
            set { }
        }
        public virtual Action<KeyboardEventArgs> OnKeyDown
        {
            get => throw new NotImplementedException();
            set { }
        }
    }
}