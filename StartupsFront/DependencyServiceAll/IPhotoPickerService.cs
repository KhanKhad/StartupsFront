using System.IO;
using System.Threading.Tasks;

namespace StartupsFront.DependencyServiceAll
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
