using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetHopital
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            if (cboRole.SelectedIndex == 0)
            {
                if (passMotDePasse.Password.Equals("admin"))
                {
                    AdminAccueil fenetreAA = new AdminAccueil();
                    fenetreAA.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le mot de passe n'est pas correct", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else if (cboRole.SelectedIndex == 1)
            {
                if (passMotDePasse.Password.Equals("medecin"))
                {
                    MedecinAccueil fenetreMA = new MedecinAccueil();
                    fenetreMA.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le mot de passe n'est pas correct", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                if (passMotDePasse.Password.Equals("prepose"))
                {
                    PreposeAccueil fenetrePA = new PreposeAccueil();
                    fenetrePA.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le mot de passe n'est pas correct", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }


        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Voulez-vous fermer l'application?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
