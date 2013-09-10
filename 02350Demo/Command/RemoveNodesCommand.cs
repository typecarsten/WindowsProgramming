using _02350Demo.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350Demo.Command
{
    // Bruges til at fjerne punkt fra punkt samlingen, fjerne også tilhørende kanter.
    public class RemoveNodesCommand : IUndoRedoCommand
    {
        private ObservableCollection<Node> nodes;
        private ObservableCollection<Edge> edges;
        private Node removeNode;
        private List<Edge> removeEdges;

        public RemoveNodesCommand(ObservableCollection<Node> _nodes, ObservableCollection<Edge> _edges, Node _removeNode) 
        { 
            nodes = _nodes; 
            edges = _edges; 
            removeNode = _removeNode; 
            removeEdges = _edges.Where(x => x.EndA.Number == removeNode.Number || x.EndB.Number == removeNode.Number).ToList(); 
        }

        public void Execute()
        {
            foreach (Edge e in removeEdges) edges.Remove(e);
            nodes.Remove(removeNode);
        }

        public void UnExecute()
        {
            nodes.Add(removeNode);
            foreach (Edge e in removeEdges) {
                if (e.EndA == null) e.EndA = removeNode;
                if (e.EndB == null) e.EndB = removeNode;
                edges.Add(e);
            }
        }
    }
}
