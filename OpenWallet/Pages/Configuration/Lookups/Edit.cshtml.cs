using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;

namespace OpenWallet.Pages.Configuration.Lookups;

public class EditModel(UserContext dbContext, IAuditService auditService) : PageModel
{
    [BindProperty]
    public LookupInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id is null)
        {
            Input.Details.Add(new LookupDetailInput { IsActive = true, SortOrder = 1 });
            return Page();
        }

        var lookup = await dbContext.Lookups.Include(l => l.Details).FirstOrDefaultAsync(l => l.Id == id);
        if (lookup is null) return NotFound();

        Input = new LookupInput
        {
            Id = lookup.Id,
            Name = lookup.Name,
            Description = lookup.Description,
            IsActive = lookup.IsActive,
            Details = lookup.Details.OrderBy(d => d.SortOrder).Select(d => new LookupDetailInput
            {
                Id = d.Id,
                Code = d.Code,
                ValueEn = d.ValueEn,
                ValueAr = d.ValueAr,
                SortOrder = d.SortOrder,
                IsActive = d.IsActive
            }).ToList()
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = User.Identity?.Name ?? "System";
        var orgId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var lookup = await dbContext.Lookups.Include(l => l.Details).FirstOrDefaultAsync(l => l.Id == Input.Id);
        if (lookup is null)
        {
            lookup = new Lookup { Id = Guid.NewGuid(), OrganizationId = orgId, CreatedBy = user };
            dbContext.Lookups.Add(lookup);
        }
        else
        {
            lookup.UpdatedAt = DateTime.UtcNow;
            lookup.UpdatedBy = user;
        }

        lookup.Name = Input.Name.Trim();
        lookup.Description = Input.Description ?? "";
        lookup.IsActive = Input.IsActive;

        foreach (var row in Input.Details.Where(d => !string.IsNullOrWhiteSpace(d.Code)))
        {
            var detail = lookup.Details.FirstOrDefault(d => d.Id == row.Id && row.Id != Guid.Empty);
            if (detail is null)
            {
                detail = new LookupDetail { Id = Guid.NewGuid(), OrganizationId = orgId, CreatedBy = user };
                lookup.Details.Add(detail);
            }
            else
            {
                detail.UpdatedAt = DateTime.UtcNow;
                detail.UpdatedBy = user;
            }

            detail.Code = row.Code.Trim();
            detail.ValueEn = row.ValueEn.Trim();
            detail.ValueAr = row.ValueAr.Trim();
            detail.SortOrder = row.SortOrder;
            detail.IsActive = row.IsActive;
        }

        await dbContext.SaveChangesAsync();
        await auditService.LogAsync("LookupSaved", nameof(Lookup), lookup.Id.ToString(), lookup.OrganizationId, user);
        return RedirectToPage("Index");
    }

    public class LookupInput
    {
        public Guid Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<LookupDetailInput> Details { get; set; } = [];
    }

    public class LookupDetailInput
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string ValueEn { get; set; } = string.Empty;
        public string ValueAr { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
