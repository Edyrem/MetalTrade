
/*
=======
>>>>>>> 70c65e1fc4c1c4268fc92db65f1b9e68c15c9d37
using MetalTrade.DataAccess.Interfaces.Repositories;

namespace MetalTrade.Domain.Entities;

//для сохраенных объявлений
public class Favorite : ISoftDeletable
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AdvertisementId { get; set; }

    public User User { get; set; } = null!;
    public Advertisement Advertisement { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
<<<<<<< HEAD
*/

