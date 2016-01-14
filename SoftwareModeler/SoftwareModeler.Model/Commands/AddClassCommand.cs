using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Area51.SoftwareModeler.Models;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddClassCommand : BaseCommand
    {
        //reference to created class
        [XmlIgnore] //Only data needed for recreation of object is stored
        public ClassData ClassDataRep { get; set; } //Representation object of ClassData

        public int? ShapeId {get; set;} //Id For recreation of object
        //Parameters for ClassGeneration
        public string ClassName { get; set; }
        public string StereoType { get; set; }
        public bool IsAbstract { get; set; }
        public Point AnchorPoint { get; set; }

        public AddClassCommand()
        {
            //When deserializing, no shape is attached, until excecuted...
        }
        public AddClassCommand(string className, string stereoType, bool isAbstract,Point anchorPoint)
        {
            ClassName = (className ?? "class " + ClassData.nextId); //null pointer check
            StereoType = stereoType;
            IsAbstract = isAbstract;
            AnchorPoint = anchorPoint;
        }

        public override void execute()
        {
            //First time the ShapeID will be null;
            if (ShapeId == null)
            {
                ClassDataRep = new ClassData(ClassName, StereoType, IsAbstract, AnchorPoint);
                ShapeId = ClassDataRep.id;
            } else
            {
                //ReExecuting (either when deserialized or redoing)
                ClassDataRep = new ClassData(ShapeId, ClassName, StereoType, IsAbstract, AnchorPoint);
            }
            ShapeCollector.GetI().ObsShapes.Add(ClassDataRep);
        }


        public override void unExecute()
        {
            //ClassView is removed and garbage collected - a new shape with the same ID 
            //will be instantiated if the command is excecuted again
            ShapeCollector.GetI().ObsShapes.Remove(ClassDataRep);
            ClassDataRep = null;
        }

    }
}
