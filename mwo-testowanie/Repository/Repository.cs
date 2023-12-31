﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mwo_testowanie.Database;

namespace mwo_testowanie.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (includeProperties is null) return await query.FirstOrDefaultAsync();
        
        query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, property) => current.Include(property));
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (includeProperties is null) return await query.ToListAsync();
        
        query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, property) => current.Include(property));
        
        var result = await query.ToListAsync();

        return result;
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}