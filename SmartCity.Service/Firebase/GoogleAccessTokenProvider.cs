using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SmartCity.Domain.Helper;
using SmartCity.Service.Abstracts;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCity.Service.Implementations.Firebase
{
    public class GoogleAccessTokenProvider : IFirebaseAccessTokenProvider
    {
        private readonly GoogleCredential _credential;
        //constractor
        public GoogleAccessTokenProvider(
         IOptions<FirebaseRealtimeConfig> cfg,
          IWebHostEnvironment env)
        {
            var relativePath = cfg.Value.ServiceAccountRelativePath;

            var fullPath = Path.Combine(env.ContentRootPath, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Service account file not found at: {fullPath}");

            _credential = GoogleCredential.FromFile(fullPath)
                .CreateScoped(new[]
                {
            "https://www.googleapis.com/auth/firebase.database",
            "https://www.googleapis.com/auth/cloud-platform"
                });
        }


        public Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            return _credential.UnderlyingCredential.GetAccessTokenForRequestAsync(null, ct);
        }
    }
}
