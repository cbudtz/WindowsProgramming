using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    class Attribute
    {
        private String type {get; set;}
        private String name { get; set; }
        private Visibility visibility { get; set; }
        private List<Modifier> modifiers { get; set; }

    }
}
