using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Common
{
    public abstract class ControllerBase<T> where T : new()
    {
        protected static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        protected ControllerBase()
        {
        }

        public static T Instance
        {
            get { return _instance.Value; }
        }
    }
}
