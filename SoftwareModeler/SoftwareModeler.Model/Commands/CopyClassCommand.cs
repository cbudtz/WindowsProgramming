using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CopyClassCommand : BaseCommand
    {
        public int? copyId;
        public int? toCopyId;

        [XmlIgnore]
        public ClassData copy;

        public CopyClassCommand()
        {
            // for deserializing
        }
        public CopyClassCommand(ClassData toCopy)
        {
            Copy(toCopy);
        }
        public override void execute()
        {
            if (copy == null) Copy(ShapeCollector.GetI().GetShapeById(toCopyId));
            if (copy != null)
            {
                copy.id = copyId;
                ShapeCollector.GetI().ObsShapes.Add(copy);
            }
        }
        public override void unExecute()
        {
            if (copy != null) ShapeCollector.GetI().ObsShapes.Remove(copy);
        }

        public void Copy(ClassData toCopy)
        {
            if (toCopy == null) return;
            toCopyId = toCopy.id;
            //ClassData shapeToCopy = ShapeCollector.GetI().GetShapeById(copyId);
            copy = new ClassData(toCopy.Name, toCopy.StereoType, toCopy.IsAbstract, new Point(toCopy.X + 30, toCopy.Y + 30));
            toCopy.Methods.ForEach(x => copy.AddMethod(x.Visibility, x.Name, x.Parameters));
            toCopy.Attributes.ForEach(x => copy.AddAttribute(x.Type, x.Name));
            copyId = copy.id;
        }

        private string _toCopyNameBackup;
        private string _copyNameBackup;

        public override string CommandName => "Copy Class";

        public override string Info
        {
            get
            {
                ClassData c1 = ShapeCollector.GetI().GetShapeById(toCopyId);
                ClassData c2 = ShapeCollector.GetI().GetShapeById(copyId);
                if (c1 != null) _toCopyNameBackup = c1.Name;
                if (c2 != null) _copyNameBackup = c2.Name;
                return "Class copied: " + _toCopyNameBackup + (c1 == null ? " (Deleted)" : "") +
                       "\n new class: " + _copyNameBackup + (c2 == null ? " (Deleted)" : "");
            }
        }
    }
}
