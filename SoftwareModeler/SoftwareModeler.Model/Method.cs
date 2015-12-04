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
        //public string parameterString { get; }
        private String name; // { get; set; }
        //private List<String> parameters { get; set; }
        private String parameters; // { get; set; }
        private String returnType; // { get; set; }
        private Visibility visibility; //{get; set;}
        private List<Modifier> modifiers { get; set; }


        public Method(Visibility visibility, string name)
        {
            this.visibility = visibility;
            this.name = name;
            //new List<string>();

        }
        public Method(Visibility visibility, string name, String parameters)
        {
            this.visibility = visibility;
            this.name = name;
            this.parameters = parameters;
            //new List<string>();

        }

        public Method()
        {
            
        }

        public Visibility Visibility { get { return visibility; } set { visibility = value; } }
        public string Name { get { return name; } set { name = value; } }
        
        //public List<string> Parameters { get { return parameters; } set { parameters = value; } }
        public string Parameters { get { return parameters; } set { parameters = value; } }
        public string ReturnType { get { return returnType; } set { returnType = value; } }

        public override string ToString()
        {
            return Name;//MethodString;
        }
        //public void addParameter(string Type)
        //{
        //    this.parameters.Add(Type);
        //}


        //public string ParameterString { get { return generateParameterString(); } }
        //private string generateParameterString()
        //{
        //    return "";
            //string results = "(";
            //for (int i = 0; i < parameters.Count - 1; i++)
            //{
            //    results += parameters.ElementAt(i);
            //    results += ", ";
            //}
            //results += parameters.ElementAt(parameters.Count - 1);
            //results += ")";
            //return results;
        //}

        //public string MethodString { get {
        //        return HelperFunctions.GetDescription(visibility) + name  + generateParameterString() + ":" + returnType; }
        //}

        
    }
}
