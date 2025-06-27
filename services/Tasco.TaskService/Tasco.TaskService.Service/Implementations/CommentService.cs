using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Repository.UnitOfWork;
using Tasco.TaskService.Repository.Paginate;
using Tasco.TaskService.Service.BusinessModels;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.Service.Implementations;

public class CommentService : BaseService<CommentService>, ICommentService
{
    public CommentService(
        IUnitOfWork<TaskManagementDbContext> unitOfWork,
        ILogger<CommentService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    ) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
    }

    public async Task<Comment> AddCommentAsync(Guid taskId, CommentBusinessModel request)
    {
        var now = DateTime.UtcNow;
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = request.UserId,
            UserName = request.UserName,
            Content = request.Content,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };
        await _unitOfWork.GetRepository<Comment>().InsertAsync(comment);
        await _unitOfWork.CommitAsync();
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId)
    {
        return await _unitOfWork.GetRepository<Comment>()
            .GetListAsync(predicate: c => c.TaskId == taskId && !c.IsDeleted,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt));
    }

    public async Task<IPaginate<Comment>> GetCommentsByTaskIdWithPaginationAsync(Guid taskId, int pageSize, int pageIndex)
    {
        return await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
            predicate: c => c.TaskId == taskId && !c.IsDeleted,
            orderBy: q => q.OrderByDescending(c => c.CreatedAt),
            page: pageIndex, // Repository already uses 1-based indexing
            size: pageSize
        );
    }

    public async Task<Comment> UpdateCommentAsync(Guid commentId, CommentBusinessModel request, Guid userId)
    {
        var comment = await _unitOfWork.GetRepository<Comment>()
            .SingleOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own comments.");
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.GetRepository<Comment>().Update(comment);
        await _unitOfWork.CommitAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _unitOfWork.GetRepository<Comment>()
            .SingleOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own comments.");
        }

        comment.IsDeleted = true;
        _unitOfWork.GetRepository<Comment>().Update(comment);
        await _unitOfWork.CommitAsync();
    }
}