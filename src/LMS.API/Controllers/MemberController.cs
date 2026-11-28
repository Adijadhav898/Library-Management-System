using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Authorize(Policy = "LibrarianOrAdmin")]
public class MemberController : ApiControllerBase
{
    private readonly IMemberService _members;

    public MemberController(IMemberService members)
    {
        _members = members;
    }

    // ---------- Members ----------

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MemberDto>>> GetAll([FromQuery] MemberPaginationRequest request, [FromQuery] string? search, CancellationToken ct)
        => Ok(await _members.GetAllAsync(request, request.IsActive, request.MemberTypeId, search, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetById(Guid id, CancellationToken ct)
    {
        var member = await _members.GetByIdAsync(id, ct);
        return member == null ? NotFound() : Ok(member);
    }

    [HttpGet("by-number/{memberNumber}")]
    public async Task<ActionResult<MemberDto>> GetByNumber(string memberNumber, CancellationToken ct)
    {
        var member = await _members.GetByMemberNumberAsync(memberNumber, ct);
        return member == null ? NotFound() : Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberRequest request, CancellationToken ct)
    {
        var member = await _members.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MemberDto>> Update(Guid id, [FromBody] UpdateMemberRequest request, CancellationToken ct)
    {
        var member = await _members.UpdateAsync(id, request, ct);
        return member == null ? NotFound() : Ok(member);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _members.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/toggle-active")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
        => await _members.ToggleActiveAsync(id, ct) ? Ok() : NotFound();

    // ---------- Member types ----------

    [HttpGet("types")]
    public async Task<ActionResult<PaginatedResult<MemberTypeDto>>> GetTypes([FromQuery] PaginationRequest request, CancellationToken ct)
        => Ok(await _members.GetMemberTypesAsync(request, ct));

    [HttpGet("types/{id:guid}")]
    public async Task<ActionResult<MemberTypeDto>> GetTypeById(Guid id, CancellationToken ct)
    {
        var type = await _members.GetMemberTypeByIdAsync(id, ct);
        return type == null ? NotFound() : Ok(type);
    }

    [HttpPost("types")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MemberTypeDto>> CreateType([FromBody] CreateMemberTypeRequest request, CancellationToken ct)
    {
        var type = await _members.CreateMemberTypeAsync(request, ct);
        return CreatedAtAction(nameof(GetTypeById), new { id = type.Id }, type);
    }

    [HttpPut("types/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MemberTypeDto>> UpdateType(Guid id, [FromBody] UpdateMemberTypeRequest request, CancellationToken ct)
    {
        var type = await _members.UpdateMemberTypeAsync(id, request, ct);
        return type == null ? NotFound() : Ok(type);
    }

    [HttpDelete("types/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteType(Guid id, CancellationToken ct)
        => await _members.DeleteMemberTypeAsync(id, ct) ? NoContent() : NotFound();
}
