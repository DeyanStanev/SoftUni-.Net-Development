using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Users")]
    public class mainDto
    {
        [XmlAttribute("Count")]
        public int Count { get; set; }

        [XmlElement("users")]
        public List<ExportUsersSoldProductsDto> ExportUsers { get; set; }
    }
}
