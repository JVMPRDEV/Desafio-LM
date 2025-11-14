namespace LM.SharedKernel.Dtos
{
    public record OrderReadItem(
        Guid Id,
        Guid CustomerId,
        DateTime OrderDate, 
        string Status,
        decimal TotalAmount,
        DateTime CreatedAt,
        Guid CreatedByUserId,  
        string CreatedByName   
    );
}