using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces {
    public interface IAuthenticationService {
        public string CreateAccessToken(User user);
        public RefreshToken CreateRefreshToken(int userId);
        public string RefreshAccessToken(string expiredToken, string refreshToken);
    }
}
