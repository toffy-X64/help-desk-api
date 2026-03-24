using HelpDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Abstractions
{
    public interface IUsersRepository
    {
        ValueTask<User[]> Get(SearchOptions options);
        ValueTask<User?> Get(Guid id);
        void Add(User user);
        void Update(User user);
        Task<bool> IsEmailTaken(string email);
    }

    public class SearchOptions
    {
        public string? SearchText { get; set; }
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
    }
}
