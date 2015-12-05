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
        public Class copy;

        public CopyClassCommand()
        {
            // for deserializing
        }
        public CopyClassCommand(Class toCopy)
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

        public void Copy(Class toCopy)
        {
            if (toCopy == null) return;
            toCopyId = toCopy.id;
            //Class shapeToCopy = ShapeCollector.GetI().GetShapeById(copyId);
            copy = new Class(toCopy.name, toCopy.StereoType, toCopy.IsAbstract, new Point(toCopy.X + 30, toCopy.Y + 30));
            toCopy.Methods.ForEach(x => copy.AddMethod(x.Visibility, x.Name, x.Parameters));
            toCopy.Attributes.ForEach(x => copy.AddAttribute(x.Type, x.Name));
            copyId = copy.id;
        }
    }
}
