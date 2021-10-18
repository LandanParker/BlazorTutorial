using System;

namespace BlazorTutorial.Web.Lib.Components
{
    public interface IReceiver
    {
        public void Receive(object obj);
    }
}