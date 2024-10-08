﻿using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Task> CreateAsync(Models.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Models.Task?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<PagedList<Models.Task>> GetAllByUserIdAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 10,
        Status? status = null,
        Priority? priority = null,
        DateTime? dueDate = null,
        string? sortBy = null,
        bool afterDueDate = false,
        bool sortDescending = false
    )
    {
        var query = _context.Tasks.Where(t => t.UserId == userId);

        // Apply filtering
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (dueDate.HasValue)
        {
            query = afterDueDate
                ? query.Where(t => t.DueDate > dueDate.Value)
                : query.Where(t => t.DueDate <= dueDate.Value);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "duedate" => sortDescending 
                    ? query.OrderByDescending(t => t.DueDate) 
                    : query.OrderBy(t => t.DueDate),
                "priority" => sortDescending
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority),
                _ => throw new ArgumentException("Invalid sort by parameter.")
            };
        }

        return await PagedList<Models.Task>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<Models.Task> UpdateAsync(Models.Task task)
    {
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}