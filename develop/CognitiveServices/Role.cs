using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices
{
    public class Role
    {
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public string Gender { get; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public Role(string roleName, int ageMin, int ageMax, string gender, string description)
        {
            this.RoleName = roleName;
            this.AgeMin = ageMin;
            this.AgeMax = ageMax;
            this.Gender = gender;
            this.Description = description;
        }
    }
}
