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
        //ClassData reference
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
        /// Updates ClassData information. If parameters are null, they are not updated
        /// </summary>
        /// <param name="classDataToUpdate">required class</param>
        /// <param name="className"></param>
        /// <param name="stereoType"></param>
        /// <param name="isAbstract"></param>
        /// <param name="methods"></param>
        /// <param name="attributes"></param>
        public UpdateClassInfoCommand(ClassData classDataToUpdate, string className, string stereoType, bool? isAbstract, List<Method> methods, List<Attribute> attributes  )
        {
            //ClassInstance
            this.classID = classDataToUpdate.id;
            this.OldclassName = classDataToUpdate.name;
            this.OldStereoType = classDataToUpdate.StereoType;
            this.OldAbstract = classDataToUpdate.IsAbstract;
            this.OldAttributes = classDataToUpdate.Attributes;
            this.OldMethods = classDataToUpdate.Methods;
            //New variables
            this.ClassName = className;
            this.StereoType = stereoType;
            this.IsAbstract = isAbstract;
            this.Attributes = attributes;
            this.Methods = methods;

        }

        public override void execute()
        {
            ClassData classDataToUpdate = ShapeCollector.GetI().GetShapeById(classID) as ClassData;
            Console.WriteLine("new name: " + ClassName);
            if (classDataToUpdate != null && ClassName != null) classDataToUpdate.Name = ClassName;
            if (classDataToUpdate != null && StereoType != null) classDataToUpdate.StereoType = StereoType;
            if (classDataToUpdate != null) classDataToUpdate.IsAbstract = IsAbstract.Value;
            if (classDataToUpdate != null) classDataToUpdate.Methods = Methods;
            if(classDataToUpdate != null) classDataToUpdate.Attributes = Attributes;
        }

        public override void unExecute()
        {
            ClassData classDataToUpdate = ShapeCollector.GetI().GetShapeById(classID) as ClassData;
            if (classDataToUpdate != null)
            {
            
                classDataToUpdate.name = OldclassName;
                classDataToUpdate.StereoType = OldStereoType;
                classDataToUpdate.IsAbstract = OldAbstract;
                classDataToUpdate.Methods = OldMethods;
                classDataToUpdate.Attributes = OldAttributes;
            }

    }

        public override string UpdateInfo()
        {
            return "\tUpdate Class\t";
        }
    }
}
