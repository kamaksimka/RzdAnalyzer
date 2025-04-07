using Microsoft.EntityFrameworkCore;
using RZD.Application.Models;
using RZD.Database;
using RZD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Services
{
    public class FeedbackService
    {
        private readonly DataContext _context;

        public FeedbackService(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CreateFeedbackRequest request,ClaimsPrincipal user)
        {
            var userId = await _context.Users
                .Where(x => x.Email == user.FindFirst(ClaimTypes.Email)!.Value)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            await _context.Feedbacks.AddAsync(new Feedback
            {
                Body = request.Body,
                CreatedDate = DateTimeOffset.UtcNow,
                UserId = userId
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<FeedbackModel>> GetAllAsync()
        {
            var feedbacks = await _context.Feedbacks.Select(x => new FeedbackModel
            {
                UserEmail = x.User.Email,
                Body = x.Body,
                CreatedDate = x.CreatedDate,
            }).ToListAsync();

            return feedbacks;
        }
    }
}
