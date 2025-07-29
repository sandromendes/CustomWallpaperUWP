using System.Threading.Tasks;

namespace CustomWallpaper.Navigation
{
    public interface INavigationAwareEx
    {
        Task OnShowAsync(object parameter = null);
    }
}
