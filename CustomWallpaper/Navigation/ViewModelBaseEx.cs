using Prism.Windows.Mvvm;
using System.Threading.Tasks;

namespace CustomWallpaper.Navigation
{
    public abstract class ViewModelBaseEx : ViewModelBase, INavigationAwareEx
    {
        public virtual Task OnShowAsync(object parameter = null)
        {
            return Task.CompletedTask;
        }
    }
}
