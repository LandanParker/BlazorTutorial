using System.Collections.Generic;
using BlazorTutorial.Web.Lib.PageView;

namespace BlazorTutorial.Web.Lib
{
    public class PageViewManager
    {
        public Dictionary<string, PageComponent> PageViewMap { get; set; } = new();
        public void AddPageComponent(string uid, PageComponent comp) => PageViewMap.Add(uid, comp);
        
        public void RemovePageComponent(string uid) => PageViewMap.Remove(uid);
    }
}