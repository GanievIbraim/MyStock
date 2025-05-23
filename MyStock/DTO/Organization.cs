﻿using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateOrganizationDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public OrganizationType Type { get; set; }

        public string? LegalForm { get; set; }
        public string? INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public Guid? PrimaryContactId { get; set; }
    }
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? LegalForm { get; set; }
        public string? INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public CodeDisplayDto Type { get; set; } = default!;
        public ReferenceDto? PrimaryContact { get; set; }
    }
}
