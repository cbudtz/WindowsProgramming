
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Model.CodeGen
{
    class AttributeCodeGen
    {
        private Attribute a;
        public AttributeCodeGen(Attribute attribute)
        {
            this.a = attribute;
        }

        public string generateJava()
        {
            string visibility = a.Visibility.ToString().ToLower().Trim();
            if (a.Visibility.Equals(Visibility.Default)) visibility = "";
            string type = fixType(a.Type.Trim());
            string name = fixName(a.Name).Trim();

            return "\t" + visibility + " " + type + " " + name + ";\n";
        }




        private string fixType(string t)
        {
            string[] tokens = t.Split(' ');
            string type = "";
            foreach (string token in tokens)
            {
                type += token.Substring(0, 1).ToUpper() + token.Substring(1, token.Length - 1);
            }

            return type;
        }

        private string fixName(string s)
        {
            string[] tokens = s.Split(' ');
            string name = "";
            foreach (string token in tokens)
            {
                name += token.Substring(0,1).ToLower() + token.Substring(1, token.Length-1);
            }

            return name;
        }
    }
}
