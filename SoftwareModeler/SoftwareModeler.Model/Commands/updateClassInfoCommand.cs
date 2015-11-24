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

        //Update parameters
        public string className { get; set; }
        public string oldclassName { get; set; }

        public string stereoType { get; set; }
        public string oldStereoType { get; set; }
        public bool? isAbstract { get; set; }
        public bool oldAbstract { get; set; }
        public Visibility? visibility { get; set; }
        public Visibility OldVisibility { get; set; }

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
        /// <param name="anchorPoint"></param>
        /// <param name="visibility"></param>
        public UpdateClassInfoCommand(Class classToUpdate, string className, string stereoType, bool? isAbstract, Visibility? visibility)
        {
            //ClassInstance
            this.classID = classToUpdate.id;
            this.oldclassName = classToUpdate.name;
            this.oldStereoType = classToUpdate.StereoType;
            this.oldAbstract = classToUpdate.IsAbstract;
            this.OldVisibility = classToUpdate.Visibility;
            //New variables
            this.className = className;
            this.stereoType = stereoType;
            this.isAbstract = isAbstract;
            this.visibility = visibility;

        }

        public override void execute()
        {
            Class classToUpdate = ShapeCollector.getI().getShapeByID(classID) as Class;
            if (className != null) classToUpdate.name = className;
            if (stereoType != null) classToUpdate.StereoType = stereoType;
            if (isAbstract != null) classToUpdate.IsAbstract = isAbstract.Value;
            if (visibility != null) classToUpdate.Visibility = visibility.Value;
        }

        public override void unExecute()
        {
            Class classToUpdate = ShapeCollector.getI().getShapeByID(classID) as Class;
            classToUpdate.name = oldclassName;
            classToUpdate.StereoType = oldStereoType;
            classToUpdate.IsAbstract = oldAbstract;
            classToUpdate.Visibility = OldVisibility;

        }
    }
}
