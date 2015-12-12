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
        //private List<String> parameters { get; set; }
        private List<Modifier> modifiers { get; set; }


        public Method(Visibility visibility, string name)
        {
            this.Visibility = visibility;
            this.Name = name;
            //new List<string>();

        }
        public Method(Visibility visibility, string name, String parameters)
        {
            this.Visibility = visibility;
            this.Name = name;
            this.Parameters = parameters;
            //new List<string>();

        }

        public Method()
        {
            
        }

        public Visibility Visibility { get; set; }

        public string Name { get; set; }

        //public List<string> Parameters { get { return parameters; } set { parameters = value; } }
        public string Parameters { get; set; }

        public string ReturnType { get; set; }

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
