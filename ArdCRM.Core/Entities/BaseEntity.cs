namespace ArdCRM.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    public DateTime? GuncellemeTarihi { get; set; }
    public bool Aktif { get; set; } = true;
}
