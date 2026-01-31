using SmartCity.AppCore.Bases;
using SmartCity.Service.Abstracts;

namespace SmartCity.AppCore.Features.Auth.Queries.Handlers
{
    public class AuthQueryHandler : ResponseHandler
    //IRequestHandler<AuthorizeUserQuery, Response<string>>

    {

        #region Fields
        private readonly IAuthService _authService;


        #endregion

        #region Constructors
        public AuthQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }


        #endregion

        #region Handle Functions

        //public async Task<Response<string>> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
        //{
        //    var result = await _authService.ValidateToken(request.AccessToken);
        //    if (result == "NotExpired")
        //        return Success(result);
        //    return Unauthorized<string>("Token Is Expired");
        //}



        #endregion
    }
}
