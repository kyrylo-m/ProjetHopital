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
    /// Logique d'interaction pour AfficherAdmission.xaml
    /// </summary>
    public partial class AfficherAdmission : Window
    {
        public AfficherAdmission(Admission admission)
        {
            InitializeComponent();
            afficherAdmission(admission);
        }
               
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           //afficherAdmission(admission);
        }

        public void afficherAdmission(Admission adm)
        {
            txtNAM.Text = adm.NumAssuranceMaladie;
            txtNomPatient.Text = adm.Patient.Nom;
            txtPrenom.Text = adm.Patient.Prenom;
            dpDateNaissance.SelectedDate = adm.Patient.DateNaissance;
            txtAge.Text = calculerAge(adm.Patient).ToString();

            if (adm.Patient.AssurancePrive == null)
            {
                txtAssurancePrivee.Text = "Aucune";
            }
            else
            {
                txtAssurancePrivee.Text = adm.Patient.CompagnieAssurance.NomCompagnie;
            }

            txtIdAdmission.Text = adm.IdAdmission.ToString();
            dpDateDebutAdmission.SelectedDate = adm.DateAdmission;

            if (adm.DateConge != null)
            {
                dpDateConge.SelectedDate = adm.DateConge;
            }

            if (adm.ChirurgieProgramme == true)
            {
                txtChirurgieOuiNon.Content = "Oui";
                dpDateChirurgie.SelectedDate = adm.DateChirurgie;
            }
            else
            {
                txtChirurgieOuiNon.Content = "Non";
            }

            txtMedecin.Text = adm.Medecin.Prenom.Trim() + " " + adm.Medecin.Nom.Trim();
            txtDepartement.Text = adm.Lit.Departement.NomDepartement;
            txtNumeroLit.Text = adm.idLit.ToString();
            txtTypeChambre.Text = adm.Lit.TypeLit.TypeLitDescription;

            if (adm.ChambrePriveGratuit == true)
            {
                txtSurclassementGratuitOuiNon.Content = "Oui";
            }
            else
            {
                txtSurclassementGratuitOuiNon.Content = "Non";
            }

            if (adm.LocationTeleviseur == true)
            {
                txtLocationTvOuiNon.Content = "Oui";
            }
            else
            {
                txtLocationTvOuiNon.Content = "Non";
            }

            if (adm.LocationTelephone == true)
            {
                txtLocationTelephoneOuiNon.Content = "Oui";
            }
            else
            {
                txtLocationTelephoneOuiNon.Content = "Non";
            }

        } // afficherAdmission

        //public void afficherAdmission(AdmissionAffichee adm)
        //{
        //    txtNAM.Text = adm.NAM;
        //    txtNomPatient.Text = adm.PatientNom;
        //    txtPrenom.Text = adm.PatientPrenom;
        //    dpDateNaissance.SelectedDate = adm.PatientDateNaissance;
        //    txtAge.Text = calculerAge(adm.PatientDateNaissance).ToString();

        //    if (adm.PatientAssurancePrivee == null)
        //    {
        //        txtAssurancePrivee.Text = "Aucune";
        //    }
        //    else
        //    {
        //        txtAssurancePrivee.Text = adm.PatientCompagnieAssurance;
        //    }

        //    txtIdAdmission.Text = adm.IdAdmission.ToString();
        //    dpDateDebutAdmission.SelectedDate = adm.DateAdmission;

        //    if (adm.DateConge != null)
        //    {
        //        dpDateConge.SelectedDate = adm.DateConge;
        //    }

        //    if (adm.Chirurgie == true)
        //    {
        //        txtChirurgieOuiNon.Content = "Oui";
        //        dpDateChirurgie.SelectedDate = adm.DateChirurgie;
        //    }
        //    else
        //    {
        //        txtChirurgieOuiNon.Content = "Non";
        //    }

        //    txtMedecin.Text = adm.Medecin;
        //    txtDepartement.Text = adm.Departement;
        //    txtNumeroLit.Text = adm.IdLit.ToString();
        //    txtTypeChambre.Text = adm.CategorieChambre;

        //    if (adm.SurclassementGratuit == true)
        //    {
        //        txtSurclassementGratuitOuiNon.Content = "Oui";
        //    }
        //    else
        //    {
        //        txtSurclassementGratuitOuiNon.Content = "Non";
        //    }

        //    if (adm.LocationTV == true)
        //    {
        //        txtLocationTvOuiNon.Content = "Oui";
        //    }
        //    else
        //    {
        //        txtLocationTvOuiNon.Content = "Non";
        //    }

        //    if (adm.LocationTelephone == true)
        //    {
        //        txtLocationTelephoneOuiNon.Content = "Oui";
        //    }
        //    else
        //    {
        //        txtLocationTelephoneOuiNon.Content = "Non";
        //    }

        //} // afficherAdmission

        public int calculerAge(DateTime dateNaissance)
        {
            var today = DateTime.Today;
            int age = today.Year - dateNaissance.Year;
            if ((dateNaissance.Month > today.Month) || (dateNaissance.Month == today.Month && dateNaissance.Day > today.Day))
            {
                age--;
            }
            return age;
        } // calculerAge

        public int calculerAge(Patient patient)
        {
            var today = DateTime.Today;
            int age = today.Year - patient.DateNaissance.Year;
            if ((patient.DateNaissance.Month > today.Month) || (patient.DateNaissance.Month == today.Month && patient.DateNaissance.Day > today.Day))
            {
                age--;
            }
            return age;
        } // calculerAge

    }
}
