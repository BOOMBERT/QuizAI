﻿using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetIdAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();
    }

    public async Task<string?> GetEmailAsync(string id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => u.Email)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.NormalizedEmail == email.ToUpper());
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
