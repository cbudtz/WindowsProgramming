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
using Area51.SoftwareModeler.Models;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddClassCommand : BaseCommand
    {
        //reference to created class
        [XmlIgnore]
        public Class classRep { get; set; }
        public int? ShapeId {get; set;}
        //Parameters for ClassGeneration
        public string className { get; set; }
        public string stereoType { get; set; }
        public bool isAbstract { get; set; }
        public Point anchorPoint { get; set; }
        public Visibility visibility { get; set; }

        public AddClassCommand()
        {
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
                classRep = new Class(ShapeId);
            }
        }


        public override void unExecute()
        {
            ShapeCollector.getI().obsShapes.Remove(classRep);
            classRep = null;
        }

    }
}
