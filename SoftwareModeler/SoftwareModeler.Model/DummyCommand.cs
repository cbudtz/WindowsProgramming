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
   public class DummyCommand : ICommandExt
    {
        public event EventHandler CanExecuteChanged;

        public DummyCommand()
        {

        }
        
        public override void addChild(ICommandExt child)
        {
            throw new NotImplementedException();
        }

        public override bool canExecute()
        {
            throw new NotImplementedException();
        }

        public override void execute()
        {
            throw new NotImplementedException();
        }

        public override List<ICommandExt> getChildren()
        {
            throw new NotImplementedException();
        }

        public override ICommandExt getParent()
        {
            throw new NotImplementedException();
        }
        public override void unExecute()
        {
            throw new NotImplementedException();
        }



        public override XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            reader.ReadEndElement();
            
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().Name);
            //TODO: Fill in attributes
            writer.WriteEndElement();
        }

      
    }
}
