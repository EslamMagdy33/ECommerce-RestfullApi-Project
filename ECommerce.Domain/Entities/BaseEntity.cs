using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? UpdatedAt  { get; protected set; }
        public bool IsDeleted { get; set; }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        // To do : CreatedById      UpdatedById     لما يكون عندي User عايز اعرف مين اللي عمل Create او update 
        // يعني لما اعمل Authentication & Authorization هعوز اتتبع مين اللي عمل تعديل  في الريكورد 
        // تالت حاجه لازم نعمل intersector علشان ال Audit columns دي لان مش كل مرة هعمل كدااستخدم Query filter علشان ال isdeleted دي 

    }
}
