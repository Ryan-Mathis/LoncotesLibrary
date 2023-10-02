using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Patron
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public List<Checkout>? Checkouts { get; set; }
    public decimal? Balance
    {
        get
        {
            if (Checkouts != null)
            {
                List<decimal?> lateFees =
                Checkouts
                .Where(co => co.Paid != true)
                .Select(co => co.LateFee)
                .ToList();
                decimal? totalBalance = lateFees.Sum();
                return totalBalance;
            }
            return null;
        }
    }
}