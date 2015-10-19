using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;

namespace Area51.SoftwareModeler.Model.Commands
{
    class AddCommentCommand : BaseCommand
    {
        protected AddCommentCommand(BaseCommand _parent) : base(_parent)
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
