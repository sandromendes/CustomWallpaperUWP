using System.Threading.Tasks;

namespace CustomWallpaper.Navigation
{
    public interface INavigationServiceEx
    {
        Task NavigateAsync(NavigationItem item);
    }
}
