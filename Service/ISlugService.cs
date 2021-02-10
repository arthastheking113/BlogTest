using BlogTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Service
{
    public interface ISlugService
    {
        string URLFriendly(string title);

        bool IsUnique(ApplicationDbContext dbContext, string slug);
    }
}
