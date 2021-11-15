using System.Collections.Generic;
using BlazorTutorial.Web.Lib.PageView;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTutorial.Web.Lib
{
    public class BackingInterop
    {
        public ContextInterop ContextInterop { get; set; }
        public PageViewManager PageViewManager { get; set; }
        
        public BackingInterop(ContextInterop ctx, PageViewManager viewManager)
        {
            ContextInterop = ctx;
            PageViewManager = viewManager;
        }
    }
}