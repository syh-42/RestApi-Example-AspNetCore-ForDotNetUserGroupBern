using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace RestApi.Model
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Vorname { get; set; }

        [MaxLength(250)]
        public string Adresse { get; set; }

        public int Plz { get; set; }

        [MaxLength(250)]
        public string Ort { get; set; }
    }
}