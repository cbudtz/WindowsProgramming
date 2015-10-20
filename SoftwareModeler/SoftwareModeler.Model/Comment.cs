using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Model
{
    class Comment
    {
        private string commentString { get; set; }

        public Comment(string commentString)
        {
            this.commentString = commentString;
        }
    }
}
