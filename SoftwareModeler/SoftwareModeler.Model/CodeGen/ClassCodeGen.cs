using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Model.CodeGen;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Model.CodeGen
{
    
    public class ClassCodeGen
    {
        private ClassData c;
        public ClassCodeGen(ClassData c)
        {
            this.c = c;

        }

        public string GenerateJava()
        {
            string type = c.StereoType.Equals("") ? "class" : c.StereoType.Replace("<<", "").Replace(">>", "");
            string name = FixName(c.Name);

            string attrStr = "";
            c.Attributes.ForEach(x => attrStr += new AttributeCodeGen(x).generateJava());
            string methStr = "";
            c.Methods.ForEach(x => methStr += new MethodCodeGen(x).generateJava());


            return "public " + (c.IsAbstract ? "abstract " : "") + type + " " + name + "{\n"
                   + attrStr + "\n"
                   + methStr + "\n"
                   + "}";
        }

        public string FileName()
        {
            return FixName(c.Name);
        }

        public static string FixName(string n)
        {
            string[] tokens = n.Split(' ');
            string name = "";
            foreach (string token in tokens)
            {
                Console.WriteLine("token: " + token);
                name += token.Substring(0, 1).ToUpper() + token.Substring(1, token.Length - 1);
                Console.WriteLine("name: " + name);
            }

            return name;
    }

       
    }
}
