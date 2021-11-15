using System.Collections.Generic;

namespace BlazorTutorial.Web.Lib
{
    public class ContextInterop
    {
        public Dictionary<string, object> ContextMap { get; set; } = new();

        public void AddContext(string uid, object obj)
        {
            ContextMap.Add(uid, obj);
        }

        public void RemoveContext(string uid)
        {
            ContextMap.Remove(uid);
        }
        
    }
}