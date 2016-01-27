using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddCommentCommand : BaseCommand
    {
        public int? ClassId;
        public Point Position;
        [XmlIgnore] public Comment CommentRep;

        public AddCommentCommand()
        {

        }

        public AddCommentCommand(Point position)
        {
            Position = position;
        }
        public override void execute()
        {
            if(CommentRep == null) CommentRep = new Comment(ClassId, Position, null);
            ClassId = CommentRep.id;
            ShapeCollector.GetI().ObsComments.Add(CommentRep);
        }

        public override void unExecute()
        {
            if (CommentRep == null) CommentRep = ShapeCollector.GetI().GetCommentById(ClassId);
            ShapeCollector.GetI().ObsComments.Remove(CommentRep);
        }

        public override string CommandName => "Add Comment";

        public override string Info
        {
            get
            {
                Comment c = ShapeCollector.GetI().GetCommentById(ClassId);
                if(c == null) return InfoBackup;
                return InfoBackup = c.Content;
            }
        }
    }
}
