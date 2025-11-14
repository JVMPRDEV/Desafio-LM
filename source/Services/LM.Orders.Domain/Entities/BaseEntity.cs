using MediatR;

namespace LM.Orders.Domain.Entities
{
    public abstract class BaseEntity
    {
        private readonly List<INotification> _domainEvents = [];
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Guid CreatedByUserId { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? UpdatedByUserId { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }
        public Guid? DeletedByUserId { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        protected BaseEntity(Guid createdByUserId) : this()
        {
            CreatedByUserId = createdByUserId;
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void SetCreationInfo(Guid userId)
        {
            CreatedByUserId = userId;
        }

        public void SetUpdateDate(Guid userId)
        {
            UpdatedAt = DateTime.Now;
            UpdatedByUserId = userId;
        }

        public void SetDeletionDate(Guid userId)
        {
            DeletedAt = DateTime.Now;
            DeletedByUserId = userId;
        }
    }
}