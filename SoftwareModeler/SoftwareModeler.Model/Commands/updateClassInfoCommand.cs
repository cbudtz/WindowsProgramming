using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class UpdateClassInfoCommand : BaseCommand
    {
        //Class reference
        public int? classID;

        //Update parameters - new values
        public string ClassName { get; set; }
        public bool? IsAbstract { get; set; }
        public string StereoType { get; set; }
        public List<Method> Methods { get; set; } 
        public List<Attribute> Attributes { get; set; }
        ///Previous values
        public string OldclassName { get; set; }
        public bool OldAbstract { get; set; }
        public string OldStereoType { get; set; }              
        public List<Method> OldMethods { get; set; }
        public List<Attribute> OldAttributes { get; set; } 

        public UpdateClassInfoCommand()
        {
            //Deserialization constructor
        }

        /// <summary>
        /// Updates Class information. If parameters are null, they are not updated
        /// </summary>
        /// <param name="classToUpdate">required class</param>
        /// <param name="className"></param>
        /// <param name="stereoType"></param>
        /// <param name="isAbstract"></param>
        /// <param name="methods"></param>
        /// <param name="attributes"></param>
        public UpdateClassInfoCommand(Class classToUpdate, string className, string stereoType, bool? isAbstract, List<Method> methods, List<Attribute> attributes  )
        {
            //ClassInstance
            this.classID = classToUpdate.id;
            this.OldclassName = classToUpdate.name;
            this.OldStereoType = classToUpdate.StereoType;
            this.OldAbstract = classToUpdate.IsAbstract;
            this.OldAttributes = classToUpdate.Attributes;
            this.OldMethods = classToUpdate.Methods;
            //New variables
            this.ClassName = className;
            this.StereoType = stereoType;
            this.IsAbstract = isAbstract;
            this.Attributes = attributes;
            this.Methods = methods;

        }

        public override void execute()
        {
            Class classToUpdate = ShapeCollector.GetI().GetShapeById(classID) as Class;
            if (classToUpdate != null && ClassName != null) classToUpdate.Name = ClassName;
            if (classToUpdate != null && StereoType != null) classToUpdate.StereoType = StereoType;
            if (classToUpdate != null) classToUpdate.IsAbstract = IsAbstract.Value;
            if (classToUpdate != null) classToUpdate.Methods = Methods;
            if(classToUpdate != null) classToUpdate.Attributes = Attributes;
        }

        public override void unExecute()
        {
            Class classToUpdate = ShapeCollector.GetI().GetShapeById(classID) as Class;
            if (classToUpdate != null)
            {
            
                classToUpdate.name = OldclassName;
                classToUpdate.StereoType = OldStereoType;
                classToUpdate.IsAbstract = OldAbstract;
                classToUpdate.Methods = OldMethods;
                classToUpdate.Attributes = OldAttributes;
            }

    }
    }
}
