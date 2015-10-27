using helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Model
{
    public class Method
    {
        //public string parameterString { get; }
        private String name { get; set; }
        private List<String> parameters { get; set; }
        private String returnType { get; set; }
        private Visibility visibility{get; set;}
        private List<Modifier> modifiers { get; set; }

        public Method(Visibility visibility, string name)
        {
            this.visibility = visibility;
            this.name = name;
            this.parameters = new List<string>();
        }


        public string Name { get { return name; } set { name = value; } }
        public Visibility Visibility { get { return visibility; } set { visibility = value; } }
        public List<string> Parameters { get { return parameters; } set { parameters = value; } }
        public string ReturnType { get { return returnType; } set { returnType = value; } }

        public void addParameter(string type)
        {
            this.parameters.Add(type);
        }


        public string ParameterString { get { return generateParameterString(); } }
        private string generateParameterString()
        {
            string results = "(";
            for (int i = 0; i < parameters.Count - 1; i++)
            {
                results += parameters.ElementAt(i);
                results += ", ";
            }
            results += parameters.ElementAt(parameters.Count - 1);
            results += ")";
            return results;
        }

        public string MethodString { get {
                return helper.GetDescription(visibility) + name  + generateParameterString() + ":" + returnType; }
        }

        public override string ToString()
        {
            return MethodString;
        }
    }
}
