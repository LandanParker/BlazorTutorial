using System;
using BlazorTutorial.Web.Lib.Components;
using BlazorTutorial.Web.Lib.Contexts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorTutorial.Web.Lib.PageView
{
    public abstract class PageComponent : ComponentBase, IDisposable
    {
        public string Id = Guid.NewGuid().ToString();
        public bool IsDestroyed { get; set; } = false;
        
        public virtual void Dispose()
        {
            IsDestroyed = true;
            BackingInterop.PageViewManager.RemovePageComponent(Id);
            Console.WriteLine("destroyed :"+Id);
        }
        
        public virtual void OnInteropSet(BackingInterop bk)=> BackingInterop.PageViewManager.AddPageComponent(Id, this);
        
        public Action<BackingInterop> InitializeInterop { get; init; }

        protected BackingInterop _BackingInterop { get; set; }

        [Inject]
        public BackingInterop BackingInterop
        {
            get => _BackingInterop;
            set => InitializeInterop(_BackingInterop = value);
        }

        protected PageComponent()
        {
            InitializeInterop = OnInteropSet;
        }
        
    }
}