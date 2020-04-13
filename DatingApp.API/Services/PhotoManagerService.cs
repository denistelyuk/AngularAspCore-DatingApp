using System;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Services
{
    public class PhotoManagerService : IPhotoManagerService
    {
        private readonly IDatingRepository _repo;
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        public PhotoManagerService(IDatingRepository repo, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._repo = repo;
            this._cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        
        public async Task DeletePhoto(int photoId, bool preventMainPhotoDeletion = true)
        {

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if (photoFromRepo.IsMain && preventMainPhotoDeletion)
                throw new Exception("You cannot  delete your main photo");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
            else
            {
                _repo.Delete(photoFromRepo);
            }

            if (!(await _repo.saveAll()))
                throw new Exception("Failed to delete the photo");

        }

        public async Task ApprovePhoto(int photoId)
        {
            var photoFromRepo = await _repo.GetPhoto(photoId);
            photoFromRepo.IsApproved = true;
            if (!(await _repo.saveAll()))
                throw new Exception("Failed to approve the photo");
        }
    }
}