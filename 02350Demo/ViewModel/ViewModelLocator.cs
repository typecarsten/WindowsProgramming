using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace _02350Demo.ViewModel
{
    /// <summary>
    /// Denne klasse kommer med MVVM Toolkit som benyttes til denne demo. 
    /// Denne klasse g�r det let at binde et vindue til en ViewModel.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            // Denne ServiceLocator holder styr p� vores ViewModels.
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Tilf�j en linje her for hver ViewModel.
            SimpleIoc.Default.Register<MainViewModel>();
        }

        // Tilf�j en metode som denne for hver ViewModel.
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        // Hvis der er noget der skal ryddes op s� g�r det her.
        public static void Cleanup()
        {
        }
    }
}