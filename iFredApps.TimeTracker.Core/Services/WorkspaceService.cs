﻿using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Services
{
   public class WorkspaceService : IWorkspaceService
   {
      private readonly IWorkspaceRepository _workspaceRepository;

      public WorkspaceService(IWorkspaceRepository sessionRepository)
      {
         _workspaceRepository = sessionRepository;
      }

      public async Task<Result<IEnumerable<Workspace>>> GetAllByUserId(int user_id)
      {
         return Result<IEnumerable<Workspace>>.Ok(await _workspaceRepository.GetAllByUserId(user_id));
      }

      public async Task<Result<Workspace>> Create(Workspace workspace)
      {
         return Result<Workspace>.Ok(await _workspaceRepository.Create(workspace));
      }

      public async Task<Result<Workspace>> Update(Workspace workspace)
      {
         return Result<Workspace>.Ok(await _workspaceRepository.Update(workspace));
      }

      public async Task<Result<bool>> Delete(int workspace_id)
      {
         await _workspaceRepository.Delete(workspace_id);
         return Result<bool>.Ok(true);
      }
   }
}
