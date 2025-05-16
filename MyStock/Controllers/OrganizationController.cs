using MyStock.Entities;
using MyStock.Services;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;

[Route("api/orgs")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationService _service;

    public OrganizationController(OrganizationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
    {
        var orgs = await _service.GetAllAsync();
        return Ok(orgs.Select(Map));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationDto>> GetById(Guid id)
    {
        var org = await _service.GetByIdAsync(id);
        return org == null ? NotFound() : Ok(Map(org));
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<OrganizationDto>>> Filter([FromQuery] OrganizationType type)
    {
        var orgs = await _service.FilterByTypeAsync(type);
        return Ok(orgs.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationDto>> Create([FromBody] CreateOrganizationDto dto)
    {
        var entity = new Organization
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            LegalForm = dto.LegalForm,
            INN = dto.INN,
            KPP = dto.KPP,
            OGRN = dto.OGRN,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Type = dto.Type,
            PrimaryContactId = dto.PrimaryContactId
        };

        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrganizationDto dto)
    {
        var updated = new Organization
        {
            Name = dto.Name,
            LegalForm = dto.LegalForm,
            INN = dto.INN,
            KPP = dto.KPP,
            OGRN = dto.OGRN,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Type = dto.Type,
            PrimaryContactId = dto.PrimaryContactId
        };

        var result = await _service.UpdateAsync(id, updated);
        return result == null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static OrganizationDto Map(Organization o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        LegalForm = o.LegalForm,
        INN = o.INN,
        KPP = o.KPP,
        OGRN = o.OGRN,
        Address = o.Address,
        Phone = o.Phone,
        Email = o.Email,
        Type = o.Type,
        PrimaryContactId = o.PrimaryContactId
    };
}
