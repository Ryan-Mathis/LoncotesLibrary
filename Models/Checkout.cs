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
    public bool Paid { get; set; }
    private static readonly decimal _lateFeePerDay = .50M;
    public decimal? LateFee
    {
        get
        {
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckoutDays);
            if (ReturnDate != null && dueDate < ReturnDate)
            {
                int daysLate = ((DateTime)ReturnDate - dueDate).Days;
                decimal fee = daysLate * _lateFeePerDay;
                return fee;
            }
            return null;
        }
    }

}