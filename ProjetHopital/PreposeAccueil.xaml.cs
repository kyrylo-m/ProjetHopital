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
using System.Windows.Shapes;

namespace ProjetHopital
{
    /// <summary>
    /// Logique d'interaction pour PreposeAccueil.xaml
    /// </summary>
    public partial class PreposeAccueil : Window
    {
        HopitalEntities myBdd;
        public PreposeAccueil()
        {
            InitializeComponent();
        }                                

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBdd = new HopitalEntities();
            afficherPatients();
        }

        public void afficherPatients()
        {
            // On sélectionne les patients qui sont présentement hospitalisés
            IEnumerable<Patient> patientsHospitalises = from p in myBdd.Patients
                                                 join adm in myBdd.Admissions
                                                 on p.NumAssuranceMaladie equals adm.NumAssuranceMaladie
                                                 where adm.DateConge == null
                                                 orderby p.Nom
                                                 select p;

            // On sélectionne les patients qui NE sont présentement PAS hospitalisés
            IEnumerable<Patient> patientsNonHospitalises = from pat in myBdd.Patients
                                          where !patientsHospitalises.Any(adm => adm.NumAssuranceMaladie == pat.NumAssuranceMaladie)
                                          select pat;

            if (chNonHospitalises.IsChecked == true)
            {
                // On affiche seulement les patients NON-hospitalisés
                dgListePatients.DataContext = patientsNonHospitalises.ToList(); 
            }
            else
            {
                // On affiche tous les patients
                dgListePatients.DataContext = (from patient in myBdd.Patients
                                               orderby patient.Nom
                                               select patient).ToList();
            }
        } // afficherPatients()

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

                
        private void btnAfficherPatient_Click(object sender, RoutedEventArgs e)
        {
            Patient sPatient = dgListePatients.SelectedItem as Patient;

            if (sPatient == null || sPatient.NumAssuranceMaladie == null)
            {
                AfficherPatient fenetreAP = new AfficherPatient();
                fenetreAP.ShowDialog();
            }
            else
            {
                AfficherPatient fenetreAP = new AfficherPatient(sPatient);
                fenetreAP.ShowDialog();
            }
            
        }

        private void btnAjouterPatient_Click(object sender, RoutedEventArgs e)
        {
            AjouterPatient fenetreAP = new AjouterPatient();
            fenetreAP.ShowDialog();
            this.Close();
        }

        private void chNonHospitalises_Click(object sender, RoutedEventArgs e)
        {
            afficherPatients();
        }

        private void btnAjouterAdmission_Click(object sender, RoutedEventArgs e)
        {
            Patient sPatient = dgListePatients.SelectedItem as Patient;

            if (sPatient == null || sPatient.NumAssuranceMaladie == null)
            {
                MessageBox.Show("Choisissez un patient pour lui accorder une admission", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                AjouterAdmission fenetreAA = new AjouterAdmission(sPatient);
                fenetreAA.ShowDialog();
            }
        }

        private void btnListerAdmissions_Click(object sender, RoutedEventArgs e)
        {
            ListerAdmissions fenetreLA = new ListerAdmissions();
            fenetreLA.ShowDialog();
        }
    }
}
