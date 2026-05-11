using System.ComponentModel.DataAnnotations;
namespace PHONGTROOA.Models
{
public class Cart
{
    [Key]
    public int Id { get; set; }

    public string? UserId { get; set; }

    public List<CartItem> Items { get; set; } = new();
}
}
