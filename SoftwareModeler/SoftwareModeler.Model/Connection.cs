using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51.SoftwareModeler.Models
{
    public class Connection : NotifyBase
    {
        private Shape start;
        private string startMultiplicity;
        private string endMultiplicity;
        private Shape end;
        public double startX;// { get; set; }
        public double StartX { get { return startX; } set { startX = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => Start); } }
        public double startY;// { get; set; }
        public double StartY { get { return startY; } set { startY = value; NotifyPropertyChanged(); NotifyPropertyChanged(() => Start); } }
        public double p1X { get; set; }
        public double p1Y { get; set; }
        public double p2X { get; set; }
        public double p2Y { get; set; }
        public double endX;// { get; set; }
        public double EndX { get { return endX; } set { endX = value;  NotifyPropertyChanged(); NotifyPropertyChanged(() => End); } }
        public double endY;// { get; set; }
        public double EndY { get { return endY; } set { endY = value;  NotifyPropertyChanged(); NotifyPropertyChanged(() => End); } }
        public string startStr { get; set; }
        private ConnectionType type;

        public Shape Start { get { return start; } set { start = value; StartX = value.CanvasCenterX; StartY = value.CanvasCenterY; NotifyPropertyChanged(); NotifyPropertyChanged(() => StartX); NotifyPropertyChanged(() => StartY); } }
        public string StartMultiplicity { get { return startMultiplicity; } set { startMultiplicity = value; } }
        public Shape End { get { return end; } set { end = value; EndX = value.CanvasCenterX; EndY = value.CanvasCenterY; NotifyPropertyChanged(); NotifyPropertyChanged(() => EndY); NotifyPropertyChanged(() => EndX); } }
        public string EndMultiplicity { get { return endMultiplicity; } set { endMultiplicity = value; } }

        public Connection(Shape _start, string _startMultiplicity, Shape _end, string _endMultiplicity, ConnectionType _type)
        {
            start = _start;
            startMultiplicity = _startMultiplicity;
            end = _end;
            endMultiplicity = _endMultiplicity;
            type = _type;

            //double vec0X = 1;
            //double vec0Y = 0;
            //double vec1X = _end.CanvasCenterX - _start.CanvasCenterX;
            //double vec1Y = _end.CanvasCenterY - _start.CanvasCenterY;
            //double phi1 = Math.Acos((vec0X * vec1X) / (Math.Sqrt(Math.Pow(vec1X, 2) + Math.Pow(vec1Y, 2))));
            //double phi2 = Math.Acos((vec0Y * vec1Y) / (Math.Sqrt(Math.Pow(vec1X, 2) + Math.Pow(vec1Y, 2))));

            //if (Math.Abs(_start.CanvasCenterX - _end.CanvasCenterX) < _start.Width)
            //{
            //    startX = _start.CanvasCenterX;
            //}
            //else if(_start.CanvasCenterX > _end.CanvasCenterX)
            //{
            //    startX = _start.CanvasCenterX - _start.Width/2;
            //}
            //else
            //{
            //    startX = _start.CanvasCenterX + _start.Width/2;
            //}
            //if(Math.Abs(_start.CanvasCenterY - _end.CanvasCenterY) < _start.Height)
            //{
            //    startY = _start.CanvasCenterY;
            //}else if(_start.CanvasCenterY > _end.CanvasCenterY)
            //{
            //    startY = _start.CanvasCenterY - _start.Height/2;
            //}
            //else
            //{
            //    startY = _start.CanvasCenterY + _start.Height/2;
            //}

            startX = _start.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? _start.Width / 2 : -_start.Width / 2);
            startY = _start.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? _start.Height / 2 : -_start.Height / 2);
            endX = _end.CanvasCenterX;// + (_start.CanvasCenterX < _end.CanvasCenterX ? -_end.Width / 2 : _end.Width / 2);
            endY = _end.CanvasCenterY;// + (_start.CanvasCenterY < _end.CanvasCenterY ? -_end.Height / 2 : _end.Height / 2);
            startStr = (startX + " " + startY);
            p1X = _start.CanvasCenterX;
            p1Y = (_start.CanvasCenterY + _end.CanvasCenterY) / 2.0;
            p2Y = p1Y;
            p2X = _end.CanvasCenterX;
        }
    }
}
