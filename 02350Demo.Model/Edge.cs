using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350Demo.Model
{
    // Kant klassen har en reference til de to noder som den er imellem.
    public class Edge : NotifyBase
    {
        // Properties.
        private Node endA;
        private Node endB;
        public Node EndA { get { return endA; } set { endA = value; NotifyPropertyChanged("EndA"); } }
        public Node EndB { get { return endB; } set { endB = value; NotifyPropertyChanged("EndB"); } }
    }
}
