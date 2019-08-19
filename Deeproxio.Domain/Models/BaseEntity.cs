using System;

namespace Deeproxio.Domain.Models
{
    public class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreateTS { get; protected set; }
        public DateTime UpdateTS { get; set; }
    }
}
