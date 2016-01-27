using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Area51.SoftwareModeler.Models;

namespace Area51.SoftwareModeler.Models.Commands
{
    [XmlInclude(typeof(AddClassCommand))]
    [XmlInclude(typeof(AddCommentCommand))]
    [XmlInclude(typeof(DummyCommand))]
    [XmlInclude(typeof(MoveShapeCommand))]
    [XmlInclude(typeof(ResizeShapeCommand))]
    [XmlInclude(typeof(DeleteShapeCommand))]
    [XmlInclude(typeof(DeleteConnectionCommand))]
    [XmlInclude(typeof(UpdateClassInfoCommand))]
    [XmlInclude(typeof(AddConnectionCommand))]
    [XmlInclude(typeof(CopyClassCommand))]
    public abstract class BaseCommand : NotifyBase
    {
        //Static
        public static int nextid = 0;
        public static int nextBranchLayer = 1;

        public int Id { get; set; }
        public int BranchLayer { get; set; }

        public String CommandType { get { return GetType().Name.Substring(0, 1); } }
        public abstract string CommandName { get; }
        public abstract string Info { get; }
        /// <summary>
        /// In case some content is not accessible for Info InfoBackup will be used instead
        /// </summary>
        public string InfoBackup { get; set; }

        public void UpdateInfo()
        {
            NotifyPropertyChanged(()=>Info);
        }

        #region collapse properties
        private static List<CollapseBounds> bounds = new List<CollapseBounds>();  
        private bool _isCollapseRoot;
        public bool IsCollapseRoot { get { return _isCollapseRoot; } set { _isCollapseRoot = value; NotifyPropertyChanged(); } }
        private bool _isRootCollapsed;
        public bool IsRootCollapsed { get {return _isRootCollapsed;} set { _isRootCollapsed = value; NotifyPropertyChanged(); } }

        private bool _isCollapsed;
        public bool IsCollapsed { get {return _isCollapsed; } set { _isCollapsed = value; NotifyPropertyChanged();
            if (value)
            {
                ShapeCollector.GetI().treeArrows.Remove(LineToParent);
            }
            else
            {
               ShapeCollector.GetI().treeArrows.Add(LineToParent);
                }
        } }

        private bool _isActive;
        public bool IsActive { get { return _isActive; } set { _isActive = value; NotifyPropertyChanged(); } }
        private bool _isCollapseable;
        public bool IsCollapseable { get { return _isCollapseable; } set { _isCollapseable = value; NotifyPropertyChanged(); } }

        
        public int X { get { return BranchLayer; } set { BranchLayer = value; NotifyPropertyChanged(); } }
        private int _y;
        public int Y { get { return _y; } set { _y = value; NotifyPropertyChanged(); } }

        

        #endregion

        [XmlIgnore] private LineCommandTree _lineToParent;
        [XmlIgnore]
        public LineCommandTree LineToParent { get { return _lineToParent; } set { _lineToParent = value; NotifyPropertyChanged(); } }
  
        //Fields
        
        protected List<BaseCommand> children = new List<BaseCommand>();
        

        //Getters and setters
        private BaseCommand _parent = null;
        [XmlIgnore]
        public BaseCommand Parent { get { return _parent; } set { _parent = value; CreateLineToParent();}}
        public List<BaseCommand> Children { get { return children; } }

        //Abstract - Command pattern
        public abstract void execute();
        public abstract void unExecute();

        public BaseCommand()
        {
            this.Id = BaseCommand.GetNextId();
            Y = Id;
            bounds.Where(x=> x.UpperBoundId < Id).ToList().ForEach(x=> Y-= x.CollapseHeight);

        }

        private static int GetNextId()
        {
            return nextid++;
        }

        private static int GetNextBranchLayer()
        {
            return nextBranchLayer++;
        }

        protected void Collapse(BaseCommand collapseRoot, int lowerBound, int upperBound)
        {
            BaseCommand child = children.ElementAtOrDefault(0);
            if (IsCollapseable)
            {
                IsCollapsed = true;
                if (Id - _parent.Id > 1)
                {
                    Console.WriteLine("added extra bound: " + lowerBound + "->" + upperBound + "==" + _parent.Id +
                                      ";" + Id + "->" + Id);
                    if (lowerBound <= _parent.Id) bounds.Add(new CollapseBounds(collapseRoot.Id, lowerBound, _parent.Id));
                    lowerBound = Id;
                }
                if (child != null)
                {
                    //Console.WriteLine("parentId-Id: " + (_parent.Id - Id));
                    
                    child.Collapse(collapseRoot, lowerBound, Id);
                    return;
                }
                
                    upperBound = Id;
                    Console.WriteLine("CHILD IS NULL");
            }
            
                bounds.Add(new CollapseBounds(collapseRoot.Id, lowerBound, IsCollapseable ? upperBound : Id));
            Console.WriteLine("##########################");
            bounds.ForEach(x => Console.WriteLine("bound: " + x.CollapseRoot + ": " + x.LowerBoundId + "-->" + x.UpperBoundId + " ; " + x.CollapseHeight));
            Console.WriteLine("##########################");
            //Console.WriteLine("collapse height: " + ShapeCollector.GetI().Commands.ElementAtOrDefault(0).Id);
            ShapeCollector.GetI().Commands.ElementAtOrDefault(0)?.CollapseHeight(collapseRoot);
            
        }

        public void CollapseNodes()
        {
            BaseCommand child = children.ElementAtOrDefault(0);
            if (child != null && child.IsCollapseable)
            {
                if (child.IsCollapsed)
                {
                    IsRootCollapsed = false;
                    int num = bounds.RemoveAll(x => x.CollapseRoot == Id);
                    child.Expand(this);
                    
                    
                    Console.WriteLine("removed bounds: " + num);
                }
                else{
                    IsRootCollapsed = true;
                    child.Collapse(this, child.Id, child.Id);
                }
            }
        }

        public void Expand(BaseCommand root)
        {
            if (_isCollapseable)
            {
                IsCollapsed = false;
                BaseCommand child = children.ElementAtOrDefault(0);
                if (child != null)
                {
                    child.Expand(root);
                    return;
                }
            }
            Console.WriteLine("##########################");
            bounds.ForEach(x => Console.WriteLine("bound: " + x.CollapseRoot + ": " + x.LowerBoundId + "-->" + x.UpperBoundId + " ; " + x.CollapseHeight));
            Console.WriteLine("##########################");
            ShapeCollector.GetI().Commands.ElementAtOrDefault(0)?.CollapseHeight(root);
        }

        /*
        can be used both when collapsing and when expanding
        */
        protected void CollapseHeight(BaseCommand collapseRoot)
        {
            
            int height = Id;
            foreach (var b in bounds)
            {
                Console.WriteLine("check: " + b.CollapseRoot + "==" + collapseRoot.Id);
                if (b.CollapseRoot == collapseRoot.Id)
                {
                    Console.WriteLine("root: " + b.LowerBoundId + "->" + b.UpperBoundId);
                    if(b.UpperBoundId <= Id) Console.WriteLine("upper bound: " + Id);
                }
            }

            //bounds.Where(x=> x.CollapseRoot == collapseRoot.Id && x.UpperBoundId <= Id).ToList().ForEach(x=> Y+= collapseRoot.IsRootCollapsed ? -x.CollapseHeight : x.CollapseHeight);
            bounds.Where(x=> x.UpperBoundId <= Id).ToList().ForEach(x => height-= x.CollapseHeight);
            Y = height;
            Console.WriteLine("height set: " + Id + ";" + height + ";" + Y);

            LineToParent?.UpdateLine();
            children.ForEach(x=> x.CollapseHeight(collapseRoot));

            UpdateLineToParent();
        }

        private void CreateLineToParent()
        {
            if(LineToParent != null) UpdateLineToParent();

            BranchLayer = Parent.children.Count == 0 ? Parent.BranchLayer : GetNextBranchLayer();
            
            LineToParent = new LineCommandTree(_parent, this);
            LineToParent.IsActive = true;
            ShapeCollector.GetI().treeArrows.Add(LineToParent);
        }

        private void UpdateLineToParent()
        {
            LineToParent?.UpdateLine();
        }

        //Inherited methods
        public  void addChild(BaseCommand child)
        {
            child.Parent = this;
            children.Add(child);
            //Console.WriteLine("1#########################" +
            //                  "\nchildcount: " + children.Count +
            //                  "\nIsRoot: " + (_parent == null ? "null" : "" + _parent.IsCollapseRoot) + "-" + IsCollapseRoot + "-" + Children.ElementAt(0).IsCollapseRoot + ";" + child.IsCollapseRoot +
            //                  "\nisCollaps: " + (_parent == null ? "null" : "" + _parent.IsCollapseable) + "-" + IsCollapseable + "-" + Children.ElementAt(0).IsCollapseable + ";" + child.IsCollapseable +
            //                  "\n#########################");
            //child.IsCollapseRoot = IsCommandCollapseRoot(child); // doesn't matter. should never be the case

            if (children.Count > 1 && IsCollapsed)
            {
                int rootId = bounds.First(x => x.LowerBoundId <= Id && x.UpperBoundId >= Id).CollapseRoot;
                ShapeCollector.GetI().Commands.First(x=> x.Id == rootId).CollapseNodes();
            }

#region _parent IsCollapsedRoot and IsCollapeable properties set
            if (_parent != null){

                if (_parent.children.Count == 1){
                    if (children.Count == 1){
                        //_parent.IsCollapseRoot = true;
                    }else{
                        _parent.IsCollapseRoot = false;
                        if (_parent._parent != null && (_parent._parent.IsCollapseRoot || _parent._parent.IsCollapseable)){
                            _parent.IsCollapseable = true;
                        }
                    }
                }else{
                    if (_parent.IsCollapseRoot && children.Count > 1){
                        _parent.IsCollapseRoot = false;
                    }
                }
            }
#endregion

#region this collapse properties

            if (_parent == null && children.Count == 1)
            {
                IsCollapseRoot = true;
                child.IsCollapsed = true;
            }
            else if (_parent != null){
                if (_parent.IsCollapseRoot){
                    if (_parent.BranchLayer != BranchLayer){
                        if (children.Count == 1 || children.ElementAt(0).IsCollapseable){
                            IsCollapseRoot = true;
                            IsCollapseable = false;
                        }
                    }else if (children.Count == 1){
                        IsCollapseRoot = false;
                        IsCollapseable = true;
                    }else{
                        if (children.ElementAt(0).IsCollapseable){
                            IsCollapseRoot = true;
                            IsCollapseable = false;
                        }
                    }
                }else if (_parent.IsCollapseable){
                    if (children.Count == 1){
                        IsCollapseRoot = false;
                        IsCollapseable = true;
                    }else if(children.ElementAt(0).IsCollapseable){
                        IsCollapseRoot = true;
                        IsCollapseable = false;
                    }
                }else{
                    if (_parent.BranchLayer != BranchLayer){
                        IsCollapseRoot = true;
                        IsCollapseable = false;
                    }
                    if (children.Count > 1 && children.ElementAt(0).IsCollapseable){
                        IsCollapseRoot = true;
                        IsCollapseable = false;
                    }
                }
            }
            #endregion

#region setting child collapse properties
            if (IsCollapseRoot || IsCollapseable){
                if (children.Count == 1){
                    child.IsCollapseable = true;
                }
            }
            if (IsCollapsed && children.Count == 1){
                CollapseBounds bound = bounds.FirstOrDefault(x => x.UpperBoundId == Id);
                if (bound != null){
                    if (child.Id - Id > 1){
                        bounds.Add(new CollapseBounds(bound.CollapseRoot, child.Id, child.Id));
                    }else{
                        bounds.Remove(bound);
                        bounds.Add(new CollapseBounds(bound.CollapseRoot, bound.LowerBoundId, child.Id));
                    }
                }
                child.IsCollapseable = true;
                child.IsCollapsed = true;
            }
#endregion
        }

        

        
    }

    class CollapseBounds
    {
        public int CollapseRoot { get; }
        public int UpperBoundId { get; }
        public int LowerBoundId { get; }
        public int CollapseHeight { get; }

        public CollapseBounds(int collapseRootId, int lowerId, int upperId)
        {
            CollapseRoot = collapseRootId;
            UpperBoundId = upperId;
            LowerBoundId = lowerId;
            CollapseHeight = upperId - lowerId+1;
        }
    }
}
