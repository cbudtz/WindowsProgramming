using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Area51.SoftwareModeler.ModelLibraryOld
{
    class DummyCommand : ICommand {

        event EventHandler ICommand.CanExecuteChanged {
            add {
                throw new NotImplementedException();
            }
            remove {
                throw new NotImplementedException();
            }
        }

        bool ICommand.CanExecute(object parameter){
            throw new NotImplementedException();
        }

        void ICommand.Execute(object parameter) {
            throw new NotImplementedException();
        }
    }
}
