using System;
using System.Collections.Generic;
using Area51.SoftwareModeler.Helpers;

namespace Area51.SoftwareModeler.Models
{
    public class Attribute
    {
        private string type {get; set;}
        private string name { get; set; }
        private Visibility visibility { get; set; }
        private List<Modifier> modifiers { get; set; }

        public Attribute()
        {

        }
        public Attribute(string type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public Attribute(Visibility visibility, string type, string name)
        {
            this.visibility = visibility;
            this.type = type;
            this.name = name;
        }

        public static implicit operator List<object>(Attribute v)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                //Console.WriteLine("Attributename:"+value.ToString());
            }
        }

        public string Type { get { return type; } set { type = value; } }

        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                Console.WriteLine("Visibility:"+ HelperFunctions.GetEnumFromDescription(HelperFunctions.GetDescription(visibility), Visibility));

            }
        }

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
