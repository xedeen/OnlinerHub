using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.AppModel
{
    public class Author : Entity
    {
        public string Name { get; set; }
        public string AvatarUri { get; set; }
        public string ProfileUri { get; set; }
    }
}
