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
    /// Logique d'interaction pour AjouterAdmission.xaml
    /// </summary>
    public partial class AjouterAdmission : Window
    {
        HopitalEntities myBdd;
        public AjouterAdmission()
        {
            InitializeComponent();
            myBdd = new HopitalEntities();
        }

        public AjouterAdmission(Patient patient)
        {
            InitializeComponent();
            myBdd = new HopitalEntities();
            afficherPatient(myBdd, patient);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboNAM.DataContext = myBdd.Patients.ToList();
            cboNom.DataContext = myBdd.Patients.ToList();
            dpDateDebutAdmission.SelectedDate = DateTime.Today;
            cboDepartement.DataContext = myBdd.Departements.ToList();
            txtLitsTotalHopital.Text = calculerLitsTotalDispHopital(myBdd).ToString();
        }

        public void afficherPatient(HopitalEntities bdd, Patient pat)
        {
            // On retrouve le patient dans la BDD et affiches ses attributs
            Patient sPatient = bdd.Patients.SingleOrDefault(x => x.NumAssuranceMaladie.Trim() == pat.NumAssuranceMaladie.Trim());
            if (sPatient != null)
            {
                cboNAM.SelectedItem = sPatient;
                cboNom.SelectedItem = sPatient;
                txtPrenom.Text = sPatient.Prenom;
                dpDateNaissance.SelectedDate = sPatient.DateNaissance;
                if (sPatient.CompagnieAssurance == null)
                {
                    txtAssurancePrivee.Text = "Aucune";
                }
                else
                {
                    txtAssurancePrivee.Text = sPatient.CompagnieAssurance.NomCompagnie;
                }

                // On verifie l'age du patient et change la valeur de cboDepartement a "Pediatrie" is necessaire
                int age = calculerAge(sPatient);
                txtAge.Text = age.ToString();

                if (age <= 16)
                {
                    cboDepartement.SelectedIndex = 1; // Index 1 de combobox - Pédiatrie
                }
                else 
                {
                    cboDepartement.SelectedIndex = 4; // Index 4 - dpt Général (choix par défault)
                }
            }
        } // afficherPatient

        public int calculerAge(Patient patient)
        {
            var today = DateTime.Today;
            int age = today.Year - patient.DateNaissance.Year;
            if ((patient.DateNaissance.Month > today.Month) || (patient.DateNaissance.Month == today.Month && patient.DateNaissance.Day > today.Day))
            {
                age--;
            }
            return age;
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

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chChirurgiePrevue_Click(object sender, RoutedEventArgs e)
        {
            if (chChirurgiePrevue.IsChecked == true)
            {
                cboDepartement.SelectedIndex = 0; // Chirurgie
            }
            else
            {
                if (cboNAM.SelectedItem != null && int.Parse(txtAge.Text) <= 16)
                    cboDepartement.SelectedIndex = 1; // Pédiatrie
                else
                    cboDepartement.SelectedIndex = 4; //Général dpt
            }
        }

        private void cboDepartement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recupererMedecins();

            // On calcule et affiche le nombre des lits disponible dans le departement choisi
            Departement sDept = cboDepartement.SelectedItem as Departement;
            if (sDept != null)
            {
                txtLitsStandardDisp.Text = calculerLitsDispDeptParCat(myBdd, sDept.IdDepartement, 1).ToString();
                txtLitsSemiPriveDisp.Text = calculerLitsDispDeptParCat(myBdd, sDept.IdDepartement, 2).ToString();
                txtLitsPriveDisp.Text = calculerLitsDispDeptParCat(myBdd, sDept.IdDepartement, 3).ToString();
            }
            else
            {
                txtLitsStandardDisp.Text = string.Empty;
                txtLitsSemiPriveDisp.Text = string.Empty;
                txtLitsPriveDisp.Text = string.Empty;
            }
        } // cboDepartement_SelectionChanged

        public int calculerLitsDispDeptParCat(HopitalEntities hopital, int idDept, int cat)
        {
            int nbreLits = (from lit in hopital.Lits
                            where lit.IdDepartement == idDept
                            && lit.IdTypeLit == cat
                            && lit.Occupe == false
                            select lit).Count();

            return nbreLits;
        }

        public int calculerLitsTotalDispHopital(HopitalEntities hopital)
        {
            int nbreLits = (from lit in hopital.Lits
                            where lit.Occupe == false
                            select lit).Count();

            return nbreLits;
        }

        private void btnEnregistrerAdmission_Click(object sender, RoutedEventArgs e)
        {
            // On vérifie s'il y a des lits disponibles dans l'hôpital
            int nbreLits = calculerLitsTotalDispHopital(myBdd);

            if (nbreLits > 0)
            {
                if (cboNAM.SelectedItem != null)
                {
                    Patient sPatient = (Patient)cboNAM.SelectedItem;
                    string idPatient = sPatient.NumAssuranceMaladie;

                    // Vérification si le patient choisi n'est pas hospitalisé présentement
                    var resultat = (from adm in myBdd.Admissions
                                    where adm.NumAssuranceMaladie.Trim() == idPatient.Trim()
                                    && adm.DateConge == null
                                    select adm).FirstOrDefault();

                    if (resultat == null) // Il n'y a pas d'admission en cours pour le patient
                    {
                        // On declare les variables dont nous aurons besoin pour creer une nouvelle admission
                        DateTime dateAdmission = (DateTime)dpDateDebutAdmission.SelectedDate;
                        Nullable<DateTime> dateChirurgie = null;
                        int age = calculerAge(sPatient);
                        bool chirurgieProgrammee = false;
                        int categorieLitChoisie, idLitAssigne;
                        bool chambrePriveGratuit = false;
                        bool locationTV = false;
                        bool locationTelephone = false;

                        // On choisit le departement
                        int idDept = 5; // Par défault: 5 - Général dpt
                        if (chChirurgiePrevue.IsChecked == true)
                        {
                            chirurgieProgrammee = true;
                            idDept = 1; // 1 - ID de dept Chirurgie;
                            if (dpDateChirurgie.SelectedDate != null)
                                dateChirurgie = (DateTime)dpDateChirurgie.SelectedDate;
                        }
                        else if (age <= 16)
                        {
                            idDept = 2; // 2 - ID de dept Pédiatrie
                        }
                        else
                        {
                            Departement dept = (Departement)cboDepartement.SelectedItem;
                            if (dept != null)
                                idDept = dept.IdDepartement;
                        }

                        // On assigne un medecin
                        string idMedecin = "";
                        Medecin medecin = (Medecin)cboMedecin.SelectedItem;
                        if (medecin != null)
                            idMedecin = medecin.IdMedecin;

                        //On choisit un lit
                        if (rbLitStandard.IsChecked == true)
                            categorieLitChoisie = 1;
                        else if (rbLitSemiPrive.IsChecked == true)
                            categorieLitChoisie = 2;
                        else
                            categorieLitChoisie = 3;

                        // On cherche un lit de la catégorie choisie dans le departement assigné
                        Lit sLit = trouverLit(myBdd, categorieLitChoisie, idDept);

                        // On vérifie le résultat de recherche
                        if (sLit != null)
                        {
                            // Un lit est retrouvé, on vérifie si c'est de la même catégorie que désiré
                            if (sLit.IdTypeLit != categorieLitChoisie && categorieLitChoisie == 1)
                            {
                                MessageBox.Show("La catégorie du lit assigné est différent de celle choisie. Surclassement gratuit",
                                                "Avertissement", MessageBoxButton.OK, MessageBoxImage.Information);
                                chambrePriveGratuit = true;
                            }
                            sLit.Occupe = true;
                            idLitAssigne = sLit.IdLit;

                            if (chLocationTV.IsChecked == true)
                            {
                                locationTV = true;
                            }

                            if (chLocationTelephone.IsChecked == true)
                            {
                                locationTelephone = true;
                            }

                            // Verification si tous les champs obligatoires sont remplis
                            bool ok = true;
                            ok = dateAdmission == null ? false : true;
                            ok = (chirurgieProgrammee == true && dateChirurgie == null) || !ok ? false : true;
                            ok = sLit == null || medecin == null || !ok ? false : true;

                            if (ok)
                            {
                                // On crée une nouvelle instance de la classe Admission
                                Admission admission = new Admission()
                                {
                                    ChirurgieProgramme = chirurgieProgrammee,
                                    DateAdmission = dateAdmission,
                                    DateChirurgie = dateChirurgie,
                                    DateConge = null,
                                    LocationTeleviseur = locationTV,
                                    LocationTelephone = locationTelephone,
                                    NumAssuranceMaladie = idPatient,
                                    idLit = idLitAssigne,
                                    idMedecin = idMedecin,
                                    ChambrePriveGratuit = chambrePriveGratuit
                                };

                                // On ajoute la nouvelle admission à notre collection d'admissions
                                myBdd.Admissions.Add(admission);

                                // On sauvegarde les changement dans la BDD
                                try
                                {
                                    myBdd.SaveChanges();

                                    // On cherche la nouvelle admission dans la BD pour afficher son numéro
                                    Admission admAjoutee = (from a in myBdd.Admissions
                                                            where a.NumAssuranceMaladie == idPatient
                                                            select a).FirstOrDefault();
                                    if (admAjoutee != null)
                                    {
                                        MessageBox.Show("L'admission numéro " + admAjoutee.IdAdmission + " ajoutée avec succès", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                                        
                                        // Affichage de la nouvelle admission
                                        AfficherAdmission fenetreAffAdm = new AfficherAdmission(admAjoutee);
                                        fenetreAffAdm.ShowDialog();
                                        this.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Erreur d'enregistrement d'admission",
                                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }
                            else // Pas tous les chemps sont remplis
                            {
                                MessageBox.Show("Remplissez tous le champs obligatoires", "Avertissement",
                                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else // Il n'y a aucun lit dans le departement choisi
                        {    // Donc on suggère de choisir un autre departement

                            MessageBox.Show("Il n'y a aucun lit disponible dans le departement choisi. " +
                                            "Vous pouvez choisir un autre departement pour y placer le patient",
                                            "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else // Le patient est presentement hospitalisé
                    {
                        MessageBox.Show("Ajout d'admission impossible: le patient choisi est présentement hospitalisé", 
                                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // le patient n'est pas choisi
                {
                    MessageBox.Show("Choisissez un patient pour ajouter une admission", "Alerte",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else // Il n'y pas de lit disponible dans tout l'hopital
            {
                MessageBox.Show("Ajout d'admission impossible: pas de lits disponibles", "Alerte",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }           

        } // bthEnregistrerAdmission_Click

        public Lit trouverLit(HopitalEntities hopital, int categorieDesiree, int idDept)
        {
            int cat = categorieDesiree;
            Lit sLit;         
                       
            do
            {
                sLit = hopital.Lits.Where(lit => lit.IdTypeLit == cat
                                            && lit.IdDepartement == idDept
                                            && lit.Occupe == false).FirstOrDefault();
                if (sLit == null)
                {
                    cat++;
                }
            } while (sLit == null && cat <= 3);

            return sLit;
        }

      

        public void recupererMedecins()
        {
            // On remplit cboMedecin avec les noms des medecins dont la specialite correspond au dept choisi
            Departement sDept = cboDepartement.SelectedItem as Departement;
            
            if (chMedecinSpecailise.IsChecked == true && sDept != null)
            {
                string specialiteMedecin = sDept.NomDepartement.Trim();
                List<Medecin> medecinsSpecialistes = (from med in myBdd.Medecins
                                                      where med.Specialite.Trim() == specialiteMedecin
                                                      && med.EmbaucheParHopital == true
                                                      orderby med.Nom
                                                      select med).ToList();

                foreach (Medecin med in medecinsSpecialistes)
                {
                    med.Prenom = med.Prenom.Trim();
                    med.Nom = med.Nom.Trim();
                }

                cboMedecin.DataContext = medecinsSpecialistes;
            }
            else
            {
                List<Medecin> medecinsTous = (from med in myBdd.Medecins
                                              where med.EmbaucheParHopital == true
                                              orderby med.Nom
                                              select med).ToList();

                foreach (Medecin med in medecinsTous)
                {
                    med.Prenom = med.Prenom.Trim();
                    med.Nom = med.Nom.Trim();
                }

                cboMedecin.DataContext = medecinsTous;
            }
        }

        private void chMedecinSpecailise_Click(object sender, RoutedEventArgs e)
        {
            recupererMedecins();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            //Nettoyage des champs
            cboDepartement.SelectedIndex = 4;
            cboMedecin.SelectedIndex = -1;
            chMedecinSpecailise.IsChecked = false;
            rbLitStandard.IsChecked = true;
            chChirurgiePrevue.IsChecked = false;
            dpDateChirurgie.SelectedDate = null;
            chLocationTelephone.IsChecked = false;
            chLocationTV.IsChecked = false;
        }
    } // class
}
