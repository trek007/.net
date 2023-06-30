using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace book.models
{
  public  class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postalcode { get; set; }
        public int? CampanyId { get; set; }
        [ForeignKey("CampanyId")]
        public Campany Campany { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
