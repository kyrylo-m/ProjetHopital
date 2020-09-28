using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetHopital
{
    public class AdmissionAffichee
    {
        public int IdAdmission { get; set; }
        public string NAM { get; set; }
        public string PatientNom { get; set; }
        public string PatientPrenom { get; set; }
        public DateTime PatientDateNaissance { get; set; }
        public Nullable<int> PatientAssurancePrivee { get; set; }
        public string PatientCompagnieAssurance { get; set; }
        public DateTime DateAdmission { get; set; }
        public Nullable<DateTime> DateConge { get; set; }
        public bool Chirurgie { get; set; }
        public Nullable<DateTime> DateChirurgie { get; set; }
        public string Medecin { get; set; }
        public int IdLit { get; set; }
        public string Departement { get; set; }
        public string CategorieChambre { get; set; }
        public Nullable<bool> SurclassementGratuit { get; set; }
        public Nullable<bool> LocationTV { get; set; }
        public Nullable<bool> LocationTelephone { get; set; }

    }
}
