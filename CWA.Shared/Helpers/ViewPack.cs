using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Helpers
{
    public class ViewPack : Dictionary<string, string>
    {
        public ViewPack() { }

        public ViewPack(string InitialKey, string Value)
        {
            this.Add(InitialKey, Value);
        }
    }

    public class ViewPack<T> : Dictionary<string, T> where T : class
    { 
        public ViewPack() { }

        public ViewPack(string InitialKey, T Value)
        {
            this.Add(InitialKey, Value);
        }
    }
}
