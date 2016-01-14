using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Model.CodeGen
{
    public class CodeGen
    {
        public static void GenerateJavaCode(string path)
        {
            ShapeCollector sc = ShapeCollector.GetI();

            if (path == null || path.Equals("")) return;
            foreach (var c in sc.ObsShapes)
            {
                TypesJava.AddType(c.Name);
                List<ConnectionData> conn = sc.ObsConnections.Where(x => x.StartShapeId == c.id).ToList();
                foreach (var connection in conn)
                {
                    ClassData endClassData = sc.GetShapeById(connection.EndShapeId);

                   c.AddAttribute(Visibility.Private, ClassCodeGen.FixName(endClassData.Name), endClassData.Name.Substring(0,1).ToLower()+endClassData.Name.Substring(1)); 
                }

                ClassCodeGen codeGen = new ClassCodeGen(c);
                string txt = codeGen.GenerateJava();
                System.IO.File.WriteAllText(path + "/" + codeGen.FileName() + ".java", txt);
            }
        }
    }
}
