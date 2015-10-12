using System;
using System.Collections.Generic;

namespace Area51.SoftwareModeler.Models
{
    class Attribute
    {
        private String type {get; set;}
        private String name { get; set; }
        private Visibility visibility { get; set; }
        private List<Modifier> modifiers { get; set; }

    }
}
