using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Checkout
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; }
    public int PatronId { get; set; }
    public Patron Patron { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}