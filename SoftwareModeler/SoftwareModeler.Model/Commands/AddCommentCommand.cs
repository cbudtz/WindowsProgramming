using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.SoftwareModeler.Models;
using Area51.SoftwareModeler.Models.Commands;
using System.Collections.ObjectModel;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class AddCommentCommand : BaseCommand
    {
        public Point Position;
        public AddCommentCommand()
        {

        }

        public AddCommentCommand(Point position)
        {
            Position = position;
        }
        public override void execute()
        {
            ShapeCollector.GetI().ObsComments.Add(new Comment(Position, null));
        }

        public override void unExecute()
        {
            
        }

        public override string UpdateInfo()
        {
            throw new NotImplementedException();
        }
    }
}
