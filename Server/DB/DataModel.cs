using System.ComponentModel.DataAnnotations.Schema;

namespace Server.DB;

[Table("Account")]
public class AccountDb
{
    public int AccountDbId { get; set; }

    public string AccountName { get; set; }
    // public ICollection<PlayerDb> players { get; set; }
}