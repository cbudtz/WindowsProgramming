using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddShapeCommand : BaseCommand
    {
        public AddShapeCommand()
        {

        }
        public AddShapeCommand(BaseCommand _parent)
            : base(_parent)
        {

        }

        public override void execute()
        {
            throw new NotImplementedException();
        }


        public override void unExecute()
        {
            throw new NotImplementedException();
        }


    }
}
