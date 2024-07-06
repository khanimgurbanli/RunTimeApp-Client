namespace RealTimeApp.Client.ViewModels
{
    public record ListedUsersQueryResponse(List<User_ViewModel> Users);
    public record User_ViewModel(Guid Id, string Firstname, string Lastname)
    {
    }
}
