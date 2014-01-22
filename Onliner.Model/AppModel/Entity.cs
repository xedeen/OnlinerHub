using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.AppModel
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = -1;
        }

        public long Id { get; set; }

        public bool IsNew()
        {
            return Id == -1;
        }
    }
}
