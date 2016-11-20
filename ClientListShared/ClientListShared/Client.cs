using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PropertyChanged;

namespace ClientList
{
    [ImplementPropertyChanged]
    public class Client
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public string Name { get { return $"{FirstName} {LastName}"; } }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string HairTexture { get; set; }
        public string HairStrand { get; set; }
        public string HairCondition { get; set; }
        public string Porosity { get; set; }
        public string Elasticity { get; set; }
        public string Formula { get; set; }
        public string Notes { get; set; }
        public bool Hide { get; set; } = false;
        //public List<DateTime> PastVisits { get; set; }
        //public List<DateTime> FutureVisits { get; set; }
    }
}
