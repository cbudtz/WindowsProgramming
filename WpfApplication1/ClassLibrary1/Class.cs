using System;

namespace ClassLibrary1
{
    public class Class
    {
        private String name;
        private String stereotype;
        private Boolean isAbstract;
        
        public Class(String name, String stereotype, Boolean isAbstract)
        {
            this.name = name;
            this.stereotype = stereotype;
            this.isAbstract = isAbstract;

        }

        public String Name  { get { return name; } set { name = value; } }

        public String StereoType { get { return stereotype; } set { stereotype = value; } }

        public Boolean IsAbstract { get { return isAbstract; } set { isAbstract = value; } }
    }
}
