﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;

namespace Area51.SoftwareModeler.Models
{
   public class DummyCommand : BaseCommand
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
        {

        }

        public void addChild(BaseCommand child)
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

        public List<BaseCommand> getChildren()
        {
            throw new NotImplementedException();
        }

        public BaseCommand getParent()
        {
            throw new NotImplementedException();
        }
        public void unExecute()
        {
            throw new NotImplementedException();
        }



        void BaseCommand.addChild(BaseCommand child)
        {
            throw new NotImplementedException();
        }

        bool BaseCommand.canExecute()
        {
            throw new NotImplementedException();
        }

        void BaseCommand.execute()
        {
            throw new NotImplementedException();
        }

        List<BaseCommand> BaseCommand.getChildren()
        {
            throw new NotImplementedException();
        }

        BaseCommand BaseCommand.getParent()
        {
            throw new NotImplementedException();
        }

        void BaseCommand.unExecute()
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
