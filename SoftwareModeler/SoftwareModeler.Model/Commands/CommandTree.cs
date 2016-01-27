using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Area51.SoftwareModeler.Models.Commands
{
    public class CommandTree : NotifyBase
    {
        public BaseCommand Root { get; set; }
        public BaseCommand Active { get; set; }
        public List<BaseCommand> Undone { get; set; } = new List<BaseCommand>();
        public int NextShapeId { get; set; }
        public int NextCommandId { get; set; }
        [XmlIgnore]
        public ObservableCollection<BaseCommand> Commands { get; set; }

        public CommandTree()
        {
            BaseCommand.nextid = 0;
        }

        public void AddAndExecute(BaseCommand command)
        {
            if (Root == null)
            {
                //Root node - serialization starts here...
                Root = command;
                SetActive(Root);
            }
            else
            {
                //Add child to tree
                Active.addChild(command);
                SetActive(command);
            }              

            ShapeCollector.GetI().Commands.Add(command);
            NotifyPropertyChanged(() => ShapeCollector.GetI().Commands);

       
            //ececute new command
            Active.execute();
            foreach (var c in ShapeCollector.GetI().Commands)
            {
                c.UpdateInfo();
            }

        }

        private void SetActive(BaseCommand node)
        {   
            if (Active != null)
            {
                Active.IsActive = false;
            }
            Active = node;
            if (Active != null)
            {
                Active.IsActive = true;
            }
        }

        public void SetActiveCommand(BaseCommand command)
        {
            //Update activeCommand
            BaseCommand newActiveNode = ReParseTree(Root, command.Id);
            Undone.Clear();
            SetActive(newActiveNode);
            ReExecute();
            foreach (var c in ShapeCollector.GetI().Commands)
            {
                c.UpdateInfo();
            }
        }

        public void ReExecute()
        {
            //Remove all objects from canvas
            ShapeCollector.GetI().Reset();
            //Execute all Commands on branch to active node.
            LinkedList<BaseCommand> reExecuteList = new LinkedList<BaseCommand>();
            BaseCommand curCommand = Active;
            while (curCommand != null)
            {
                reExecuteList.AddFirst(curCommand);
                curCommand = curCommand.Parent;
            }

            foreach (var l in ShapeCollector.GetI().treeArrows)
            {
                l.IsActive = false;
            }

            foreach (BaseCommand b in reExecuteList)
            {

                if (b.Parent != null && b.LineToParent != null)
                {
                    b.LineToParent.IsActive = true;
                }

                b.execute();
            }

        }

        private static BaseCommand ReParseTree(BaseCommand node, int id)
        {
            BaseCommand activeNode = null;
            if (node == null) return null;

            if (node.Id == id)
            {
                activeNode = node;
            }
            if (!node.Children.Equals(null) && node.Children.Count > 0)
            {

                foreach (BaseCommand child in node.Children)
                {
                    if(child.Parent == null) child.Parent = node;
                    BaseCommand recNode = CommandTree.ReParseTree(child, id);
                    if (recNode != null) activeNode = recNode;

                }
            }
            return activeNode;

        }

        public static void Save(CommandTree commandTree)
        {
            Save(commandTree, new StreamWriter(@"output.xml"));
        }

        public static async void AsyncSave(CommandTree commandTree, FileStream fileStream)
        {
            await Task.Run(() => Save(commandTree, fileStream));
        }

        public static async void AsyncSave(CommandTree commandTree)
        {
            await Task.Run(() => Save(commandTree));
        }

        public static void Save(CommandTree commandTree, StreamWriter saveWriter)
        {

            //Making sure that new shapes will get a new ID when deSerializing
            commandTree.NextShapeId = ClassData.nextId;
            //Serialize CommandTree TODO: Add FileSelectBox
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (StreamWriter writer = saveWriter)
                serializer.Serialize(writer, commandTree);
        }

        public static void Save(CommandTree commandTree, FileStream fileStream)
        {

            //Making sure that new shapes will get a new ID when deSerializing
            commandTree.NextShapeId = ClassData.nextId;
            commandTree.NextCommandId = BaseCommand.nextid;
            //Serialize CommandTree TODO: Add FileSelectBox
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
            {
                serializer.Serialize(writer, commandTree);
            }
            fileStream.Close();
        }

        public static CommandTree Load()
        {
            return Load(new StreamReader(@"output.xml"));
        }

        public static CommandTree Load(StreamReader loadReader)
        {
            //Empty ShapeCollector Singleton
            ShapeCollector.GetI().Reset();
            ShapeCollector.GetI().Commands.Clear();
            ShapeCollector.GetI().treeArrows.Clear();
            //restore Tree
            XmlSerializer serializer = new XmlSerializer(typeof(CommandTree), new XmlRootAttribute("Commandtree"));
            CommandTree restoredTree;
            using (StreamReader reader = loadReader)
                restoredTree = serializer.Deserialize(reader) as CommandTree;
            //Make sure that newly Added Shapes get a new ID
            ClassData.nextId = restoredTree.NextShapeId;
            BaseCommand.nextid = restoredTree.NextCommandId;
           

            //Reestablishing parents and finding active node
            restoredTree.SetActive(CommandTree.ReParseTree(restoredTree.Root, restoredTree.Active.Id));

            ShapeCollector.GetI().Commands.Clear();
            VisitChildren(restoredTree.Root);
 
            //Moving diagram to active state
            restoredTree.ReExecute();

            return restoredTree;
        }

        public void Undo()
        {
            if (Active == Root)
            {

                return;
            }
            Active.LineToParent.IsActive = false;
            Active.unExecute();
            Undone.Add(Active);
            SetActive(Active.Parent);

        }

        public static void VisitChildren(BaseCommand root)
        {
            if (!ShapeCollector.GetI().Commands.Contains(root))
            {
                ShapeCollector.GetI().Commands.Add(root);
                if (root.Parent != null)
                {
                    LineCommandTree line = new LineCommandTree(root.Parent, root);
                    ShapeCollector.GetI().treeArrows.Add(line);
                    root.LineToParent = line;
                }
            }
            root.Children?.ForEach(VisitChildren);
            
        }

        public void Redo()
        {
            if (Undone == null || Undone.Count == 0) return;
            BaseCommand reDoCommand = Undone.Last();
            reDoCommand.LineToParent.IsActive = true;
            reDoCommand.execute();
            Undone.Remove(reDoCommand);
            SetActive(reDoCommand);
        }

    }
}
