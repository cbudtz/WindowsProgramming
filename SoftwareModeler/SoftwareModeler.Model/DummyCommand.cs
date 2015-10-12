using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models
{
   public class DummyCommand : ICommandExt
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
        {

        }

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



        void ICommandExt.addChild(ICommandExt child)
        {
            throw new NotImplementedException();
        }

        bool ICommandExt.canExecute()
        {
            throw new NotImplementedException();
        }

        void ICommandExt.execute()
        {
            throw new NotImplementedException();
        }

        List<ICommandExt> ICommandExt.getChildren()
        {
            throw new NotImplementedException();
        }

        ICommandExt ICommandExt.getParent()
        {
            throw new NotImplementedException();
        }

        void ICommandExt.unExecute()
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            reader.ReadEndElement();
            
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            writer.WriteEndElement();
        }

      
    }
}
