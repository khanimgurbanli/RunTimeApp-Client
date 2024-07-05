namespace RealTimeApp.Client.ViewModels
{
    public record ListedRolesQueryResponse(List<Role_ViewModel> Roles);

    public record Role_ViewModel(Guid Id, string Name)
    {
    }
}
