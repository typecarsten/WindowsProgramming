using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _02350Demo.Model
{
    public class Node : NotifyBase
    {
        // Properties. Normalt ville automatiske attributter benyttes, som svarer til at lave et privat felt (fx x), 
        // og offentlige get og set metoder (fx GetX() og SetX()) i Java. 
        // Fx: public int X { get; set; }
        // Men på grund af at NotifyPropertyChanged skal smides i setter metoden må vi her benytte os af almindelige attributer og get/set metoder.
        // Get / set metoderne er dog stadig meget anderledes end i Java hvor man ville have en GetX() og SetX() metode.
        private static int counter = 0;
        public int Number { get; private set; }
        private int x;
        public int X { get { return x; } set { x = value; NotifyPropertyChanged("X"); NotifyPropertyChanged("CanvasCenterX"); } }
        private int y;
        public int Y { get { return y; } set { y = value; NotifyPropertyChanged("Y"); NotifyPropertyChanged("CanvasCenterY"); } }
        private int width;
        public int Width { get { return width; } set { width = value; NotifyPropertyChanged("Width"); NotifyPropertyChanged("CenterX"); NotifyPropertyChanged("CanvasCenterX"); } }
        private int height;
        public int Height { get { return height; } set { height = value; NotifyPropertyChanged("Height"); NotifyPropertyChanged("CenterY"); NotifyPropertyChanged("CanvasCenterY"); } }
        
        // Derived properties. Svarer til at lave en get metode i Java (fx GetCenterX()) som ikke har sit eget bagvedliggende felt.
        public int CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; NotifyPropertyChanged("X"); } }
        public int CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; NotifyPropertyChanged("Y"); } }
        public int CenterX { get { return Width / 2; } }
        public int CenterY { get { return Height / 2; } }

        // ViewModel properties. Den burde være i sin egen ViewModel klasse som Node klasserne wrappes af, men i dette tilfælde er det her meget lettere.
        private bool isSelected;
        public bool IsSelected { get { return isSelected; } set { isSelected = value; NotifyPropertyChanged("IsSelected"); NotifyPropertyChanged("SelectedColor"); } }
        public Brush SelectedColor { get { return IsSelected ? Brushes.Red : Brushes.Yellow; } }

        // Konstruktoren bruges i dette tilfælde til at sætte standard værdi for attributerne.
        public Node()
        {
            Number = ++counter;
            X = Y = 200;
            Width = Height = 100;
        }

        // Ved at overskrive ToString() metoden som alle klasser har, sørges der for at når en node vises så ser man nodens unikke nummer.
        public override string ToString()
        {
            return Number.ToString();
        }
    }
}
