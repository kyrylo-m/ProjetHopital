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
using System.Text.RegularExpressions;


namespace ProjetHopital
{
    /// <summary>
    /// Logique d'interaction pour AjouterPatient.xaml
    /// </summary>
    public partial class AjouterPatient : Window
    {
        HopitalEntities myBdd;
        List<Province> provinces;
        public AjouterPatient()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBdd = new HopitalEntities();
            // Binding de comboboxes
            provinces = Province.RecupererProvinces();
            cboProvince.ItemsSource = (from province in provinces
                                       orderby province.abbreviation
                                       select province).ToList(); 

            cboParentProvince.ItemsSource = (from province in provinces
                                             orderby province.abbreviation
                                             select province).ToList();

            cboAssurancePrivee.DataContext = myBdd.CompagnieAssurances.ToList();
        }

        private void btnEnregistrerPatient_Click(object sender, RoutedEventArgs e)
        {
            // Verification si tous les champs obligatoires de Parent sont remplis
            bool continuerAjout = false; // Pour permetre l'ajout de Patient sans Parent
            
            bool ok = true;
            ok = string.IsNullOrEmpty(txtParentNom.Text.Trim()) ? false : true;
            ok = string.IsNullOrEmpty(txtParentPrenom.Text.Trim()) || !ok ? false : true;
            //ok = string.IsNullOrEmpty(txtParentTelephone.Text.Trim()) || !ok ? false : true;

            Parent nouveauParent = null;

            if (ok)
            {
                // Si le numéro de téléphone du parent est fourni, on valide le format (000)000-0000
                Regex regexNumTel = new Regex(@"^[(]{1}[0-9]{3}[)]{1}[0-9]{3}[-]{1}[0-9]{4}$");
                if (string.IsNullOrEmpty(txtParentTelephone.Text.Trim()) || regexNumTel.IsMatch(txtParentTelephone.Text))
                {
                    // Si le code postal du parent est fourni, on valide le format A0A 0A0
                    Regex regexCodePostal = new Regex(@"^[a-zA-Z]{1}[0-9]{1}[a-zA-Z]{1}[ -]?[0-9]{1}[a-zA-Z]{1}[0-9]{1}$");
                    if (string.IsNullOrEmpty(txtParentCodePostal.Text.Trim()) || regexCodePostal.IsMatch(txtParentCodePostal.Text))
                    {
                        // Creation de nouveau Parent pour le nouveau Patient
                        nouveauParent = new Parent
                        {
                            Nom = txtParentNom.Text,
                            Prenom = txtParentPrenom.Text,
                            Telephone = txtParentTelephone.Text,
                            Adresse = txtParentAdresse.Text,
                            Ville = txtParentVille.Text,
                            Province = cboParentProvince.Text,
                            CodePostal = txtParentCodePostal.Text.ToUpper()
                        };

                        // Creation et verification de ID du nouveau Parent
                        int compteur = 1;
                        string nouvelleId = nouveauParent.Nom.Substring(0, 3).ToUpper() + nouveauParent.Prenom.Substring(0, 1).ToUpper() + "0" + compteur.ToString();

                        Parent parent = myBdd.Parents.SingleOrDefault(x => x.IdParent.Trim() == nouvelleId);
                        while (parent != null) // Id appartient à un autre parent
                        {
                            if (++compteur < 10)
                            {
                                nouvelleId = nouveauParent.Nom.Substring(0, 3).ToUpper() + nouveauParent.Prenom.Substring(0, 1).ToUpper() + "0" + compteur.ToString();
                            }
                            else
                            {
                                nouvelleId = nouveauParent.Nom.Substring(0, 3).ToUpper() + nouveauParent.Prenom.Substring(0, 1).ToUpper() + compteur.ToString();
                            }

                            parent = myBdd.Parents.SingleOrDefault(x => x.IdParent.Trim() == nouvelleId);
                        }

                        nouveauParent.IdParent = nouvelleId;

                        //Ajout du nouvelle parent à la collection
                        myBdd.Parents.Add(nouveauParent);

                        // Saving nouveau parent dans la BDD;
                        try
                        {
                            myBdd.SaveChanges();
                            MessageBox.Show("Le parent " + nouveauParent.Prenom.Trim() + " " + nouveauParent.Nom.Trim() +
                                            " ajouté avec succès", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

                            continuerAjout = true; // Pour qu'on puisse proceder a l'enregistrement du Patient

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur d'enregistrement de parent",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Le code postal doit être saisi en format A0A 0A0", "Erreur de format - Parent", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de téléphone doit être saisi en format (000)000-0000", "Erreur de format - Parent", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
                
            }
            else
            {
                var resultat = MessageBox.Show("Remplissez Nom, Prenom et Telephone du Parent. Sinon continuez sans ajout de Parent", "Alerte", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (resultat == MessageBoxResult.No) // On veut continuer sans creer un Parent
                {
                    continuerAjout = true;
                }                
            }

            if (continuerAjout)
            {
                // Verification si tous les champs obligatoires de Patient sont remplis
                ok = true;
                ok = string.IsNullOrEmpty(txtNom.Text.Trim()) ? false : true;
                ok = string.IsNullOrEmpty(txtPrenom.Text.Trim()) || !ok ? false : true;
                ok = string.IsNullOrEmpty(txtNAM.Text.Trim()) || !ok ? false : true;
                ok = dpDateNaissance.SelectedDate == null || !ok ? false : true;

                // Verification si un Parent a été ajouté
                string idNouveauParent = nouveauParent != null ? nouveauParent.IdParent : null;

                // Recupération de l'ID de compagnie d'assurance à partir de combobox
                Nullable<int> idCompAss = null;
                CompagnieAssurance compAss = (CompagnieAssurance)cboAssurancePrivee.SelectedItem;
                if (compAss != null)
                {
                    idCompAss = compAss.IdCompagnie;
                }

                if (ok)
                {
                    // Si le numéro de téléphone du patient est fourni, on valide le format (000)000-0000
                    Regex regexNumTel = new Regex(@"^[(]{1}[0-9]{3}[)]{1}[0-9]{3}[-]{1}[0-9]{4}$");
                    if (string.IsNullOrEmpty(txtTelephone.Text.Trim()) || regexNumTel.IsMatch(txtTelephone.Text))
                    {
                        // Si le code postal du patient est fourni, on valide le format A0A 0A0
                        Regex regexCodePostal = new Regex(@"^[a-zA-Z]{1}[0-9]{1}[a-zA-Z]{1}[ -]?[0-9]{1}[a-zA-Z]{1}[0-9]{1}$");
                        if (string.IsNullOrEmpty(txtCodePostal.Text.Trim()) || regexCodePostal.IsMatch(txtCodePostal.Text))
                        {
                            // Creation de nouveau Patient
                            Patient nouveauPatient = new Patient()
                            {
                                NumAssuranceMaladie = txtNAM.Text.Trim().ToUpper(),
                                Nom = txtNom.Text,
                                Prenom = txtPrenom.Text,
                                Telephone = txtTelephone.Text,
                                Adresse = txtAdresse.Text,
                                Ville = txtVille.Text,
                                Province = cboProvince.Text,
                                CodePostal = txtCodePostal.Text.ToUpper(),
                                DateNaissance = (DateTime)dpDateNaissance.SelectedDate,
                                AssurancePrive = idCompAss,
                                IdParent = idNouveauParent
                            };

                            // Verification pour le doublon de patient
                            Patient patient = myBdd.Patients.SingleOrDefault(x => x.NumAssuranceMaladie.Trim() == nouveauPatient.NumAssuranceMaladie.Trim());
                            if (patient == null) // Le patient ne se trouve pas encore dans BDD
                            {
                                myBdd.Patients.Add(nouveauPatient);

                                // Saving nouveau patient dans la BDD;
                                try
                                {
                                    myBdd.SaveChanges();
                                    MessageBox.Show("Le patient " + nouveauPatient.Prenom.Trim() + " " + nouveauPatient.Nom.Trim() +
                                                    " ajouté avec succès", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

                                    PreposeAccueil fenetrePA = new PreposeAccueil();
                                    fenetrePA.ShowDialog();
                                    this.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Erreur d'enregistrement de patient",
                                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }
                            else // Le patient se trouve deja dans la BDD
                            {
                                MessageBox.Show("Ajout impossible: le patient était déjà ajouté", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Le code postal doit être saisi en format A0A 0A0", "Erreur de format - Patient", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }  
                        
                    }
                    else
                    {
                        MessageBox.Show("Le numéro de téléphone doit être saisi en format (000)000-0000", "Erreur de format - Patient", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Remplissez Numéro AM, Nom, Prenom, Date Naissance du Patient", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

            } // if (continuerAjout)           

        } // btnEnregistrerPatient_Click

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Nettoyage des champs
            foreach (Control ctl in gridAjouterPatient.Children)
            {
                if (ctl.GetType() == typeof(CheckBox))
                    ((CheckBox)ctl).IsChecked = false;
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Clear();
            }
        }

        private void chMemeAdresse_Click(object sender, RoutedEventArgs e)
        {
            if (chMemeAdresse.IsChecked == true)
            {
                txtParentAdresse.Text = txtAdresse.Text;
                txtParentVille.Text = txtVille.Text;
                txtParentCodePostal.Text = txtCodePostal.Text;
                cboParentProvince.SelectedItem = cboProvince.SelectedItem;
            }
            else
            {
                txtParentAdresse.Clear();
                txtParentVille.Clear();
                txtParentCodePostal.Clear();
                cboParentProvince.SelectedItem = null;
            }
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            PreposeAccueil fenetrePA = new PreposeAccueil();
            fenetrePA.ShowDialog();
            this.Close();
        }
    }
}
