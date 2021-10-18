using System;
using BlazorTutorial.Web.Lib.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTutorial.Web.Lib.Contexts
{
    
    public class BasicDivContext : IReceiver
    {
        public string Text { get; set; }
        public BasicDiv SourceComponent { get; set; }

        public void SetText(string text)
        {
            this.SourceComponent.Text = this.Text = text;
            this.SourceComponent.SyncComponent();
        }
        
        public void Receive(object component)
        {
            SourceComponent = component as BasicDiv;
            SourceComponent.Text = Text;

            SourceComponent.OnMouseDown -= DoMouseDown;
            SourceComponent.OnMouseDown += DoMouseDown;
        }

        public Action<MouseEventArgs> OnMouseDown { get; set; } = _ => { };

        public void DoMouseDown(MouseEventArgs args)
        {
            OnMouseDown.Invoke(args);
        }

    }
}