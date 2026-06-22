using Ecommerce.Application.DTOs.Domain.Base;

namespace Ecommerce.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public int UserId { get; set; }
        public string SessionId { get; set; }   
        public string SenderType { get; set; }     
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
    }
}