using PHONGTROOA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public Cart? Cart { get; set; }

    public int PhongId { get; set; }   // ✅ ใช้ตัวนี้
    public Phong? Phong { get; set; }

    public int Quantity { get; set; }  // ✅ ใช้ตัวนี้
    public DateTime StartDate { get; set; }
}