using LMS.Application.DTOs.Member;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "LibrarianOrAdmin")]
[Produces("application/json")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MemberController> _logger;

    public MemberController(IMemberService memberService, ILogger<MemberController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    /// <summary>
    /// Get all members with pagination and search
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<MemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<MemberDto>>> GetMembers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? memberTypeId = null)
    {
        var result = await _memberService.GetMembersAsync(page, pageSize, search, isActive, memberTypeId);
        return Ok(result);
    }

    /// <summary>
    /// Get member by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMember(Guid id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound(new { message = "Member not found" });
        }
        return Ok(member);
    }

    /// <summary>
    /// Get member by user ID
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberByUserId(Guid userId)
    {
        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null)
        {
            return NotFound(new { message = "Member not found" });
        }
        return Ok(member);
    }

    /// <summary>
    /// Create a new member
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberRequestDto request)
    {
        try
        {
            var member = await _memberService.CreateMemberAsync(request);
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update member
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> UpdateMember(Guid id, [FromBody] UpdateMemberRequestDto request)
    {
        var member = await _memberService.UpdateMemberAsync(id, request);
        if (member == null)
        {
            return NotFound(new { message = "Member not found" });
        }
        return Ok(member);
    }

    /// <summary>
    /// Delete member (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        var result = await _memberService.DeleteMemberAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Member not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Renew member membership
    /// </summary>
    [HttpPost("{id:guid}/renew")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> RenewMembership(Guid id, [FromBody] RenewMembershipRequestDto request)
    {
        var member = await _memberService.RenewMembershipAsync(id, request);
        if (member == null)
        {
            return NotFound(new { message = "Member not found" });
        }
        return Ok(member);
    }

    /// <summary>
    /// Get expiring members
    /// </summary>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(IEnumerable<MemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetExpiringMembers([FromQuery] int days = 30)
    {
        var members = await _memberService.GetExpiringMembersAsync(days);
        return Ok(members);
    }

    /// <summary>
    /// Get member types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<MemberTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberTypeDto>>> GetMemberTypes()
    {
        var types = await _memberService.GetMemberTypesAsync();
        return Ok(types);
    }

    /// <summary>
    /// Create member type
    /// </summary>
    [HttpPost("types")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(MemberTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MemberTypeDto>> CreateMemberType([FromBody] CreateMemberTypeRequestDto request)
    {
        var type = await _memberService.CreateMemberTypeAsync(request);
        return CreatedAtAction(nameof(GetMemberTypes), new { }, type);
    }

    /// <summary>
    /// Update member type
    /// </summary>
    [HttpPut("types/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(MemberTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberTypeDto>> UpdateMemberType(Guid id, [FromBody] UpdateMemberTypeRequestDto request)
    {
        var type = await _memberService.UpdateMemberTypeAsync(id, request);
        if (type == null)
        {
            return NotFound(new { message = "Member type not found" });
        }
        return Ok(type);
    }

    /// <summary>
    /// Delete member type
    /// </summary>
    [HttpDelete("types/{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMemberType(Guid id)
    {
        var result = await _memberService.DeleteMemberTypeAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Member type not found" });
        }
        return NoContent();
    }
}