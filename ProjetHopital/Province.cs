using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetHopital
{
    public class Province
    {

        public string nom { get; set; }
        public string abbreviation { get; set; }

        public Province(string nom, string abbreviation)
        {
            this.nom = nom;
            this.abbreviation = abbreviation;
        }
        
        public static List<Province> RecupererProvinces()
        {         
            List<Province> listeProvinces = new List<Province>();

            listeProvinces.Add(new Province("Québec", "QC"));
            listeProvinces.Add(new Province("Ontario", "ON"));
            listeProvinces.Add(new Province("Alberta", "AB"));
            listeProvinces.Add(new Province("Nova Scotia", "NS"));
            listeProvinces.Add(new Province("New Brunswick", "NB"));
            listeProvinces.Add(new Province("Manitoba", "MB"));
            listeProvinces.Add(new Province("British Columbia", "BC"));
            listeProvinces.Add(new Province("Prince Edward Island", "PE"));
            listeProvinces.Add(new Province("Saskatchewan", "SK"));
            listeProvinces.Add(new Province("Newfoundland and Labrador", "NL"));
            listeProvinces.Add(new Province("Northwest Territories", "NT"));
            listeProvinces.Add(new Province("Yukon", "YT"));
            listeProvinces.Add(new Province("Nunavut", "NU"));

            return listeProvinces;
        }

    }
}
