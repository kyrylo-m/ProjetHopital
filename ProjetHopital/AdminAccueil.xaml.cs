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
    /// Logique d'interaction pour AdminAccueil.xaml
    /// </summary>
    public partial class AdminAccueil : Window
    {
        HopitalEntities myBdd;
        public AdminAccueil()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBdd = new HopitalEntities();
            afficherMedecins();            
        }

        private void chkEmbauches_Click(object sender, RoutedEventArgs e)
        {
            afficherMedecins();            
        }

        private void btnEnregistrerMedecin_Click(object sender, RoutedEventArgs e)
        {
            bool ok = true;
            Medecin sMedecin = (Medecin)dgListeMedecins.SelectedItem;

            if (sMedecin != null && sMedecin.Prenom != null && sMedecin.Nom != null && sMedecin.Specialite != null)
            {
                // Verification de la validite des informations saisies
                ok = string.IsNullOrEmpty(sMedecin.Prenom.Trim()) ? false : true;
                ok = string.IsNullOrEmpty(sMedecin.Nom.Trim()) || !ok ? false : true;
                ok = string.IsNullOrEmpty(sMedecin.Specialite.Trim()) || !ok ? false : true;
                //ok = (sMedecin.EmbaucheParHopital != true && sMedecin.EmbaucheParHopital != false) || !ok ? false : true;
                ok = sMedecin.EmbaucheParHopital == null || !ok ? false : true;

                if (ok)
                {
                    // Se l'ID est egal a null, il s'agit d'un nouveau medecin
                    if (sMedecin.IdMedecin == null)
                    {
                        // Vérification si le médecin saisi existe déjà dans la liste
                        Medecin med = myBdd.Medecins.SingleOrDefault(x => x.Nom.Trim() == sMedecin.Nom.Trim()
                                                                     && x.Prenom.Trim() == sMedecin.Prenom.Trim()
                                                                     && x.Specialite.Trim() == sMedecin.Specialite.Trim());
                        if (med == null) // Le medecin n'existe pas
                        {
                            // Composition et verification de la nouvelle ID de medecin
                            int compteur = 1;
                            string nouvelleId = "MED" + sMedecin.Prenom.Substring(0, 1).ToUpper() + sMedecin.Nom.Substring(0, 1).ToUpper() + "0" + compteur.ToString();

                            med = myBdd.Medecins.SingleOrDefault(x => x.IdMedecin.Trim() == nouvelleId);
                            while (med != null) // Id appartient à un autre médecin
                            {
                                if (++compteur < 10)
                                {
                                    nouvelleId = "MED" + sMedecin.Prenom.Substring(0, 1).ToUpper() + sMedecin.Nom.Substring(0, 1).ToUpper() + "0" + compteur.ToString();
                                }
                                else
                                {
                                    nouvelleId = "MED" + sMedecin.Prenom.Substring(0, 1).ToUpper() + sMedecin.Nom.Substring(0, 1).ToUpper() + compteur.ToString();
                                }

                                med = myBdd.Medecins.SingleOrDefault(x => x.IdMedecin.Trim() == nouvelleId);
                            }

                            // Creation de nouveau objet de type Medecin
                            Medecin nouveauMedecin = new Medecin
                            {
                                IdMedecin = nouvelleId,
                                Nom = sMedecin.Nom,
                                Prenom = sMedecin.Prenom,
                                Specialite = sMedecin.Specialite,
                                EmbaucheParHopital = sMedecin.EmbaucheParHopital
                            };

                            // Ajout du nouvel médecin à la collection Medecins
                            myBdd.Medecins.Add(nouveauMedecin);

                            // Saving changes to the DB
                            try
                            {
                                myBdd.SaveChanges();
                                MessageBox.Show("Le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() +
                                                " ajouté avec succès", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Erreur d'enregistrement",
                                                MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        }
                        else // Le medecin deja existe
                        {
                            MessageBox.Show("Le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() + ", " +
                                sMedecin.Specialite.Trim() + ", existe déjà dans la liste: saisie impossible", "Alerte",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    } // if (sMedecin.IdMedecin == null) - nouveau médecin

                    // Sinon, il s'agit d'un medecin existant
                    else
                    {
                        Medecin med = myBdd.Medecins.SingleOrDefault(x => x.IdMedecin.Trim() == sMedecin.IdMedecin.Trim());
                        med.Nom = sMedecin.Nom.Trim();
                        med.Prenom = sMedecin.Prenom.Trim();
                        med.Specialite = sMedecin.Specialite.Trim();
                        med.EmbaucheParHopital = sMedecin.EmbaucheParHopital;

                        // Saving changes to the DB
                        try
                        {
                            myBdd.SaveChanges();
                            MessageBox.Show("Le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() +
                                            " modifié avec succès", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur d'enregistrement",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    afficherMedecins();

                } // if (ok)
                else
                {
                    MessageBox.Show("Vous devez remplir tous les champs sauf ID", "Alerte",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Aucun changement n'a été fait. \nVous devez remplir tous les champs sauf ID", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        } // btnEnregistrerMedecin_Click

        public void afficherMedecins()
        {
            if (chkEmbauches.IsChecked == true)
            {
                dgListeMedecins.DataContext = (from med in myBdd.Medecins
                                               where med.EmbaucheParHopital == true
                                               orderby med.Nom
                                               select med).ToList();
            }
            else
            {
                dgListeMedecins.DataContext = (from med in myBdd.Medecins                                                   
                                               orderby med.Nom
                                               select med).ToList();
            }

        } // afficherMedecins

        private void btnCongedierMedecin_Click(object sender, RoutedEventArgs e)
        {
            Medecin sMedecin = (Medecin)dgListeMedecins.SelectedItem;
            if (sMedecin != null && sMedecin.IdMedecin != null)
            {
                Medecin med = myBdd.Medecins.SingleOrDefault(x => x.IdMedecin.Trim() == sMedecin.IdMedecin.Trim());
                med.EmbaucheParHopital = false;

                // Saving changes to the DB
                try
                {
                    myBdd.SaveChanges();
                    MessageBox.Show("Le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() + " congédié avec succès", "Confirmation",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur d'enregistrement",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }

                afficherMedecins();
            }
            else
            {
                MessageBox.Show("Aucun médecin n'est choisi", "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        } // btnCongedierMedecin_Click

        private void btnSupprimerMedecin_Click(object sender, RoutedEventArgs e)
        {
            Medecin sMedecin = dgListeMedecins.SelectedItem as Medecin;

            if (sMedecin != null && sMedecin.IdMedecin != null)
            {
                // Verification si le medecin choisi pour suppression est lie a un dossier d'admission
                Admission dossierAdmission = myBdd.Admissions.FirstOrDefault(x => x.idMedecin == sMedecin.IdMedecin);
                
                if (dossierAdmission != null)
                {
                    MessageBox.Show("Suppression est impossible: le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() + "est lié à un dossier d'admission",
                                    "Alerte", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var resultat = MessageBox.Show("Êtes-vous certain de vouloir supprimer le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() + "?",
                                                    "Attention", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        
                        Medecin med = myBdd.Medecins.SingleOrDefault(x => x.IdMedecin == sMedecin.IdMedecin);
                        myBdd.Medecins.Remove(med);

                        try
                        {
                            myBdd.SaveChanges();
                            MessageBox.Show("Le médecin " + sMedecin.Prenom.Trim() + " " + sMedecin.Nom.Trim() + " supprimé avec succès", 
                                            "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Erreur d'enregistrement",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        afficherMedecins();

                    } // if MessageBoxResult.Yes                
                } // else i.e. dossierAdmission == null
            } // if (sMedecin != null)
            else
            {
                MessageBox.Show("Choisissez un médecin à supprimer", "Rappel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        } // btnSupprimerMedecin_Click

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    } // class AdminAccueil

}
