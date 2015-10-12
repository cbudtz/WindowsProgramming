using System;
using System.Collections.Generic;

namespace Area51.SoftwareModeler.Models
{
    public class Attribute
    {
        private String type {get; set;}
        private String name { get; set; }
        private Visibility visibility { get; set; }
        private List<Modifier> modifiers { get; set; }

        public static implicit operator List<object>(Attribute v)
        {
            throw new NotImplementedException();
        }
    }
}
