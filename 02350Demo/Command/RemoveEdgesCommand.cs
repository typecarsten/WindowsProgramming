using _02350Demo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350Demo.Command
{
    // Bruges til at fjerne kant fra kant samlingen.
    public class RemoveEdgesCommand : IUndoRedoCommand
    {
        private ObservableCollection<Edge> edges;
        private List<Edge> removeEdges;

        public RemoveEdgesCommand(ObservableCollection<Edge> _edges, List<Edge> _removeEdges) { edges = _edges; removeEdges = _removeEdges; }

        public void Execute()
        {
            foreach (Edge e in removeEdges) edges.Remove(e);
        }

        public void UnExecute()
        {
            foreach (Edge e in removeEdges) edges.Add(e);
        }
    }
}
