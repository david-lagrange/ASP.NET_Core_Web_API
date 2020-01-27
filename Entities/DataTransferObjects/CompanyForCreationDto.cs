using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class CompanyForCreationDto
    {
        [Required(ErrorMessage = "Company name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Company Address is a required field.")]
        [MaxLength(40, ErrorMessage = "Maximum length for the Address is 30 characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Company country is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Country is 20 characters.")]
        public string Country { get; set; }
        public IEnumerable<EmployeeForCreationDto> Employees { get; set; }
    }
}
