using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models
{
   public class DummyCommand : BaseCommand
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
            :base(null)
        {

        }
        public DummyCommand(BaseCommand parent)
            :base(parent)
        {

        }



        public override void execute()
        {
            throw new NotImplementedException();
        }

        public List<BaseCommand> getChildren()
        {
            throw new NotImplementedException();
        }

        public BaseCommand getParent()
        {
            throw new NotImplementedException();
        }

        public override void unExecute()
        {
            throw new NotImplementedException();
        }

    }
}
