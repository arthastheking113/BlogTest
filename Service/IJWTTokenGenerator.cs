using BlogTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogTest.Service
{
	public interface IJWTTokenGenerator
	{
		string GenerateToken(BlogUser user, IList<string> roles, IList<Claim> claims);
	}
}
