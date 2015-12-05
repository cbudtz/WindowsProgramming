using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Area51.SoftwareModeler.Models
{
    
    public class Class : Shape
    {
        private string _stereotype;
        private bool _isAbstract;
        //private Point AnchorPoint;
        //private Visibility visibility;
        private List<Attribute> _attributes;
        private List<Method> _methods;

        public Class() : base()
        {

        }

        public Class(string name, string stereotype, bool isAbstract, Point anchorPoint)
        {
            this.name = name;
            _stereotype = stereotype;
            _isAbstract = isAbstract;
            X = anchorPoint.X;
            Y = anchorPoint.Y;
            _attributes = new List<Attribute>();
            _methods = new List<Method>();


        }

        public Class(int? id, string name, string stereotype, bool isAbstract, Point anchorPoint) : base(id)
        {
            this.name = name;
            _stereotype = stereotype;
            _isAbstract = isAbstract;
            X = anchorPoint.X;
            Y = anchorPoint.Y;
            _attributes = new List<Attribute>();
            _methods = new List<Method>();

        }

        //Getters and setters
        public string Name { get { return name;} set { name = value; NotifyPropertyChanged(); } }
        public string StereoType { get { return ( _stereotype==null || _stereotype.Equals("") ? "" : "<<"+_stereotype+">>"); } set { _stereotype = value; NotifyPropertyChanged();} }
        public bool IsAbstract { get { return _isAbstract; } set { _isAbstract = value; NotifyPropertyChanged();} }
        public List<Attribute> Attributes { get { return _attributes; } set { _attributes = value; NotifyPropertyChanged();} }
        public List<Method> Methods { get { return _methods; } set { _methods = value; NotifyPropertyChanged();} }



        public void AddAttribute(string type, string name)
        {
            _attributes.Add(new Attribute(type, name));
        }

        public void AddAttribute(Visibility visibility, string type, string name)
        {
            _attributes.Add(new Attribute(visibility, type, name));
        }

        public void AddMethod(Visibility visibility, string name, string[] parameters)
        {
            Method m = new Method(visibility, name);
            string param = "";
            for (int i = 0; i < parameters.Length; i++)
                param += parameters + ", ";
            //m.addParameter(parameters[i]);
            param = param.Substring(0, param.Length - 1);
            m.Parameters = param;
            _methods.Add(m);
        }

       
        public override string ToString()
        {
            return name;
        }
    }
}
