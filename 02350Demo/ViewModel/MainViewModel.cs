using _02350Demo.Command;
using _02350Demo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _02350Demo.ViewModel
{
    /// <summary>
    /// Denne ViewModel er bundet til MainWindow.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // Holder styr på undo/redo.
        private UndoRedoController undoRedoController = UndoRedoController.GetInstance();

        // Bruges til at ændre tilstand når en kant er ved at blive tilføjet.
        private bool isAddingEdge;
        // Bruges til at gemme det første punkt når en kant tilføjes. 
        private Node addingEdgeEndA;
        // Gemmer det første punkt som punktet har under en flytning.
        private Point moveNodePoint;
        // Bruges til at gøre punkterne gennemsigtige når en ny kant tilføjes.
        public double ModeOpacity { get { return isAddingEdge ? 0.4 : 1.0; } }

        // Formålet med at benytte en ObservableCollection er at den implementere INotifyCollectionChanged, der er forskellige fra INotifyPropertyChanged.
        // INotifyCollectionChanged smider en event når mængden af elementer i en kollektion ændres (altså når et element fjernes eller tilføjes).
        // Denne event giver GUI'en besked om ændringen.
        // Dette er en generisk kollektion. Det betyder at den kan defineres til at indeholde alle slags klasser, 
        // men den holder kun klasser af en type når den benyttes.
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }

        // Kommandoer som UI bindes til.
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        // Kommandoer som UI bindes til.
        public ICommand AddNodeCommand { get; private set; }
        public ICommand RemoveNodeCommand { get; private set; }
        public ICommand AddEdgeCommand { get; private set; }
        public ICommand RemoveEdgesCommand { get; private set; }

        // Kommandoer som UI bindes til.
        public ICommand MouseDownNodeCommand { get; private set; }
        public ICommand MouseMoveNodeCommand { get; private set; }
        public ICommand MouseUpNodeCommand { get; private set; }

        public MainViewModel()
        {
            // Her fyldes listen af noder med to noder. Her benyttes et alternativ til konstruktorer med syntaksen 'new Type(){ Attribut = Værdi }'
            // Det der sker er at der først laves et nyt object og så sættes objektets attributer til de givne værdier.
            Nodes = new ObservableCollection<Node>() { 
                new Node() { X = 30, Y = 40, Width = 80, Height = 80 }, 
                new Node() { X = 140, Y = 230, Width = 100, Height = 100 } };
            // ElementAt() er en LINQ udvidelses metode som ligesom mange andre kan benyttes på stort set alle slags kollektioner i .NET.
            Edges = new ObservableCollection<Edge>() { 
                new Edge() { EndA = Nodes.ElementAt(0), EndB = Nodes.ElementAt(1) } };

            // Kommandoerne som UI kan kaldes bindes til de metoder der skal kaldes. Her vidersendes metode kaldne til UndoRedoControlleren.
            UndoCommand = new RelayCommand(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand(undoRedoController.Redo, undoRedoController.CanRedo);

            // Kommandoerne som UI kan kaldes bindes til de metoder der skal kaldes.
            AddNodeCommand = new RelayCommand(AddNode);
            RemoveNodeCommand = new RelayCommand<IList>(RemoveNode, CanRemoveNode);
            AddEdgeCommand = new RelayCommand(AddEdge);
            RemoveEdgesCommand = new RelayCommand<IList>(RemoveEdges, CanRemoveEdges);

            // Kommandoerne som UI kan kaldes bindes til de metoder der skal kaldes.
            MouseDownNodeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownNode);
            MouseMoveNodeCommand = new RelayCommand<MouseEventArgs>(MouseMoveNode);
            MouseUpNodeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpNode);
        }

        // Tilføjer punkt med kommando.
        public void AddNode()
        {
            undoRedoController.AddAndExecute(new AddNodeCommand(Nodes));
        }

        // Tjekker om valgte punkt/er kan fjernes. Det kan de hvis der er nogle der er valgt.
        public bool CanRemoveNode(IList _nodes)
        {
            return _nodes.Count == 1;
        }

        // Fjerner valgte punkter med kommando.
        public void RemoveNode(IList _nodes)
        {
            undoRedoController.AddAndExecute(new RemoveNodesCommand(Nodes, Edges, _nodes.Cast<Node>().First()));
        }

        // Starter proceduren der tilføjer en kant.
        public void AddEdge()
        {
            isAddingEdge = true;
            RaisePropertyChanged("ModeOpacity");
        }

        // Tjekker om valgte kant/er kan fjernes. Det kan de hvis der er nogle der er valgt.
        public bool CanRemoveEdges(IList _edges)
        {
            return _edges.Count > 0;
        }

        // Fjerner valgte kanter med kommando.
        public void RemoveEdges(IList _edges)
        {
            undoRedoController.AddAndExecute(new RemoveEdgesCommand(Edges, _edges.Cast<Edge>().ToList()));
        }

        // Hvis der ikke er ved at blive tilføjet en kant så fanges musen når en musetast trykkes ned. Dette bruges til at flytte punkter.
        public void MouseDownNode(MouseButtonEventArgs e)
        {
            if(!isAddingEdge) e.MouseDevice.Target.CaptureMouse();
        }

        // Bruges til at flytter punkter.
        public void MouseMoveNode(MouseEventArgs e)
        {
            // Tjek at musen er fanget og at der ikke er ved at blive tilføjet en kant.
            if (Mouse.Captured != null && !isAddingEdge) {
                // Musen er nu fanget af ellipserne og den ellipse som musen befinder sig over skaffes her.
                FrameworkElement movingEllipse = (FrameworkElement)e.MouseDevice.Target;
                // Fra ellipsen skaffes punktet som den er bundet til.
                Node movingNode = (Node)movingEllipse.DataContext;
                // Canvaset findes her udfra ellipsen.
                Canvas canvas = FindParentOfType<Canvas>(movingEllipse);
                // Musens position i forhold til canvas skaffes her.
                Point mousePosition = Mouse.GetPosition(canvas);
                // Når man flytter noget med musen vil denne metode blive kaldt mange gange for hvert lille ryk, 
                // derfor gemmes her positionen før det første ryk så den sammen med den sidste position kan benyttes til at flytte punktet med en kommando.
                if (moveNodePoint == default(Point)) moveNodePoint = mousePosition;
                // Punktets position ændres og beskeden bliver så sendt til UI med INotifyPropertyChanged mønsteret.
                movingNode.CanvasCenterX = (int)mousePosition.X;
                movingNode.CanvasCenterY = (int)mousePosition.Y;
            }
        }

        // Benyttes til at flytte punkter og tilføje kanter.
        public void MouseUpNode(MouseButtonEventArgs e)
        {
            // Benyttes til at tilføje kanter.
            if (isAddingEdge) {
                // Skaffer ellipsen som musen er over.
                FrameworkElement ellipseEnd = (FrameworkElement)e.MouseDevice.Target;
                // Skaffer ellipsens node.
                Node nodeEnd = (Node)ellipseEnd.DataContext;
                // Hvis det er den første ellipse der er blevet trykket på under tilføjelsen af kanten, så gemmes punktet bare og punktet bliver markeret som valgt.
                if (addingEdgeEndA == null) { addingEdgeEndA = nodeEnd; addingEdgeEndA.IsSelected = true; }
                // Ellers hvis det ikke er den første og de to noder der hører til ellipserne er forskellige, så oprettes kanten med kommando.
                else if (addingEdgeEndA != nodeEnd) {
                    undoRedoController.AddAndExecute(new AddEdgeCommand(Edges, addingEdgeEndA, (Node)ellipseEnd.DataContext));
                    // De tilhørende værdier nulstilles.
                    isAddingEdge = false;
                    RaisePropertyChanged("ModeOpacity");
                    addingEdgeEndA.IsSelected = false;
                    addingEdgeEndA = null;
                }
            }
            // Benyttes til at flytte punkter.
            else {
                // Ellipsen skaffes.
                FrameworkElement movingEllipse = (FrameworkElement)e.MouseDevice.Target;
                // Ellipsens node skaffes.
                Node movingNode = (Node)movingEllipse.DataContext;
                // Canvaset skaffes.
                Canvas canvas = FindParentOfType<Canvas>(movingEllipse);
                // Musens position på canvas skaffes.
                Point mousePosition = Mouse.GetPosition(canvas);
                // Punktet flyttes med kommando. Den flyttes egentlig bare det sidste stykke i en række af mange men da de originale punkt gemmes er der ikke noget problem med undo/redo.
                undoRedoController.AddAndExecute(new MoveNodeCommand(movingNode, (int)mousePosition.X, (int)mousePosition.Y, (int)moveNodePoint.X, (int)moveNodePoint.Y));
                // Nulstil værdier.
                moveNodePoint = new Point();
                // Musen frigøres.
                e.MouseDevice.Target.ReleaseMouseCapture(); 
            }
        }

        // Rekursiv metode der benyttes til at finde et af et grafisk elements forfædre ved hjælp af typen, der ledes højere og højere op indtil en af typen findes.
        // Syntaksen "() ? () : ()" betyder hvis den første del bliver sand så skal værdien være den anden del, ellers skal den være den tredje del.
        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}