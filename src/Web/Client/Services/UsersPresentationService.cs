namespace Web.Client.Services;

public class UsersPresentationService
{
    private readonly UsersService _usersService;
    private Dictionary<Guid, string>? _userAvatarColorById;

    public UsersPresentationService(UsersService usersService)
    {
        _usersService = usersService;
        
    }

    public async Task<string> GetUserAvatarColor(Guid userId)
    {
        if(_userAvatarColorById is null)
        {
            var presentationData = await _usersService.GetAllPresentationData();
            _userAvatarColorById = presentationData?.Data.ToDictionary(k => k.UserId, v => v.AvatarColor);
        }

        if(_userAvatarColorById is null || !_userAvatarColorById.ContainsKey(userId))
        {
            return "#000000";
        }

        return _userAvatarColorById[userId];
    }
}
