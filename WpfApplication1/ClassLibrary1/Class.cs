using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class
    {
        private String name;
        private String stereotype;
        private Boolean isAbstract;

        public string Name  { get { return name; } set { name = value; } }

        public string StereoType { get { return stereotype; } set { stereotype = value; } }

        public bool IsAbstract { get { return isAbstract; } set { isAbstract = value;
            }
        }
    }
}
