using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
   public class DummyCommand : BaseCommand
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
        {

        }
    



        public override void execute()
        {
            Console.WriteLine("Executing DummyCommand!");
        }

        public override void unExecute()
        {
            Console.WriteLine("Undoing DummyCommand");
        }

    }
}
