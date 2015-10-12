using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Area51.SoftwareModeler.Models
{
   public class DummyCommand : ICommandExt
    {
        public event EventHandler CanExecuteChanged;

        public void addChild(ICommandExt child)
        {
            throw new NotImplementedException();
        }

        public bool canExecute()
        {
            throw new NotImplementedException();
        }

        public void execute()
        {
            throw new NotImplementedException();
        }

        public List<ICommandExt> getChildren()
        {
            throw new NotImplementedException();
        }

        public ICommandExt getParent()
        {
            throw new NotImplementedException();
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }
    }
}
