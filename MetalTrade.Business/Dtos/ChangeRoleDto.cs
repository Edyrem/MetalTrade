namespace MetalTrade.Business.Dtos;

public class ChangeRoleDto
{
    public int UserId { get; set; }
    public string Role { get; set; }
    public bool isAdd { get; set; }
}