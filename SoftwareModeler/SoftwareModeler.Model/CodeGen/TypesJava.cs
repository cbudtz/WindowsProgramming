using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Model.CodeGen
{
    class TypesJava
    {
        public string Type { get; set; }
        public string DefaultValue { get; set; }

        public TypesJava(string type, string defaultValue)
        {
            Type = type;
            DefaultValue = defaultValue;
        }

        private static readonly List<string> Types = new List<string>()
        {
            "byte", "short", "int", "long", "float", "double", "char", "String", "boolean",
            "Byte", "Short", "Integer", "Long", "Float", "Double", "Character", "Boolean",
            "Object", "ArrayList", "LinkedList"
        };

        private static readonly List<TypesJava> TypesDef = new List<TypesJava>()
        {
            new TypesJava("byte","0"), new TypesJava("short", "0"), new TypesJava("int", "0"), new TypesJava("long", "0L"),
            new TypesJava("float", "0F"), new TypesJava("double", "0D"), new TypesJava("char", "'\\0'"), new TypesJava("String", "null"),
            new TypesJava("boolean", "false"), new TypesJava("Byte", "null"), new TypesJava("Short", "null"), new TypesJava("Integer", "null"),
            new TypesJava("Long", "null"), new TypesJava("Float", "null"), new TypesJava("Double", "null"), new TypesJava("Character", "null"),
            new TypesJava("Boolean", "null"), new TypesJava("Object", "null"), new TypesJava("ArrayList", "null"), new TypesJava("LinkedList", "null")
        };

        public static bool Exists(TypesJava type)
        {
            return TypesDef.Any(x => x.Type.Equals(type.Type));
        }


        public static bool Exists(string type)
        {
            return Types.Contains(RemoveArrayType(type));
        }

        public static void AddType(string type)
        {
            type = RemoveArrayType(type);
            if(type != null && !type.Equals("") && !Types.Contains(type)) Types.Add(type);
            TypesJava extType = new TypesJava(type, "null");
            if(!Exists(extType)) TypesDef.Add(extType);
        }

      

        public static void RemoveType(string type)
        {
            Types.Remove(RemoveArrayType(type));
        }

        private static string RemoveArrayType(string s)
        {
            return s.Replace("[", "").Replace(" ", "").Replace("]", "");
        }
    }
}
