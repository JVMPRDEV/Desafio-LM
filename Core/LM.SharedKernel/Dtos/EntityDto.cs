namespace LM.SharedKernel.Dtos
{
    public abstract class EntityDto
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Guid CreatedByUserId { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? UpdatedByUserId { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }
        public Guid? DeletedByUserId { get; protected set; }

        protected EntityDto()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        protected EntityDto(Guid createdByUserId) : this()
        {
            CreatedByUserId = createdByUserId;
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