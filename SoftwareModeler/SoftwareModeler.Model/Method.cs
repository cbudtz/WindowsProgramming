using Area51.SoftwareModeler.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public class Method
    {

        private string _annotation;
        private string _name;  
        private string _parameters;
        private string _throwsDeclaration;
        private string _returnType;
        private Visibility _visibility;


        public Method(Visibility visibility, string name)
        {
            this._visibility = visibility;
            this._name = name;
            //new List<string>();

        }
        public Method(Visibility visibility, string name, String parameters)
        {
            this._visibility = visibility;
            this._name = name;
            this._parameters = parameters;
            //new List<string>();

        }

        public Method()
        {
            
        }

        public Visibility Visibility { get { return _visibility; } set { _visibility = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        
        //public List<string> Parameters { get { return _parameters; } set { _parameters = value; } }
        public string Parameters { get { return _parameters; } set { _parameters = value; } }
        public string ReturnType { get { return _returnType; } set { _returnType = value; } }

        public override string ToString()
        {
            return Name;
        }

    }
}
