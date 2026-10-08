using InvoiceApi.DTOs;
using InvoiceApi.Models;
using InvoiceApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace InvoiceApi.Controllers;

[ApiController]
[Authorize]

[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
   

    public InvoicesController(IInvoiceService service)
    {
        _service = service;
    }
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetAll(
        [FromQuery] int? userId,
        [FromQuery] InvoiceStatus? status)
    {
        return Ok(await _service.GetAllAsync(userId, status));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetById(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetComments(int id)
    {
        return Ok(await _service.GetCommentsAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> Update(int id, UpdateInvoiceDto dto)
    {
        return Ok(await _service.UpdateAsync(id, CurrentUserId, dto));
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Approver,Admin")]
    public async Task<ActionResult<InvoiceDto>> Approve(int id)
    {
        return Ok(await _service.ApproveAsync(id, CurrentUserId));
    }

    [HttpPost("{id:int}/reject")] 
    [Authorize(Roles = "Approver,Admin")]
    public async Task<ActionResult<InvoiceDto>> Reject(int id)
    {
        return Ok(await _service.RejectAsync(id, CurrentUserId));
    }

    [HttpPost("{id:int}/forward")]
    public async Task<ActionResult<InvoiceDto>> Forward(
        int id,
        ForwardInvoiceDto dto)
    {
        return Ok(await _service.ForwardAsync(id, CurrentUserId, dto));
    }

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(
        int id,
        
        CreateCommentDto dto)
    {
        var comment = await _service.AddCommentAsync(id, CurrentUserId, dto);
        return CreatedAtAction(nameof(GetComments), new { id }, comment);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
      await _service.DeleteAsync(id);
      return NoContent();
    }


}
