using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Model.Commands
{
    class AddShapeCommand : ICommandExt
    {
        public List<ICommandExt> Children
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICommandExt Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void addChild(ICommandExt child)
        {
            throw new NotImplementedException();
        }

        public void execute()
        {
            throw new NotImplementedException();
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }
    }
}
