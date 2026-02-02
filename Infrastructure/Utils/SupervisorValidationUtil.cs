using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;

namespace TaskSurvey.Infrastructure.Utils
{
    public class SupervisorValidationUtil
    {
        public static async Task<bool> WouldCreateCircularReference(
            AppDbContext context, 
            string userId, 
            string proposedSupervisorId)
        {
            if (userId == proposedSupervisorId)
            {
                return true;
            }

            return await IsSubordinateOf(context, proposedSupervisorId, userId);
        }

        private static async Task<bool> IsSubordinateOf(
            AppDbContext context, 
            string targetUserId, 
            string supervisorId)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(targetUserId);

            while (queue.Count > 0)
            {
                var currentUserId = queue.Dequeue();

                if (visited.Contains(currentUserId))
                {
                    continue;
                }
                visited.Add(currentUserId);

                var supervisorRelation = await context.UserRelations
                    .FirstOrDefaultAsync(ur => ur.UserId == currentUserId);

                if (supervisorRelation != null)
                {
                    if (supervisorRelation.SupervisorId == supervisorId)
                    {
                        return true;
                    }

                    queue.Enqueue(supervisorRelation.SupervisorId);
                }
            }

            return false;
        }

        public static async Task<List<string>> GetSupervisorChain(
            AppDbContext context, 
            string userId)
        {
            var chain = new List<string>();
            var visited = new HashSet<string>();
            var currentUserId = userId;

            while (currentUserId != null)
            {
                if (visited.Contains(currentUserId))
                {
                    break;
                }
                visited.Add(currentUserId);

                var relation = await context.UserRelations
                    .FirstOrDefaultAsync(ur => ur.UserId == currentUserId);

                if (relation != null)
                {
                    chain.Add(relation.SupervisorId);
                    currentUserId = relation.SupervisorId;
                }
                else
                {
                    break;
                }
            }

            return chain;
        }

        public static async Task<List<string>> GetAllSubordinateIds(
            AppDbContext context, 
            string supervisorId)
        {
            var allSubordinates = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(supervisorId);
            var visited = new HashSet<string>();

            while (queue.Count > 0)
            {
                var currentSupervisorId = queue.Dequeue();

                if (visited.Contains(currentSupervisorId))
                {
                    continue;
                }
                visited.Add(currentSupervisorId);

                var directSubordinates = await context.UserRelations
                    .Where(ur => ur.SupervisorId == currentSupervisorId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                foreach (var subordinateId in directSubordinates)
                {
                    allSubordinates.Add(subordinateId);
                    queue.Enqueue(subordinateId);
                }
            }

            return allSubordinates.ToList();
        }
    }
}