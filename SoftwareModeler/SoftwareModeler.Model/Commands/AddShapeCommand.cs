using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Model.Commands
{
    class AddShapeCommand : BaseCommand
    {
        public AddShapeCommand(BaseCommand _parent)
            : base(_parent)
        {

        }
        public override void addChild(BaseCommand child)
        {
            throw new NotImplementedException();
        }

        public override void execute()
        {
            throw new NotImplementedException();
        }

        public override XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void unExecute()
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
