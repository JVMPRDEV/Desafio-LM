using System.ComponentModel.DataAnnotations;

namespace LM.SharedKernel.Dtos
{
    public record CreateOrderItemDto
    {
        [Required]
        public required Guid ProductId { get; init; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public required int Quantity { get; init; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço unitário deve ser maior que zero.")]
        public required decimal UnitPrice { get; init; }
    }
}