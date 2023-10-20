using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Entities.Bases
{
    public interface IIdentidad<T> where T : IEquatable<T>
    {
        public T Id { get; set; }
    }
}
