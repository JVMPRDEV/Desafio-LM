using LM.SharedKernel.Enums;

namespace LM.Orders.Contracts.Orders.Responses
{
    public record OrderItemResponse
    {
        public required Guid Id { get; init; }
        public required Guid ProductId { get; init; }
        public required int Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    public class OrderResponse
    {
        public required Guid Id { get; set; }
        public required Guid CustomerId { get; set; }
        public required OrderStatus Status { get; set; }
        public required decimal TotalAmount { get; set; }
        public required DateTime OrderDate { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public required List<OrderItemResponse> Items { get; set; }
    }
}