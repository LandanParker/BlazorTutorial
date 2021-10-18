using System;
using BlazorTutorial.Web.Lib.Components;

namespace BlazorTutorial.Web.Lib.Contexts
{
    
    public class BasicDivContext : IReceiver
    {
        
        public BasicDiv SourceComponent { get; set; }

        public void SetText(string text)
        {
            this.SourceComponent.Text = text;
            this.SourceComponent.SyncComponent();
        }
        
        public void Receive(object component)
        {
            SourceComponent = component as BasicDiv;
        }

    }
}