using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class ExportUsersSoldProductsDto
    {
        [XmlAttribute("firstName")]
        public string FirstName { get; set; }

        [XmlAttribute("lastName")]
        public string LastName { get; set; }

        [XmlAttribute("Age")]
        public int? Age { get; set; }

        [XmlArray("soldProducts")]
        public SoldProductsDto[] SoldProducts { get; set; }

    }
}
