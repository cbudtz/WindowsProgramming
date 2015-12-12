using System;
using System.Collections.Generic;
using Area51.SoftwareModeler.Helpers;

namespace Area51.SoftwareModeler.Models
{
    public class Attribute
    {
        private Visibility visibility; // { get; set; }

        private List<Modifier> modifiers { get; set; }

        public Attribute()
        {

        }
        public Attribute(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public Attribute(Visibility visibility, string type, string name)
        {
            this.visibility = visibility;
            this.Type = type;
            this.Name = name;
        }

        public static implicit operator List<object>(Attribute v)
        {
            throw new NotImplementedException();
        }

        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                //Console.WriteLine("Visibility:" + HelperFunctions.GetEnumFromDescription(HelperFunctions.GetDescription(visibility), Visibility));

            }
        }

        public string Name { get; set; }

        public string Type { get; set; }


        public override string ToString()
        {
            return HelperFunctions.GetDescription(visibility) + Name + ":" + Type;
    
        }

        public string getVisibilityAsString()
        {
            return HelperFunctions.GetDescription(visibility);
        }
    }
}
