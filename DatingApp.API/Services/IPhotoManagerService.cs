using System.Threading.Tasks;

namespace DatingApp.API.Services
{
    public interface IPhotoManagerService
    {
         Task DeletePhoto(int photoId, bool preventMainPhotoDeletion = true);
         Task ApprovePhoto(int photoId);
    }
}