using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Area51.SoftwareModeler.Models
{
    public interface ICommandExt
    {
        void execute();
        void unExecute();
        bool canExecute();
        ICommandExt getParent();
        List<ICommandExt> getChildren();
        void addChild(ICommandExt child);
    }
}
