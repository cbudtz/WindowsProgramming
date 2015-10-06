﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    class Method
    {
        private String name { get; set; }
        private List<String> parameters { get; set; }
        private String returnType { get; set; }
        private Visibility visibility{get; set;}
        private List<Modifier> modifiers { get; set; }
    }
}
