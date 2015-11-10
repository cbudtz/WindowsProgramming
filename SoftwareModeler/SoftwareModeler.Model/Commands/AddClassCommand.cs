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
        public Class classRep { get; set; } //Representation object of Class
        public int? ShapeId {get; set;} //Id For recreation of object
        //Parameters for ClassGeneration
        public string className { get; set; }
        public string stereoType { get; set; }
        public bool isAbstract { get; set; }
        public Point anchorPoint { get; set; }
        public Visibility visibility { get; set; }

        public AddClassCommand()
        {
            //When deserializing, no shape is attached, until excecuted...
            ShapeId = null;
        }
        public AddClassCommand(string className, string stereoType, bool isAbstract,Point anchorPoint, Visibility visibility)
        {
            this.className = className;
            this.stereoType = stereoType;
            this.isAbstract = isAbstract;
            this.anchorPoint = anchorPoint;
            this.visibility = visibility;
        }

        public override void execute()
        {
            //First time the ShapeID will be null;
            if (ShapeId == null)
            {
                classRep = new Class(className, stereoType, isAbstract, anchorPoint, visibility);
                ShapeId = classRep.id;
            } else
            {
                //ReExecuting (either when deserialized or redoing)
                classRep = new Class(ShapeId, className, stereoType, isAbstract, anchorPoint, visibility);
            }
            Console.WriteLine("Executing Add Shape Command, Shape with ID: " + classRep.id + "Added");
            ShapeCollector.getI().obsShapes.Add(classRep);
        }


        public override void unExecute()
        {
            //Shape is removed and garbage collected - a new shape with the same ID 
            //will be instantiated if the command is excecuted again
            ShapeCollector.getI().obsShapes.Remove(classRep);
            classRep = null;
        }

    }
}
