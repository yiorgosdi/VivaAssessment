using System.ComponentModel.DataAnnotations;

namespace VivaAssessment.Domain.Entities;

public sealed class RequestObj
{
    [Required]
    [MinLength(2)]
    public IEnumerable<int>? RequestArrayObj { get; set; }
}
