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
using Area51.SoftwareModeler.Model;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddClassCommand : BaseCommand
    {
        //reference to created class
        [XmlIgnore]
        public Shape shapeToAdd {get; set; }
        //Parameters for ClassGeneration
        public string className { get; private set; }
        public string stereoType { get; private set; }
        public bool isAbstract { get; private set; }
        public Point anchorPoint { get; private set; }
        public Visibility visibility { get; private set; }

        public AddClassCommand()
        {

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
            Class c = new Class(className, stereoType, isAbstract, anchorPoint, visibility);
            ShapeCollector.getI().obsShapes.Add(c);
            shapeToAdd = c;
        }


        public override void unExecute()
        {
            ShapeCollector.getI().obsShapes.Remove(shapeToAdd);
        }


    }
}
