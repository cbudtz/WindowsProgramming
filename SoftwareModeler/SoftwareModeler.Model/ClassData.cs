using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Area51.SoftwareModeler.Models
{
    
    public class ClassData : ClassView
    {
        private string _stereotype;
        private bool _isAbstract;
        private string _package;
        private List<Attribute> _attributes;
        private List<Method> _methods;
        private string _name;

#region Properties
        /// <summary>
        /// Get or set Name of Class representation
        /// </summary>
        public string Name { get { return _name; } set { _name = value; NotifyPropertyChanged(); } }
        
        /// <summary>
        ///  Get or set stereotype, fx interface. '<< >>' tag is automatically added 
        /// </summary>
        public string StereoType { get { return (_stereotype == null || _stereotype.Equals("") ? "" : "<<" + _stereotype + ">>"); } set { _stereotype = value; NotifyPropertyChanged(); } }
        public bool IsAbstract { get { return _isAbstract; } set { _isAbstract = value; NotifyPropertyChanged(); } }
        public List<Attribute> Attributes { get { return _attributes; } set { _attributes = value; NotifyPropertyChanged(); } }
        public List<Method> Methods { get { return _methods; } set { _methods = value; NotifyPropertyChanged(); } }
#endregion

        public ClassData()
        {
            nextId++;
        }

        public ClassData(string name, string stereotype, bool isAbstract, Point anchorPoint) 
            : this(null, name, stereotype, isAbstract, anchorPoint)
        {}

        public ClassData(int? id, string name, string stereotype, bool isAbstract, Point anchorPoint)
        {
            this.id = id ?? nextId;
            nextId = (id ?? nextId) + 1;
            _name = name ?? "class" + id;
            _stereotype = stereotype;
            _isAbstract = isAbstract;
            X = anchorPoint.X;
            Y = anchorPoint.Y;
            _attributes = new List<Attribute>();
            _methods = new List<Method>();

        }

        


        public void AddAttribute(string type, string name)
        {
            _attributes.Add(new Attribute(type, name));
        }

        public void AddAttribute(Visibility visibility, string type, string name)
        {
            _attributes.Add(new Attribute(visibility, type, name));
        }

        public void AddMethod(Visibility visibility, string name, string parameters)
        {
            _methods.Add(new Method(visibility, name, parameters));
        }

       
        public override string ToString()
        {
            return _name;
        }
    }
}
