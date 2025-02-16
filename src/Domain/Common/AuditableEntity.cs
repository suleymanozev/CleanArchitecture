﻿namespace CleanArchitecture.Domain.Common;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime Created { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public Guid? LastModifiedBy { get; set; }
}