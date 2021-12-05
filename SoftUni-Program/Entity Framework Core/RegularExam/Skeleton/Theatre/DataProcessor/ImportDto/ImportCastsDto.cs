using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public  class ImportCastsDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [Required]
        [XmlElement("IsMainCharacter")]
        public string IsMainCharacter { get; set; }

        [Required]
        [XmlElement("PhoneNumber")]
        [RegularExpression(@"^(\+?44)-\d{2}-\d{3}-\d{4}$")]
        public string PhoneNumber { get; set; }

        [Required]
        [XmlElement("PlayId")]
        public int PlayId { get; set; }
    }
}
