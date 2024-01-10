﻿using CloudinaryDotNet.Actions;

namespace ChattingApp.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoASync(IFormFile file);
        Task<DeletionResult> DeletePhotoASync(string publicId);
    }
}
