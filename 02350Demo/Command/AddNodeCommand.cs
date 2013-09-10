using _02350Demo.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350Demo.Command
{
    // Bruges til at tilføje punkt til punkt samlingen.
    public class AddNodeCommand : IUndoRedoCommand
    {
        private ObservableCollection<Node> nodes;
        private Node node;

        public AddNodeCommand(ObservableCollection<Node> _nodes) { nodes = _nodes; }

        public void Execute()
        {
            nodes.Add(node = new Node());
        }

        public void UnExecute()
        {
            nodes.Remove(node);
        }
    }
}
