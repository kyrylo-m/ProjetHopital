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
    /// Logique d'interaction pour MedecinAccueil.xaml
    /// </summary>
    public partial class MedecinAccueil : Window
    {
        HopitalEntities myBdd;
        public MedecinAccueil()
        {
            InitializeComponent();
            myBdd = new HopitalEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listerAdmissions();
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                dgListeAdmissions.DataContext = (from a in admissions
                                                 where a.DateConge == null
                                                 select a).ToList();
            }
            else
            {
                dgListeAdmissions.DataContext = admissions;
            }                       
            
        } // listerAdmissions()

        private void btnDonnerConge_Click(object sender, RoutedEventArgs e)
        {
            AdmissionAffichee admissionAffichee = dgListeAdmissions.SelectedItem as AdmissionAffichee;
            if (admissionAffichee != null)
            {
                // On retrouve l'admission choisie dans notre table Admissions
                Admission sAdmission = myBdd.Admissions.SingleOrDefault(a => a.IdAdmission == admissionAffichee.IdAdmission);

                if (sAdmission != null)
                {
                    if (sAdmission.DateConge == null)
                    {
                        // On met la date d'aujourd'hui comme la date de congé et on libère le lit
                        sAdmission.DateConge = DateTime.Today;
                        sAdmission.Lit.Occupe = false;

                        // On calcule la facture à payer
                        double totalFacture = 0;
                        double coutQuotidien = 0;

                        if (sAdmission.ChambrePriveGratuit != true && sAdmission.Patient.AssurancePrive == null)
                        {
                            if (sAdmission.Lit.IdTypeLit == 2)
                            {
                                coutQuotidien += 267;
                            }
                            else if (sAdmission.Lit.IdTypeLit == 3)
                            {
                                coutQuotidien += 571;
                            }
                        }

                        if (sAdmission.LocationTelephone == true)
                        {
                            coutQuotidien += 7.50;
                        }

                        if (sAdmission.LocationTeleviseur == true)
                        {
                            coutQuotidien += 42.50;
                        }

                        // Calcul des jours passés à l'hôpital et du total de la facture
                        if (coutQuotidien > 0)
                        {
                            int Nbjours = (sAdmission.DateConge.Value.Date - sAdmission.DateAdmission.Date).Days;
                            totalFacture = coutQuotidien * Nbjours;
                        }

                        // Sauvegarde des changements et affichage de la facture
                        try
                        {
                            myBdd.SaveChanges();
                            MessageBox.Show("Le patient " + sAdmission.Patient.Nom.Trim() + " " + sAdmission.Patient.Prenom.Trim() +
                                " a reçu son congé et le total de sa facture est: " + totalFacture + "$",
                            "Congé", MessageBoxButton.OK, MessageBoxImage.Information);

                            listerAdmissions();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur d'enregistrement", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("L'admission choisie était déjà fermée", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Aucune admission n'est sélectionnée!", "Alerte",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Aucune admission n'est sélectionnée!", "Alerte",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        } // btnDonnerConge_Click

        private void chAdmissionsActuelles_Click(object sender, RoutedEventArgs e)
        {
            listerAdmissions();
        }
    }
}
