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
    /// Logique d'interaction pour ListerAdmissions.xaml
    /// </summary>
    public partial class ListerAdmissions : Window
    {
        HopitalEntities myBdd;
        public ListerAdmissions()
        {
            InitializeComponent();
            myBdd = new HopitalEntities();
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listerAdmissions();
        }

        public void listerAdmissions()
        {
            List<AdmissionAffichee> admissions = (from a in myBdd.Admissions
                             select new AdmissionAffichee()
                             {
                                 IdAdmission = a.IdAdmission,
                                 NAM = a.NumAssuranceMaladie,
                                 PatientNom = a.Patient.Nom,
                                 PatientPrenom = a.Patient.Prenom,
                                 PatientDateNaissance = a.Patient.DateNaissance,
                                 PatientAssurancePrivee = a.Patient.AssurancePrive,
                                 PatientCompagnieAssurance = a.Patient.CompagnieAssurance.NomCompagnie,
                                 DateAdmission = a.DateAdmission,
                                 DateConge = a.DateConge,
                                 Chirurgie = a.ChirurgieProgramme,
                                 DateChirurgie = a.DateChirurgie,
                                 Medecin = a.Medecin.Prenom.Trim() + " " + a.Medecin.Nom.Trim(),
                                 IdLit = a.idLit,
                                 Departement = a.Lit.Departement.NomDepartement,
                                 CategorieChambre = a.Lit.TypeLit.TypeLitDescription,
                                 SurclassementGratuit = a.ChambrePriveGratuit,
                                 LocationTV = a.LocationTeleviseur,
                                 LocationTelephone = a.LocationTelephone
                             }).ToList();

            if (chAdmissionsActuelles.IsChecked == true)
            {
                dgListeAdmissions.DataContext = (from adm in admissions
                                                 where adm.DateConge == null
                                                 select adm).ToList();                                
            }
            else
            {
                dgListeAdmissions.DataContext = admissions.ToList();
            }
        }

        
        private void btnAfficherAdmission_Click(object sender, RoutedEventArgs e)
        {
            AdmissionAffichee adm = dgListeAdmissions.SelectedItem as AdmissionAffichee;

            if (adm != null)
            {
                //AfficherAdmission fenetreAA = new AfficherAdmission(adm);
                //fenetreAA.ShowDialog();

                Admission sAdmission = myBdd.Admissions.SingleOrDefault(a => a.IdAdmission == adm.IdAdmission);
                if (sAdmission != null)
                {
                    AfficherAdmission fenetreAA = new AfficherAdmission(sAdmission);
                    fenetreAA.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Aucune admission n'est choisie", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Aucune admission n'est choisie", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void chAdmissionsActuelles_Click(object sender, RoutedEventArgs e)
        {
            listerAdmissions();
        }
    }
}
