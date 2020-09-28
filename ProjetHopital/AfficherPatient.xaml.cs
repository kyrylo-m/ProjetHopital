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
    /// Logique d'interaction pour AfficherPatient.xaml
    /// </summary>
    public partial class AfficherPatient : Window
    {
        HopitalEntities myBdd;
        public AfficherPatient()
        {
            InitializeComponent();
            myBdd = new HopitalEntities();
            
        }

        public AfficherPatient(Patient pat)
        {
            InitializeComponent();
            myBdd = new HopitalEntities();            
            afficherPatient(myBdd, pat);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboNAM.DataContext = myBdd.Patients.ToList();
            cboNom.DataContext = myBdd.Patients.ToList();
            // EXEMPLE:
            //dgListeMedecins.DataContext = (from med in myBdd.Medecins
            //                               orderby med.Nom
            //                               select med).ToList();
        }

        private void cboNAM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Patient sPatient = cboNAM.SelectedItem as Patient;
            afficherPatient(myBdd, sPatient);          
        }

        private void cboNom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Patient sPatient = cboNom.SelectedItem as Patient;
            afficherPatient(myBdd, sPatient);        
        }

        public void afficherPatient(HopitalEntities bdd, Patient pat)
        {
            // Nettoyage des champs non-obligatoires avant d'affichage d'un autre patient
            txtTelephone.Text = string.Empty;
            txtAdresse.Text = string.Empty;
            txtAssurancePrivee.Text = string.Empty;
            txtParentNom.Text = string.Empty;
            txtParentTelephone.Text = string.Empty;

            // On retrouve le patient dans la BDD et affiches ses attributs
            Patient sPatient = bdd.Patients.SingleOrDefault(x => x.NumAssuranceMaladie.Trim() == pat.NumAssuranceMaladie.Trim());
            if (sPatient != null)
            {
                cboNAM.SelectedItem = sPatient;
                cboNom.SelectedItem = sPatient;
                txtPrenom.Text = sPatient.Prenom;
                txtTelephone.Text = sPatient.Telephone;
                txtAdresse.Text = sPatient.Adresse.Trim() + " " + sPatient.Ville.Trim() + " " + sPatient.Province + " " + sPatient.CodePostal;
                dpDateNaissance.SelectedDate = sPatient.DateNaissance;

                if (sPatient.CompagnieAssurance == null)
                {
                    txtAssurancePrivee.Text = "Aucune";
                }
                else
                {
                    txtAssurancePrivee.Text = sPatient.CompagnieAssurance.NomCompagnie;
                }

                if (sPatient.IdParent != null)
                {
                    txtParentNom.Text = sPatient.Parent.Nom.Trim() + ", " + sPatient.Parent.Prenom.Trim();
                    txtParentTelephone.Text = sPatient.Parent.Telephone;
                }
            }
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAjouterAdmission_Click(object sender, RoutedEventArgs e)
        {
            Patient sPatient = cboNAM.SelectedItem as Patient;
            if (sPatient != null)
            {
                AjouterAdmission fenetreAA = new AjouterAdmission(sPatient);
                fenetreAA.ShowDialog();
            }
            else
            {
                MessageBox.Show("Choisissez un patient pour lui accorder une admission", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
